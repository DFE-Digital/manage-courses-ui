using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Threading.Tasks;

namespace GovUk.Education.ManageCourses.ApiClient
{
    public class ManageApi
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
        public async Task<dynamic> GetOrganisationCoursesTotal(string ucasCode)
        {
            var courses = await _apiClient.Data_ExportByOrganisationAsync(ucasCode);
            dynamic organisationCoursesTotal = new ExpandoObject();

            organisationCoursesTotal.OrganisationName = courses.OrganisationName;

            organisationCoursesTotal.TotalCount = courses.ProviderCourses.SelectMany(x => x.CourseDetails.FirstOrDefault()?.Variants).Count();

            return organisationCoursesTotal;
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
    }
}
