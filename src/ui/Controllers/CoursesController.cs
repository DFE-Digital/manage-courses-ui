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
    [Route("courses")]
    public class CoursesController : CommonAttributesControllerBase
    {
        private readonly ManageApi _manageApi;

        public CoursesController(ManageApi manageApi)
        {
            _manageApi = manageApi;
        }

        [Route("{organisationId}")]        
        public async Task<IActionResult> Index(string organisationId)
        {
            var courses = await _manageApi.GetCoursesByOrganisation(organisationId);
            var orgs = await _manageApi.GetOrganisations();

            var data = await _manageApi.GetOrganisationCoursesTotal(organisationId);
            var model = new CourseListViewModel
            {
                Courses = courses,
                TotalCount = data.TotalCount,
                MultipleOrganisations = orgs.Count() > 1
            };
            return View("Index", model);
        }
    }
}
