using System;
using Microsoft.AspNetCore.Mvc;

namespace GovUk.Education.ManageCourses.Ui.Services
{
    public class FrontendUrlService : IFrontendUrlService
    {
        private string frontendBaseUrl;

        public FrontendUrlService(string frontendBaseUrl)
        {
            this.frontendBaseUrl = frontendBaseUrl;
        }

        public RedirectResult RedirectToFrontend(string path)
        {
            return new RedirectResult(frontendBaseUrl + path);
        }
    }
}
