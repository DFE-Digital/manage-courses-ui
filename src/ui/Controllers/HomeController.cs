using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using GovUk.Education.ManageCourses.ApiClient;
using GovUk.Education.ManageCourses.Ui.ViewModels;
using GovUk.Education.ManageCourses.Ui;

namespace GovUk.Education.ManageCourses.Ui.Controllers
{
    public class HomeController : CommonAttributesControllerBase
    {
        private readonly IManageApi _manageApi;

        public HomeController(IManageApi manageApi)
        {
            _manageApi = manageApi;
        }

        // GET: Home
        [Authorize]
        public async Task<ActionResult> Index()
        {
            var orgs = await _manageApi.GetOrganisations();

            var userOrganisations = orgs.ToList();
            if (userOrganisations.Count() == 1)
            {
                return this.RedirectToAction("Courses", "Organisation", new { ucasCode = userOrganisations[0].UcasCode });
            }

            if (userOrganisations.Count() > 1)
            {
                return this.RedirectToAction("Index", "Organisations");
            }

            throw new Exception("No organisations returned from API for this user");
        }
    }
}
