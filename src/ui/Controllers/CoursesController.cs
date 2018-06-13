using System.Collections.Generic;
using System.Threading.Tasks;
using GovUk.Education.ManageCourses.ApiClient;
using ManageCoursesUi.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
namespace ManageCoursesUi.Controllers
{
    [Authorize]
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
            return View(courses);
        }

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
                // todo: more details:
                AboutCourse = new AboutCourseViewModel(),
                AboutOrganisation = new AboutOrganisationViewModel(),
                CourseRequirements = new CourseRequirementsViewModel(),
                Salary = new SalaryViewModel()
            };

            return View(courseDetails);
        }
    }
}
