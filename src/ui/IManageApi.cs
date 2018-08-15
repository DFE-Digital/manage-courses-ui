using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Threading.Tasks;
using GovUk.Education.ManageCourses.ApiClient;


namespace GovUk.Education.ManageCourses.Ui
{
    public interface IManageApi
    {
        Task<InstitutionCourses> GetCoursesByOrganisation(string ucasCode);
        Task<Course> GetCourseByUcasCode(string instCode, string ucasCode);
        Task<IEnumerable<UserOrganisation>> GetOrganisations();
        Task<UserOrganisation> GetOrganisation(string instCode);
        Task LogAccessRequest(AccessRequest accessRequest);
        Task  SaveEnrichmentOrganisation(string institutionCode, UcasInstitutionEnrichmentPostModel organisation);
        Task<UcasInstitutionEnrichmentGetModel> GetEnrichmentOrganisation(string ucasCode);
    }
}
