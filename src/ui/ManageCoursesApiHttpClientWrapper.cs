using GovUk.Education.ManageCourses.ApiClient;
using Microsoft.AspNetCore.Http;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;

namespace GovUk.Education.ManageCourses.Ui
{

    public class ManageCoursesApiHttpClientWrapper : HttpClientWrapper
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ManageCoursesApiHttpClientWrapper(IHttpContextAccessor httpContextAccessor, HttpClient httpClient) : base(httpClient)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public override string GetAccessToken()
        {
            var claims = (_httpContextAccessor.HttpContext.User.Identity as ClaimsIdentity).Claims;
            var accessToken = claims.FirstOrDefault(x => x.Type =="access_token")?.Value ?? "";
            return accessToken;
        }
    }
}
