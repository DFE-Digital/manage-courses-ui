using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using GovUk.Education.ManageCourses.ApiClient;
using GovUk.Education.ManageCourses.Ui.ActionFilters;
using GovUk.Education.ManageCourses.Ui.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;

namespace GovUk.Education.ManageCourses.Ui
{
    public class Startup
    {
        private readonly ILogger _logger;

        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration, ILoggerFactory logFactory)
        {
            _logger = logFactory.CreateLogger<Startup>();
            Configuration = configuration;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();
            const string envKeyApiConnection = "ApiConnection:Uri";
            var apiUri = Configuration[envKeyApiConnection];
            _logger.LogInformation("Using API base URL: " + apiUri);
            //services.AddScoped<IManageCoursesApi>(provider => new IManageCoursesApi(new HttpClient(), apiUri));

            services.AddAuthentication(options =>
            {
                options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
            }).AddCookie(options => {
                options.ExpireTimeSpan = TimeSpan.FromMinutes(20);
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
                options.UseTokenLifetime = true;
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
            });

            services.AddSingleton<IManageCoursesApiClientConfiguration, ManageCoursesApiClientConfiguration>();
            services.AddScoped<AnalyticsPolicy>(provider => AnalyticsPolicy.FromEnv());
            services.AddScoped<AnalyticsAttribute>();
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddSingleton<ManageCoursesConfig, ManageCoursesConfig>();
            services.AddSingleton<ManageApi, ManageApi>();
            services.AddSingleton(serviceProvider =>
            {
                var manageCoursesApiClientConfiguration = serviceProvider.GetService<IManageCoursesApiClientConfiguration>();
                var manageCoursesApiClient = new ManageCoursesApiClient(manageCoursesApiClientConfiguration);
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
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStatusCodePagesWithReExecute("/Home/Error");

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
            });
        }
    }
}
