using Microsoft.AspNetCore.Mvc;
namespace GovUk.Education.ManageCourses.Ui.Controllers
{
    public class LegalController : CommonAttributesControllerBase
    {
        [HttpGet("cookies")]
        public IActionResult Cookies()
        {
            return View();
        }

        [HttpGet("privacy-policy")]
        public IActionResult Privacy()
        {
            return View();
        }

        [HttpGet("terms-conditions")]
        public IActionResult TandC()
        {
            return View();
        }
    }
}
