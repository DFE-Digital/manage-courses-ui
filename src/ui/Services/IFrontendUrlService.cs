using System;
using Microsoft.AspNetCore.Mvc;

namespace GovUk.Education.ManageCourses.Ui.Services
{
    public interface IFrontendUrlService
    {
        RedirectResult RedirectToFrontend(string path);
    }
}
