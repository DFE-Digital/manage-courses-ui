using Microsoft.Extensions.Configuration;

namespace GovUk.Education.ManageCourses.Ui
{
    public class ManageCoursesConfig
    {
        private readonly IConfiguration _configuration;

        public ManageCoursesConfig(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        /// <summary>
        /// Users returning from registration on DfE Sign-in will arrive here.
        /// This url has to be registered with DfE Sign-in to be allowed,
        /// so don't change it without getting the new path registered with DfE Sign-in.
        /// </summary>
        public string RegisterCallbackPath => _configuration["register:registrationCallbackPath"];

        public string ClientId => _configuration["auth:oidc:clientId"];

        public string SiteBaseUrl => _configuration["url:site"];

        public string RegisterBaseUrl => _configuration["url:register"];

        public string ExternalRegistrationUrl => $"{this.RegisterBaseUrl}?client_id={this.ClientId}&redirect_uri={this.SiteBaseUrl}/{this.RegisterCallbackPath}";
    }
}
