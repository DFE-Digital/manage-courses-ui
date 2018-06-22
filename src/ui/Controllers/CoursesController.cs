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
    public class CoursesController : Controller
    {
        private readonly ManageApi _manageApi;

        public CoursesController(ManageApi manageApi)
        {
            _manageApi = manageApi;
        }

        public async Task<IActionResult> Index()
        {
            var courses = await _manageApi.GetCourses();
            return View(courses.OrderBy(x => x.Title));
        }

        [Route("/course/{ucasCode}")]
        public async Task<IActionResult> Details(string ucasCode)
        {
            var course = await _manageApi.GetCourse(ucasCode);
            var courseDetails = new CourseDetailsViewModel
            {
                CourseTitle = course.Title,
                Subjects = new List<SubjectViewModel>
                {
                    new SubjectViewModel
                    {
                        Subject = "Biology(todo)", // todo - real subject
                        Type = course.Type,
                        Code = course.UcasCode
                    }
                },

            };

            return View(courseDetails);
        }
    }
}
