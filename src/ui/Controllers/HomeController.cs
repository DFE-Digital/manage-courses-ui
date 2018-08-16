using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using GovUk.Education.ManageCourses.ApiClient;
using GovUk.Education.ManageCourses.Ui.ViewModels;
using GovUk.Education.ManageCourses.Ui;
using System.Collections.Generic;

namespace GovUk.Education.ManageCourses.Ui.Controllers
{
    public class HomeController : CommonAttributesControllerBase
    {
        private readonly IManageApi _manageApi;

        public HomeController(IManageApi manageApi)
        {
            _manageApi = manageApi;
        }

        // GET: Home
        [Authorize]
        public async Task<ActionResult> Index()
        {            
            IEnumerable<UserOrganisation> orgs;
            
            try
            {
                orgs = await _manageApi.GetOrganisations();
            }
            catch (SwaggerException e)
            {
                if (e.StatusCode == 401)
                {
                    return StatusCode(401);
                }
                else
                {
                    throw;
                }
            }

            var userOrganisations = orgs.ToList();
            if (userOrganisations.Count() == 1)
            {
                return this.RedirectToAction("Courses", "Organisation", new { ucasCode = userOrganisations[0].UcasCode });
            }

            if (userOrganisations.Count() > 1)
            {
                return this.RedirectToAction("Index", "Organisations");
            }

            return StatusCode(401);
        }
    }
}
