using System.Threading.Tasks;
using GovUk.Education.ManageCourses.ApiClient;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ManageCoursesUi.Controllers
{
    [Authorize]
    public class CoursesController : Controller
    {
        public async Task<IActionResult> Index()
        {
            var manageApi = new ManageApi();
            var courses = await manageApi.GetCourses();
            return View(courses);
        }
    }
}
