using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using System.Threading.Tasks;

namespace ManageCoursesUi.Controllers
{
    public class AuthController : Controller
    {
        [Authorize]
        public ActionResult WhoAmI()
        {
            return View(User as ClaimsPrincipal);
        }

        public async Task Login(string returnUrl = "/")
        {
            await HttpContext.ChallengeAsync("oidc", new AuthenticationProperties() { RedirectUri = returnUrl });
        }

        [Authorize]
        public async Task Logout()
        {
            await HttpContext.SignOutAsync("oidc");
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        }
    }
}
