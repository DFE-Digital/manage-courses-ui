using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GovUk.Education.ManageCourses.Api.Model;
using GovUk.Education.ManageCourses.Domain.Models;
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

        public CourseController(IManageApi manageApi, ISearchAndCompareUrlService searchAndCompareUrlHelper)
        {
            _manageApi = manageApi;
            this.searchAndCompareUrlService = searchAndCompareUrlHelper;
        }

        [Route("{providerCode}/course/{accreditingProviderCode=self}/{courseCode}")]
        public async Task<IActionResult> Show(string providerCode, string accreditingProviderCode, string courseCode)
        {
            Validate(providerCode, accreditingProviderCode, courseCode);

            var orgsList = await _manageApi.GetProviderSummaries();
            var userOrganisations = orgsList.ToList();
            var multipleOrganisations = userOrganisations.Count() > 1;
            var org = userOrganisations.ToList().FirstOrDefault(x => providerCode.ToLower() == x.ProviderCode.ToLower());

            if (org == null) { return NotFound(); }

            var course = await _manageApi.GetCourse(providerCode, courseCode);

            if (course == null) return NotFound();

            var ucasCourseEnrichmentGetModel = await _manageApi.GetCourseEnrichment(providerCode, courseCode);

            var routeData = GetCourseRouteDataViewModel(providerCode, courseCode);

            var viewModel = LoadViewModel(org, course, multipleOrganisations, ucasCourseEnrichmentGetModel, routeData);

            return View("Show", viewModel);
        }

        [HttpPost]
        [Route("{providerCode}/course/{accreditingProviderCode=self}/{courseCode}", Name = "publish")]
        public async Task<IActionResult> ShowPublish(string providerCode, string accreditingProviderCode, string courseCode)
        {
            var course = await _manageApi.GetCourse(providerCode, courseCode);
            var isSalary = course.ProgramType.Equals("SS", StringComparison.InvariantCultureIgnoreCase)
                        || course.ProgramType.Equals("TA", StringComparison.InvariantCultureIgnoreCase);
            var enrichment = await _manageApi.GetCourseEnrichment(providerCode, courseCode);
            var enrichmentModel = GetCourseEnrichmentViewModel(enrichment, isSalary);

            ModelState.Clear();
            TryValidateModel(enrichmentModel);

            if (!ModelState.IsValid)
            {
                return await Show(providerCode, accreditingProviderCode, courseCode);
            }

            var result = await _manageApi.PublishCourseToSearchAndCompare(providerCode, courseCode);

            if (result)
            {
                TempData["MessageType"] = "success";
                TempData["MessageTitle"] = "Your course has been published";
                var searchUrl = searchAndCompareUrlService.GetCoursePageUri(course.Provider.ProviderCode, course.CourseCode);
                TempData["MessageBodyHtml"] = $@"
                    <p class=""govuk-body"">
                        The link for this course is:
                        <br />
                        <a href='{searchUrl}'>{searchUrl}</a>
                    </p>";
            }

            return RedirectToAction("Show", new { providerCode = providerCode, accreditingProviderCode = accreditingProviderCode, courseCode });
        }

        [Route("{providerCode}/course/{accreditingProviderCode=self}/{courseCode}/preview")]
        public IActionResult Preview(string providerCode, string accreditingProviderCode, string courseCode)
        {
            var course = _manageApi.GetSearchAndCompareCourse(providerCode, courseCode).Result;
            if (course == null)
            {
                return NotFound();
            }

            return View(new SearchAndCompare.UI.Shared.ViewModels.CourseDetailsViewModel
            {
                AboutYourOrgLink = Url.Action("About", "Organisation", new { providerCode = providerCode }),
                PreviewMode = true,
                Course = course,
                Finance = new FinanceViewModel(course, new FeeCaps())
            });
        }

        [HttpGet]
        [Route("{providerCode}/course/{accreditingProviderCode=self}/{courseCode}/about")]
        public async Task<IActionResult> About(string providerCode, string accreditingProviderCode, string courseCode, string copyFrom = null)
        {
            var courseDetails = await _manageApi.GetCourse(providerCode, courseCode);
            var ucasCourseEnrichmentGetModel = await _manageApi.GetCourseEnrichment(providerCode, courseCode);

            var routeData = GetCourseRouteDataViewModel(providerCode, courseCode);
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

            await LoadCopyableCoursesIntoViewBag(providerCode, courseCode);

            if (!string.IsNullOrEmpty(copyFrom))
            {
                copyFrom = copyFrom.ToUpper();
                var copiedEnrichment = await _manageApi.GetCourseEnrichment(providerCode, copyFrom);
                ViewBag.CopiedFrom = new CourseInfoViewModel
                {
                    ProgrammeCode = copyFrom,
                    Name = (ViewBag.CopyableCourses as IEnumerable<Domain.Models.Course>).SingleOrDefault(x => x.CourseCode == copyFrom)?.Name
                };

                ViewBag.CopiedFields = model.CopyFrom(copiedEnrichment?.EnrichmentModel);
            }


            return View(model);
        }

        private async Task LoadCopyableCoursesIntoViewBag(string providerCode, string courseCode)
        {
            providerCode = providerCode.ToUpper();
            courseCode = courseCode.ToUpper();

            var copyable = await _manageApi.GetCoursesOfProvider(providerCode);
            ViewBag.CopyableCourses = copyable != null ? copyable.Where(x => x.EnrichmentWorkflowStatus != null && x.CourseCode != courseCode) : new List<Domain.Models.Course>();
        }

        [HttpPost]
        [Route("{providerCode}/course/{accreditingProviderCode=self}/{courseCode}/about")]
        public async Task<IActionResult> AboutPost(string providerCode, string accreditingProviderCode, string courseCode, AboutCourseEnrichmentViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                var courseDetails = await _manageApi.GetCourse(providerCode, courseCode);
                var courseInfo = new CourseInfoViewModel { ProgrammeCode = courseDetails.CourseCode, Name = courseDetails.Name };
                var routeData = GetCourseRouteDataViewModel(providerCode, courseCode);
                viewModel.RouteData = routeData;
                viewModel.CourseInfo = courseInfo;
                await LoadCopyableCoursesIntoViewBag(providerCode, courseCode);
                return View("About", viewModel);
            }

            if (await SaveEnrichment(providerCode, courseCode, viewModel))
            {
                CourseSavedMessage();
            }

            return RedirectToAction("Show", new { providerCode = providerCode, accreditingProviderCode = accreditingProviderCode, courseCode });
        }

        [HttpGet]
        [Route("{providerCode}/course/{accreditingProviderCode=self}/{courseCode}/requirements")]
        public async Task<IActionResult> Requirements(string providerCode, string accreditingProviderCode, string courseCode, string copyFrom = null)
        {
            var courseDetails = await _manageApi.GetCourse(providerCode, courseCode);
            var ucasCourseEnrichmentGetModel = await _manageApi.GetCourseEnrichment(providerCode, courseCode);
            var routeData = GetCourseRouteDataViewModel(providerCode, courseCode);
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

            await LoadCopyableCoursesIntoViewBag(providerCode, courseCode);

            if (!string.IsNullOrEmpty(copyFrom))
            {
                copyFrom = copyFrom.ToUpper();
                var copiedEnrichment = await _manageApi.GetCourseEnrichment(providerCode, copyFrom);
                ViewBag.CopiedFrom = new CourseInfoViewModel
                {
                    ProgrammeCode = copyFrom,
                    Name = (ViewBag.CopyableCourses as IEnumerable<Domain.Models.Course>).SingleOrDefault(x => x.CourseCode == copyFrom)?.Name
                };

                ViewBag.CopiedFields = model.CopyFrom(copiedEnrichment?.EnrichmentModel);
            }

            return View(model);
        }

        [HttpPost]
        [Route("{providerCode}/course/{accreditingProviderCode=self}/{courseCode}/requirements")]
        public async Task<IActionResult> RequirementsPost(string providerCode, string accreditingProviderCode, string courseCode, CourseRequirementsEnrichmentViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                var courseDetails = await _manageApi.GetCourse(providerCode, courseCode);
                var courseInfo = new CourseInfoViewModel { ProgrammeCode = courseDetails.CourseCode, Name = courseDetails.Name };
                var routeData = GetCourseRouteDataViewModel(providerCode, courseCode);
                viewModel.RouteData = routeData;
                viewModel.CourseInfo = courseInfo;
                await LoadCopyableCoursesIntoViewBag(providerCode, courseCode);
                return View("Requirements", viewModel);
            }

            if (await SaveEnrichment(providerCode, courseCode, viewModel))
            {
                CourseSavedMessage();
            }

            return RedirectToAction("Show", new { providerCode = providerCode, accreditingProviderCode = accreditingProviderCode, courseCode });
        }

        [HttpGet]
        [Route("{providerCode}/course/{accreditingProviderCode=self}/{courseCode}/salary")]
        public async Task<IActionResult> Salary(string providerCode, string accreditingProviderCode, string courseCode, string copyFrom = null)
        {
            var courseDetails = await _manageApi.GetCourse(providerCode, courseCode);
            var ucasCourseEnrichmentGetModel = await _manageApi.GetCourseEnrichment(providerCode, courseCode);
            var routeData = GetCourseRouteDataViewModel(providerCode, courseCode);
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

            await LoadCopyableCoursesIntoViewBag(providerCode, courseCode);

            if (!string.IsNullOrEmpty(copyFrom))
            {
                copyFrom = copyFrom.ToUpper();
                var copiedEnrichment = await _manageApi.GetCourseEnrichment(providerCode, copyFrom);
                ViewBag.CopiedFrom = new CourseInfoViewModel
                {
                    ProgrammeCode = copyFrom,
                    Name = (ViewBag.CopyableCourses as IEnumerable<Domain.Models.Course>).SingleOrDefault(x => x.CourseCode == copyFrom)?.Name
                };

                ViewBag.CopiedFields = model.CopyFrom(copiedEnrichment?.EnrichmentModel);
            }

            return View(model);
        }

        [HttpPost]
        [Route("{providerCode}/course/{accreditingProviderCode=self}/{courseCode}/salary")]
        public async Task<IActionResult> SalaryPost(string providerCode, string accreditingProviderCode, string courseCode, CourseSalaryEnrichmentViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                var routeData = GetCourseRouteDataViewModel(providerCode, courseCode);
                var courseDetails = await _manageApi.GetCourse(providerCode, courseCode);
                var courseInfo = new CourseInfoViewModel { ProgrammeCode = courseDetails.CourseCode, Name = courseDetails.Name };
                viewModel.RouteData = routeData;
                viewModel.CourseInfo = courseInfo;
                await LoadCopyableCoursesIntoViewBag(providerCode, courseCode);
                return View("Salary", viewModel);
            }
            if (await SaveEnrichment(providerCode, courseCode, viewModel))
            {
                CourseSavedMessage();
            }
            return RedirectToAction("Show", new { providerCode = providerCode, accreditingProviderCode = accreditingProviderCode, courseCode });
        }

        [HttpGet]
        [Route("{providerCode}/course/{accreditingProviderCode=self}/{courseCode}/fees-and-length")]
        public async Task<IActionResult> Fees(string providerCode, string accreditingProviderCode, string courseCode, string copyFrom = null)
        {
            var courseDetails = await _manageApi.GetCourse(providerCode, courseCode);
            var ucasCourseEnrichmentGetModel = await _manageApi.GetCourseEnrichment(providerCode, courseCode);
            var routeData = GetCourseRouteDataViewModel(providerCode, courseCode);
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

            await LoadCopyableCoursesIntoViewBag(providerCode, courseCode);

            if (!string.IsNullOrEmpty(copyFrom))
            {
                copyFrom = copyFrom.ToUpper();
                var copiedEnrichment = await _manageApi.GetCourseEnrichment(providerCode, copyFrom);
                ViewBag.CopiedFrom = new CourseInfoViewModel
                {
                    ProgrammeCode = copyFrom,
                    Name = (ViewBag.CopyableCourses as IEnumerable<Domain.Models.Course>).SingleOrDefault(x => x.CourseCode == copyFrom)?.Name
                };

                ViewBag.CopiedFields = model.CopyFrom(copiedEnrichment?.EnrichmentModel);
            }

            return View(model);
        }

        [HttpPost]
        [Route("{providerCode}/course/{accreditingProviderCode=self}/{courseCode}/fees-and-length")]
        public async Task<IActionResult> FeesPost(string providerCode, string accreditingProviderCode, string courseCode, CourseFeesEnrichmentViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                var routeData = GetCourseRouteDataViewModel(providerCode, courseCode);
                var courseDetails = await _manageApi.GetCourse(providerCode, courseCode);
                var courseInfo = new CourseInfoViewModel { ProgrammeCode = courseDetails.CourseCode, Name = courseDetails.Name };
                viewModel.RouteData = routeData;
                viewModel.CourseInfo = courseInfo;
                await LoadCopyableCoursesIntoViewBag(providerCode, courseCode);
                return View("Fees", viewModel);
            }
            if (await SaveEnrichment(providerCode, courseCode, viewModel))
            {
                CourseSavedMessage();
            }
            return RedirectToAction("Show", new { providerCode = providerCode, accreditingProviderCode = accreditingProviderCode, courseCode });
        }

        private async Task<bool> SaveEnrichment(string providerCode, string courseCode, ICourseEnrichmentViewModel viewModel)
        {
            var course = await _manageApi.GetCourseEnrichment(providerCode, courseCode);

            if (course == null && viewModel.IsEmpty())
            {
                // Draft state is "New" and no changes have been made - don't insert a draft
                return false;
            }

            var enrichmentModel = course?.EnrichmentModel ?? new CourseEnrichmentModel();
            viewModel.MapInto(ref enrichmentModel);

            await _manageApi.SaveCourseEnrichment(providerCode, courseCode, enrichmentModel);
            return true;
        }

        private void Validate(string providerCode, string accreditingProviderCode, string courseCode)
        {
            if (string.IsNullOrEmpty(providerCode)) { throw new ArgumentNullException(providerCode, "providerCode cannot be null or empty"); }
            if (string.IsNullOrEmpty(accreditingProviderCode)) { throw new ArgumentNullException(accreditingProviderCode, "accreditingProviderCode cannot be null or empty"); }
            if (string.IsNullOrEmpty(courseCode)) { throw new ArgumentNullException(courseCode, "courseCode cannot be null or empty"); }
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

        private CourseViewModel LoadViewModel(ProviderSummary org, Domain.Models.Course course, bool multipleOrganisations, UcasCourseEnrichmentGetModel ucasCourseEnrichmentGetModel, CourseRouteDataViewModel routeData)
        {
            var courseVariant =
                new ViewModels.CourseDetailsViewModel
                {
                    CourseTitle = course.Name,
                    Type = course.TypeDescription,
                    AccreditingProviderName = course.AccreditingProvider?.ProviderName,
                    AccreditingProviderCode = course.AccreditingProvider?.ProviderCode,
                    CourseCode = course.CourseCode,
                    ProviderCode = course.Provider.ProviderCode,
                    AgeRange = course.AgeRange,
                    Route = course.ProgramType,
                    Qualifications = course.ProfpostFlag,
                    StudyMode = course.StudyMode,
                    Subjects = course.Subjects,
                    Status = course.GetCourseStatus(),
                    Sites = course.CourseSites.Select(courseSite =>
                    {
                        var addressLines = (new List<string>()
                            {
                                    courseSite.Site.Address1,
                                    courseSite.Site.Address2,
                                    courseSite.Site.Address3,
                                    courseSite.Site.Address4,
                                    courseSite.Site.Postcode
                            })
                            .Where(line => !string.IsNullOrEmpty(line));

                        var address = addressLines.Count() > 0 ? addressLines.Where(line => !string.IsNullOrEmpty(line))
                            .Aggregate((string current, string next) => current + ", " + next) : "";

                        return new SiteViewModel
                        {
                            ApplicationsAcceptedFrom = courseSite.ApplicationsAcceptedFrom,
                            Code = courseSite.Site.Code,
                            LocationName = courseSite.Site.LocationName,
                            Address = address,
                            Status = courseSite.Status,
                            VacStatus = courseSite.VacStatus
                        };
                    })
                };

            var isSalary = course.ProgramType.Equals("SS", StringComparison.InvariantCultureIgnoreCase)
                        || course.ProgramType.Equals("TA", StringComparison.InvariantCultureIgnoreCase);
            var courseEnrichmentViewModel = GetCourseEnrichmentViewModel(ucasCourseEnrichmentGetModel, isSalary, routeData);
            var viewModel = new CourseViewModel
            {
                ProviderName = org.ProviderName,
                ProviderCode = org.ProviderCode,
                CourseTitle = course.Name,
                AccreditingProviderCode = course.AccreditingProvider?.ProviderCode,
                MultipleOrganisations = multipleOrganisations,
                Course = courseVariant,
                CourseEnrichment = courseEnrichmentViewModel,
                LiveSearchUrl = searchAndCompareUrlService.GetCoursePageUri(org.ProviderCode, courseVariant.CourseCode),
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

        private CourseRouteDataViewModel GetCourseRouteDataViewModel(string providerCode, string courseCode)
        {
            return new CourseRouteDataViewModel
            {
                ProviderCode = providerCode,
                CourseCode = courseCode
            };
        }
    }
}
