using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using GovUk.Education.ManageCourses.ApiClient;
using GovUk.Education.ManageCourses.Api.Model;
using GovUk.Education.ManageCourses.Domain.Models;
using GovUk.Education.ManageCourses.Ui.ViewModels;
using System.Collections.Generic;


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
            IEnumerable<ProviderSummary> orgs;

            try
            {
                orgs = await _manageApi.GetProviderSummaries();
            }
            catch (ManageCoursesApiException e)
            {
                if ((int)e.StatusCode == 401)
                {
                    return StatusCode(401);
                }
                else
                {
                    throw;
                }
            }

            var userOrganisations = orgs.ToList();
            if (userOrganisations.Count() == 1)
            {
                return this.RedirectToAction("Show", "Organisation", new { providerCode = userOrganisations[0].ProviderCode });
            }

            if (userOrganisations.Count() > 1)
            {
                return this.RedirectToAction("Index", "Organisation");
            }

            return StatusCode(401);
        }

        [Authorize]
        [HttpGet("accept-terms")]
        public IActionResult AcceptTerms()
        {
            return View(new AcceptTermsViewModel());
        }

        [Authorize]
        [HttpPost("accept-terms")]
        public async Task<IActionResult> AcceptTermsPost(AcceptTermsViewModel model)
        {
            if (!ModelState.IsValid || model.TermsAccepted == false)
            {
                return View("AcceptTerms", model);
            }

            await _manageApi.LogAcceptTerms();

            return new RedirectResult("/");
        }
    }
}
