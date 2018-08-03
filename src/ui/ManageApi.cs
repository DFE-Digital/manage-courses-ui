using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Threading.Tasks;
using GovUk.Education.ManageCourses.ApiClient;

namespace GovUk.Education.ManageCourses.Ui
{
    public class ManageApi : IManageApi
    {
        private readonly ManageCoursesApiClient _apiClient;

        public ManageApi(ManageCoursesApiClient apiClient)
        {
            _apiClient = apiClient;
        }

        public async Task<OrganisationCourses> GetCoursesByOrganisation(string ucasCode)
        {
            try
            {
                var courses = await _apiClient.Data_ExportByOrganisationAsync(ucasCode);
                return courses;
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to get course data from " + _apiClient.BaseUrl, ex);
            }
        }

        public async Task<IEnumerable<UserOrganisation>> GetOrganisations()
        {
            var orgs = await _apiClient.Organisations_GetAsync();
            return orgs;
        }

        public async Task LogAccessRequest(AccessRequest accessRequest)
        {
            await _apiClient.AccessRequest_IndexAsync(accessRequest);
        }

        public async Task SaveEnrichmentOrganisation(EnrichmentOrganisationModel organisation)
        {
            return;
        }

        public async Task<EnrichmentOrganisationModel> GetEnrichmentOrganisation(string ucasCode)
        {
            var result = await _apiClient.Enrichment_GetOrganisationAsync(ucasCode);
            return result;
        }
    }
}
