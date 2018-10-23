using System.Collections.Generic;
using System.Threading.Tasks;
using GovUk.Education.ManageCourses.ApiClient;
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
        public async Task<UserOrganisation> GetOrganisation(string instCode)
        {
            var courses = await _apiClient.Organisations_GetAsync(instCode);
            return courses;
        }

        public async Task<UcasInstitution> GetUcasInstitution(string instCode)
        {
            var inst = await _apiClient.Organisations_GetUcasInstitutionAsync(instCode);
            return inst;
        }

        public async Task<InstitutionCourses> GetCoursesByOrganisation(string instCode)
        {
            var courses = await _apiClient.Courses_GetAllAsync(instCode);
            return courses;
        }
        public async Task<Course> GetCourseByUcasCode(string instCode, string courseCode)
        {
            var course = await _apiClient.Courses_GetAsync(instCode, courseCode);
            return course;
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

        public async Task LogAcceptTerms()
        {
            await _apiClient.AcceptTerms_IndexAsync();
        }

        public async Task SaveEnrichmentOrganisation(string instCode, UcasInstitutionEnrichmentPostModel organisation)
        {
            await _apiClient.Enrichment_SaveInstitutionAsync(instCode, organisation);
        }

        public async Task<UcasInstitutionEnrichmentGetModel> GetEnrichmentOrganisation(string instCode)
        {
            var result = await _apiClient.Enrichment_GetInstitutionAsync(instCode);

            return result;
        }

        public async Task<bool> PublishCoursesToSearchAndCompare(string instCode)
        {
            var result = await _apiClient.Publish_PublishCoursesToSearchAndCompareAsync(instCode);

            return result;
        }

        public async Task<UcasCourseEnrichmentGetModel> GetEnrichmentCourse(string instCode, string courseCode)
        {
            var result = await _apiClient.Enrichment_GetCourseAsync(instCode, courseCode);

            return result;
        }
        public async Task SaveEnrichmentCourse(string instCode, string courseCode, CourseEnrichmentModel course)
        {
            await _apiClient.Enrichment_SaveCourseAsync(instCode, courseCode, course);
        }

        public async Task<SearchAndCompare.Domain.Models.Course> GetSearchAndCompareCourse(string instCode, string courseCode)
        {
            var result = await _apiClient.Publish_GetSearchAndCompareCourseAsync(instCode, courseCode);
            //the result is an identical obect to the SearchAnCompareCourse that we want only it's an ApiClient version of it
            //so we need to serialize/deserialize in order to get the required object to return
            var jsonCourse = JsonConvert.SerializeObject(result, _jsonSerializerSettings);
            SearchAndCompare.Domain.Models.Course deserializedCourse = JsonConvert.DeserializeObject<SearchAndCompare.Domain.Models.Course>(jsonCourse);

            return deserializedCourse;
        }


        public async Task<bool> PublishCourseToSearchAndCompare(string instCode, string courseCode)
        {
            var result = await _apiClient.Publish_PublishCourseToSearchAndCompareAsync(instCode, courseCode);

            return result;
        }
    }
}
