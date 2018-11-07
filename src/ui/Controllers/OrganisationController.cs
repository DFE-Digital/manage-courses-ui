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

        [Route("{instCode}")]
        public async Task<IActionResult> Show(string instCode)
        {
            var ucasProviderEnrichmentGetModel = await _manageApi.GetProviderEnrichment(instCode);
            var providerCourses = await _manageApi.GetCoursesOfProvider(instCode);
            var summary = await _manageApi.GetProviderSummary(instCode);
            var multipleOrganisations = (await _manageApi.GetProviderSummaries()).Count() > 1;
            var providers = GetProviders(providerCourses);

            var status = ucasProviderEnrichmentGetModel?.Status.ToString() ?? "Empty";

            var model = new CourseListViewModel
            {
                InstName = summary.ProviderName,
                InstCode = summary.ProviderCode,
                Providers = providers,
                MultipleOrganisations = multipleOrganisations,
                Status = status
            };

            return View(model);
        }

        [HttpGet]
        [Route("{instCode}/details")]
        public async Task<IActionResult> Details(string instCode)
        {
            var ucasProviderEnrichmentGetModel = await _manageApi.GetProviderEnrichment(instCode);

            ucasProviderEnrichmentGetModel = ucasProviderEnrichmentGetModel ?? new UcasProviderEnrichmentGetModel { EnrichmentModel = new ProviderEnrichmentModel() { AccreditingProviderEnrichments = new List<AccreditingProviderEnrichment>() } };

            var enrichmentModel = ucasProviderEnrichmentGetModel.EnrichmentModel;
            var aboutAccreditingTrainingProviders = await MergeTrainingProviderViewModels(instCode, enrichmentModel);
            var providerSummary = await _manageApi.GetProviderSummary(instCode);

            var model = OrganisationViewModel.FromEnrichmentModel(
                ucasProviderEnrichmentGetModel, aboutAccreditingTrainingProviders, providerSummary);

            return View(model);
        }

        [HttpPost]
        [Route("{instCode}/details")]
        public async Task<ActionResult> DetailsPost(string instCode, OrganisationViewModel model)
        {
            var ucasProviderEnrichmentGetModel = await _manageApi.GetProviderEnrichment(instCode);
            var result = await PublishOrgansation(ucasProviderEnrichmentGetModel, instCode);
            return result;
        }

        [HttpGet]
        [Route("{instCode}/about")]
        public async Task<IActionResult> About(string instCode)
        {
            var ucasProviderEnrichmentGetModel = await _manageApi.GetProviderEnrichment(instCode);

            ucasProviderEnrichmentGetModel = ucasProviderEnrichmentGetModel ?? new UcasProviderEnrichmentGetModel { EnrichmentModel = new ProviderEnrichmentModel() { AccreditingProviderEnrichments = new List<AccreditingProviderEnrichment>() } };

            var enrichmentModel = ucasProviderEnrichmentGetModel.EnrichmentModel;
            var aboutAccreditingTrainingProviders = await MergeTrainingProviderViewModels(instCode, enrichmentModel);
            var providerSummary = await _manageApi.GetProviderSummary(instCode);

            var model = OrganisationViewModel.FromEnrichmentModel(ucasProviderEnrichmentGetModel, aboutAccreditingTrainingProviders, providerSummary);

            var aboutModel = OrganisationViewModelForAbout.FromGeneralViewModel(model);

            return View(aboutModel);
        }

        [HttpPost]
        [Route("{instCode}/about")]
        public async Task<ActionResult> AboutPost(string instCode, OrganisationViewModelForAbout model)
        {
            var ucasProviderEnrichmentGetModel = await _manageApi.GetProviderEnrichment(instCode);
            var enrichmentModel = ucasProviderEnrichmentGetModel?.EnrichmentModel;

            var aboutAccreditingTrainingProviders = await MergeTrainingProviderViewModels(instCode, enrichmentModel, model?.AboutTrainingProviders);
            model.AboutTrainingProviders = aboutAccreditingTrainingProviders;

            ValidateAboutModel(model);

            if (!ModelState.IsValid)
            {
                model.InstName = (await _manageApi.GetProviderSummaries()).FirstOrDefault(x => x.ProviderCode == instCode.ToUpperInvariant())?.ProviderName;
                return View("About", model);
            }

            if (enrichmentModel == null && model.IsEmpty())
            {
                // Draft state is "New" and no changes have been made - don't insert a draft
                return RedirectToAction("Details", "Organisation", new { instCode });
            }

            model.MergeIntoEnrichmentModel(ref enrichmentModel);

            var result = await SaveValidatedOrgansation(enrichmentModel, instCode);
            return result;
        }

        // TODO: Not currently visible to the public, add edit link on details view
        [HttpGet]
        [Route("{instCode}/contact")]
        public async Task<IActionResult> Contact(string instCode)
        {
            var ucasProviderEnrichmentGetModel = await _manageApi.GetProviderEnrichment(instCode);

            ucasProviderEnrichmentGetModel = ucasProviderEnrichmentGetModel ?? new UcasProviderEnrichmentGetModel { EnrichmentModel = new ProviderEnrichmentModel() { AccreditingProviderEnrichments = new List<AccreditingProviderEnrichment>() } };

            var enrichmentModel = ucasProviderEnrichmentGetModel.EnrichmentModel;
            var providerSummary = await _manageApi.GetProviderSummary(instCode);

            var model = OrganisationViewModel.FromEnrichmentModel(ucasProviderEnrichmentGetModel, null, providerSummary);

            var contactModel = OrganisationViewModelForContact.FromGeneralViewModel(model);

            return View(contactModel);
        }

        // TODO: Ensure this actually persists the contact data correctly
        [HttpPost]
        [Route("{instCode}/contact")]
        public async Task<ActionResult> ContactPost(string instCode, OrganisationViewModelForContact model)
        {
            var providerEnrichmentGetModel = await _manageApi.GetProviderEnrichment(instCode);
            var enrichmentModel = providerEnrichmentGetModel?.EnrichmentModel;

            ValidateContactModel(model);

            if (!ModelState.IsValid)
            {
                return View("Contact", model);
            }

            if (enrichmentModel == null && model.IsEmpty())
            {
                // Draft state is "New" and no changes have been made - don't insert a draft
                return RedirectToAction("Details", "Organisation", new { instCode });
            }

            model.MergeIntoEnrichmentModel(ref enrichmentModel);

            var result = await SaveValidatedOrgansation(enrichmentModel, instCode);
            return result;
        }

        [HttpGet]
        [Route("{instCode}/request-access")]
        public ViewResult RequestAccess(string instCode)
        {
            ViewBag.InstCode = instCode;
            return View(new RequestAccessViewModel() );
        }

        [HttpPost]
        [Route("{instCode}/request-access")]
        public async Task<ActionResult> RequestAccessPost(string instCode, RequestAccessViewModel model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.InstCode = instCode;
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

            return new RedirectToActionResult("Show", "Organisation", new { instCode });
        }

        private async Task<List<TrainingProviderViewModel>> MergeTrainingProviderViewModels(string instCode, ProviderEnrichmentModel enrichmentModel, IEnumerable<TrainingProviderViewModel> fromModel = null)
        {
            var ucasData = await _manageApi.GetCoursesOfProvider(instCode);

            var accreditingProviders = ucasData
                .Where(x =>
                    false == string.Equals(x.AccreditingProvider?.ProviderCode, instCode, StringComparison.InvariantCultureIgnoreCase) &&
                    false == string.IsNullOrWhiteSpace(x.AccreditingProvider?.ProviderCode))
                .Distinct(new AccreditingInstCodeComparer()).ToList();
            var accreditingProviderEnrichments = enrichmentModel?.AccreditingProviderEnrichments ?? new List<AccreditingProviderEnrichment>();

            var result = accreditingProviders.Select(x =>

                    {
                        var description = accreditingProviderEnrichments.FirstOrDefault(TrainingProviderMatchesProviderCourse(x))?.Description ?? "";

                        var aboutTrainingProviders = (fromModel ?? new List<TrainingProviderViewModel>());

                        var newAccreditingProviderModel = aboutTrainingProviders.FirstOrDefault(
                            atp => (atp.InstCode.Equals(x.AccreditingProvider?.ProviderCode, StringComparison.InvariantCultureIgnoreCase)));

                        if (newAccreditingProviderModel != null)
                        {
                            // empty descriptions get bound as `null` by ASP.NET MVC model binding
                            description = newAccreditingProviderModel.Description ?? "";
                        }

                        var tpvm = new TrainingProviderViewModel()
                        {
                            InstName = x.AccreditingProvider?.ProviderName,
                            InstCode = x.AccreditingProvider?.ProviderCode,
                            Description = description
                        };

                        return tpvm;
                    })

                .ToList();

            return result;
        }

        private async Task<ActionResult> PublishOrgansation(UcasProviderEnrichmentGetModel ucasProviderEnrichmentGetModel, string instCode)
        {
            var providerSummary = await _manageApi.GetProviderSummary(instCode);

            var enrichmentModel = ucasProviderEnrichmentGetModel.EnrichmentModel;
            var aboutAccreditingTrainingProviders = await MergeTrainingProviderViewModels(instCode, enrichmentModel);
            var model = OrganisationViewModel.FromEnrichmentModel(ucasProviderEnrichmentGetModel, aboutAccreditingTrainingProviders, providerSummary);

            ValidateModelForPublication(model);

            if (!ModelState.IsValid)
            {
                return View("Details", model);
            }
            else
            {
                var result = await _manageApi.PublishAllCoursesOfProviderToSearchAndCompare(instCode);

                if (result)
                {

                    TempData["MessageType"] = "success";
                    TempData["MessageTitle"] = "Your changes have been published – this content will appear on all of your course pages";

                    return RedirectToAction("Details", new { instCode });
                }
                else
                {
                    // This is a no ops, there should not be any viable valid reason that api rejects, hence dead end.
                    throw new InvalidOperationException("attempting to publish nonexistent organisation enrichment: " + instCode);
                }
            }
        }
        private async Task<ActionResult> SaveValidatedOrgansation(ProviderEnrichmentModel enrichmentModel, string instCode)
        {
            var postModel = new UcasProviderEnrichmentPostModel { EnrichmentModel = enrichmentModel };

            await _manageApi.SaveProviderEnrichment(instCode, postModel);

            TempData["MessageType"] = "success";
            TempData["MessageTitle"] = "Your changes have been saved";
            TempData["MessageBodyHtml"] = "<p class=\"govuk-body\">Preview any course to see how it will look to applicants.</p>";
            return RedirectToAction("Details", "Organisation", new { instCode });
        }

        private static Func<AccreditingProviderEnrichment, bool> TrainingProviderMatchesProviderCourse(Domain.Models.Course x)
        {
            return y => String.Equals(x.AccreditingProvider?.ProviderCode,
                y.UcasProviderCode, StringComparison.InvariantCultureIgnoreCase);
        }

        private List<ViewModels.Provider> GetProviders(List<Domain.Models.Course> providerCourses)
        {
            var uniqueAccreditingInstCodes = providerCourses.Select(c => c.AccreditingProvider?.ProviderCode).Distinct();
            var providers = new List<ViewModels.Provider>();
            foreach (var uniqueAccreditingInstCode in uniqueAccreditingInstCodes)
            {
                var name = providerCourses.First(c => c.AccreditingProvider?.ProviderCode == uniqueAccreditingInstCode)
                    .AccreditingProvider?.ProviderName;
                var courses = providerCourses.Where(c => c.AccreditingProvider?.ProviderCode == uniqueAccreditingInstCode).ToList();
                providers.Add(new ViewModels.Provider
                {
                    InstCode = uniqueAccreditingInstCode,
                    InstName = name,
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
