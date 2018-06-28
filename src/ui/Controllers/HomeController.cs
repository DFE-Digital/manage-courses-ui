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
        public ActionResult Index()
        {

            return this.RedirectToAction("imported");
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
