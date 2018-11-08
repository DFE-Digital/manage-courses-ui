using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Threading.Tasks;
using GovUk.Education.ManageCourses.Api.Model;
using GovUk.Education.ManageCourses.Ui.ViewModels;

namespace GovUk.Education.ManageCourses.Ui
{
    public interface IManageApi
    {
        Task<List<Domain.Models.Course>> GetCoursesOfProvider(string providerCode);
        Task<Domain.Models.Course> GetCourse(string providerCode, string courseCode);
        Task<IEnumerable<ProviderSummary>> GetProviderSummaries();
        Task<ProviderSummary> GetProviderSummary(string providerCode);
        Task CreateAccessRequest(AccessRequest accessRequest);
        Task LogAcceptTerms();
        Task SaveProviderEnrichment(string providerCode, UcasProviderEnrichmentPostModel organisation);
        Task<UcasProviderEnrichmentGetModel> GetProviderEnrichment(string providerCode);
        Task<bool> PublishAllCoursesOfProviderToSearchAndCompare(string providerCode);
        Task<UcasCourseEnrichmentGetModel> GetCourseEnrichment(string providerCode, string courseCode);
        Task SaveCourseEnrichment(string providerCode, string courseCode, CourseEnrichmentModel course);
        Task<SearchAndCompare.Domain.Models.Course> GetSearchAndCompareCourse(string providerCode, string courseCode);
        Task<bool> PublishCourseToSearchAndCompare(string providerCode, string courseCode);
    }
}
