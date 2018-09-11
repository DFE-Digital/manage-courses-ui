using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Reflection;
using System.Security.Claims;
using System.Threading.Tasks;
using GovUk.Education.ManageCourses.ApiClient;
using GovUk.Education.ManageCourses.Ui.ActionFilters;
using GovUk.Education.ManageCourses.Ui.Services;
using GovUk.Education.SearchAndCompare.UI.Shared.ViewComponents;
using IdentityModel.Client;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.SpaServices.Webpack;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Serilog;

namespace GovUk.Education.ManageCourses.Ui
{
    public class Startup
    {
        private readonly Microsoft.Extensions.Logging.ILogger _logger;

        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration, ILoggerFactory logFactory)
        {
            _logger = logFactory.CreateLogger<Startup>();
            Configuration = configuration;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddLogging(loggingBuilder =>
                loggingBuilder.AddSerilog(dispose: true));

            var sharedAssembly = typeof(CourseDetailsViewComponent).GetTypeInfo().Assembly;
            services.AddMvc(options =>
                options.Filters.Add(typeof(McExceptionFilter))
            ).AddApplicationPart(sharedAssembly);

            services.Configure<RazorViewEngineOptions>(o => o.FileProviders.Add(new EmbeddedFileProvider(sharedAssembly, "GovUk.Education.SearchAndCompare.UI.Shared")));

            services.AddRouting(options => options.LowercaseUrls = true);

            services.AddAuthentication(options =>
            {
                options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;

            }).AddCookie(options =>
            {
                options.ExpireTimeSpan = TimeSpan.FromMinutes(120);
                options.Events = new CookieAuthenticationEvents
                {

                    // refer to
                    //  https://github.com/mderriey/TokenRenewal
                    //  https://stackoverflow.com/questions/40032851/how-to-handle-expired-access-token-in-asp-net-core-using-refresh-token-with-open
                    // for more details

                    // this event is fired everytime the cookie has been validated by the cookie middleware,
                    // so basically during every authenticated request
                    // the decryption of the cookie has already happened so we have access to the user claims
                    // and cookie properties - expiration, etc..
                    OnValidatePrincipal = async x =>
                    {
                        // since our cookie lifetime is based on the access token one,
                        // check if we're more than halfway of the cookie lifetime
                        // assume a timeout of 20 minutes.
                        var timeElapsed = DateTimeOffset.UtcNow.Subtract(x.Properties.IssuedUtc.Value);

                        if (timeElapsed > TimeSpan.FromMinutes(19.5))
                        {
                            var identity = (ClaimsIdentity)x.Principal.Identity;
                            var accessTokenClaim = identity.FindFirst("access_token");
                            var refreshTokenClaim = identity.FindFirst("refresh_token");

                            // if we have to refresh, grab the refresh token from the claims, and request
                            // new access token and refresh token
                            var refreshToken = refreshTokenClaim.Value;

                            var clientId = Configuration["auth:oidc:clientId"];
                            const string envKeyClientSecret = "DFE_SIGNIN_CLIENT_SECRET";
                            var clientSecret = Configuration[envKeyClientSecret];
                            if (string.IsNullOrWhiteSpace(clientSecret))
                            {
                                throw new Exception("Missing environment variable " + envKeyClientSecret + " - get this from the DfE Sign-in team.");
                            }
                            var tokenEndpoint = Configuration["auth:oidc:tokenEndpoint"];

                            var client = new TokenClient(tokenEndpoint, clientId, clientSecret);
                            var response = await client.RequestRefreshTokenAsync(refreshToken, new { client_secret = clientSecret });

                            if (!response.IsError)
                            {
                                // everything went right, remove old tokens and add new ones
                                identity.RemoveClaim(accessTokenClaim);
                                identity.RemoveClaim(refreshTokenClaim);

                                identity.AddClaims(new[]
                                {
                                    new Claim("access_token", response.AccessToken),
                                        new Claim("refresh_token", response.RefreshToken)
                                });

                                // indicate to the cookie middleware to renew the session cookie
                                // the new lifetime will be the same as the old one, so the alignment
                                // between cookie and access token is preserved
                                x.ShouldRenew = true;
                            }
                            else 
                            {
                                // could not refresh - log the user out
                                _logger.LogWarning("Token refresh failed with message: " + response.ErrorDescription);
                                x.RejectPrincipal();
                            }
                        }
                    }
                };
            }).AddOpenIdConnect(options =>
            {
                options.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.MetadataAddress = Configuration["auth:oidc:metadataAddress"];

                options.ClientId = Configuration["auth:oidc:clientId"];
                const string envKeyClientSecret = "DFE_SIGNIN_CLIENT_SECRET";
                var clientSecret = Configuration[envKeyClientSecret];
                if (string.IsNullOrWhiteSpace(clientSecret))
                {
                    throw new Exception("Missing environment variable " + envKeyClientSecret + " - get this from the DfE Sign-in team.");
                }

                options.ClientSecret = clientSecret;
                options.ResponseType = OpenIdConnectResponseType.Code;
                options.GetClaimsFromUserInfoEndpoint = true;

                // using this property would align the expiration of the cookie
                // with the expiration of the identity token
                // UseTokenLifetime = true;

                options.Scope.Clear();
                options.Scope.Add("openid");
                options.Scope.Add("email");
                options.Scope.Add("profile");

                options.Scope.Add("offline_access");

                options.SaveTokens = true;
                options.CallbackPath = new PathString(Configuration["auth:oidc:callbackPath"]);
                options.SignedOutCallbackPath = new PathString(Configuration["auth:oidc:signedOutCallbackPath"]);
                options.SecurityTokenValidator = new JwtSecurityTokenHandler
                {
                    InboundClaimTypeMap = new Dictionary<string, string>(),
                    TokenLifetimeInMinutes = 20,
                    SetDefaultTimesOnTokenCreation = true,
                };
                options.ProtocolValidator = new OpenIdConnectProtocolValidator
                {
                    RequireSub = true,
                    RequireStateValidation = false,
                    NonceLifetime = TimeSpan.FromMinutes(15)
                };

                options.DisableTelemetry = true;
                options.Events = new OpenIdConnectEvents
                {

                    OnMessageReceived = context =>
                        {
                            var isSpuriousAuthCbRequest =
                                context.Request.Path == new Microsoft.AspNetCore.Http.PathString("/auth/cb") &&
                                context.Request.Method == "GET" &&
                                !context.Request.Query.ContainsKey("code");

                            if (isSpuriousAuthCbRequest)
                            {
                                context.HandleResponse();
                                context.Response.StatusCode = 302;
                                context.Response.Headers["Location"] = "/";
                            }

                            return Task.CompletedTask;
                        },

                    OnRedirectToIdentityProvider = context =>
                    {
                        context.ProtocolMessage.Prompt = "consent";
                        return Task.CompletedTask;
                    },

                    // that event is called after the OIDC middleware received the auhorisation code,
                    // redeemed it for an access token and a refresh token,
                    // and validated the identity token
                    OnTokenValidated = x =>
                    {
                        // store both access and refresh token in the claims - hence in the cookie
                        var identity = (ClaimsIdentity)x.Principal.Identity;
                        identity.AddClaims(new[]
                        {
                                new Claim("access_token", x.TokenEndpointResponse.AccessToken),
                                    new Claim("refresh_token", x.TokenEndpointResponse.RefreshToken)
                        });

                        // so that we don't issue a session cookie but one with a fixed expiration
                        x.Properties.IsPersistent = true;

                        // align expiration of the cookie with expiration of the
                        // access token
                        var accessToken = new JwtSecurityToken(x.TokenEndpointResponse.IdToken);
                        x.Properties.ExpiresUtc = accessToken.ValidTo;

                        return Task.CompletedTask;
                    }
                };
            });

            services.AddSingleton<IFeatureFlags>(x => new FeatureFlags(Configuration.GetSection("features")));
            services.AddSingleton<ISearchAndCompareUrlService>(x => new SearchAndCompareUrlService(Configuration.GetValue("SearchAndCompare:UiBaseUrl", "")));
            services.AddSingleton<IManageCoursesApiClientConfiguration, ManageCoursesApiClientConfiguration>();
            services.AddScoped(provider => AnalyticsPolicy.FromEnv());
            services.AddScoped<AnalyticsAttribute>();
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddSingleton<ManageCoursesConfig, ManageCoursesConfig>();
            services.AddSingleton<IManageApi, ManageApi>();
            services.AddScoped<ICourseMapper, CourseMapper>();
            services.AddSingleton(serviceProvider =>
            {
                var manageCoursesApiClientConfiguration = serviceProvider.GetService<IManageCoursesApiClientConfiguration>();
                var manageCoursesApiClient = new ManageCoursesApiClient(manageCoursesApiClientConfiguration, new System.Net.Http.HttpClient());
                var config = serviceProvider.GetService<ManageCoursesConfig>();
                manageCoursesApiClient.BaseUrl = config.ApiUrl;
                return manageCoursesApiClient;
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, IServiceProvider serviceProvider)
        {
            if (env.IsDevelopment())
            {
                app.UseBrowserLink();
                app.UseWebpackDevMiddleware(new WebpackDevMiddlewareOptions
                {
                    HotModuleReplacement = true
                });
            }

            app.UseStatusCodePagesWithReExecute("/error/{0}");

            app.UseStaticFiles();

            // https://docs.microsoft.com/en-gb/aspnet/core/host-and-deploy/linux-nginx?view=aspnetcore-2.1&tabs=aspnetcore2x
            // Prod uses edge-terminated TLS, so we need to tell openid connect the right scheme
            var forwardedHeadersOptions = new ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto,
            };
            // trust everything so that the edge-terminated TLS proxy on openshift are accepted
            forwardedHeadersOptions.KnownProxies.Clear();
            forwardedHeadersOptions.KnownNetworks.Clear();

            app.UseForwardedHeaders(forwardedHeadersOptions);

            app.UseAuthentication();

            var config = serviceProvider.GetService<ManageCoursesConfig>();

            _logger.LogInformation("Using API base URL: {0}", config.ApiUrl);

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
                routes.MapRoute(
                    name: "default2",
                    template: "{controller}/{action}/{id?}");
                routes.MapRoute("registrationCallback",
                    config.RegisterCallbackPath,
                    new { controller = "Auth", action = "RegistrationComplete" });
                routes.MapRoute("cookies", "cookies",
                    defaults: new { controller = "Legal", action = "Cookies" });
                routes.MapRoute("privacy", "privacy-policy",
                    defaults: new { controller = "Legal", action = "Privacy" });
                routes.MapRoute("tandc", "terms-conditions",
                    defaults: new { controller = "Legal", action = "TandC" });
            });
        }
    }
}
