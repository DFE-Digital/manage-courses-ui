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
        Task<OrganisationCourses> GetCoursesByOrganisation(string ucasCode);
        Task<IEnumerable<UserOrganisation>> GetOrganisations();
        Task LogAccessRequest(AccessRequest accessRequest);
        Task<EnrichmentOrganisationModel>  SaveEnrichmentOrganisation(EnrichmentOrganisationModel organisation);
        Task<EnrichmentOrganisationModel> GetEnrichmentOrganisation(string ucasCode);
    }
}
