using ManageCoursesUi.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace ManageCoursesUi.Controllers
{
    public class HomeController : Controller
    {
        private readonly IConfiguration _configuration;

        public HomeController(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        // GET: Home
        public ActionResult Index()
        {
            var model = new HomeViewModel
            {
                RegistrationUrl = _configuration["register:registrationCallbackPath"],
            };
            return View(model);
        }

        [AllowAnonymous]
        public IActionResult Error()
        {
            return View();
        }
    }
}
