using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using GovUk.Education.ManageCourses.Api.Model;
using GovUk.Education.ManageCourses.Domain.Models;
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

        [Route("/organisations")]
        public async Task<IActionResult> Index()
        {
            var orgs = await _manageApi.GetProviderSummaries();
            var model = new OrganisationListViewModel
            {
                ProviderSummaries = orgs
            };
            return View(model);
        }

        [Route("{providerCode}")]
        public async Task<IActionResult> Show(string providerCode)
        {
            var ucasProviderEnrichmentGetModel = await _manageApi.GetProviderEnrichment(providerCode);
            var providerCourses = await _manageApi.GetCoursesOfProvider(providerCode);
            var summary = await _manageApi.GetProviderSummary(providerCode);
            var multipleOrganisations = (await _manageApi.GetProviderSummaries()).Count() > 1;

            var status = ucasProviderEnrichmentGetModel?.Status.ToString() ?? "Empty";

            var model = new CourseListViewModel
            {
                ProviderName = summary.ProviderName,
                ProviderCode = summary.ProviderCode,
                MultipleOrganisations = multipleOrganisations,
            };

            return View(model);
        }

        [HttpGet]
        [Route("{providerCode}/details")]
        public async Task<IActionResult> Details(string providerCode)
        {
            var ucasProviderEnrichmentGetModel = await _manageApi.GetProviderEnrichment(providerCode);

            ucasProviderEnrichmentGetModel = ucasProviderEnrichmentGetModel ?? new UcasProviderEnrichmentGetModel { EnrichmentModel = new ProviderEnrichmentModel() { AccreditingProviderEnrichments = new List<AccreditingProviderEnrichment>() } };

            var enrichmentModel = ucasProviderEnrichmentGetModel.EnrichmentModel;
            var aboutAccreditingTrainingProviders = await MergeTrainingProviderViewModels(providerCode, enrichmentModel);
            var providerSummary = await _manageApi.GetProviderSummary(providerCode);

            var model = OrganisationViewModel.FromEnrichmentModel(
                ucasProviderEnrichmentGetModel, aboutAccreditingTrainingProviders, providerSummary);

            return View(model);
        }

        [HttpPost]
        [Route("{providerCode}/details")]
        public async Task<ActionResult> DetailsPost(string providerCode, OrganisationViewModel model)
        {
            var ucasProviderEnrichmentGetModel = await _manageApi.GetProviderEnrichment(providerCode);
            var result = await PublishOrgansation(ucasProviderEnrichmentGetModel, providerCode);
            return result;
        }

        [HttpGet]
        [Route("{providerCode}/about")]
        public async Task<IActionResult> About(string providerCode)
        {
            var ucasProviderEnrichmentGetModel = await _manageApi.GetProviderEnrichment(providerCode);

            ucasProviderEnrichmentGetModel = ucasProviderEnrichmentGetModel ?? new UcasProviderEnrichmentGetModel { EnrichmentModel = new ProviderEnrichmentModel() { AccreditingProviderEnrichments = new List<AccreditingProviderEnrichment>() } };

            var enrichmentModel = ucasProviderEnrichmentGetModel.EnrichmentModel;
            var aboutAccreditingTrainingProviders = await MergeTrainingProviderViewModels(providerCode, enrichmentModel);
            var providerSummary = await _manageApi.GetProviderSummary(providerCode);

            var model = OrganisationViewModel.FromEnrichmentModel(ucasProviderEnrichmentGetModel, aboutAccreditingTrainingProviders, providerSummary);

            var aboutModel = OrganisationViewModelForAbout.FromGeneralViewModel(model);

            return View(aboutModel);
        }

        [HttpPost]
        [Route("{providerCode}/about")]
        public async Task<ActionResult> AboutPost(string providerCode, OrganisationViewModelForAbout model)
        {
            var ucasProviderEnrichmentGetModel = await _manageApi.GetProviderEnrichment(providerCode);
            var enrichmentModel = ucasProviderEnrichmentGetModel?.EnrichmentModel;

            var aboutAccreditingTrainingProviders = await MergeTrainingProviderViewModels(providerCode, enrichmentModel, model?.AboutTrainingProviders);
            model.AboutTrainingProviders = aboutAccreditingTrainingProviders;

            ValidateAboutModel(model);

            if (!ModelState.IsValid)
            {
                model.ProviderName = (await _manageApi.GetProviderSummaries()).FirstOrDefault(x => x.ProviderCode == providerCode.ToUpperInvariant())?.ProviderName;
                return View("About", model);
            }

            if (enrichmentModel == null && model.IsEmpty())
            {
                // Draft state is "New" and no changes have been made - don't insert a draft
                return RedirectToAction("Details", "Organisation", new { providerCode = providerCode });
            }

            model.MergeIntoEnrichmentModel(ref enrichmentModel);

            var result = await SaveValidatedOrgansation(enrichmentModel, providerCode);
            return result;
        }

        // TODO: Not currently visible to the public, add edit link on details view
        [HttpGet]
        [Route("{providerCode}/contact")]
        public async Task<IActionResult> Contact(string providerCode)
        {
            var ucasProviderEnrichmentGetModel = await _manageApi.GetProviderEnrichment(providerCode);

            ucasProviderEnrichmentGetModel = ucasProviderEnrichmentGetModel ?? new UcasProviderEnrichmentGetModel { EnrichmentModel = new ProviderEnrichmentModel() { AccreditingProviderEnrichments = new List<AccreditingProviderEnrichment>() } };

            var enrichmentModel = ucasProviderEnrichmentGetModel.EnrichmentModel;
            var providerSummary = await _manageApi.GetProviderSummary(providerCode);

            var model = OrganisationViewModel.FromEnrichmentModel(ucasProviderEnrichmentGetModel, null, providerSummary);

            var contactModel = OrganisationViewModelForContact.FromGeneralViewModel(model);

            return View(contactModel);
        }

        // TODO: Ensure this actually persists the contact data correctly
        [HttpPost]
        [Route("{providerCode}/contact")]
        public async Task<ActionResult> ContactPost(string providerCode, OrganisationViewModelForContact model)
        {
            var providerEnrichmentGetModel = await _manageApi.GetProviderEnrichment(providerCode);
            var enrichmentModel = providerEnrichmentGetModel?.EnrichmentModel;

            ValidateContactModel(model);

            if (!ModelState.IsValid)
            {
                return View("Contact", model);
            }

            if (enrichmentModel == null && model.IsEmpty())
            {
                // Draft state is "New" and no changes have been made - don't insert a draft
                return RedirectToAction("Details", "Organisation", new { providerCode = providerCode });
            }

            model.MergeIntoEnrichmentModel(ref enrichmentModel);

            var result = await SaveValidatedOrgansation(enrichmentModel, providerCode);
            return result;
        }

        [HttpGet]
        [Route("{providerCode}/request-access")]
        public ViewResult RequestAccess(string providerCode)
        {
            ViewBag.ProviderCode = providerCode;
            return View(new RequestAccessViewModel() );
        }

        [HttpPost]
        [Route("{providerCode}/request-access")]
        public async Task<ActionResult> RequestAccessPost(string providerCode, RequestAccessViewModel model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.ProviderCode = providerCode;
                return View("RequestAccess", model);
            }

            await _manageApi.CreateAccessRequest(new Api.Model.AccessRequest()
            {
                FirstName = model.FirstName,
                LastName = model.LastName,
                EmailAddress = model.EmailAddress,
                Organisation = model.Organisation,
                Reason = model.Reason,
            });

            this.TempData.Add("RequestAccess_To_Name", $"{model.FirstName} {model.LastName}");

            return new RedirectToActionResult("Show", "Organisation", new { providerCode = providerCode });
        }

        private async Task<List<TrainingProviderViewModel>> MergeTrainingProviderViewModels(string providerCode, ProviderEnrichmentModel enrichmentModel, IEnumerable<TrainingProviderViewModel> fromModel = null)
        {
            var ucasData = await _manageApi.GetCoursesOfProvider(providerCode);

            var accreditingProviders = ucasData
                .Where(x =>
                    false == string.Equals(x.AccreditingProvider?.ProviderCode, providerCode, StringComparison.InvariantCultureIgnoreCase) &&
                    false == string.IsNullOrWhiteSpace(x.AccreditingProvider?.ProviderCode))
                .Distinct(new AccreditingProviderCodeComparer()).ToList();
            var accreditingProviderEnrichments = enrichmentModel?.AccreditingProviderEnrichments ?? new List<AccreditingProviderEnrichment>();

            var result = accreditingProviders.Select(x =>

                    {
                        var description = accreditingProviderEnrichments.FirstOrDefault(TrainingProviderMatchesProviderCourse(x))?.Description ?? "";

                        var aboutTrainingProviders = (fromModel ?? new List<TrainingProviderViewModel>());

                        var newAccreditingProviderModel = aboutTrainingProviders.FirstOrDefault(
                            atp => (atp.ProviderCode.Equals(x.AccreditingProvider?.ProviderCode, StringComparison.InvariantCultureIgnoreCase)));

                        if (newAccreditingProviderModel != null)
                        {
                            // empty descriptions get bound as `null` by ASP.NET MVC model binding
                            description = newAccreditingProviderModel.Description ?? "";
                        }

                        var tpvm = new TrainingProviderViewModel()
                        {
                            ProviderName = x.AccreditingProvider?.ProviderName,
                            ProviderCode = x.AccreditingProvider?.ProviderCode,
                            Description = description
                        };

                        return tpvm;
                    })

                .ToList();

            return result;
        }

        private async Task<ActionResult> PublishOrgansation(UcasProviderEnrichmentGetModel ucasProviderEnrichmentGetModel, string providerCode)
        {
            var providerSummary = await _manageApi.GetProviderSummary(providerCode);

            var enrichmentModel = ucasProviderEnrichmentGetModel.EnrichmentModel;
            var aboutAccreditingTrainingProviders = await MergeTrainingProviderViewModels(providerCode, enrichmentModel);
            var model = OrganisationViewModel.FromEnrichmentModel(ucasProviderEnrichmentGetModel, aboutAccreditingTrainingProviders, providerSummary);

            ValidateModelForPublication(model);

            if (!ModelState.IsValid)
            {
                return View("Details", model);
            }
            else
            {
                var result = await _manageApi.PublishAllCoursesOfProviderToSearchAndCompare(providerCode);

                if (result)
                {

                    TempData["MessageType"] = "success";
                    TempData["MessageTitle"] = "Your changes have been published – this content will appear on all of your course pages";

                    return RedirectToAction("Details", new { providerCode = providerCode });
                }
                else
                {
                    // This is a no ops, there should not be any viable valid reason that api rejects, hence dead end.
                    throw new InvalidOperationException("attempting to publish nonexistent organisation enrichment: " + providerCode);
                }
            }
        }
        private async Task<ActionResult> SaveValidatedOrgansation(ProviderEnrichmentModel enrichmentModel, string providerCode)
        {
            var postModel = new UcasProviderEnrichmentPostModel { EnrichmentModel = enrichmentModel };

            await _manageApi.SaveProviderEnrichment(providerCode, postModel);

            TempData["MessageType"] = "success";
            TempData["MessageTitle"] = "Your changes have been saved";
            TempData["MessageBodyHtml"] = "<p class=\"govuk-body\">Preview any course to see how it will look to applicants.</p>";
            return RedirectToAction("Details", "Organisation", new { providerCode = providerCode });
        }

        private static Func<AccreditingProviderEnrichment, bool> TrainingProviderMatchesProviderCourse(Domain.Models.Course x)
        {
            return y => String.Equals(x.AccreditingProvider?.ProviderCode,
                y.UcasProviderCode, StringComparison.InvariantCultureIgnoreCase);
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
