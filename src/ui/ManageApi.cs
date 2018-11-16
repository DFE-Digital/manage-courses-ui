using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GovUk.Education.ManageCourses.ApiClient;
using GovUk.Education.ManageCourses.Api.Model;

using GovUk.Education.ManageCourses.Ui.Helpers;
using Newtonsoft.Json;

namespace GovUk.Education.ManageCourses.Ui
{
    public class ManageApi : IManageApi
    {
        private readonly ManageCoursesApiClient _apiClient;
        private static JsonSerializerSettings _jsonSerializerSettings;

        public ManageApi(ManageCoursesApiClient apiClient)
        {
            _apiClient = apiClient;
            _jsonSerializerSettings = new JsonSerializerSettings
            {
                MissingMemberHandling = MissingMemberHandling.Ignore,
                NullValueHandling = NullValueHandling.Ignore
            };
        }

        // Do not handled any exception let it thro as it should be handled by McExceptionFilter or startup configuration.
        public async Task<ProviderSummary> GetProviderSummary(string providerCode)
        {
            var courses = await _apiClient.Organisations_GetAsync(providerCode);
            return courses;
        }

        public async Task<List<Domain.Models.Course>> GetCoursesOfProvider(string providerCode)
        {
            var courses = await _apiClient.Courses_GetAllAsync(providerCode);
            return courses.ToList();
        }
        public async Task<Domain.Models.Course> GetCourse(string providerCode, string courseCode)
        {
            var course = await _apiClient.Courses_GetAsync(providerCode, courseCode);
            return course;
        }

        public async Task<IEnumerable<ProviderSummary>> GetProviderSummaries()
        {
            var orgs = await _apiClient.Organisations_GetAllAsync();
            return orgs;
        }

        public async Task CreateAccessRequest(AccessRequest accessRequest)
        {
            await _apiClient.AccessRequest_IndexAsync(accessRequest);
        }

        public async Task LogAcceptTerms()
        {
            await _apiClient.AcceptTerms_IndexAsync();
        }

        public async Task SaveProviderEnrichment(string providerCode, UcasProviderEnrichmentPostModel organisation)
        {
            await _apiClient.Enrichment_SaveProviderAsync(providerCode, organisation);
        }

        public async Task<UcasProviderEnrichmentGetModel> GetProviderEnrichment(string providerCode)
        {
            var result = await _apiClient.Enrichment_GetProviderAsync(providerCode);

            return result;
        }

        public async Task<bool> PublishAllCoursesOfProviderToSearchAndCompare(string providerCode)
        {
            var result = await _apiClient.Publish_PublishCoursesToSearchAndCompareAsync(providerCode);

            return result;
        }

        public async Task<UcasCourseEnrichmentGetModel> GetCourseEnrichment(string providerCode, string courseCode)
        {
            var result = await _apiClient.Enrichment_GetCourseAsync(providerCode, courseCode);

            return result;
        }
        public async Task SaveCourseEnrichment(string providerCode, string courseCode, CourseEnrichmentModel course)
        {
            await _apiClient.Enrichment_SaveCourseAsync(providerCode, courseCode, course);
        }

        public async Task<SearchAndCompare.Domain.Models.Course> GetSearchAndCompareCourse(string providerCode, string courseCode)
        {
            var result = await _apiClient.Publish_GetSearchAndCompareCourseAsync(providerCode, courseCode);
            
            return result;
        }

        public async Task<bool> PublishCourseToSearchAndCompare(string providerCode, string courseCode)
        {
            var result = await _apiClient.Publish_PublishCourseToSearchAndCompareAsync(providerCode, courseCode);

            return result;
        }
    }
}
