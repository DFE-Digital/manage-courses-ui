using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using GovUk.Education.ManageCourses.ApiClient;
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
            IEnumerable<UserOrganisation> orgs;

            try
            {
                orgs = await _manageApi.GetInstitutionSummaries();
            }
            catch (SwaggerException e)
            {
                if (e.StatusCode == 401)
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
                return this.RedirectToAction("Show", "Organisation", new { instCode = userOrganisations[0].UcasCode });
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
