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
        Task<InstitutionCourses> GetCoursesOfInstitution(string instCode);
        Task<Domain.Models.Course> GetCourse(string instCode, string courseCode);
        Task<IEnumerable<UserOrganisation>> GetInstitutionSummaries();
        Task<UserOrganisation> GetInstitutionSummary(string instCode);
        Task CreateAccessRequest(AccessRequest accessRequest);
        Task LogAcceptTerms();
        Task SaveInstitutionEnrichment(string instCode, UcasInstitutionEnrichmentPostModel organisation);
        Task<UcasInstitutionEnrichmentGetModel> GetInstitutitionEnrichment(string instCode);
        Task<bool> PublishAllCoursesOfInstitutionToSearchAndCompare(string instCode);
        Task<UcasCourseEnrichmentGetModel> GetCourseEnrichment(string instCode, string courseCode);
        Task SaveCourseEnrichment(string instCode, string courseCode, CourseEnrichmentModel course);
        Task<SearchAndCompare.Domain.Models.Course> GetSearchAndCompareCourse(string instCode, string courseCode);
        Task<bool> PublishCourseToSearchAndCompare(string instCode, string courseCode);
    }
}
