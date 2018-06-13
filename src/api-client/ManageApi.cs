using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GovUk.Education.ManageCourses.ApiClient.Generated;

namespace GovUk.Education.ManageCourses.ApiClient
{
    public class ManageApi
    {
        private readonly ManageCoursesApiClient _apiClient;

        public ManageApi(ManageCoursesApiClient apiClient)
        {
            _apiClient = apiClient;
        }

        public async Task<IEnumerable<Course>> GetCourses()
        {
            var courses = await _apiClient.ExportAsync();
            return courses;
        }

        public async Task<Course> GetCourse(string ucasCode)
        {
            // todo: expand api to allow fetching single course
            var courses = await _apiClient.ExportAsync();
            return courses.Single(c => c.UcasCode == ucasCode);
        }
    }
}
