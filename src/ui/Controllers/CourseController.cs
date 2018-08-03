using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using GovUk.Education.ManageCourses.Ui.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using GovUk.Education.ManageCourses.Ui;
using GovUk.Education.ManageCourses.Ui.Helpers;

namespace GovUk.Education.ManageCourses.Ui.Controllers
{
    [Authorize]
    [Route("organisation")]
    public class CourseController : CommonAttributesControllerBase
    {
        private readonly IManageApi _manageApi;

        public CourseController(IManageApi manageApi)
        {
            _manageApi = manageApi;
        }

        [Route("{instCode}/course/{accreditingProviderId=self}/{ucasCode}")]
        public async Task<IActionResult> Variants(string instCode, string accreditingProviderId, string ucasCode)
        {
            Validate(instCode, accreditingProviderId, ucasCode);

            var course = await _manageApi.GetCoursesByOrganisation(instCode);
            if (course == null) return NotFound();

            var providerCourse = "self".Equals(accreditingProviderId, StringComparison.InvariantCultureIgnoreCase)
                ? course.ProviderCourses.SingleOrDefault(c => String.IsNullOrEmpty(c.AccreditingProviderId))
                : course.ProviderCourses
                    .SingleOrDefault(c => c.AccreditingProviderId.Equals(accreditingProviderId, StringComparison.InvariantCultureIgnoreCase));

            if (providerCourse == null) { return NotFound(); }

            var courseDetail = providerCourse.CourseDetails.SingleOrDefault(c => c.Variants.Any(v => ucasCode.Equals(v.UcasCode, StringComparison.InvariantCultureIgnoreCase)));

            if (courseDetail == null) { throw new InvalidOperationException($"Course variant with ucas code '{ucasCode}' not found"); }

            var variant = courseDetail.Variants.SingleOrDefault(v => ucasCode.Equals(v.UcasCode, StringComparison.InvariantCultureIgnoreCase));

            if (variant == null) { throw new Exception("Unexpected error: variant should not be null"); }

            var subjects = variant.Subjects.Any() ? variant.Subjects.Aggregate((current, next) => current + ", " + next) : "";

            var courseVariant =
                new CourseVariantViewModel
                {
                    Name = courseDetail.CourseTitle,
                    Type = variant.GetCourseVariantType(),
                    Accrediting = providerCourse.AccreditingProviderName,
                    ProviderCode = providerCourse.AccreditingProviderId,
                    ProgrammeCode = variant.CourseCode,
                    UcasCode = course.UcasCode,
                    AgeRange = courseDetail.AgeRange,
                    Route = variant.ProgramType,
                    Qualifications = variant.ProfPostFlag,
                    StudyMode = variant.StudyMode,
                    Subjects = subjects,
                    Schools = variant.Campuses.Select(campus =>
                    {
                        var addressLines = (new List<string>()
                        {
                            campus.Address1,
                            campus.Address2,
                            campus.Address3,
                            campus.Address4,
                            campus.PostCode
                        })
                        .Where(line => !String.IsNullOrEmpty(line));

                        var address = addressLines.Count() > 0 ? addressLines.Where(line => !String.IsNullOrEmpty(line))
                            .Aggregate((current, next) => current + ", " + next) : "";

                        return new SchoolViewModel
                        {
                            ApplicationsAcceptedFrom = campus.CourseOpenDate,
                            Code = campus.Code,
                            LocationName = campus.Name,
                            Address = address
                        };
                    })
                };

            var orgs = await _manageApi.GetOrganisations();

            var viewModel = new FromUcasViewModel
            {
                OrganisationName = course.OrganisationName,
                OrganisationId = course.OrganisationId,
                MultipleOrganisations = orgs.Count() > 1,
                CourseTitle = courseDetail.CourseTitle,
                AccreditingProviderId = providerCourse.AccreditingProviderId,
                Course = courseVariant
            };

            return View(viewModel);
        }

        private void Validate(string instCode, string accreditingProviderId, string ucasCode)
        {
            if (string.IsNullOrEmpty(instCode)) { throw new ArgumentNullException(instCode, "instCode cannot be null or empty"); }
            if (string.IsNullOrEmpty(accreditingProviderId)) { throw new ArgumentNullException(accreditingProviderId, "accreditingProviderId cannot be null or empty"); }
            if (string.IsNullOrEmpty(ucasCode)) { throw new ArgumentNullException(ucasCode, "ucasCode cannot be null or empty"); }
        }
    }
}
