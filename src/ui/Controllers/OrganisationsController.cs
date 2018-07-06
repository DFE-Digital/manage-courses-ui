using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using GovUk.Education.ManageCourses.ApiClient;
using GovUk.Education.ManageCourses.Ui.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartBreadcrumbs;

namespace GovUk.Education.ManageCourses.Ui.Controllers
{
    [Authorize]
    [Route("organisations")]
    public class OrganisationsController : CommonAttributesControllerBase
    {
        private readonly ManageApi _manageApi;

        public OrganisationsController(ManageApi manageApi)
        {
            _manageApi = manageApi;
        }
        [Breadcrumb("Organisations", FromAction = "Home.Index")]
        public async Task<IActionResult> Index()
        {
            var orgs = await _manageApi.GetOrganisations();
            var model = new OrganisationListViewModel
            {
                Oganisations = orgs
            };
            return View(model);
        }
    }
}
