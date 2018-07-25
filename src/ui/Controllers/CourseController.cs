using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using GovUk.Education.ManageCourses.Ui.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using GovUk.Education.ManageCourses.Ui.Helpers;

namespace GovUk.Education.ManageCourses.Ui.Controllers
{
    [Authorize]
    [Route("organisation")]
    public class CourseController : CommonAttributesControllerBase
    {
        private readonly ManageApi _manageApi;

        public CourseController(ManageApi manageApi)
        {
            _manageApi = manageApi;
        }
        [Route("{instCode}/course/{accreditingProviderId=self}/{ucasCode}")]
        public async Task<IActionResult> Variants(string instCode, string accreditingProviderId, string ucasCode)
        {
            var course = await _manageApi.GetCoursesByOrganisation(instCode);

            var providerCourse = "self".Equals(accreditingProviderId, StringComparison.InvariantCultureIgnoreCase)
                ? course.ProviderCourses.First(c => String.IsNullOrEmpty(c.AccreditingProviderId))
                : course.ProviderCourses
                    .First(c => c.AccreditingProviderId.Equals(accreditingProviderId, StringComparison.InvariantCultureIgnoreCase));
            var courseDetail = providerCourse.CourseDetails.First(x => x.Variants.Select(v => v.UcasCode == ucasCode).Any());

            var variant = courseDetail.Variants.FirstOrDefault(v => v.UcasCode == ucasCode);
            if (variant == null) return null;

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
    }
}
