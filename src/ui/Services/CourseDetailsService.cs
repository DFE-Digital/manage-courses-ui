using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using GovUk.Education.ManageCourses.ApiClient;
using GovUk.Education.SearchAndCompare.Domain.Models;
using GovUk.Education.SearchAndCompare.Domain.Models.Enums;
using GovUk.Education.SearchAndCompare.Domain.Models.Joins;
using GovUk.Education.SearchAndCompare.UI.Shared.Services;

namespace GovUk.Education.ManageCourses.Ui.Services
{
    public class CourseDetailsService : ICourseDetailsService
    {
        private readonly IManageApi api;
        private readonly ICourseMapper courseMapper;

        public CourseDetailsService(IManageApi api, ICourseMapper courseMapper)
        {
            this.api = api;
            this.courseMapper = courseMapper;
        }

        public SearchAndCompare.Domain.Models.Course GetCourse(string providerCode, string courseCode)
        {
            var ucasInstData = api.GetUcasInstitution(providerCode).Result;
            var ucasCourseData = api.GetCourseByUcasCode(providerCode, courseCode).Result;
            var orgEnrichmentData = api.GetEnrichmentOrganisation(providerCode).Result;
            var courseEnrichmentData = api.GetEnrichmentCourse(providerCode, courseCode).Result;

            return courseMapper.MapToSearchAndCompareCourse(
                ucasInstData,
                ucasCourseData,
                orgEnrichmentData?.EnrichmentModel,
                courseEnrichmentData?.EnrichmentModel);
        }

        public List<FeeCaps> GetFeeCaps()
        {
            // Todo: Is this still needed?
            return new List<FeeCaps> {new FeeCaps()};
        }
    }
}