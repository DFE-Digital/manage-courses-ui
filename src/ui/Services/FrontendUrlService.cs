using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace GovUk.Education.ManageCourses.Ui.Services
{
    public class FrontendUrlService : IFrontendUrlService
    {
        private string frontendBaseUrl;
        private readonly IConfiguration _configuration;

        public FrontendUrlService(IConfiguration configuration)
        {
            _configuration = configuration;
            this.frontendBaseUrl = _configuration["ManageCourses:FrontendBaseUrl"];
        }

        public bool ShouldRedirectOrganisationShow()
        {
            return _configuration["FEATURE_FRONTEND_ORGANISATION_SHOW"] == "true";
        }

        public RedirectResult RedirectToFrontend(string path)
        {
            return new RedirectResult(frontendBaseUrl + path);
        }
    }
}
