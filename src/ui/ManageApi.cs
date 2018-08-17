using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

        public async Task<UserOrganisation> GetOrganisation(string instCode)
        {
            try
            {
                var courses = await _apiClient.Organisations_GetAsync(instCode);
                return courses;
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to get courses data from " + _apiClient.BaseUrl, ex);
            }
        }

        public async Task<InstitutionCourses> GetCoursesByOrganisation(string instCode)
        {
            try
            {
                var courses = await _apiClient.Courses_GetAllAsync(instCode);
                return courses;
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to get courses data from " + _apiClient.BaseUrl, ex);
            }
        }
        public async Task<Course> GetCourseByUcasCode(string instCode, string ucasCode)
        {
            try
            {
                var course = await _apiClient.Courses_GetAsync(instCode, ucasCode);
                return course;
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to get course data from " + _apiClient.BaseUrl, ex);
            }
        }

        public async Task<IEnumerable<UserOrganisation>> GetOrganisations()
        {
            var orgs = await _apiClient.Organisations_GetAllAsync();
            return orgs;
        }

        public async Task LogAccessRequest(AccessRequest accessRequest)
        {
            await _apiClient.AccessRequest_IndexAsync(accessRequest);
        }

        public async Task SaveEnrichmentOrganisation(string institutionCode, UcasInstitutionEnrichmentPostModel organisation)
        {
            await _apiClient.Enrichment_SaveInstitutionAsync(institutionCode, organisation);
        }

        public async Task<UcasInstitutionEnrichmentGetModel> GetEnrichmentOrganisation(string ucasCode)
        {
            var result = await _apiClient.Enrichment_GetInstitutionAsync(ucasCode);

            result = result ?? new UcasInstitutionEnrichmentGetModel { EnrichmentModel = new InstitutionEnrichmentModel() { AccreditingProviderEnrichments = new ObservableCollection<AccreditingProviderEnrichment>() } };

            return result;
        }

        public async Task<bool> PublishEnrichmentOrganisation(string ucasCode)
        {
            var result = await _apiClient.Enrichment_PublishInstitutionAsync(ucasCode);

            return result;
        }
    }
}
