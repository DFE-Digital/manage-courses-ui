using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.RegularExpressions;
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
        private readonly IFeatureFlags _featureFlags;

        public OrganisationController(IManageApi manageApi, IFeatureFlags featureFlags)
        {
            _manageApi = manageApi;
            _featureFlags = featureFlags;
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

        [HttpGet]
        [Route("{ucasCode}/about")]
        public async Task<IActionResult> About(string ucasCode)
        {

            if (!_featureFlags.ShowOrgEnrichment)
            {
                return RedirectToAction("Courses", new { ucasCode = ucasCode });
            }

            var ucasInstitutionEnrichmentGetModel = await _manageApi.GetEnrichmentOrganisation(ucasCode);

            var tabViewModel = await GetTabViewModelAsync(ucasCode, "about");

            var enrichmentModel = ucasInstitutionEnrichmentGetModel.EnrichmentModel;
            var aboutAccreditingTrainingProviders = await GetTrainingProviderViewModels(ucasCode, enrichmentModel);

            var model = GetOrganisationViewModel(ucasCode, ucasInstitutionEnrichmentGetModel);

            model.TabViewModel = tabViewModel;
            model.AboutTrainingProviders = aboutAccreditingTrainingProviders;

            return View(model);
        }

        [HttpPost]
        [Route("{ucasCode}/about")]
        public async Task<ActionResult> AboutPost(string ucasCode, OrganisationViewModel model)
        {
            var ucasInstitutionEnrichmentGetModel = await _manageApi.GetEnrichmentOrganisation(ucasCode);

            if (model.PublishOrganisation)
            {
                var result = await PublishOrgansation(ucasInstitutionEnrichmentGetModel, model, ucasCode);
                return result;
            }
            else
            {
                var result = await SaveOrgansation(ucasInstitutionEnrichmentGetModel, model, ucasCode);
                return result;
            }
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

            return new RedirectToActionResult("RequestAccess", "Organisation", new { ucasCode });
        }

        private async Task<List<TrainingProviderViewModel>> GetTrainingProviderViewModels(string ucasCode, InstitutionEnrichmentModel enrichmentModel, OrganisationViewModel model = null)
        {
            var ucasData = await _manageApi.GetCoursesByOrganisation(ucasCode);

            var accreditingProviders = ucasData.Courses
                .Where(x =>
                    false == string.Equals(x.AccreditingProviderId, ucasCode, StringComparison.InvariantCultureIgnoreCase) &&
                    false == string.IsNullOrWhiteSpace(x.AccreditingProviderId))
                .Distinct(new AccreditingProviderIdComparer()).ToList();
            var accreditingProviderEnrichments = enrichmentModel.AccreditingProviderEnrichments;

            var result = accreditingProviders.Select(x =>

                    {
                        var description = accreditingProviderEnrichments.FirstOrDefault(TrainingProviderMatchesProviderCourse(x))?.Description ?? "";

                        if (model != null)
                        {

                            var aboutTrainingProviders = (model.AboutTrainingProviders ?? new List<TrainingProviderViewModel>());

                            description = aboutTrainingProviders.FirstOrDefault(
                                atp =>(atp.InstitutionCode.Equals(x.AccreditingProviderId, StringComparison.InvariantCultureIgnoreCase)))?.Description ?? description;
                        }

                        var tpvm = new TrainingProviderViewModel()
                        {
                            InstitutionName = x.AccreditingProviderName,
                            InstitutionCode = x.AccreditingProviderId,
                            Description = description
                        };

                        return tpvm;
                    })

                .ToList();

            return result;
        }

        private OrganisationViewModel GetOrganisationViewModel(string ucasCode, UcasInstitutionEnrichmentGetModel ucasInstitutionEnrichmentGetModel)
        {
            var enrichmentModel = ucasInstitutionEnrichmentGetModel.EnrichmentModel;

            var result = new OrganisationViewModel
            {
                InstitutionCode = ucasCode,
                TrainWithUs = enrichmentModel.TrainWithUs,
                TrainWithDisability = enrichmentModel.TrainWithDisability,
                LastPublishedTimestampUtc = ucasInstitutionEnrichmentGetModel.LastPublishedTimestampUtc,
                Status = ucasInstitutionEnrichmentGetModel.Status,
                PublishOrganisation = false
            };

            return result;
        }

        private async Task<ActionResult> PublishOrgansation(UcasInstitutionEnrichmentGetModel ucasInstitutionEnrichmentGetModel, OrganisationViewModel model, string ucasCode)
        {
            model = GetOrganisationViewModel(ucasCode, ucasInstitutionEnrichmentGetModel);

            var enrichmentModel = ucasInstitutionEnrichmentGetModel.EnrichmentModel;
            var aboutAccreditingTrainingProviders = await GetTrainingProviderViewModels(ucasCode, enrichmentModel, model);

            model.AboutTrainingProviders = aboutAccreditingTrainingProviders;

            // The model that is sent is always going to invalid as it is always empty
            // Therefore fetch from api and map it across to validate it.

            var wordCountValidationOnly = false;
            ValidateModel(model, wordCountValidationOnly);

            if (!ModelState.IsValid)
            {
                var tabViewModel = await GetTabViewModelAsync(ucasCode, "about");
                model.TabViewModel = tabViewModel;

                return View("about", model);
            }
            else
            {
                var result = await _manageApi.PublishEnrichmentOrganisation(ucasCode);

                if (result)
                {
                    return RedirectToAction("About", new { ucasCode });
                }
                else
                {
                    // This is a no ops, there should not be any viable valid reason that api rejects, hence dead end.
                    return RedirectToAction("Index", "Error", new { statusCode = 500 });
                }
            }
        }
        private async Task<ActionResult> SaveOrgansation(UcasInstitutionEnrichmentGetModel ucasInstitutionEnrichmentGetModel, OrganisationViewModel model, string ucasCode)
        {

            var enrichmentModel = ucasInstitutionEnrichmentGetModel.EnrichmentModel;
            var aboutAccreditingTrainingProviders = await GetTrainingProviderViewModels(ucasCode, enrichmentModel, model);

            model.AboutTrainingProviders = aboutAccreditingTrainingProviders;

            // The validation that needs to occurs are word count only on saving
            var wordCountValidationOnly = true;
            ValidateModel(model, wordCountValidationOnly);

            if (!ModelState.IsValid)
            {
                var tabViewModel = await GetTabViewModelAsync(ucasCode, "about");
                model.TabViewModel = tabViewModel;

                return View("about", model);
            }
            else
            {
                var aboutTrainingProviders = new ObservableCollection<AccreditingProviderEnrichment>(
                    model.AboutTrainingProviders.Select(x => new AccreditingProviderEnrichment
                    {
                        UcasInstitutionCode = x.InstitutionCode,
                            Description = x.Description
                    }));

                enrichmentModel.TrainWithUs = model.TrainWithUs;
                enrichmentModel.AccreditingProviderEnrichments = aboutTrainingProviders;
                enrichmentModel.TrainWithDisability = model.TrainWithDisability;

                var postModel = new UcasInstitutionEnrichmentPostModel { EnrichmentModel = enrichmentModel };

                await _manageApi.SaveEnrichmentOrganisation(ucasCode, postModel);

                TempData["MessageType"] = "success";
                TempData["MessageTitle"] = "Your changes have been saved";

                return RedirectToAction("About", "Organisation", new { ucasCode });
            }
        }

        private static Func<AccreditingProviderEnrichment, bool> TrainingProviderMatchesProviderCourse(Course x)
        {
            return y => String.Equals(x.AccreditingProviderId,
                y.UcasInstitutionCode, StringComparison.InvariantCultureIgnoreCase);
        }

        private List<Provider> GetProviders(InstitutionCourses institutionCourses)
        {
            var providerNames = institutionCourses.Courses.Select(x => x.AccreditingProviderName).Distinct().ToList();
            var providers = new List<Provider>();
            foreach (var providerName in providerNames)
            {
                var providerCourses = institutionCourses.Courses.Where(x => x.AccreditingProviderName == providerName).ToList();
                var providerId = providerCourses.Select(x => x.AccreditingProviderId).Distinct().SingleOrDefault(); //should all be the same

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

        private async Task<TabViewModel> GetTabViewModelAsync(string ucasCode, string currentTab)
        {
            var orgs = await _manageApi.GetOrganisations();
            var organisationName = orgs.FirstOrDefault(o => ucasCode.Equals(o.UcasCode, StringComparison.InvariantCultureIgnoreCase))?.OrganisationName;
            var result = new TabViewModel
            {
                CurrentTab = currentTab,
                MultipleOrganisations = orgs.Count() > 1,
                OrganisationName = organisationName,
                UcasCode = ucasCode,
                ShowAboutTab = _featureFlags.ShowOrgEnrichment
            };

            return result;
        }

        public void ValidateModel(OrganisationViewModel model, bool wordCountValidationOnly)
        {
            ModelState.Clear();

            if (wordCountValidationOnly)
            {
                var wordCountValidationModel = new WordCountOrganisationViewModel(model);
                TryValidateModel(wordCountValidationModel);
            }
            else
            {
                TryValidateModel(model);
            }

            for (int i = 0; i < model.AboutTrainingProviders.Count; i++)
            {
                var trainingProvider = model.AboutTrainingProviders[i];

                if (!string.IsNullOrWhiteSpace(trainingProvider.Description))
                {
                    var regex = new Regex(trainingProvider.ValidationRegex, RegexOptions.Compiled);

                    var match = regex.Match(trainingProvider.Description);

                    if (!match.Success)
                    {
                        var key = $"AboutTrainingProviders_{i}__Description";
                        ModelState.AddModelError(key, trainingProvider.ValidationMessage);
                    }
                }
            }
        }
    }
}
