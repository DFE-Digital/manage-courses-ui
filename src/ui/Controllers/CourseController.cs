using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GovUk.Education.ManageCourses.ApiClient;
using GovUk.Education.ManageCourses.Ui;
using GovUk.Education.ManageCourses.Ui.Helpers;
using GovUk.Education.ManageCourses.Ui.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GovUk.Education.ManageCourses.Ui.Controllers
{
    [Authorize]
    [Route("organisation")]
    public class CourseController : CommonAttributesControllerBase
    {
        private readonly IManageApi _manageApi;

        public CourseController(IManageApi manageApi)
        {
            _manageApi = manageApi;
        }

        [Route("{instCode}/course/{accreditingProviderId=self}/{ucasCode}")]
        public async Task<IActionResult> Variants(string instCode, string accreditingProviderId, string ucasCode)
        {
            Validate(instCode, accreditingProviderId, ucasCode);

            var orgsList = await _manageApi.GetOrganisations();
            var userOrganisations = orgsList.ToList();
            var multipleOrganisations = userOrganisations.Count() > 1;
            var org = userOrganisations.ToList().FirstOrDefault(x => instCode.ToLower() == x.UcasCode.ToLower());

            if (org == null) { throw new InvalidOperationException($"Organisation with code '{ucasCode}' not found"); }

            var course = await _manageApi.GetCourseByUcasCode(instCode, ucasCode);

            if (course == null) return NotFound();

            var ucasCourseEnrichmentGetModel = await _manageApi.GetEnrichmentCourse(instCode, ucasCode);

            var routeData = GetCourseRouteDataViewModel(instCode, accreditingProviderId, ucasCode);

            var viewModel = LoadViewModel(org, course, multipleOrganisations, ucasCourseEnrichmentGetModel, routeData);

            return View(viewModel);
        }

        [HttpGet]
        [Route("{instCode}/course/{accreditingProviderId=self}/{ucasCode}/about")]
        public async Task<IActionResult> About(string instCode, string accreditingProviderId, string ucasCode)
        {
            var course = await _manageApi.GetEnrichmentCourse(instCode, ucasCode);
            var routeData = GetCourseRouteDataViewModel(instCode, accreditingProviderId, ucasCode);

            var enrichmentModel = course.EnrichmentModel;

            var model = new AboutCourseEnrichmentViewModel
            {
                AboutCourse = enrichmentModel.AboutCourse,
                InterviewProcess = enrichmentModel.InterviewProcess,
                HowSchoolPlacementsWork = enrichmentModel.HowSchoolPlacementsWork,
                RouteData = routeData
            };
            return View(model);
        }

        [HttpPost]
        [Route("{instCode}/course/{accreditingProviderId=self}/{ucasCode}/about")]
        public async Task<IActionResult> AboutPost(string instCode, string accreditingProviderId, string ucasCode, AboutCourseEnrichmentViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                var routeData = GetCourseRouteDataViewModel(instCode, accreditingProviderId, ucasCode);
                viewModel.RouteData = routeData;
                return View("About", viewModel);
            }

            await SaveEnrichment(instCode, ucasCode, viewModel);

            SetSucessMessage();

            return RedirectToAction("Variants", new { instCode, accreditingProviderId, ucasCode });
        }

        [HttpGet]
        [Route("{instCode}/course/{accreditingProviderId=self}/{ucasCode}/requirements")]
        public async Task<IActionResult> Requirements(string instCode, string accreditingProviderId, string ucasCode)
        {
            var course = await _manageApi.GetEnrichmentCourse(instCode, ucasCode);
            var routeData = GetCourseRouteDataViewModel(instCode, accreditingProviderId, ucasCode);

            var enrichmentModel = course.EnrichmentModel;
            var model = new CourseRequirementsEnrichmentViewModel
            {
                Qualifications = enrichmentModel.Qualifications,
                PersonalQualities = enrichmentModel.PersonalQualities,
                OtherRequirements = enrichmentModel.OtherRequirements,
                RouteData = routeData
            };

            return View(model);
        }

        [HttpPost]
        [Route("{instCode}/course/{accreditingProviderId=self}/{ucasCode}/requirements")]
        public async Task<IActionResult> RequirementsPost(string instCode, string accreditingProviderId, string ucasCode, CourseRequirementsEnrichmentViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                var routeData = GetCourseRouteDataViewModel(instCode, accreditingProviderId, ucasCode);
                viewModel.RouteData = routeData;
                return View("Requirements", viewModel);
            }

            await SaveEnrichment(instCode, ucasCode, viewModel);
            SetSucessMessage();

            return RedirectToAction("Variants", new { instCode, accreditingProviderId, ucasCode });
        }

        private void SetSucessMessage()
        {
            TempData.Add("MessageType", "success");
            TempData.Add("MessageTitle", "Your changes have been saved");
        }
        private void MapEnrichment(CourseEnrichmentModel enrichmentModel, ICourseEnrichmentViewModel viewModel)
        {
            var aboutCourseEnrichmentViewModel = viewModel as AboutCourseEnrichmentViewModel;

            if (aboutCourseEnrichmentViewModel != null)
            {
                enrichmentModel.AboutCourse = aboutCourseEnrichmentViewModel.AboutCourse;
                enrichmentModel.InterviewProcess = aboutCourseEnrichmentViewModel.InterviewProcess;
                enrichmentModel.HowSchoolPlacementsWork = aboutCourseEnrichmentViewModel.HowSchoolPlacementsWork;
            }

            var courseRequirementsEnrichmentViewModel = viewModel as CourseRequirementsEnrichmentViewModel;

            if (courseRequirementsEnrichmentViewModel != null)
            {
                enrichmentModel.Qualifications = courseRequirementsEnrichmentViewModel.Qualifications;
                enrichmentModel.PersonalQualities = courseRequirementsEnrichmentViewModel.PersonalQualities;
                enrichmentModel.OtherRequirements = courseRequirementsEnrichmentViewModel.OtherRequirements;
            }
        }

        private async Task SaveEnrichment(string instCode, string ucasCode, ICourseEnrichmentViewModel viewModel)
        {
            var course = await _manageApi.GetEnrichmentCourse(instCode, ucasCode);

            var enrichmentModel = course.EnrichmentModel;
            MapEnrichment(enrichmentModel, viewModel);

            await _manageApi.SaveEnrichmentCourse(instCode, ucasCode, enrichmentModel);

        }
        private void Validate(string instCode, string accreditingProviderId, string ucasCode)
        {
            if (string.IsNullOrEmpty(instCode)) { throw new ArgumentNullException(instCode, "instCode cannot be null or empty"); }
            if (string.IsNullOrEmpty(accreditingProviderId)) { throw new ArgumentNullException(accreditingProviderId, "accreditingProviderId cannot be null or empty"); }
            if (string.IsNullOrEmpty(ucasCode)) { throw new ArgumentNullException(ucasCode, "ucasCode cannot be null or empty"); }
        }

        private FromUcasViewModel LoadViewModel(UserOrganisation org, Course course, bool multipleOrganisations, UcasCourseEnrichmentGetModel ucasCourseEnrichmentGetModel, CourseRouteDataViewModel routeData)
        {
            var courseVariant =
                new CourseVariantViewModel
                {
                    Name = course.Name,
                        Type = course.GetCourseVariantType(),
                        Accrediting = course.AccreditingProviderName,
                        ProviderCode = course.AccreditingProviderId,
                        ProgrammeCode = course.CourseCode,
                        UcasCode = course.InstCode,
                        AgeRange = course.AgeRange,
                        Route = course.ProgramType,
                        Qualifications = course.ProfpostFlag,
                        StudyMode = course.StudyMode,
                        Subjects = course.Subjects,
                        Schools = course.Schools.Select(campus =>
                        {
                            var addressLines = (new List<string>()
                                {
                                    campus.Address1,
                                        campus.Address2,
                                        campus.Address3,
                                        campus.Address4,
                                        campus.PostCode
                                })
                                .Where(line => !String.IsNullOrEmpty(line));

                            var address = addressLines.Count() > 0 ? addressLines.Where(line => !String.IsNullOrEmpty(line))
                                .Aggregate((current, next) => current + ", " + next) : "";

                            return new SchoolViewModel
                            {
                                ApplicationsAcceptedFrom = campus.ApplicationsAcceptedFrom,
                                    Code = campus.Code,
                                    LocationName = campus.LocationName,
                                    Address = address
                            };
                        })
                };

            var courseEnrichmentViewModel = GetCourseEnrichmentViewModel(ucasCourseEnrichmentGetModel);
            var viewModel = new FromUcasViewModel
            {
                OrganisationName = org.OrganisationName,
                OrganisationId = org.OrganisationId,
                CourseTitle = course.Name,
                AccreditingProviderId = course.AccreditingProviderId,
                MultipleOrganisations = multipleOrganisations,
                Course = courseVariant,
                CourseEnrichment = courseEnrichmentViewModel,
                RouteData = routeData
            };
            return viewModel;
        }

        private static CourseEnrichmentViewModel GetCourseEnrichmentViewModel(UcasCourseEnrichmentGetModel ucasCourseEnrichmentGetModel)
        {
            if (ucasCourseEnrichmentGetModel == null)
            {
                return null;
            }

            var enrichmentModel = ucasCourseEnrichmentGetModel.EnrichmentModel;
            var result = new CourseEnrichmentViewModel()
            {
                AboutCourse = enrichmentModel.AboutCourse,
                InterviewProcess = enrichmentModel.InterviewProcess,
                HowSchoolPlacementsWork = enrichmentModel.HowSchoolPlacementsWork,

                Qualifications = enrichmentModel.Qualifications,
                PersonalQualities = enrichmentModel.PersonalQualities,
                OtherRequirements = enrichmentModel.OtherRequirements,

            };

            return result;
        }

        private CourseRouteDataViewModel GetCourseRouteDataViewModel(string instCode, string accreditingProviderId, string ucasCode)
        {
            return new CourseRouteDataViewModel
            {
                InstCode = instCode,
                    AccreditingProviderId = accreditingProviderId,
                    UcasCode = ucasCode
            };
        }
    }
}
