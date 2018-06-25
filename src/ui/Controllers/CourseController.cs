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
    public class CourseController : CommonAttributesControllerBase
    {
        private readonly ManageApi _manageApi;

        public CourseController(ManageApi manageApi)
        {
            _manageApi = manageApi;
        }

        [Route("{courseTitle}/from-ucas")]
        public async Task<IActionResult> Variants(string courseTitle)
        {
            var course = await _manageApi.GetCourse(courseTitle);

            var viewModel = new FromUcasViewModel {
                OrganisationName = course.OrganisationName,
                CourseTitle = course.Title,
                UcasCode = course.UcasCode,
                Courses = new List<CourseVariantViewModel>()
            };

            return View(viewModel);
        }

        [Route("{courseTitle}")]
        public async Task<IActionResult> Details(string courseTitle)
        {
            var course = await _manageApi.GetCourse(courseTitle);
            var subjects = course.Variants.Select(x =>
            
                new SubjectViewModel
                {
                    Name = course.Title,
                    Type = "TODO: type",
                    ProviderCode = x.ProviderCode,
                    ProgrammeCode = x.CourseCode
                }
            );
            var courseDetails = new CourseDetailsViewModel
            {
                OrganisationName = course.OrganisationName,
                CourseTitle = course.Title,
                Subjects = subjects,
                UcasCode = course.UcasCode
            };

            return View(courseDetails);
        }
    }
}
