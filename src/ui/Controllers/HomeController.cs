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
        public ActionResult Index()
        {
            var orgs = _manageApi.GetOrganisations().Result.ToList();

            if (orgs.Count() == 1)
            {
                return this.RedirectToAction("Index", "Courses", new {organisationId = orgs[0].OrganisationId });
            }

            if (orgs.Count() > 1)
            {
                return this.RedirectToAction("Index", "Organisations");
            }

            throw new Exception("No organisations returned from API for this user");
        }

        [Authorize]
        [Route("/we-imported")]
        public async Task<ActionResult> Imported()
        {
            var data = await _manageApi.GetOrganisationCoursesTotal();
            var viewModel = new ImportedCoursesViewModel()
            {
                OrganisationName = data.OrganisationName,
                TotalCount = data.TotalCount
            };
            
            return View(viewModel);
        }

        [AllowAnonymous]
        public IActionResult Error()
        {
            return View();
        }
    }
}
