using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
namespace ManageCoursesUi
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
            })
            .AddCookie()
            .AddOpenIdConnect(options =>
            {
                options.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.MetadataAddress = Configuration["auth:oidc:metadataAddress"];

                options.ClientId = Configuration["auth:oidc:clientId"];
                const string envKeyClientSecret = "DFE_SIGNIN_CLIENT_SECRET";
                var clientSecret = Environment.GetEnvironmentVariable(envKeyClientSecret);
                if (string.IsNullOrWhiteSpace(clientSecret))
                {
                    throw new Exception("Missing environment variable " + envKeyClientSecret + " - get this from the DfE Sign-in team.");
                }
                options.ClientSecret = clientSecret;
                options.ResponseType = OpenIdConnectResponseType.Code;
                
                options.UseTokenLifetime = false;
                options.Scope.Clear();
                options.Scope.Add("openid");
                options.Scope.Add("email");
                options.Scope.Add("profile");
                options.Scope.Add("organisation");
                
                options.SaveTokens = true;
                options.CallbackPath = new PathString(Configuration["auth:oidc:callbackPath"]);
                options.SignedOutCallbackPath = new PathString(Configuration["auth:oidc:signedOutCallbackPath"]);
                options.SecurityTokenValidator = new JwtSecurityTokenHandler
                {
                    InboundClaimTypeMap = new Dictionary<string, string>()
                };
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
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

            app.UseStaticFiles();

            app.UseAuthentication();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
                routes.MapRoute(
                    name: "default2",
                    template: "{controller}/{action}/{id?}");
            });
        }
    }
}
