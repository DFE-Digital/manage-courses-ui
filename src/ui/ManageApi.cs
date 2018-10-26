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
        public async Task<InstitutionSummary> GetInstitutionSummary(string instCode)
        {
            var courses = await _apiClient.Organisations_GetAsync(instCode);
            return courses;
        }

        public async Task<List<Course>> GetCoursesOfInstitution(string instCode)
        {
            var courses = await _apiClient.Courses_GetAllAsync(instCode);
            return courses.ToList();
        }
        public async Task<Domain.Models.Course> GetCourse(string instCode, string courseCode)
        {
            var course = await _apiClient.Courses_GetAsync(instCode, courseCode);
            return course;
        }

        public async Task<IEnumerable<InstitutionSummary>> GetInstitutionSummaries()
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

        public async Task SaveInstitutionEnrichment(string instCode, UcasInstitutionEnrichmentPostModel organisation)
        {
            await _apiClient.Enrichment_SaveInstitutionAsync(instCode, organisation);
        }

        public async Task<UcasInstitutionEnrichmentGetModel> GetInstitutitionEnrichment(string instCode)
        {
            var result = await _apiClient.Enrichment_GetInstitutionAsync(instCode);

            return result;
        }

        public async Task<bool> PublishAllCoursesOfInstitutionToSearchAndCompare(string instCode)
        {
            var result = await _apiClient.Publish_PublishCoursesToSearchAndCompareAsync(instCode);

            return result;
        }

        public async Task<UcasCourseEnrichmentGetModel> GetCourseEnrichment(string instCode, string courseCode)
        {
            var result = await _apiClient.Enrichment_GetCourseAsync(instCode, courseCode);

            return result;
        }
        public async Task SaveCourseEnrichment(string instCode, string courseCode, CourseEnrichmentModel course)
        {
            await _apiClient.Enrichment_SaveCourseAsync(instCode, courseCode, course);
        }

        public async Task<SearchAndCompare.Domain.Models.Course> GetSearchAndCompareCourse(string instCode, string courseCode)
        {
            var result = await _apiClient.Publish_GetSearchAndCompareCourseAsync(instCode, courseCode);
            // //the result is an identical obect to the SearchAnCompareCourse that we want only it's an ApiClient version of it
            // //so we need to serialize/deserialize in order to get the required object to return
            // var jsonCourse = JsonConvert.SerializeObject(result, _jsonSerializerSettings);
            // SearchAndCompare.Domain.Models.Course deserializedCourse = JsonConvert.DeserializeObject<SearchAndCompare.Domain.Models.Course>(jsonCourse);

            return result;
        }


        public async Task<bool> PublishCourseToSearchAndCompare(string instCode, string courseCode)
        {
            var result = await _apiClient.Publish_PublishCourseToSearchAndCompareAsync(instCode, courseCode);

            return result;
        }
    }
}
