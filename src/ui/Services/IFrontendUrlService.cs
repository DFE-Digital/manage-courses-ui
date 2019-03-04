using System;
using Microsoft.AspNetCore.Mvc;

namespace GovUk.Education.ManageCourses.Ui.Services
{
    public interface IFrontendUrlService
    {
        bool ShouldRedirectOrganisationShow();
        RedirectResult RedirectToFrontend(string path);
    }
}
