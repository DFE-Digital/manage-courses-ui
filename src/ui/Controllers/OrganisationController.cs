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
            this._featureFlags = featureFlags;
        }

        [Route("/organisations")]
        public async Task<IActionResult> Index()
        {
            var orgs = await _manageApi.GetOrganisations();
            var model = new OrganisationListViewModel
            {
                Oganisations = orgs
            };
            return View(model);
        }

        [Route("{ucasCode}")]
        public async Task<IActionResult> Show(string ucasCode)
        {
            var ucasInstitutionEnrichmentGetModel = await _manageApi.GetEnrichmentOrganisation(ucasCode);
            var institutionCourses = await _manageApi.GetCoursesByOrganisation(ucasCode);
            var multipleOrganisations = (await _manageApi.GetOrganisations()).Count() > 1;
            var providers = GetProviders(institutionCourses);

            var status = ucasInstitutionEnrichmentGetModel?.Status.ToString() ?? "Empty";

            var model = new CourseListViewModel
            {
                InstitutionName = institutionCourses.InstitutionName,
                InstitutionId = institutionCourses.InstitutionCode,
                Providers = providers,
                MultipleOrganisations = multipleOrganisations,
                Status = status
            };

            return View(model);
        }

        [HttpGet]
        [Route("{ucasCode}/about")]
        public async Task<IActionResult> About(string ucasCode)
        {
            var ucasInstitutionEnrichmentGetModel = await _manageApi.GetEnrichmentOrganisation(ucasCode);

            ucasInstitutionEnrichmentGetModel = ucasInstitutionEnrichmentGetModel ?? new UcasInstitutionEnrichmentGetModel { EnrichmentModel = new InstitutionEnrichmentModel() { AccreditingProviderEnrichments = new ObservableCollection<AccreditingProviderEnrichment>() } };

            var enrichmentModel = ucasInstitutionEnrichmentGetModel.EnrichmentModel;
            var aboutAccreditingTrainingProviders = await GetTrainingProviderViewModels(ucasCode, enrichmentModel);

            var model = GetOrganisationViewModel(ucasCode, ucasInstitutionEnrichmentGetModel);
            model.InstitutionName = (await _manageApi.GetOrganisations()).FirstOrDefault(x => x.UcasCode == ucasCode.ToUpperInvariant())?.OrganisationName;;

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
            ViewBag.UcasCode = ucasCode;
            return View(new RequestAccessViewModel() );
        }

        [HttpPost]
        [Route("{ucasCode}/request-access")]
        public async Task<ActionResult> RequestAccessPost(string ucasCode, RequestAccessViewModel model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.UcasCode = ucasCode;
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

            return new RedirectToActionResult("Show", "Organisation", new { ucasCode });
        }

        private async Task<List<TrainingProviderViewModel>> GetTrainingProviderViewModels(string ucasCode, InstitutionEnrichmentModel enrichmentModel, OrganisationViewModel model = null)
        {
            var ucasData = await _manageApi.GetCoursesByOrganisation(ucasCode);

            var accreditingProviders = ucasData.Courses
                .Where(x =>
                    false == string.Equals(x.AccreditingProviderId, ucasCode, StringComparison.InvariantCultureIgnoreCase) &&
                    false == string.IsNullOrWhiteSpace(x.AccreditingProviderId))
                .Distinct(new AccreditingProviderIdComparer()).ToList();
            var accreditingProviderEnrichments = enrichmentModel?.AccreditingProviderEnrichments ?? new ObservableCollection<AccreditingProviderEnrichment>();

            var result = accreditingProviders.Select(x =>

                    {
                        var description = accreditingProviderEnrichments.FirstOrDefault(TrainingProviderMatchesProviderCourse(x))?.Description ?? "";

                        if (model != null)
                        {

                            var aboutTrainingProviders = (model.AboutTrainingProviders ?? new List<TrainingProviderViewModel>());

                            description = aboutTrainingProviders.FirstOrDefault(
                                atp => (atp.InstitutionCode.Equals(x.AccreditingProviderId, StringComparison.InvariantCultureIgnoreCase)))?.Description ?? description;
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
                PublishOrganisation = false,
                AllowPreview = _featureFlags.ShowCoursePreview
            };

            return result;
        }

        private async Task<ActionResult> PublishOrgansation(UcasInstitutionEnrichmentGetModel ucasInstitutionEnrichmentGetModel, OrganisationViewModel model, string ucasCode)
        {
            ucasInstitutionEnrichmentGetModel = ucasInstitutionEnrichmentGetModel ?? new UcasInstitutionEnrichmentGetModel { EnrichmentModel = new InstitutionEnrichmentModel() { AccreditingProviderEnrichments = new ObservableCollection<AccreditingProviderEnrichment>() } };

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
                model.InstitutionName = (await _manageApi.GetOrganisations()).FirstOrDefault(x => x.UcasCode == ucasCode.ToUpperInvariant())?.OrganisationName;

                return View("about", model);
            }
            else
            {
                var result = await _manageApi.PublishEnrichmentOrganisation(ucasCode);

                if (result)
                {

                    TempData["MessageType"] = "success";
                    TempData["MessageTitle"] = "Your changes have been published";
                    TempData["MessageBodyHtml"] = "<p class=\"govuk-body\">Applicants will see this on all your courses from October.</p>";

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

            var enrichmentModel = ucasInstitutionEnrichmentGetModel?.EnrichmentModel;
            var aboutAccreditingTrainingProviders = await GetTrainingProviderViewModels(ucasCode, enrichmentModel, model);
            
            model.AboutTrainingProviders = aboutAccreditingTrainingProviders;

            // The validation that needs to occurs are word count only on saving
            var wordCountValidationOnly = true;
            ValidateModel(model, wordCountValidationOnly);

            if (!ModelState.IsValid)
            {
                model.InstitutionName = (await _manageApi.GetOrganisations()).FirstOrDefault(x => x.UcasCode == ucasCode.ToUpperInvariant())?.OrganisationName;;

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

                if (enrichmentModel == null)
                {
                    var editIsEmpty = string.IsNullOrEmpty(model.TrainWithUs)
                        && string.IsNullOrEmpty(model.TrainWithDisability)
                        && !model.AboutTrainingProviders.Any(x => !string.IsNullOrEmpty(x.Description));

                    if (editIsEmpty)
                    {
                        // Draft state is "New" and no changes have been made - don't insert a draft
                        return RedirectToAction("About", "Organisation", new { ucasCode });
                    }
                    enrichmentModel = new InstitutionEnrichmentModel();
                }

                enrichmentModel.TrainWithUs = model.TrainWithUs;
                enrichmentModel.AccreditingProviderEnrichments = aboutTrainingProviders;
                enrichmentModel.TrainWithDisability = model.TrainWithDisability;

                var postModel = new UcasInstitutionEnrichmentPostModel { EnrichmentModel = enrichmentModel };

                await _manageApi.SaveEnrichmentOrganisation(ucasCode, postModel);

                TempData["MessageType"] = "success";
                TempData["MessageTitle"] = "Your changes have been saved";
                if (_featureFlags.ShowCoursePreview)
                {
                    TempData["MessageBodyHtml"] = "<p class=\"govuk-body\">Preview any course to see how it will look to applicants.</p>";
                }

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
            var uniqueAccreditingProviderIds = institutionCourses.Courses.Select(c => c.AccreditingProviderId).Distinct();
            var providers = new List<Provider>();
            foreach (var uniqueAccreditingProviderId in uniqueAccreditingProviderIds)
            {
                var name = institutionCourses.Courses.First(c => c.AccreditingProviderId == uniqueAccreditingProviderId)
                    .AccreditingProviderName;
                var courses = institutionCourses.Courses.Where(c => c.AccreditingProviderId == uniqueAccreditingProviderId).ToList();
                providers.Add(new Provider
                {
                    ProviderId = uniqueAccreditingProviderId,
                    ProviderName = name,
                    Courses = courses,
                    TotalCount = courses.Count,
                });
            }
            return providers;
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
