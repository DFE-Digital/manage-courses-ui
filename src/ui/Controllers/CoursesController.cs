using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using GovUk.Education.ManageCourses.ApiClient;
using GovUk.Education.ManageCourses.Ui.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GovUk.Education.ManageCourses.Ui.Controllers
{
    [Authorize]
    [Route("organisation")]
    public class CoursesController : CommonAttributesControllerBase
    {
        private readonly IManageApi _manageApi;

        public CoursesController(IManageApi manageApi)
        {
            _manageApi = manageApi;
        }

        [Route("{ucasCode}/courses")]        
        public async Task<IActionResult> Index(string ucasCode)
        {
            var courses = await _manageApi.GetCoursesByOrganisation(ucasCode);
            var orgs = await _manageApi.GetOrganisations();
           
            var model = new CourseListViewModel
            {
                Courses = courses,
                TotalCount = courses.ProviderCourses.SelectMany(x => x.CourseDetails).SelectMany(v => v.Variants).Count(),
                MultipleOrganisations = orgs.Count() > 1
            };
            return View(model);
        }
    }
}
