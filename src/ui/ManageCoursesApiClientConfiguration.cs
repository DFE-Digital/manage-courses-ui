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
            return await _httpContextAccessor.HttpContext.GetTokenAsync("access_token");
        }
    }
}
