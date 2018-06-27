using System;
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

        [Route("{accreditingProviderId}/{courseTitle}/from-ucas")]
        public async Task<IActionResult> Variants(string accreditingProviderId, string courseTitle)
        {
            var course = await _manageApi.GetCourses();

            var providerCourse = course.ProviderCourses
                .First(c => c.AccreditingProviderId.Equals(accreditingProviderId, StringComparison.InvariantCultureIgnoreCase));

            var courseDetail = providerCourse.CourseDetails.First(x => x.CourseTitle.Equals(courseTitle, StringComparison.InvariantCultureIgnoreCase));
            
            var viewModel = new FromUcasViewModel {
                OrganisationName = course.OrganisationName,
                CourseTitle = courseDetail.CourseTitle,
                UcasCode = providerCourse.AccreditingProviderId,
                Courses = new List<CourseVariantViewModel>()
            };

            return View(viewModel);
        }

        [Route("{accreditingProviderId}/{courseTitle}")]
        public async Task<IActionResult> Details(string accreditingProviderId, string courseTitle)
        {
            var course = await _manageApi.GetCourses();

            var providerCourse = course.ProviderCourses
                .First(c => c.AccreditingProviderId.Equals(accreditingProviderId, StringComparison.InvariantCultureIgnoreCase));

            var courseDetail = providerCourse.CourseDetails
                .First(x => x.CourseTitle.Equals(courseTitle, StringComparison.InvariantCultureIgnoreCase));

            var subjects = courseDetail.Variants.Select(x =>

                new SubjectViewModel
                {
                    Name = courseDetail.CourseTitle,
                    Type = $"{x.ProfPostFlag}, {x.ProgramType}, {x.StudyMode}",
                    ProviderCode = x.TrainingProviderCode,
                    ProgrammeCode = x.CourseCode
                }
            );

            var courseDetails = new CourseDetailsViewModel
            {
                OrganisationName = course.OrganisationName,
                CourseTitle = courseDetail.CourseTitle,
                Subjects = subjects,
                UcasCode = providerCourse.AccreditingProviderId,
            };

            return View(courseDetails);
        }
    }
}
