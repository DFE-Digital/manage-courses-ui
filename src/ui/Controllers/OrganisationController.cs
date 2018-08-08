using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GovUk.Education.ManageCourses.Ui;
using GovUk.Education.ManageCourses.Ui.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using GovUk.Education.ManageCourses.Ui.Helpers;
using GovUk.Education.ManageCourses.ApiClient;

namespace GovUk.Education.ManageCourses.Ui.Controllers
{
    [Authorize]
    [Route("[controller]")]
    public class OrganisationController : CommonAttributesControllerBase
    {
        private readonly IManageApi _manageApi;

        public OrganisationController(IManageApi manageApi)
        {
            _manageApi = manageApi;
        }

        [Route("{ucasCode}/courses")]
        public async Task<IActionResult> Courses(string ucasCode)
        {
            var institutionCourses = await _manageApi.GetCoursesByOrganisation(ucasCode);
            var tabViewModel = await GetTabViewModelAsync(ucasCode, "courses");
            var providers = GetProviders(institutionCourses);

            var model = new CourseListViewModel
            {
                InstitutionName = institutionCourses.InstitutionName,
                InstitutionId = institutionCourses.InstitutionCode,
                Providers = providers,
                TabViewModel = tabViewModel
            };

            return View(model);
        }

        private List<Provider> GetProviders(InstitutionCourses institutionCourses)
        {
            var providerNames = institutionCourses.Courses.Select(x => x.AccreditingProviderName).Distinct().ToList();
            var providers = new List<Provider>();
            foreach (var providerName in providerNames)
            {
                var providerCourses = institutionCourses.Courses.Where(x => x.AccreditingProviderName == providerName).ToList();
                var providerId = providerCourses.Select(x => x.AccreditingProviderId).Distinct().SingleOrDefault();//should all be the same

                var provider = new Provider
                {
                    ProviderId = providerId,
                    ProviderName = providerName,
                    Courses = providerCourses,
                    TotalCount = providerCourses.Count
                };
                providers.Add(provider);
            }

            return providers;
        }
        private async Task<TabViewModel> GetTabViewModelAsync(string ucasCode, string currentTab) {
            var orgs = await _manageApi.GetOrganisations();
            var organisationName = orgs.FirstOrDefault(o => ucasCode.Equals(o.UcasCode, StringComparison.InvariantCultureIgnoreCase))?.OrganisationName;
            var result = new TabViewModel
            {
                CurrentTab = currentTab,
                MultipleOrganisations = orgs.Count() > 1,
                OrganisationName = organisationName,
                UcasCode = ucasCode
            };

            return result;
        }

        [HttpGet]
        [Route("{ucasCode}/request-access")]
        public async Task<ViewResult> RequestAccess(string ucasCode)
        {
            var tabViewModel = await GetTabViewModelAsync(ucasCode, "request-access");
            var model = new RequestAccessViewModel { TabViewModel = tabViewModel };

            return View(model);
        }

        [HttpPost]
        [Route("{ucasCode}/request-access")]
        public async Task<ActionResult> RequestAccessPost(string ucasCode, RequestAccessViewModel model)
        {
            if (!ModelState.IsValid)
            {
                var tabViewModel = await GetTabViewModelAsync(ucasCode, "request-access");
                model.TabViewModel = tabViewModel;
                return View("RequestAccess", model);
            }

            await _manageApi.LogAccessRequest(new AccessRequest()
            {
                FirstName = model.FirstName,
                LastName = model.LastName,
                EmailAddress = model.EmailAddress,
                Organisation = model.Organisation,
                Reason = model.Reason,
            });

            this.TempData.Add("RequestAccess_To_Name", $"{model.FirstName} {model.LastName}");

            return new RedirectToActionResult("RequestAccess","Organisation", new {ucasCode });
        }
    }
}
