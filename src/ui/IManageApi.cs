using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Threading.Tasks;
using GovUk.Education.ManageCourses.ApiClient;
using GovUk.Education.ManageCourses.Ui.ViewModels;

namespace GovUk.Education.ManageCourses.Ui
{
    public interface IManageApi
    {
        Task<InstitutionCourses> GetCoursesByOrganisation(string instCode);
        Task<Course> GetCourseByUcasCode(string instCode, string courseCode);
        Task<IEnumerable<UserOrganisation>> GetOrganisations();
        Task<UserOrganisation> GetOrganisation(string instCode);
        Task<UcasInstitution> GetUcasInstitution(string instCode);
        Task LogAccessRequest(AccessRequest accessRequest);
        Task LogAcceptTerms();
        Task SaveEnrichmentOrganisation(string instCode, UcasInstitutionEnrichmentPostModel organisation);
        Task<UcasInstitutionEnrichmentGetModel> GetEnrichmentOrganisation(string instCode);
        Task<bool> PublishCoursesToSearchAndCompare(string instCode);
        Task<UcasCourseEnrichmentGetModel> GetEnrichmentCourse(string instCode, string courseCode);
        Task SaveEnrichmentCourse(string instCode, string courseCode, CourseEnrichmentModel course);
        Task<SearchAndCompare.Domain.Models.Course> GetSearchAndCompareCourse(string instCode, string courseCode);
        Task<bool> PublishCourseToSearchAndCompare(string instCode, string courseCode);
    }
}
