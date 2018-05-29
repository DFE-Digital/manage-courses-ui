using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using ManageCoursesUi.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace ManageCoursesUi.Controllers
{
    public class AuthController : Controller
    {
        /// <summary>
        /// Users returning from registration on DfE Sign-in will arrive here.
        /// This url has to be registered with DfE Sign-in to be allowed,
        /// so don't change it without getting the new path registered with DfE Sign-in.
        /// </summary>
        public const string RegistrationCompleteCallbackPath = "register/complete";

        [Authorize]
        public ActionResult WhoAmI()
        {
            var claims = User.Claims.ToList();
            var model = new WhoAmIViewModel { RawClaims = claims };
            const string organisationClaimType = "organisation";
            var orgs = claims.Where(c => c.Type == organisationClaimType).ToList();
            if (orgs.Count > 1)
            {
                throw new NotSupportedException("multiple " + organisationClaimType + " claims found");
            }
            if (orgs.Count == 1)
            {
                var json = orgs.Single().Value;
                if (json != "{}")
                {
                    var org = JsonConvert.DeserializeObject<OrgClaim>(json);
                    model.Org = org;
                }
            }
            return View(model);
        }

        public async Task Login(string returnUrl = "/")
        {
            await HttpContext.ChallengeAsync(new AuthenticationProperties() { RedirectUri = returnUrl });
        }

        [Authorize]
        public async Task Logout()
        {
            await HttpContext.SignOutAsync(OpenIdConnectDefaults.AuthenticationScheme);
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        }

        [Route(RegistrationCompleteCallbackPath)]
        public ActionResult RegistrationComplete()
        {
            return RedirectToAction("WhoAmI");
        }
    }

    public class WhoAmIViewModel
    {
        public IEnumerable<Claim> RawClaims { get; set; }
        public OrgClaim Org { get; set; }
    }
}
