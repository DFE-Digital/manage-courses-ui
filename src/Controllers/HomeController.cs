using ManageCoursesUi.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ManageCoursesUi.Controllers
{
    public class HomeController : Controller
    {
        private readonly ManageCoursesConfig _config;

        public HomeController(ManageCoursesConfig config)
        {
            _config = config;
        }

        // GET: Home
        public ActionResult Index()
        {
            var model = new HomeViewModel { RegistrationUrl = _config.RegisterCallbackPath };
            return View(model);
        }

        [AllowAnonymous]
        public IActionResult Error()
        {
            return View();
        }
    }
}
