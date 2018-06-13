using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using GovUk.Education.ManageCourses.ApiClient;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using ManageCoursesUi.ViewModels;
namespace ManageCoursesUi.Controllers
{
    [Authorize]
    public class CoursesController : Controller
    {
        private readonly ManageApi manageApi;
        public CoursesController()
        {
            manageApi = new ManageApi();
        }
        public async Task<IActionResult> Index()
        {
            var courses = await manageApi.GetCourses();
            return View(courses);
        }

        public async Task<IActionResult> Details(int id)
        {
            var courseDetails = new CourseDetailsViewModel
            {
                CourseTitle = "Biology",
                Subjects = new List<SubjectViewModel> { new SubjectViewModel { Subject = "Biology", Type = "	QTS, 1 year full time with salary", Code = "32Q7" } },
                AboutCourse = new AboutCourseViewModel(),
                AboutOrganisation = new AboutOrganisationViewModel(),
                CourseRequirements = new CourseRequirementsViewModel(),
                Salary = new SalaryViewModel()
            };

            return View(courseDetails);
        }
    }
}
