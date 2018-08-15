using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using GovUk.Education.ManageCourses.ApiClient;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;

namespace GovUk.Education.ManageCourses.Ui
{

    public class ManageCoursesApiClientConfiguration : IManageCoursesApiClientConfiguration
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        public ManageCoursesApiClientConfiguration(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<string> GetAccessTokenAsync()
        {
            var claims = (_httpContextAccessor.HttpContext.User.Identity as ClaimsIdentity).Claims;
            var accessToken = claims.FirstOrDefault(x => x.Type =="access_token")?.Value ?? "";
            return accessToken;
        }
    }
}
