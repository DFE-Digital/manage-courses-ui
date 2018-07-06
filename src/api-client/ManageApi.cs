using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Dynamic;

namespace GovUk.Education.ManageCourses.ApiClient
{
    public class ManageApi
    {
        private readonly ManageCoursesApiClient _apiClient;

        public ManageApi(ManageCoursesApiClient apiClient)
        {
            _apiClient = apiClient;
        }

        public async Task<OrganisationCourses> GetCourses()
        {
            try
            {
                var courses = await _apiClient.Data_ExportAsync();
                return courses;
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to get course data from " + _apiClient.BaseUrl, ex);
            }
        }

        public async Task<dynamic> GetOrganisationCoursesTotal()
        {
            // todo: await _apiClient.GetOrganisationCoursesTotal()
            var courses = await _apiClient.Data_ExportAsync();
            dynamic organisationCoursesTotal = new ExpandoObject();
            
            organisationCoursesTotal.OrganisationName = courses.OrganisationName;

            organisationCoursesTotal.TotalCount = courses.ProviderCourses.SelectMany( x =>x .CourseDetails).Count();

            return organisationCoursesTotal;
        }

        public async Task<CourseDetail> GetCourse(string accreditingProviderId, string courseTitle)
        {
            // todo: expand api to allow fetching single course
            var courses = await _apiClient.Data_ExportAsync();
            // todo: don't use first once we have course-folding in place
            //return courses.First(c => c.UcasCode == ucasCode);
            return courses.ProviderCourses
                .First(c => c.AccreditingProviderId.Equals(accreditingProviderId, StringComparison.InvariantCultureIgnoreCase))
                .CourseDetails.First(x => x.CourseTitle.Equals(courseTitle, StringComparison.InvariantCultureIgnoreCase));
        }

        public async Task LogAccessRequest(AccessRequest accessRequest) 
        {
            await _apiClient.AccessRequest_IndexAsync(accessRequest);
        } 
    }
}
