using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using GovUk.Education.ManageCourses.ApiClient;
using GovUk.Education.ManageCourses.Ui.ViewModels;
using GovUk.Education.ManageCourses.Ui;
using System.Collections.Generic;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;
using GovUk.Education.ManageCourses.Ui.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;


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
                orgs = await _manageApi.GetOrganisations();
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
                return this.RedirectToAction("Courses", "Organisation", new { ucasCode = userOrganisations[0].UcasCode });
            }

            if (userOrganisations.Count() > 1)
            {
                return this.RedirectToAction("Index", "Organisations");
            }

            return StatusCode(401);
        }

        [Authorize]
        [HttpGet("accept-terms")]
        public IActionResult AcceptTerms()
        {
            return View();
        }

        [Authorize(Policy = "AcceptedTerms")] // => [AgreedTermsAuthorize()]
        [HttpGet("Example")]
        public IActionResult Example()
        {
            return View();
        }

        [Authorize]
        [HttpPost("accept-terms")]
        public ActionResult AcceptTermsPost(AcceptTermsViewModel model)
        {
            // Example of accessing claims
            //         var name = "";
            // if (this.User.Identity.IsAuthenticated)
            // {
            //     var given_name = User.Claims.FirstOrDefault(c => c.Type == "given_name")?.Value;
            //     var family_name = User.Claims.FirstOrDefault(c => c.Type == "family_name")?.Value;
            //     name = $"{given_name} {family_name}";
            // }

            // var identity =  this.User.Identity;
            // identity.claims.addOrUpdate("agreed_terms", "");

            // await HttpContext.ChallengeAsync(new AuthenticationProperties() { identity =identity,  RedirectUri = returnUrl });

            // await HttpContext.AuthenticateAsync(new AuthenticationProperties() { RedirectUri = returnUrl });

            // redirect to home

            // else struck on this view
            // return View();
        }
    }
}
