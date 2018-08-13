using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using GovUk.Education.ManageCourses.ApiClient;
using GovUk.Education.ManageCourses.Ui;
using GovUk.Education.ManageCourses.Ui.Helpers;
using GovUk.Education.ManageCourses.Ui.Utilities;
using GovUk.Education.ManageCourses.Ui.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

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
            var courses = await _manageApi.GetCoursesByOrganisation(ucasCode);
            var tabViewModel = await GetTabViewModelAsync(ucasCode, "courses");
            var variants = courses.ProviderCourses
                .SelectMany(pc => pc.CourseDetails)
                .SelectMany(cd => cd.Variants);

            var model = new CourseListViewModel
            {
                Courses = courses,
                TotalCount = variants.Count(),
                TabViewModel = tabViewModel
            };

            return View(model);
        }

        private async Task<TabViewModel> GetTabViewModelAsync(string ucasCode, string currentTab)
        {
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
        [Route("{ucasCode}/about")]
        public async Task<ViewResult> About(string ucasCode)
        {
            var ucasData = (await _manageApi.GetCoursesByOrganisation(ucasCode));
            var organisation = await _manageApi.GetEnrichmentOrganisation(ucasCode);

            var aboutAccreditingTrainingProviders = ucasData.ProviderCourses
                .Where(x =>
                    false == string.Equals(x.AccreditingProviderId, ucasCode, StringComparison.InvariantCultureIgnoreCase) &&
                    false == string.IsNullOrWhiteSpace(x.AccreditingProviderId))
                .Distinct(new ProviderCourseIdComparer())
                .Select(x => new TrainingProviderViewModel()
                {
                    InstitutionName = x.AccreditingProviderName,
                    InstitutionCode = x.AccreditingProviderId,
                    Description = organisation.Content.AboutTrainingProviders.FirstOrDefault(TrainingProviderMatchesProviderCourse(x))?.Description ?? ""
                }).ToList();

            var tabViewModel = await GetTabViewModelAsync(ucasCode, "about");

            var model = new OrganisationViewModel
            {
                InstitutionCode = organisation.InstitutionCode,
                TabViewModel = tabViewModel,
                TrainWithUs = organisation.Content.TrainWithUs,
                AboutTrainingProviders = aboutAccreditingTrainingProviders,
                TrainWithDisability = organisation.Content.TrainWithDisability
            };

            return View(model);
        }

        private static Func<TrainingProvider, bool> TrainingProviderMatchesProviderCourse(ProviderCourse x)
        {
            return y => String.Equals(x.AccreditingProviderId, y.InstitutionCode, StringComparison.InvariantCultureIgnoreCase);
        }

        [HttpPost]
        [Route("{ucasCode}/about")]
        public async Task<ActionResult> AboutPost(string ucasCode, OrganisationViewModel model)
        {
            var organisationContent = (await _manageApi.GetEnrichmentOrganisation(ucasCode)).Content;

            var aboutTrainingProviders = new ObservableCollection<TrainingProvider>(
                model.AboutTrainingProviders.Select(x => new TrainingProvider
                {
                    InstitutionCode = x.InstitutionCode,
                    Description = x.Description
                }));            

            organisationContent.TrainWithUs = model.TrainWithUs;
            organisationContent.AboutTrainingProviders = aboutTrainingProviders;
            organisationContent.TrainWithDisability = model.TrainWithDisability;

            await _manageApi.SaveEnrichmentOrganisation(ucasCode, organisationContent);

            return new RedirectToActionResult("About", "Organisation", new { ucasCode });
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
