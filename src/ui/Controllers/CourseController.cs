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
            var courseVariants = courseDetail.Variants.Select(x =>
                new CourseVariantViewModel
                {
                    Name = courseDetail.CourseTitle,
                    Accredited = course.OrganisationName, //course.UcaseCode
                    ProviderCode = providerCourse.AccreditingProviderId,
                    ProgrammeCode = x.CourseCode,
                    AgeRange = courseDetail.AgeRange,
                    Qualifications = x.ProfPostFlag,
                    Route = x.ProgramType,
                    StudyMode = x.StudyMode,
                    //Subjects = courseDetail.Subjects.Aggregate((current, next) => current + ", " + next),
                    // Subjects = "ToDo"
                    Schools = x.Campuses.Select(campus => {
                        var addressLines = new List<string>() { campus.Address1, campus.Address2, campus.Address3, campus.Address4, campus.PostCode };
                        var address = addressLines.Where(line => !String.IsNullOrEmpty(line)).Aggregate((current, next) => current + ", " + next);
                        return new SchoolViewModel
                        {
                            ApplicationsAcceptedFrom = campus.CourseOpenDate,
                            Code = campus.Code,
                            LocationName = campus.Name,
                            Address = address
                        };
                    })
                }
            );

            var viewModel = new FromUcasViewModel
            {
                OrganisationName = course.OrganisationName,
                CourseTitle = courseDetail.CourseTitle,
                AccreditingProviderId = providerCourse.AccreditingProviderId,
                Courses = courseVariants
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
                UcasCode = providerCourse.AccreditingProviderId
            };

            return View(courseDetails);
        }
    }
}
