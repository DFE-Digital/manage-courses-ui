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
        [Route("{ucasCode}/details")]
        public async Task<IActionResult> Details(string ucasCode)
        {
            var ucasInstitutionEnrichmentGetModel = await _manageApi.GetEnrichmentOrganisation(ucasCode);

            ucasInstitutionEnrichmentGetModel = ucasInstitutionEnrichmentGetModel ?? new UcasInstitutionEnrichmentGetModel { EnrichmentModel = new InstitutionEnrichmentModel() { AccreditingProviderEnrichments = new ObservableCollection<AccreditingProviderEnrichment>() } };

            var enrichmentModel = ucasInstitutionEnrichmentGetModel.EnrichmentModel;
            var aboutAccreditingTrainingProviders = await MergeTrainingProviderViewModels(ucasCode, enrichmentModel);
            var ucasInsitution = await _manageApi.GetUcasInstitution(ucasCode);

            var model = OrganisationViewModel.FromEnrichmentModel(
                ucasInstitutionEnrichmentGetModel, aboutAccreditingTrainingProviders, ucasInsitution);

            return View(model);
        }

        [HttpPost]
        [Route("{ucasCode}/details")]
        public async Task<ActionResult> DetailsPost(string ucasCode, OrganisationViewModel model)
        {
            var ucasInstitutionEnrichmentGetModel = await _manageApi.GetEnrichmentOrganisation(ucasCode);
            var result = await PublishOrgansation(ucasInstitutionEnrichmentGetModel, ucasCode);
            return result;
        }

        [HttpGet]
        [Route("{ucasCode}/about")]
        public async Task<IActionResult> About(string ucasCode)
        {
            var ucasInstitutionEnrichmentGetModel = await _manageApi.GetEnrichmentOrganisation(ucasCode);

            ucasInstitutionEnrichmentGetModel = ucasInstitutionEnrichmentGetModel ?? new UcasInstitutionEnrichmentGetModel { EnrichmentModel = new InstitutionEnrichmentModel() { AccreditingProviderEnrichments = new ObservableCollection<AccreditingProviderEnrichment>() } };

            var enrichmentModel = ucasInstitutionEnrichmentGetModel.EnrichmentModel;
            var aboutAccreditingTrainingProviders = await MergeTrainingProviderViewModels(ucasCode, enrichmentModel);
            var ucasInsitution = await _manageApi.GetUcasInstitution(ucasCode);

            var model = OrganisationViewModel.FromEnrichmentModel(ucasInstitutionEnrichmentGetModel, aboutAccreditingTrainingProviders, ucasInsitution);

            var aboutModel = OrganisationViewModelForAbout.FromGeneralViewModel(model);

            return View(aboutModel);
        }

        [HttpPost]
        [Route("{ucasCode}/about")]
        public async Task<ActionResult> AboutPost(string ucasCode, OrganisationViewModelForAbout model)
        {
            var ucasInstitutionEnrichmentGetModel = await _manageApi.GetEnrichmentOrganisation(ucasCode);
            var enrichmentModel = ucasInstitutionEnrichmentGetModel?.EnrichmentModel;

            var aboutAccreditingTrainingProviders = await MergeTrainingProviderViewModels(ucasCode, enrichmentModel, model?.AboutTrainingProviders);
            model.AboutTrainingProviders = aboutAccreditingTrainingProviders;

            ValidateAboutModel(model);

            if (!ModelState.IsValid)
            {
                model.InstitutionName = (await _manageApi.GetOrganisations()).FirstOrDefault(x => x.UcasCode == ucasCode.ToUpperInvariant())?.OrganisationName;
                return View("About", model);
            }

            if (enrichmentModel == null && model.IsEmpty())
            {
                // Draft state is "New" and no changes have been made - don't insert a draft
                return RedirectToAction("Details", "Organisation", new { ucasCode });                
            }

            model.MergeIntoEnrichmentModel(ref enrichmentModel);
            
            var result = await SaveValidatedOrgansation(enrichmentModel, ucasCode);
            return result;
        }

        // TODO: Not currently visible to the public, add edit link on details view
        [HttpGet]
        [Route("{ucasCode}/contact")]
        public async Task<IActionResult> Contact(string ucasCode)
        {
            var ucasInstitutionEnrichmentGetModel = await _manageApi.GetEnrichmentOrganisation(ucasCode);

            ucasInstitutionEnrichmentGetModel = ucasInstitutionEnrichmentGetModel ?? new UcasInstitutionEnrichmentGetModel { EnrichmentModel = new InstitutionEnrichmentModel() { AccreditingProviderEnrichments = new ObservableCollection<AccreditingProviderEnrichment>() } };

            var enrichmentModel = ucasInstitutionEnrichmentGetModel.EnrichmentModel;
            var ucasInsitution = await _manageApi.GetUcasInstitution(ucasCode);

            var model = OrganisationViewModel.FromEnrichmentModel(ucasInstitutionEnrichmentGetModel, null, ucasInsitution);

            var contactModel = OrganisationViewModelForContact.FromGeneralViewModel(model);

            return View(contactModel);
        }

        // TODO: Ensure this actually persists the contact data correctly
        [HttpPost]
        [Route("{ucasCode}/contact")]
        public async Task<ActionResult> ContactPost(string ucasCode, OrganisationViewModelForContact model)
        {
            var ucasInstitutionEnrichmentGetModel = await _manageApi.GetEnrichmentOrganisation(ucasCode);
            var enrichmentModel = ucasInstitutionEnrichmentGetModel?.EnrichmentModel;

            ValidateContactModel(model);

            if (!ModelState.IsValid)
            {
                return View("Contact", model);
            }

            if (enrichmentModel == null && model.IsEmpty())
            {
                // Draft state is "New" and no changes have been made - don't insert a draft
                return RedirectToAction("Details", "Organisation", new { ucasCode });                
            }

            model.MergeIntoEnrichmentModel(ref enrichmentModel);
            
            var result = await SaveValidatedOrgansation(enrichmentModel, ucasCode);
            return result;
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

        private async Task<List<TrainingProviderViewModel>> MergeTrainingProviderViewModels(string ucasCode, InstitutionEnrichmentModel enrichmentModel, IEnumerable<TrainingProviderViewModel> fromModel = null)
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

                        var aboutTrainingProviders = (fromModel ?? new List<TrainingProviderViewModel>());

                        description = aboutTrainingProviders.FirstOrDefault(
                            atp => (atp.InstitutionCode.Equals(x.AccreditingProviderId, StringComparison.InvariantCultureIgnoreCase)))?.Description ?? description;
                        
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

        private async Task<ActionResult> PublishOrgansation(UcasInstitutionEnrichmentGetModel ucasInstitutionEnrichmentGetModel, string ucasCode)
        {
            var ucasInsitution = await _manageApi.GetUcasInstitution(ucasCode);

            var enrichmentModel = ucasInstitutionEnrichmentGetModel.EnrichmentModel;
            var aboutAccreditingTrainingProviders = await MergeTrainingProviderViewModels(ucasCode, enrichmentModel);
            var model = OrganisationViewModel.FromEnrichmentModel(ucasInstitutionEnrichmentGetModel, aboutAccreditingTrainingProviders, ucasInsitution);

            ValidateModelForPublication(model);

            if (!ModelState.IsValid)
            {
                return View("Details", model);
            }
            else
            {
                var result = await _manageApi.PublishEnrichmentOrganisation(ucasCode);

                if (result)
                {

                    TempData["MessageType"] = "success";
                    TempData["MessageTitle"] = "Your changes have been published";
                    TempData["MessageBodyHtml"] = "<p class=\"govuk-body\">Applicants will see this on all your courses from October.</p>";

                    return RedirectToAction("Details", new { ucasCode });
                }
                else
                {
                    // This is a no ops, there should not be any viable valid reason that api rejects, hence dead end.
                    throw new InvalidOperationException("attempting to publish nonexistent organisation enrichment: " + ucasCode);
                }
            }
        }
        private async Task<ActionResult> SaveValidatedOrgansation(InstitutionEnrichmentModel enrichmentModel, string ucasCode)
        {      
            var postModel = new UcasInstitutionEnrichmentPostModel { EnrichmentModel = enrichmentModel };

            await _manageApi.SaveEnrichmentOrganisation(ucasCode, postModel);

            TempData["MessageType"] = "success";
            TempData["MessageTitle"] = "Your changes have been saved";
            TempData["MessageBodyHtml"] = "<p class=\"govuk-body\">Preview any course to see how it will look to applicants.</p>";
            return RedirectToAction("Details", "Organisation", new { ucasCode });
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

        public void ValidateContactModel(OrganisationViewModelForContact model)
        {
            ModelState.Clear();
            TryValidateModel(model);
        }

        public void ValidateAboutModel(OrganisationViewModelForAbout model)
        {
            ModelState.Clear();
            TryValidateModel(model);
            ValidateAboutAccreditedTrainingProviders(model.AboutTrainingProviders);
        }

        public void ValidateModelForPublication(OrganisationViewModel model)
        {
            ModelState.Clear();
            TryValidateModel(model);
            ValidateAboutAccreditedTrainingProviders(model.AboutTrainingProviders);
        }

        private void ValidateAboutAccreditedTrainingProviders(List<TrainingProviderViewModel> aboutAccreditingTrainingProviders)
        {
            for (int i = 0; i < aboutAccreditingTrainingProviders.Count; i++)
            {
                var trainingProvider = aboutAccreditingTrainingProviders[i];

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
