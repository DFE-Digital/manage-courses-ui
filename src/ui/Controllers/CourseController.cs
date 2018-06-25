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
    [Route("course")]
    public class CourseController : Controller
    {
        private readonly ManageApi _manageApi;

        public CourseController(ManageApi manageApi)
        {
            _manageApi = manageApi;
        }

        [Route("{ucasCode}/{subjectId}")]
        public async Task<IActionResult> Index(int subjectId)
        {
            var courses = await _manageApi.GetCourses();
            return View(courses.OrderBy(x => x.Title));
        }

        [Route("{ucasCode}")]
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
                        //Type = course.Type,
                        Code = course.UcasCode
                    }
                },

            };

            return View(courseDetails);
        }
    }
}
