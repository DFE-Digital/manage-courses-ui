using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using GovUk.Education.ManageCourses.Api.Model;
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
            var orgs = await _manageApi.GetInstitutionSummaries();
            var model = new OrganisationListViewModel
            {
                InstitutionSummaries = orgs
            };
            return View(model);
        }

        [Route("{instCode}")]
        public async Task<IActionResult> Show(string instCode)
        {
            var ucasInstitutionEnrichmentGetModel = await _manageApi.GetInstitutitionEnrichment(instCode);
            var institutionCourses = await _manageApi.GetCoursesOfInstitution(instCode);
            var summary = await _manageApi.GetInstitutionSummary(instCode);
            var multipleOrganisations = (await _manageApi.GetInstitutionSummaries()).Count() > 1;
            var providers = GetProviders(institutionCourses);

            var status = ucasInstitutionEnrichmentGetModel?.Status.ToString() ?? "Empty";

            var model = new CourseListViewModel
            {
                InstName = summary.InstName,
                InstCode = summary.InstCode,
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
            var ucasInstitutionEnrichmentGetModel = await _manageApi.GetInstitutitionEnrichment(instCode);

            ucasInstitutionEnrichmentGetModel = ucasInstitutionEnrichmentGetModel ?? new UcasInstitutionEnrichmentGetModel { EnrichmentModel = new InstitutionEnrichmentModel() { AccreditingProviderEnrichments = new ObservableCollection<AccreditingProviderEnrichment>() } };

            var enrichmentModel = ucasInstitutionEnrichmentGetModel.EnrichmentModel;
            var aboutAccreditingTrainingProviders = await MergeTrainingProviderViewModels(instCode, enrichmentModel);
            var institutionSummary = await _manageApi.GetInstitutionSummary(instCode);

            var model = OrganisationViewModel.FromEnrichmentModel(
                ucasInstitutionEnrichmentGetModel, aboutAccreditingTrainingProviders, institutionSummary);

            return View(model);
        }

        [HttpPost]
        [Route("{instCode}/details")]
        public async Task<ActionResult> DetailsPost(string instCode, OrganisationViewModel model)
        {
            var ucasInstitutionEnrichmentGetModel = await _manageApi.GetInstitutitionEnrichment(instCode);
            var result = await PublishOrgansation(ucasInstitutionEnrichmentGetModel, instCode);
            return result;
        }

        [HttpGet]
        [Route("{instCode}/about")]
        public async Task<IActionResult> About(string instCode)
        {
            var ucasInstitutionEnrichmentGetModel = await _manageApi.GetInstitutitionEnrichment(instCode);

            ucasInstitutionEnrichmentGetModel = ucasInstitutionEnrichmentGetModel ?? new UcasInstitutionEnrichmentGetModel { EnrichmentModel = new InstitutionEnrichmentModel() { AccreditingProviderEnrichments = new ObservableCollection<AccreditingProviderEnrichment>() } };

            var enrichmentModel = ucasInstitutionEnrichmentGetModel.EnrichmentModel;
            var aboutAccreditingTrainingProviders = await MergeTrainingProviderViewModels(instCode, enrichmentModel);
            var institutionSummary = await _manageApi.GetInstitutionSummary(instCode);

            var model = OrganisationViewModel.FromEnrichmentModel(ucasInstitutionEnrichmentGetModel, aboutAccreditingTrainingProviders, institutionSummary);

            var aboutModel = OrganisationViewModelForAbout.FromGeneralViewModel(model);

            return View(aboutModel);
        }

        [HttpPost]
        [Route("{instCode}/about")]
        public async Task<ActionResult> AboutPost(string instCode, OrganisationViewModelForAbout model)
        {
            var ucasInstitutionEnrichmentGetModel = await _manageApi.GetInstitutitionEnrichment(instCode);
            var enrichmentModel = ucasInstitutionEnrichmentGetModel?.EnrichmentModel;

            var aboutAccreditingTrainingProviders = await MergeTrainingProviderViewModels(instCode, enrichmentModel, model?.AboutTrainingProviders);
            model.AboutTrainingProviders = aboutAccreditingTrainingProviders;

            ValidateAboutModel(model);

            if (!ModelState.IsValid)
            {
                model.InstName = (await _manageApi.GetInstitutionSummaries()).FirstOrDefault(x => x.InstCode == instCode.ToUpperInvariant())?.InstName;
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
            var ucasInstitutionEnrichmentGetModel = await _manageApi.GetInstitutitionEnrichment(instCode);

            ucasInstitutionEnrichmentGetModel = ucasInstitutionEnrichmentGetModel ?? new UcasInstitutionEnrichmentGetModel { EnrichmentModel = new InstitutionEnrichmentModel() { AccreditingProviderEnrichments = new ObservableCollection<AccreditingProviderEnrichment>() } };

            var enrichmentModel = ucasInstitutionEnrichmentGetModel.EnrichmentModel;
            var institutionSummary = await _manageApi.GetInstitutionSummary(instCode);

            var model = OrganisationViewModel.FromEnrichmentModel(ucasInstitutionEnrichmentGetModel, null, institutionSummary);

            var contactModel = OrganisationViewModelForContact.FromGeneralViewModel(model);

            return View(contactModel);
        }

        // TODO: Ensure this actually persists the contact data correctly
        [HttpPost]
        [Route("{instCode}/contact")]
        public async Task<ActionResult> ContactPost(string instCode, OrganisationViewModelForContact model)
        {
            var ucasInstitutionEnrichmentGetModel = await _manageApi.GetInstitutitionEnrichment(instCode);
            var enrichmentModel = ucasInstitutionEnrichmentGetModel?.EnrichmentModel;

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

            await _manageApi.CreateAccessRequest(new AccessRequest()
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

        private async Task<List<TrainingProviderViewModel>> MergeTrainingProviderViewModels(string instCode, InstitutionEnrichmentModel enrichmentModel, IEnumerable<TrainingProviderViewModel> fromModel = null)
        {
            var ucasData = await _manageApi.GetCoursesOfInstitution(instCode);

            var accreditingProviders = ucasData
                .Where(x =>
                    false == string.Equals(x.AccreditingInstitution?.InstCode, instCode, StringComparison.InvariantCultureIgnoreCase) &&
                    false == string.IsNullOrWhiteSpace(x.AccreditingInstitution?.InstCode))
                .Distinct(new AccreditingInstCodeComparer()).ToList();
            var accreditingProviderEnrichments = enrichmentModel?.AccreditingProviderEnrichments ?? new ObservableCollection<AccreditingProviderEnrichment>();

            var result = accreditingProviders.Select(x =>

                    {
                        var description = accreditingProviderEnrichments.FirstOrDefault(TrainingProviderMatchesProviderCourse(x))?.Description ?? "";

                        var aboutTrainingProviders = (fromModel ?? new List<TrainingProviderViewModel>());

                        var newAccreditingProviderModel = aboutTrainingProviders.FirstOrDefault(
                            atp => (atp.InstCode.Equals(x.AccreditingInstitution?.InstCode, StringComparison.InvariantCultureIgnoreCase)));

                        if (newAccreditingProviderModel != null)
                        {
                            // empty descriptions get bound as `null` by ASP.NET MVC model binding
                            description = newAccreditingProviderModel.Description ?? "";
                        }

                        var tpvm = new TrainingProviderViewModel()
                        {
                            InstName = x.AccreditingInstitution?.InstName,
                            InstCode = x.AccreditingInstitution?.InstCode,
                            Description = description
                        };

                        return tpvm;
                    })

                .ToList();

            return result;
        }

        private async Task<ActionResult> PublishOrgansation(UcasInstitutionEnrichmentGetModel ucasInstitutionEnrichmentGetModel, string instCode)
        {
            var institutionSummary = await _manageApi.GetInstitutionSummary(instCode);

            var enrichmentModel = ucasInstitutionEnrichmentGetModel.EnrichmentModel;
            var aboutAccreditingTrainingProviders = await MergeTrainingProviderViewModels(instCode, enrichmentModel);
            var model = OrganisationViewModel.FromEnrichmentModel(ucasInstitutionEnrichmentGetModel, aboutAccreditingTrainingProviders, institutionSummary);

            ValidateModelForPublication(model);

            if (!ModelState.IsValid)
            {
                return View("Details", model);
            }
            else
            {
                var result = await _manageApi.PublishAllCoursesOfInstitutionToSearchAndCompare(instCode);

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
        private async Task<ActionResult> SaveValidatedOrgansation(InstitutionEnrichmentModel enrichmentModel, string instCode)
        {
            var postModel = new UcasInstitutionEnrichmentPostModel { EnrichmentModel = enrichmentModel };

            await _manageApi.SaveInstitutionEnrichment(instCode, postModel);

            TempData["MessageType"] = "success";
            TempData["MessageTitle"] = "Your changes have been saved";
            TempData["MessageBodyHtml"] = "<p class=\"govuk-body\">Preview any course to see how it will look to applicants.</p>";
            return RedirectToAction("Details", "Organisation", new { instCode });
        }

        private static Func<AccreditingProviderEnrichment, bool> TrainingProviderMatchesProviderCourse(Domain.Models.Course x)
        {
            return y => String.Equals(x.AccreditingInstitution?.InstCode,
                y.UcasInstitutionCode, StringComparison.InvariantCultureIgnoreCase);
        }

        private List<ViewModels.Provider> GetProviders(List<Course> institutionCourses)
        {
            var uniqueAccreditingInstCodes = institutionCourses.Select(c => c.AccreditingInstitution?.InstCode).Distinct();
            var providers = new List<ViewModels.Provider>();
            foreach (var uniqueAccreditingInstCode in uniqueAccreditingInstCodes)
            {
                var name = institutionCourses.First(c => c.AccreditingInstitution?.InstCode == uniqueAccreditingInstCode)
                    .AccreditingInstitution?.InstName;
                var courses = institutionCourses.Where(c => c.AccreditingInstitution?.InstCode == uniqueAccreditingInstCode).ToList();
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
