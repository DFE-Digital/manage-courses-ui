using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using GovUk.Education.ManageCourses.Ui;
using GovUk.Education.ManageCourses.Ui.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GovUk.Education.ManageCourses.Ui.Controllers
{
    [Authorize(Policy = "AcceptedTerms")]
    [Route("organisations")]
    public class OrganisationsController : CommonAttributesControllerBase
    {
        private readonly IManageApi _manageApi;

        public OrganisationsController(IManageApi manageApi)
        {
            _manageApi = manageApi;
        }
        [HttpGet]
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
