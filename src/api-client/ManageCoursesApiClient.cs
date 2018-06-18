using System.Net.Http.Headers;

namespace GovUk.Education.ManageCourses.ApiClient
{
    public partial class ManageCoursesApiClient
    {
        partial void PrepareRequest(System.Net.Http.HttpClient client, System.Net.Http.HttpRequestMessage request, string url)
        {
            var accessToken = ApiClientConfiguration.GetAccessTokenAsync().Result;
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
        }
    }
}
