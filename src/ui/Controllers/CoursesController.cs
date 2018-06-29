using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using GovUk.Education.ManageCourses.ApiClient;
using GovUk.Education.ManageCourses.Ui.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GovUk.Education.ManageCourses.Ui.Controllers
{
    [Authorize]
    [Route("courses")]
    public class CoursesController : CommonAttributesControllerBase
    {
        private readonly ManageApi _manageApi;

        public CoursesController(ManageApi manageApi)
        {
            _manageApi = manageApi;
        }

        public async Task<IActionResult> Index()
        {
            var courses = await _manageApi.GetCourses();
            var data = await _manageApi.GetOrganisationCoursesTotal();
            var model = new CourseListViewModel
            {
                Courses = courses,
                TotalCount = data.TotalCount
            };
            return View(model);
        }
    }
}
