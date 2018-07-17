using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using GovUk.Education.ManageCourses.ApiClient;
using GovUk.Education.ManageCourses.Ui.ViewModels;

namespace GovUk.Education.ManageCourses.Ui.Controllers
{
    public class HomeController : CommonAttributesControllerBase
    {
        private readonly ManageApi _manageApi;

        public HomeController(ManageApi manageApi)
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
                return this.RedirectToAction("Index", "Courses", new { ucasCode = userOrganisations[0].UcasCode });
            }

            if (userOrganisations.Count() > 1)
            {
                return this.RedirectToAction("Index", "Organisations");
            }

            throw new Exception("No organisations returned from API for this user");
        }

           [AllowAnonymous]
        public IActionResult Error()
        {
            return View();
        }
    }
}
