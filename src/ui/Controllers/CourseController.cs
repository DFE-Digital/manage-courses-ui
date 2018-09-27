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
        private readonly ISearchAndCompareUrlService searchAndCompareUrlService;
        private readonly IFeatureFlags featureFlags;

        public CourseController(IManageApi manageApi, ISearchAndCompareUrlService searchAndCompareUrlHelper, IFeatureFlags featureFlags)
        {
            _manageApi = manageApi;
            this.searchAndCompareUrlService = searchAndCompareUrlHelper;
            this.featureFlags = featureFlags;
        }

        [Route("{instCode}/course/{accreditingProviderId=self}/{ucasCode}")]
        public async Task<IActionResult> Show(string instCode, string accreditingProviderId, string ucasCode)
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

            return View("Show", viewModel);
        }

        [HttpPost]
        [Route("{instCode}/course/{accreditingProviderId=self}/{ucasCode}", Name = "publish")]
        public async Task<IActionResult> ShowPublish(string instCode, string accreditingProviderId, string ucasCode)
        {
            var course = await _manageApi.GetCourseByUcasCode(instCode, ucasCode);
            var isSalary = course.ProgramType.Equals("SS", StringComparison.InvariantCultureIgnoreCase)
                        || course.ProgramType.Equals("TA", StringComparison.InvariantCultureIgnoreCase);
            var enrichment = await _manageApi.GetEnrichmentCourse(instCode, ucasCode);
            var enrichmentModel = GetCourseEnrichmentViewModel(enrichment, isSalary);

            ModelState.Clear();
            TryValidateModel(enrichmentModel);

            if (!ModelState.IsValid)
            {
                return await Show(instCode, accreditingProviderId, ucasCode);
            }

            var result = await _manageApi.PublishCourseToSearchAndCompare(instCode, ucasCode);

            if (result)
            {
                TempData["MessageType"] = "success";

                if (featureFlags.ShowCourseLiveView)
                {
                    TempData["MessageTitle"] = "Your changes will be published within 1 working day, usually within 2 hours";
                    var searchUrl = searchAndCompareUrlService.GetCoursePageUri(course.InstCode, course.CourseCode);
                    TempData["MessageBodyHtml"] = $@"
                        <p class=""govuk-body"">
                            The link for this course is:
                            <br />
                            <a href='{searchUrl}'>{searchUrl}</a>
                        </p>";
                }
                else
                {
                    TempData["MessageTitle"] = "Your course has been published";
                }
            }

            return RedirectToAction("Show", new { instCode, accreditingProviderId, ucasCode });
        }

        [Route("{instCode}/course/{accreditingProviderId=self}/{ucasCode}/preview")]
        public IActionResult Preview(string instCode, string accreditingProviderId, string ucasCode)
        {
            var course = _manageApi.GetSearchAndCompareCourse(instCode, ucasCode).Result;
            if (course == null)
            {
                return NotFound();
            }

            return View(new SearchAndCompare.UI.Shared.ViewModels.CourseDetailsViewModel
            {
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

            await LoadCopyableCoursesIntoViewBag(instCode, ucasCode);

            if (!string.IsNullOrEmpty(copyFrom))
            {
                copyFrom = copyFrom.ToUpper();
                var copiedEnrichment = await _manageApi.GetEnrichmentCourse(instCode, copyFrom);
                ViewBag.CopiedFrom = new CourseInfoViewModel {
                    ProgrammeCode = copyFrom,
                    Name = (ViewBag.CopyableCourses as IEnumerable<ApiClient.Course>).SingleOrDefault(x => x.CourseCode == copyFrom)?.Name
                };

                ViewBag.CopiedFields = model.CopyFrom(copiedEnrichment?.EnrichmentModel);
            }


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
                await LoadCopyableCoursesIntoViewBag(instCode, ucasCode);
                return View("About", viewModel);
            }

            if (await SaveEnrichment(instCode, ucasCode, viewModel))
            {
                CourseSavedMessage();
            }

            return RedirectToAction("Show", new { instCode, accreditingProviderId, ucasCode });
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

            await LoadCopyableCoursesIntoViewBag(instCode, ucasCode);

            if (!string.IsNullOrEmpty(copyFrom))
            {
                copyFrom = copyFrom.ToUpper();
                var copiedEnrichment = await _manageApi.GetEnrichmentCourse(instCode, copyFrom);
                ViewBag.CopiedFrom = new CourseInfoViewModel {
                    ProgrammeCode = copyFrom,
                    Name = (ViewBag.CopyableCourses as IEnumerable<ApiClient.Course>).SingleOrDefault(x => x.CourseCode == copyFrom)?.Name
                };

                ViewBag.CopiedFields = model.CopyFrom(copiedEnrichment?.EnrichmentModel);
            }

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
                await LoadCopyableCoursesIntoViewBag(instCode, ucasCode);
                return View("Requirements", viewModel);
            }

            if (await SaveEnrichment(instCode, ucasCode, viewModel))
            {
                CourseSavedMessage();
            }

            return RedirectToAction("Show", new { instCode, accreditingProviderId, ucasCode });
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
                CourseLengthInput = enrichmentModel.CourseLength.GetCourseLengthInput(),
                SalaryDetails = enrichmentModel?.SalaryDetails,
                RouteData = routeData,
                CourseInfo = courseInfo
            };

            await LoadCopyableCoursesIntoViewBag(instCode, ucasCode);

            if (!string.IsNullOrEmpty(copyFrom))
            {
                copyFrom = copyFrom.ToUpper();
                var copiedEnrichment = await _manageApi.GetEnrichmentCourse(instCode, copyFrom);
                ViewBag.CopiedFrom = new CourseInfoViewModel {
                    ProgrammeCode = copyFrom,
                    Name = (ViewBag.CopyableCourses as IEnumerable<ApiClient.Course>).SingleOrDefault(x => x.CourseCode == copyFrom)?.Name
                };

                ViewBag.CopiedFields = model.CopyFrom(copiedEnrichment?.EnrichmentModel);
            }

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
                await LoadCopyableCoursesIntoViewBag(instCode, ucasCode);
                return View("Salary", viewModel);
            }
            if (await SaveEnrichment(instCode, ucasCode, viewModel))
            {
                CourseSavedMessage();
            }
            return RedirectToAction("Show", new { instCode, accreditingProviderId, ucasCode });
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
		        CourseLengthInput = enrichmentModel.CourseLength.GetCourseLengthInput(),
		        FeeUkEu = enrichmentModel?.FeeUkEu.GetFeeValue(),
                FeeInternational = enrichmentModel?.FeeInternational.GetFeeValue(),
                FeeDetails = enrichmentModel?.FeeDetails,
                FinancialSupport = enrichmentModel?.FinancialSupport,
                RouteData = routeData,
                CourseInfo = courseInfo
            };

            await LoadCopyableCoursesIntoViewBag(instCode, ucasCode);

            if (!string.IsNullOrEmpty(copyFrom))
            {
                copyFrom = copyFrom.ToUpper();
                var copiedEnrichment = await _manageApi.GetEnrichmentCourse(instCode, copyFrom);
                ViewBag.CopiedFrom = new CourseInfoViewModel {
                    ProgrammeCode = copyFrom,
                    Name = (ViewBag.CopyableCourses as IEnumerable<ApiClient.Course>).SingleOrDefault(x => x.CourseCode == copyFrom)?.Name
                };

                ViewBag.CopiedFields = model.CopyFrom(copiedEnrichment?.EnrichmentModel);
            }

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
                await LoadCopyableCoursesIntoViewBag(instCode, ucasCode);
                return View("Fees", viewModel);
            }
            if (await SaveEnrichment(instCode, ucasCode, viewModel))
            {
                CourseSavedMessage();
            }
            return RedirectToAction("Show", new { instCode, accreditingProviderId, ucasCode });
        }

        private async Task<bool> SaveEnrichment(string instCode, string ucasCode, ICourseEnrichmentViewModel viewModel)
        {
            var course = await _manageApi.GetEnrichmentCourse(instCode, ucasCode);

            if (course == null && viewModel.IsEmpty())
            {
                // Draft state is "New" and no changes have been made - don't insert a draft
                return false;
            }

            var enrichmentModel = course?.EnrichmentModel ?? new CourseEnrichmentModel();
            viewModel.MapInto(ref enrichmentModel);

            await _manageApi.SaveEnrichmentCourse(instCode, ucasCode, enrichmentModel);
            return true;
        }

        private void Validate(string instCode, string accreditingProviderId, string ucasCode)
        {
            if (string.IsNullOrEmpty(instCode)) { throw new ArgumentNullException(instCode, "instCode cannot be null or empty"); }
            if (string.IsNullOrEmpty(accreditingProviderId)) { throw new ArgumentNullException(accreditingProviderId, "accreditingProviderId cannot be null or empty"); }
            if (string.IsNullOrEmpty(ucasCode)) { throw new ArgumentNullException(ucasCode, "ucasCode cannot be null or empty"); }
        }

        private void CourseSavedMessage()
        {
            TempData["MessageType"] = "success";
            TempData["MessageTitle"] = "Your changes have been saved";
            var previewLink = Url.Action("Preview");
            var messageBodyHtml = $@"
                <p class=""govuk-body"">
                    <a href='{previewLink}'>Preview your course</a>
                    to check for mistakes before publishing.
                </p>";
            TempData["MessageBodyHtml"] = messageBodyHtml;
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

            var isSalary = course.ProgramType.Equals("SS", StringComparison.InvariantCultureIgnoreCase)
                        || course.ProgramType.Equals("TA", StringComparison.InvariantCultureIgnoreCase);
            var courseEnrichmentViewModel = GetCourseEnrichmentViewModel(ucasCourseEnrichmentGetModel, isSalary, routeData);
            var viewModel = new VariantViewModel
            {
                OrganisationName = org.OrganisationName,
                OrganisationId = org.OrganisationId,
                CourseTitle = course.Name,
                AccreditingProviderId = course.AccreditingProviderId,
                MultipleOrganisations = multipleOrganisations,
                Course = courseVariant,
                CourseEnrichment = courseEnrichmentViewModel,
                LiveSearchUrl = searchAndCompareUrlService.GetCoursePageUri(org.UcasCode, courseVariant.ProgrammeCode),
                AllowLiveView = featureFlags.ShowCourseLiveView,
                IsSalary = isSalary
            };
            return viewModel;
        }

        private static BaseCourseEnrichmentViewModel GetCourseEnrichmentViewModel(UcasCourseEnrichmentGetModel ucasCourseEnrichmentGetModel, bool isSalary, CourseRouteDataViewModel routeData = null)
        {
            BaseCourseEnrichmentViewModel result = null;

            ucasCourseEnrichmentGetModel = ucasCourseEnrichmentGetModel ?? new UcasCourseEnrichmentGetModel();

            var enrichmentModel = ucasCourseEnrichmentGetModel?.EnrichmentModel ?? new CourseEnrichmentModel();

            if (isSalary)
            {

                result = new SalaryBasedCourseEnrichmentViewModel()
                {
                    AboutCourse = enrichmentModel.AboutCourse,
                    InterviewProcess = enrichmentModel.InterviewProcess,
                    HowSchoolPlacementsWork = enrichmentModel.HowSchoolPlacementsWork,
                    Qualifications = enrichmentModel.Qualifications,
                    PersonalQualities = enrichmentModel.PersonalQualities,
                    OtherRequirements = enrichmentModel.OtherRequirements,
                    CourseLength = enrichmentModel.CourseLength.GetCourseLength(),
                    CourseLengthInput = enrichmentModel.CourseLength.GetCourseLengthInput(),
                    SalaryDetails = enrichmentModel.SalaryDetails,
                    DraftLastUpdatedUtc = ucasCourseEnrichmentGetModel.UpdatedTimestampUtc,
                    LastPublishedUtc = ucasCourseEnrichmentGetModel.LastPublishedTimestampUtc,
                    RouteData = routeData
                };
            }
            else
            {
                result = new FeeBasedCourseEnrichmentViewModel()
                {
                    AboutCourse = enrichmentModel.AboutCourse,
                    InterviewProcess = enrichmentModel.InterviewProcess,
                    HowSchoolPlacementsWork = enrichmentModel.HowSchoolPlacementsWork,
                    Qualifications = enrichmentModel.Qualifications,
                    PersonalQualities = enrichmentModel.PersonalQualities,
                    OtherRequirements = enrichmentModel.OtherRequirements,
                    CourseLength = enrichmentModel.CourseLength.GetCourseLength(),
                    CourseLengthInput = enrichmentModel.CourseLength.GetCourseLengthInput(),
                    FeeUkEu = enrichmentModel.FeeUkEu.GetFeeValue(),
                    FeeInternational = enrichmentModel.FeeInternational.GetFeeValue(),
                    FeeDetails = enrichmentModel.FeeDetails,
                    FinancialSupport = enrichmentModel.FinancialSupport,
                    DraftLastUpdatedUtc = ucasCourseEnrichmentGetModel.UpdatedTimestampUtc,
                    LastPublishedUtc = ucasCourseEnrichmentGetModel.LastPublishedTimestampUtc,
                    RouteData = routeData
                };
            }

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
