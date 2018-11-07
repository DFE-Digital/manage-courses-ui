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
        Task<List<Domain.Models.Course>> GetCoursesOfProvider(string instCode);
        Task<Domain.Models.Course> GetCourse(string instCode, string courseCode);
        Task<IEnumerable<ProviderSummary>> GetProviderSummaries();
        Task<ProviderSummary> GetProviderSummary(string instCode);
        Task CreateAccessRequest(AccessRequest accessRequest);
        Task LogAcceptTerms();
        Task SaveProviderEnrichment(string instCode, UcasProviderEnrichmentPostModel organisation);
        Task<UcasProviderEnrichmentGetModel> GetProviderEnrichment(string instCode);
        Task<bool> PublishAllCoursesOfProviderToSearchAndCompare(string instCode);
        Task<UcasCourseEnrichmentGetModel> GetCourseEnrichment(string instCode, string courseCode);
        Task SaveCourseEnrichment(string instCode, string courseCode, CourseEnrichmentModel course);
        Task<SearchAndCompare.Domain.Models.Course> GetSearchAndCompareCourse(string instCode, string courseCode);
        Task<bool> PublishCourseToSearchAndCompare(string instCode, string courseCode);
    }
}
