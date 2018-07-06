﻿using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using GovUk.Education.ManageCourses.ApiClient;
using GovUk.Education.ManageCourses.Ui.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using GovUk.Education.ManageCourses.Ui.Helpers;
using SmartBreadcrumbs;

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
        [Route("{accreditingProviderId=self}/{courseTitle}/{ucasCode}")]
        //[Breadcrumb("Course", FromAction = "Courses.Index")]
        public async Task<IActionResult> Variants(string accreditingProviderId, string courseTitle, string ucasCode, string organisationId, int organisationCount)
        {
            var course = await _manageApi.GetCoursesByOrganisation(organisationId);

            var providerCourse = "self".Equals(accreditingProviderId, StringComparison.InvariantCultureIgnoreCase)
                ? course.ProviderCourses.First(c => String.IsNullOrEmpty(c.AccreditingProviderId))
                : course.ProviderCourses
                    .First(c => c.AccreditingProviderId.Equals(accreditingProviderId, StringComparison.InvariantCultureIgnoreCase));

            var courseDetail = providerCourse.CourseDetails.First(x => x.CourseTitle.Equals(courseTitle, StringComparison.InvariantCultureIgnoreCase));
            var variant = courseDetail.Variants.FirstOrDefault(v => v.UcasCode == ucasCode);
            if (variant == null) return null;

            var subjects = variant.Subjects.Count() > 0 ? variant.Subjects.Aggregate((current, next) => current + ", " + next) : "";

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
                    Qualifications= variant.ProfPostFlag,
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

            var viewModel = new FromUcasViewModel//TODO change view model to show on course variant
            {
                OrganisationName = course.OrganisationName,
                OrganisationId = course.OrganisationId,
                OrganisationCount = organisationCount,
                CourseTitle = courseDetail.CourseTitle,
                AccreditingProviderId = providerCourse.AccreditingProviderId,
                Course = courseVariant
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
                    Type = x.GetCourseVariantType(),
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
