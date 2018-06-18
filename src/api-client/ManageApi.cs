using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GovUk.Education.ManageCourses.ApiClient.Generated;
using Microsoft.Extensions.Configuration;

namespace GovUk.Education.ManageCourses.ApiClient
{
    public class ManageApi
    {
        private readonly ManageCoursesApiClient _apiClient;

        public ManageApi(IConfiguration configuration)
        {
            const string key = "ApiConnection:Url";
            var baseUrl = configuration[key];
            if (string.IsNullOrWhiteSpace(baseUrl))
            {
                throw new Exception("Missing configuration for " + key);
            }
            _apiClient = new ManageCoursesApiClient
            {
                BaseUrl = baseUrl
            };
        }

        public async Task<IEnumerable<Course>> GetCourses()
        {
            try
            {

                var courses = await _apiClient.ExportAsync();
                return courses;
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to get course data from " + _apiClient.BaseUrl, ex);
            }
        }

        public async Task<Course> GetCourse(string ucasCode)
        {
            // todo: expand api to allow fetching single course
            var courses = await _apiClient.ExportAsync();
            // todo: don't use first once we have course-folding in place
            return courses.First(c => c.UcasCode == ucasCode);
        }
    }
}
