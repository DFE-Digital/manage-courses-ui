using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GovUk.Education.ManageCourses.ApiClient;
using GovUk.Education.ManageCourses.Ui;
using GovUk.Education.ManageCourses.Ui.Helpers;
using GovUk.Education.ManageCourses.Ui.Services;
using GovUk.Education.ManageCourses.Ui.ViewModels;
using GovUk.Education.ManageCourses.Ui.ViewModels.Enums;
using GovUk.Education.SearchAndCompare.Domain.Models;
using GovUk.Education.SearchAndCompare.UI.Shared.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GovUk.Education.ManageCourses.Ui.Controllers
{
    [Authorize]
    [Route("organisation")]
    public class CourseController : CommonAttributesControllerBase
    {
        private readonly IManageApi _manageApi;
        private readonly ICourseMapper courseMapper;
        private readonly ISearchAndCompareUrlService searchAndCompareUrlService;
        private readonly IFeatureFlags featureFlags;

        public CourseController(IManageApi manageApi, ICourseMapper courseMapper, ISearchAndCompareUrlService searchAndCompareUrlHelper, IFeatureFlags featureFlags)
        {
            _manageApi = manageApi;
            this.courseMapper = courseMapper;
            this.searchAndCompareUrlService = searchAndCompareUrlHelper;
            this.featureFlags = featureFlags;
        }

        [Route("{instCode}/course/{accreditingProviderId=self}/{ucasCode}")]
        public async Task<IActionResult> Variants(string instCode, string accreditingProviderId, string ucasCode)
        {
            Validate(instCode, accreditingProviderId, ucasCode);

            var orgsList = await _manageApi.GetOrganisations();
            var userOrganisations = orgsList.ToList();
            var multipleOrganisations = userOrganisations.Count() > 1;
            var org = userOrganisations.ToList().FirstOrDefault(x => instCode.ToLower() == x.UcasCode.ToLower());

            if (org == null) { return NotFound($"Organisation with code '{ucasCode}' not found"); }

            var course = await _manageApi.GetCourseByUcasCode(instCode, ucasCode);

            if (course == null) return NotFound();

            var ucasCourseEnrichmentGetModel = await _manageApi.GetEnrichmentCourse(instCode, ucasCode);

            var routeData = GetCourseRouteDataViewModel(instCode, accreditingProviderId, ucasCode);

            var viewModel = LoadViewModel(org, course, multipleOrganisations, ucasCourseEnrichmentGetModel, routeData);

            return View("Variants", viewModel);
        }

        [HttpPost]
        [Route("{instCode}/course/{accreditingProviderId=self}/{ucasCode}", Name="publish")]
        public async Task<IActionResult> VariantsPublish(string instCode, string accreditingProviderId, string ucasCode)
        {
            if(!featureFlags.ShowCoursePublish)
            {
                return RedirectToAction("Variants", new { instCode, accreditingProviderId, ucasCode });
            }
            var enrichment = await _manageApi.GetEnrichmentCourse(instCode, ucasCode);
            var enrichmentModel = GetCourseEnrichmentViewModel(enrichment);

            ModelState.Clear();
            TryValidateModel(enrichmentModel);

            if (!ModelState.IsValid)
            {
                return await Variants(instCode, accreditingProviderId, ucasCode);
            }

            var result = await _manageApi.PublishEnrichmentCourse(instCode, ucasCode);

            if (result)
            {
                SetSucessMessage("Your course has been published");
            }

            return RedirectToAction("Variants", new { instCode, accreditingProviderId, ucasCode });
        }

        [Route("{instCode}/course/{accreditingProviderId=self}/{ucasCode}/preview")]
        public IActionResult Preview(string instCode, string accreditingProviderId, string ucasCode)
        {
            if (!featureFlags.ShowCoursePreview)
            {
                return RedirectToAction("Variants", new { instCode, accreditingProviderId, ucasCode });
            }

            var ucasInstData = _manageApi.GetUcasInstitution(instCode).Result;
            var ucasCourseData = _manageApi.GetCourseByUcasCode(instCode, ucasCode).Result;
            var orgEnrichmentData = _manageApi.GetEnrichmentOrganisation(instCode).Result;
            var courseEnrichmentData = _manageApi.GetEnrichmentCourse(instCode, ucasCode).Result;

            if (ucasInstData == null || ucasCourseData == null)
            {
                return NotFound();
            }

            var course = courseMapper.MapToSearchAndCompareCourse(
                ucasInstData,
                ucasCourseData,
                orgEnrichmentData?.EnrichmentModel,
                courseEnrichmentData?.EnrichmentModel);

            return View(new SearchAndCompare.UI.Shared.ViewModels.CourseDetailsViewModel{
                AboutYourOrgLink = Url.Action("About", "Organisation", new { ucasCode = instCode }),
                PreviewMode = true,
                Course = course,
                Finance = new FinanceViewModel(course, new FeeCaps())
            });
        }

        [HttpGet]
        [Route("{instCode}/course/{accreditingProviderId=self}/{ucasCode}/about")]
        public async Task<IActionResult> About(string instCode, string accreditingProviderId, string ucasCode, string copyFrom = null)
        {
            var courseDetails = await _manageApi.GetCourseByUcasCode(instCode, ucasCode);
            var ucasCourseEnrichmentGetModel = await _manageApi.GetEnrichmentCourse(instCode, ucasCode);

            var routeData = GetCourseRouteDataViewModel(instCode, accreditingProviderId, ucasCode);
            var courseInfo = new CourseInfoViewModel { ProgrammeCode = courseDetails.CourseCode, Name = courseDetails.Name };

            var enrichmentModel = ucasCourseEnrichmentGetModel?.EnrichmentModel ?? new CourseEnrichmentModel();

            var model = new AboutCourseEnrichmentViewModel
            {
                AboutCourse = enrichmentModel?.AboutCourse,
                InterviewProcess = enrichmentModel?.InterviewProcess,
                HowSchoolPlacementsWork = enrichmentModel?.HowSchoolPlacementsWork,
                RouteData = routeData,
                CourseInfo = courseInfo
            };
            
            if (!string.IsNullOrEmpty(copyFrom))
            {
                var copiedEnrichment = await _manageApi.GetEnrichmentCourse(instCode, copyFrom); 
                ViewBag.CopiedFields = model.CopyFrom(copiedEnrichment?.EnrichmentModel);                
            }

            await LoadCopyableCoursesIntoViewBag(instCode, ucasCode);

            return View(model);
        }

        private async Task LoadCopyableCoursesIntoViewBag(string instCode, string ucasCode)
        {
            instCode = instCode.ToUpper();
            ucasCode = ucasCode.ToUpper(); 

            var copyable = await _manageApi.GetCoursesByOrganisation(instCode);
            ViewBag.CopyableCourses = copyable != null ? copyable.Courses.Where(x => x.EnrichmentWorkflowStatus != null && x.CourseCode != ucasCode) : new List<ApiClient.Course>();
        }

        [HttpPost]
        [Route("{instCode}/course/{accreditingProviderId=self}/{ucasCode}/about")]
        public async Task<IActionResult> AboutPost(string instCode, string accreditingProviderId, string ucasCode, AboutCourseEnrichmentViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                var courseDetails = await _manageApi.GetCourseByUcasCode(instCode, ucasCode);
                var courseInfo = new CourseInfoViewModel { ProgrammeCode = courseDetails.CourseCode, Name = courseDetails.Name };
                var routeData = GetCourseRouteDataViewModel(instCode, accreditingProviderId, ucasCode);
                viewModel.RouteData = routeData;
                viewModel.CourseInfo = courseInfo;
                return View("About", viewModel);
            }

            await SaveEnrichment(instCode, ucasCode, viewModel);

            SetSucessMessage();

            return RedirectToAction("Variants", new { instCode, accreditingProviderId, ucasCode });
        }

        [HttpGet]
        [Route("{instCode}/course/{accreditingProviderId=self}/{ucasCode}/requirements")]
        public async Task<IActionResult> Requirements(string instCode, string accreditingProviderId, string ucasCode, string copyFrom = null)
        {
            var courseDetails = await _manageApi.GetCourseByUcasCode(instCode, ucasCode);
            var ucasCourseEnrichmentGetModel = await _manageApi.GetEnrichmentCourse(instCode, ucasCode);
            var routeData = GetCourseRouteDataViewModel(instCode, accreditingProviderId, ucasCode);
            var courseInfo = new CourseInfoViewModel { ProgrammeCode = courseDetails.CourseCode, Name = courseDetails.Name };

            var enrichmentModel = ucasCourseEnrichmentGetModel?.EnrichmentModel ?? new CourseEnrichmentModel();

            var model = new CourseRequirementsEnrichmentViewModel
            {
                Qualifications = enrichmentModel?.Qualifications,
                PersonalQualities = enrichmentModel?.PersonalQualities,
                OtherRequirements = enrichmentModel?.OtherRequirements,
                RouteData = routeData,
                CourseInfo = courseInfo
            };

            if (!string.IsNullOrEmpty(copyFrom))
            {
                var copiedEnrichment = await _manageApi.GetEnrichmentCourse(instCode, copyFrom); 
                ViewBag.CopiedFields = model.CopyFrom(copiedEnrichment?.EnrichmentModel);                
            }

            await LoadCopyableCoursesIntoViewBag(instCode, ucasCode);

            return View(model);
        }

        [HttpPost]
        [Route("{instCode}/course/{accreditingProviderId=self}/{ucasCode}/requirements")]
        public async Task<IActionResult> RequirementsPost(string instCode, string accreditingProviderId, string ucasCode, CourseRequirementsEnrichmentViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                var courseDetails = await _manageApi.GetCourseByUcasCode(instCode, ucasCode);
                var courseInfo = new CourseInfoViewModel { ProgrammeCode = courseDetails.CourseCode, Name = courseDetails.Name };
                var routeData = GetCourseRouteDataViewModel(instCode, accreditingProviderId, ucasCode);
                viewModel.RouteData = routeData;
                viewModel.CourseInfo = courseInfo;
                return View("Requirements", viewModel);
            }

            await SaveEnrichment(instCode, ucasCode, viewModel);
            SetSucessMessage();

            return RedirectToAction("Variants", new { instCode, accreditingProviderId, ucasCode });
        }

        [HttpGet]
        [Route("{instCode}/course/{accreditingProviderId=self}/{ucasCode}/salary")]
        public async Task<IActionResult> Salary(string instCode, string accreditingProviderId, string ucasCode, string copyFrom = null)
        {
            var courseDetails = await _manageApi.GetCourseByUcasCode(instCode, ucasCode);
            var ucasCourseEnrichmentGetModel = await _manageApi.GetEnrichmentCourse(instCode, ucasCode);
            var routeData = GetCourseRouteDataViewModel(instCode, accreditingProviderId, ucasCode);
            var courseInfo = new CourseInfoViewModel { ProgrammeCode = courseDetails.CourseCode, Name = courseDetails.Name };

            var enrichmentModel = ucasCourseEnrichmentGetModel?.EnrichmentModel ?? new CourseEnrichmentModel();

            var model = new CourseSalaryEnrichmentViewModel
            {
                CourseLength = enrichmentModel?.CourseLength.GetCourseLength(),
                SalaryDetails = enrichmentModel?.SalaryDetails,
                RouteData = routeData,
                CourseInfo = courseInfo
            };

            if (!string.IsNullOrEmpty(copyFrom))
            {
                var copiedEnrichment = await _manageApi.GetEnrichmentCourse(instCode, copyFrom); 
                ViewBag.CopiedFields = model.CopyFrom(copiedEnrichment?.EnrichmentModel);                
            }

            await LoadCopyableCoursesIntoViewBag(instCode, ucasCode);

            return View(model);
        }

        [HttpPost]
        [Route("{instCode}/course/{accreditingProviderId=self}/{ucasCode}/salary")]
        public async Task<IActionResult> SalaryPost(string instCode, string accreditingProviderId, string ucasCode, CourseSalaryEnrichmentViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                var routeData = GetCourseRouteDataViewModel(instCode, accreditingProviderId, ucasCode);
                var courseDetails = await _manageApi.GetCourseByUcasCode(instCode, ucasCode);
                var courseInfo = new CourseInfoViewModel { ProgrammeCode = courseDetails.CourseCode, Name = courseDetails.Name };
                viewModel.RouteData = routeData;
                viewModel.CourseInfo = courseInfo;
                return View("Salary", viewModel);
            }
            await SaveEnrichment(instCode, ucasCode, viewModel);
            SetSucessMessage();
            return RedirectToAction("Variants", new { instCode, accreditingProviderId, ucasCode });
        }

        [HttpGet]
        [Route("{instCode}/course/{accreditingProviderId=self}/{ucasCode}/fees-and-length")]
        public async Task<IActionResult> Fees(string instCode, string accreditingProviderId, string ucasCode, string copyFrom = null)
        {
            var courseDetails = await _manageApi.GetCourseByUcasCode(instCode, ucasCode);
            var ucasCourseEnrichmentGetModel = await _manageApi.GetEnrichmentCourse(instCode, ucasCode);
            var routeData = GetCourseRouteDataViewModel(instCode, accreditingProviderId, ucasCode);
            var courseInfo = new CourseInfoViewModel { ProgrammeCode = courseDetails.CourseCode, Name = courseDetails.Name };

            var enrichmentModel = ucasCourseEnrichmentGetModel?.EnrichmentModel ?? new CourseEnrichmentModel();

            var model = new CourseFeesEnrichmentViewModel
            {
                CourseLength = enrichmentModel?.CourseLength.GetCourseLength(),
                FeeUkEu = enrichmentModel?.FeeUkEu,
                FeeInternational = enrichmentModel?.FeeInternational,
                FeeDetails = enrichmentModel?.FeeDetails,
                FinancialSupport = enrichmentModel?.FinancialSupport,
                RouteData = routeData,
                CourseInfo = courseInfo
            };

            if (!string.IsNullOrEmpty(copyFrom))
            {
                var copiedEnrichment = await _manageApi.GetEnrichmentCourse(instCode, copyFrom); 
                ViewBag.CopiedFields = model.CopyFrom(copiedEnrichment?.EnrichmentModel);                
            }

            await LoadCopyableCoursesIntoViewBag(instCode, ucasCode);
            return View(model);
        }

        [HttpPost]
        [Route("{instCode}/course/{accreditingProviderId=self}/{ucasCode}/fees-and-length")]
        public async Task<IActionResult> FeesPost(string instCode, string accreditingProviderId, string ucasCode, CourseFeesEnrichmentViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                var routeData = GetCourseRouteDataViewModel(instCode, accreditingProviderId, ucasCode);
                var courseDetails = await _manageApi.GetCourseByUcasCode(instCode, ucasCode);
                var courseInfo = new CourseInfoViewModel { ProgrammeCode = courseDetails.CourseCode, Name = courseDetails.Name };
                viewModel.RouteData = routeData;
                viewModel.CourseInfo = courseInfo;
                return View("Fees", viewModel);
            }
            await SaveEnrichment(instCode, ucasCode, viewModel);
            SetSucessMessage();
            return RedirectToAction("Variants", new { instCode, accreditingProviderId, ucasCode });
        }

        private async Task SaveEnrichment(string instCode, string ucasCode, ICourseEnrichmentViewModel viewModel)
        {
            var course = await _manageApi.GetEnrichmentCourse(instCode, ucasCode);

            var enrichmentModel = course?.EnrichmentModel ?? new CourseEnrichmentModel();
            MapEnrichment(enrichmentModel, viewModel);

            await _manageApi.SaveEnrichmentCourse(instCode, ucasCode, enrichmentModel);

        }

        private void Validate(string instCode, string accreditingProviderId, string ucasCode)
        {
            if (string.IsNullOrEmpty(instCode)) { throw new ArgumentNullException(instCode, "instCode cannot be null or empty"); }
            if (string.IsNullOrEmpty(accreditingProviderId)) { throw new ArgumentNullException(accreditingProviderId, "accreditingProviderId cannot be null or empty"); }
            if (string.IsNullOrEmpty(ucasCode)) { throw new ArgumentNullException(ucasCode, "ucasCode cannot be null or empty"); }
        }

        private void SetSucessMessage(string message = null)
        {
            TempData.Add("MessageType", "success");
            TempData.Add("MessageTitle", message ?? "Your changes have been saved");
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

            var courseFeesEnrichmentViewModel = viewModel as CourseFeesEnrichmentViewModel;
            if (courseFeesEnrichmentViewModel != null)
            {

                var courseLength = courseFeesEnrichmentViewModel.CourseLength.HasValue ? courseFeesEnrichmentViewModel.CourseLength.Value.ToString() : null;

                enrichmentModel.CourseLength = courseLength;
                enrichmentModel.FeeUkEu = courseFeesEnrichmentViewModel.FeeUkEu;
                enrichmentModel.FeeInternational = courseFeesEnrichmentViewModel.FeeInternational;
                enrichmentModel.FeeDetails = courseFeesEnrichmentViewModel.FeeDetails;
                enrichmentModel.FinancialSupport = courseFeesEnrichmentViewModel.FinancialSupport;
            }

            var courseSalaryEnrichmentViewModel = viewModel as CourseSalaryEnrichmentViewModel;
            if (courseSalaryEnrichmentViewModel != null)
            {

                var courseLength = courseSalaryEnrichmentViewModel.CourseLength.HasValue ? courseSalaryEnrichmentViewModel.CourseLength.Value.ToString() : null;

                enrichmentModel.CourseLength = courseLength;;
                enrichmentModel.SalaryDetails = courseSalaryEnrichmentViewModel.SalaryDetails;
            }
        }

        private VariantViewModel LoadViewModel(UserOrganisation org, ApiClient.Course course, bool multipleOrganisations, UcasCourseEnrichmentGetModel ucasCourseEnrichmentGetModel, CourseRouteDataViewModel routeData)
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
                        Status = course.GetCourseStatus(),
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
                                Address = address,
                                Status = campus.Status
                            };
                        })
                };

            var courseEnrichmentViewModel = GetCourseEnrichmentViewModel(ucasCourseEnrichmentGetModel);
            var viewModel = new VariantViewModel
            {
                OrganisationName = org.OrganisationName,
                OrganisationId = org.OrganisationId,
                CourseTitle = course.Name,
                AccreditingProviderId = course.AccreditingProviderId,
                MultipleOrganisations = multipleOrganisations,
                Course = courseVariant,
                CourseEnrichment = courseEnrichmentViewModel,
                RouteData = routeData,
                LiveSearchUrl = searchAndCompareUrlService.GetCoursePageUri(org.UcasCode, courseVariant.ProgrammeCode),
                AllowPreview = featureFlags.ShowCoursePreview,
                AllowPublish = featureFlags.ShowCoursePublish,
                AllowLiveView = featureFlags.ShowCourseLiveView,
                IsSalary = course.ProgramType.Equals("SS", StringComparison.InvariantCultureIgnoreCase)
            };
            return viewModel;
        }

        private static CourseEnrichmentViewModel GetCourseEnrichmentViewModel(UcasCourseEnrichmentGetModel ucasCourseEnrichmentGetModel)
        {
            if (ucasCourseEnrichmentGetModel == null)
            {
                return new CourseEnrichmentViewModel();
            }
            var enrichmentModel = ucasCourseEnrichmentGetModel?.EnrichmentModel ?? new CourseEnrichmentModel();

            var result = new CourseEnrichmentViewModel()
            {
                AboutCourse = enrichmentModel.AboutCourse,
                InterviewProcess = enrichmentModel.InterviewProcess,
                HowSchoolPlacementsWork = enrichmentModel.HowSchoolPlacementsWork,
                Qualifications = enrichmentModel.Qualifications,
                PersonalQualities = enrichmentModel.PersonalQualities,
                OtherRequirements = enrichmentModel.OtherRequirements,
                CourseLength = enrichmentModel.CourseLength.GetCourseLength(),
                FeeUkEu = enrichmentModel.FeeUkEu,
                FeeInternational = enrichmentModel.FeeInternational,
                FeeDetails = enrichmentModel.FeeDetails,
                FinancialSupport = enrichmentModel.FinancialSupport,
                SalaryDetails = enrichmentModel.SalaryDetails,
                DraftLastUpdatedUtc = ucasCourseEnrichmentGetModel.UpdatedTimestampUtc,
                LastPublishedUtc = ucasCourseEnrichmentGetModel.LastPublishedTimestampUtc
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
