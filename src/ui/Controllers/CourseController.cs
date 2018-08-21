﻿using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using GovUk.Education.ManageCourses.ApiClient;
using GovUk.Education.ManageCourses.Ui.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using GovUk.Education.ManageCourses.Ui;
using GovUk.Education.ManageCourses.Ui.Helpers;

namespace GovUk.Education.ManageCourses.Ui.Controllers
{
    [Authorize(Policy = "AcceptedTerms")]
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

            var orgsList = await _manageApi.GetOrganisations();
            var userOrganisations = orgsList.ToList();
            var multipleOrganisations = userOrganisations.Count() > 1;
            var org = userOrganisations.ToList().FirstOrDefault(x => instCode.ToLower() == x.UcasCode.ToLower());

            if (org == null) { throw new InvalidOperationException($"Organisation with code '{ucasCode}' not found"); }

            var course = await _manageApi.GetCourseByUcasCode(instCode, ucasCode);

            if (course == null) return NotFound();

            var viewModel = LoadViewModel(org, course, multipleOrganisations);

            return View(viewModel);
        }

        private void Validate(string instCode, string accreditingProviderId, string ucasCode)
        {
            if (string.IsNullOrEmpty(instCode)) { throw new ArgumentNullException(instCode, "instCode cannot be null or empty"); }
            if (string.IsNullOrEmpty(accreditingProviderId)) { throw new ArgumentNullException(accreditingProviderId, "accreditingProviderId cannot be null or empty"); }
            if (string.IsNullOrEmpty(ucasCode)) { throw new ArgumentNullException(ucasCode, "ucasCode cannot be null or empty"); }
        }

        private FromUcasViewModel LoadViewModel(UserOrganisation org, Course course, bool multipleOrganisations)
        {
            var courseVariant =
                new CourseVariantViewModel
                {
                    Name = course.Name,
                    Type = course.GetCourseVariantType(),
                    Accrediting = course.AccreditingProviderName,
                    ProviderCode = course.AccreditingProviderId,
                    ProgrammeCode = course.CourseCode,
                    UcasCode = course.InstCode,
                    AgeRange = course.AgeRange,
                    Route = course.ProgramType,
                    Qualifications = course.ProfpostFlag,
                    StudyMode = course.StudyMode,
                    Subjects = course.Subjects,
                    Schools = course.Schools.Select(campus =>
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
                            ApplicationsAcceptedFrom = campus.ApplicationsAcceptedFrom,
                            Code = campus.Code,
                            LocationName = campus.LocationName,
                            Address = address
                        };
                    })
                };


            var viewModel = new FromUcasViewModel
            {
                OrganisationName = org.OrganisationName,
                OrganisationId = org.OrganisationId,                
                CourseTitle = course.Name,
                AccreditingProviderId = course.AccreditingProviderId,
                MultipleOrganisations = multipleOrganisations,
                Course = courseVariant
            };
            return viewModel;
        }
    }
}
