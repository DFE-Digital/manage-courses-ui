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

        public CourseController(IManageApi manageApi, ISearchAndCompareUrlService searchAndCompareUrlHelper)
        {
            _manageApi = manageApi;
            this.searchAndCompareUrlService = searchAndCompareUrlHelper;
        }

        [Route("{instCode}/course/{accreditingInstCode=self}/{courseCode}")]
        public async Task<IActionResult> Show(string instCode, string accreditingInstCode, string courseCode)
        {
            Validate(instCode, accreditingInstCode, courseCode);

            var orgsList = await _manageApi.GetInstitutionSummaries();
            var userOrganisations = orgsList.ToList();
            var multipleOrganisations = userOrganisations.Count() > 1;
            var org = userOrganisations.ToList().FirstOrDefault(x => instCode.ToLower() == x.UcasCode.ToLower());

            if (org == null) { return NotFound($"Organisation with code '{courseCode}' not found"); }

            var course = await _manageApi.GetCourseByCode(instCode, courseCode);

            if (course == null) return NotFound();

            var ucasCourseEnrichmentGetModel = await _manageApi.GetCourseEnrichment(instCode, courseCode);

            var routeData = GetCourseRouteDataViewModel(instCode, courseCode);

            var viewModel = LoadViewModel(org, course, multipleOrganisations, ucasCourseEnrichmentGetModel, routeData);

            return View("Show", viewModel);
        }

        [HttpPost]
        [Route("{instCode}/course/{accreditingInstCode=self}/{courseCode}", Name = "publish")]
        public async Task<IActionResult> ShowPublish(string instCode, string accreditingInstCode, string courseCode)
        {
            var course = await _manageApi.GetCourseByCode(instCode, courseCode);
            var isSalary = course.ProgramType.Equals("SS", StringComparison.InvariantCultureIgnoreCase)
                        || course.ProgramType.Equals("TA", StringComparison.InvariantCultureIgnoreCase);
            var enrichment = await _manageApi.GetCourseEnrichment(instCode, courseCode);
            var enrichmentModel = GetCourseEnrichmentViewModel(enrichment, isSalary);

            ModelState.Clear();
            TryValidateModel(enrichmentModel);

            if (!ModelState.IsValid)
            {
                return await Show(instCode, accreditingInstCode, courseCode);
            }

            var result = await _manageApi.PublishCourseToSearchAndCompare(instCode, courseCode);

            if (result)
            {
                TempData["MessageType"] = "success";
                TempData["MessageTitle"] = "Your course has been published";
                var searchUrl = searchAndCompareUrlService.GetCoursePageUri(course.InstCode, course.CourseCode);
                TempData["MessageBodyHtml"] = $@"
                    <p class=""govuk-body"">
                        The link for this course is:
                        <br />
                        <a href='{searchUrl}'>{searchUrl}</a>
                    </p>";
            }

            return RedirectToAction("Show", new { instCode, accreditingInstCode, courseCode });
        }

        [Route("{instCode}/course/{accreditingInstCode=self}/{courseCode}/preview")]
        public IActionResult Preview(string instCode, string accreditingInstCode, string courseCode)
        {
            var course = _manageApi.GetSearchAndCompareCourse(instCode, courseCode).Result;
            if (course == null)
            {
                return NotFound();
            }

            return View(new SearchAndCompare.UI.Shared.ViewModels.CourseDetailsViewModel
            {
                AboutYourOrgLink = Url.Action("About", "Organisation", new { instCode = instCode }),
                PreviewMode = true,
                Course = course,
                Finance = new FinanceViewModel(course, new FeeCaps())
            });
        }

        [HttpGet]
        [Route("{instCode}/course/{accreditingInstCode=self}/{courseCode}/about")]
        public async Task<IActionResult> About(string instCode, string accreditingInstCode, string courseCode, string copyFrom = null)
        {
            var courseDetails = await _manageApi.GetCourseByCode(instCode, courseCode);
            var ucasCourseEnrichmentGetModel = await _manageApi.GetCourseEnrichment(instCode, courseCode);

            var routeData = GetCourseRouteDataViewModel(instCode, courseCode);
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

            await LoadCopyableCoursesIntoViewBag(instCode, courseCode);

            if (!string.IsNullOrEmpty(copyFrom))
            {
                copyFrom = copyFrom.ToUpper();
                var copiedEnrichment = await _manageApi.GetCourseEnrichment(instCode, copyFrom);
                ViewBag.CopiedFrom = new CourseInfoViewModel
                {
                    ProgrammeCode = copyFrom,
                    Name = (ViewBag.CopyableCourses as IEnumerable<ApiClient.Course>).SingleOrDefault(x => x.CourseCode == copyFrom)?.Name
                };

                ViewBag.CopiedFields = model.CopyFrom(copiedEnrichment?.EnrichmentModel);
            }


            return View(model);
        }

        private async Task LoadCopyableCoursesIntoViewBag(string instCode, string courseCode)
        {
            instCode = instCode.ToUpper();
            courseCode = courseCode.ToUpper();

            var copyable = await _manageApi.GetCoursesOfInstitution(instCode);
            ViewBag.CopyableCourses = copyable != null ? copyable.Courses.Where(x => x.EnrichmentWorkflowStatus != null && x.CourseCode != courseCode) : new List<ApiClient.Course>();
        }

        [HttpPost]
        [Route("{instCode}/course/{accreditingInstCode=self}/{courseCode}/about")]
        public async Task<IActionResult> AboutPost(string instCode, string accreditingInstCode, string courseCode, AboutCourseEnrichmentViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                var courseDetails = await _manageApi.GetCourseByCode(instCode, courseCode);
                var courseInfo = new CourseInfoViewModel { ProgrammeCode = courseDetails.CourseCode, Name = courseDetails.Name };
                var routeData = GetCourseRouteDataViewModel(instCode, courseCode);
                viewModel.RouteData = routeData;
                viewModel.CourseInfo = courseInfo;
                await LoadCopyableCoursesIntoViewBag(instCode, courseCode);
                return View("About", viewModel);
            }

            if (await SaveEnrichment(instCode, courseCode, viewModel))
            {
                CourseSavedMessage();
            }

            return RedirectToAction("Show", new { instCode, accreditingInstCode, courseCode });
        }

        [HttpGet]
        [Route("{instCode}/course/{accreditingInstCode=self}/{courseCode}/requirements")]
        public async Task<IActionResult> Requirements(string instCode, string accreditingInstCode, string courseCode, string copyFrom = null)
        {
            var courseDetails = await _manageApi.GetCourseByCode(instCode, courseCode);
            var ucasCourseEnrichmentGetModel = await _manageApi.GetCourseEnrichment(instCode, courseCode);
            var routeData = GetCourseRouteDataViewModel(instCode, courseCode);
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

            await LoadCopyableCoursesIntoViewBag(instCode, courseCode);

            if (!string.IsNullOrEmpty(copyFrom))
            {
                copyFrom = copyFrom.ToUpper();
                var copiedEnrichment = await _manageApi.GetCourseEnrichment(instCode, copyFrom);
                ViewBag.CopiedFrom = new CourseInfoViewModel
                {
                    ProgrammeCode = copyFrom,
                    Name = (ViewBag.CopyableCourses as IEnumerable<ApiClient.Course>).SingleOrDefault(x => x.CourseCode == copyFrom)?.Name
                };

                ViewBag.CopiedFields = model.CopyFrom(copiedEnrichment?.EnrichmentModel);
            }

            return View(model);
        }

        [HttpPost]
        [Route("{instCode}/course/{accreditingInstCode=self}/{courseCode}/requirements")]
        public async Task<IActionResult> RequirementsPost(string instCode, string accreditingInstCode, string courseCode, CourseRequirementsEnrichmentViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                var courseDetails = await _manageApi.GetCourseByCode(instCode, courseCode);
                var courseInfo = new CourseInfoViewModel { ProgrammeCode = courseDetails.CourseCode, Name = courseDetails.Name };
                var routeData = GetCourseRouteDataViewModel(instCode, courseCode);
                viewModel.RouteData = routeData;
                viewModel.CourseInfo = courseInfo;
                await LoadCopyableCoursesIntoViewBag(instCode, courseCode);
                return View("Requirements", viewModel);
            }

            if (await SaveEnrichment(instCode, courseCode, viewModel))
            {
                CourseSavedMessage();
            }

            return RedirectToAction("Show", new { instCode, accreditingInstCode, courseCode });
        }

        [HttpGet]
        [Route("{instCode}/course/{accreditingInstCode=self}/{courseCode}/salary")]
        public async Task<IActionResult> Salary(string instCode, string accreditingInstCode, string courseCode, string copyFrom = null)
        {
            var courseDetails = await _manageApi.GetCourseByCode(instCode, courseCode);
            var ucasCourseEnrichmentGetModel = await _manageApi.GetCourseEnrichment(instCode, courseCode);
            var routeData = GetCourseRouteDataViewModel(instCode, courseCode);
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

            await LoadCopyableCoursesIntoViewBag(instCode, courseCode);

            if (!string.IsNullOrEmpty(copyFrom))
            {
                copyFrom = copyFrom.ToUpper();
                var copiedEnrichment = await _manageApi.GetCourseEnrichment(instCode, copyFrom);
                ViewBag.CopiedFrom = new CourseInfoViewModel
                {
                    ProgrammeCode = copyFrom,
                    Name = (ViewBag.CopyableCourses as IEnumerable<ApiClient.Course>).SingleOrDefault(x => x.CourseCode == copyFrom)?.Name
                };

                ViewBag.CopiedFields = model.CopyFrom(copiedEnrichment?.EnrichmentModel);
            }

            return View(model);
        }

        [HttpPost]
        [Route("{instCode}/course/{accreditingInstCode=self}/{courseCode}/salary")]
        public async Task<IActionResult> SalaryPost(string instCode, string accreditingInstCode, string courseCode, CourseSalaryEnrichmentViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                var routeData = GetCourseRouteDataViewModel(instCode, courseCode);
                var courseDetails = await _manageApi.GetCourseByCode(instCode, courseCode);
                var courseInfo = new CourseInfoViewModel { ProgrammeCode = courseDetails.CourseCode, Name = courseDetails.Name };
                viewModel.RouteData = routeData;
                viewModel.CourseInfo = courseInfo;
                await LoadCopyableCoursesIntoViewBag(instCode, courseCode);
                return View("Salary", viewModel);
            }
            if (await SaveEnrichment(instCode, courseCode, viewModel))
            {
                CourseSavedMessage();
            }
            return RedirectToAction("Show", new { instCode, accreditingInstCode, courseCode });
        }

        [HttpGet]
        [Route("{instCode}/course/{accreditingInstCode=self}/{courseCode}/fees-and-length")]
        public async Task<IActionResult> Fees(string instCode, string accreditingInstCode, string courseCode, string copyFrom = null)
        {
            var courseDetails = await _manageApi.GetCourseByCode(instCode, courseCode);
            var ucasCourseEnrichmentGetModel = await _manageApi.GetCourseEnrichment(instCode, courseCode);
            var routeData = GetCourseRouteDataViewModel(instCode, courseCode);
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

            await LoadCopyableCoursesIntoViewBag(instCode, courseCode);

            if (!string.IsNullOrEmpty(copyFrom))
            {
                copyFrom = copyFrom.ToUpper();
                var copiedEnrichment = await _manageApi.GetCourseEnrichment(instCode, copyFrom);
                ViewBag.CopiedFrom = new CourseInfoViewModel
                {
                    ProgrammeCode = copyFrom,
                    Name = (ViewBag.CopyableCourses as IEnumerable<ApiClient.Course>).SingleOrDefault(x => x.CourseCode == copyFrom)?.Name
                };

                ViewBag.CopiedFields = model.CopyFrom(copiedEnrichment?.EnrichmentModel);
            }

            return View(model);
        }

        [HttpPost]
        [Route("{instCode}/course/{accreditingInstCode=self}/{courseCode}/fees-and-length")]
        public async Task<IActionResult> FeesPost(string instCode, string accreditingInstCode, string courseCode, CourseFeesEnrichmentViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                var routeData = GetCourseRouteDataViewModel(instCode, courseCode);
                var courseDetails = await _manageApi.GetCourseByCode(instCode, courseCode);
                var courseInfo = new CourseInfoViewModel { ProgrammeCode = courseDetails.CourseCode, Name = courseDetails.Name };
                viewModel.RouteData = routeData;
                viewModel.CourseInfo = courseInfo;
                await LoadCopyableCoursesIntoViewBag(instCode, courseCode);
                return View("Fees", viewModel);
            }
            if (await SaveEnrichment(instCode, courseCode, viewModel))
            {
                CourseSavedMessage();
            }
            return RedirectToAction("Show", new { instCode, accreditingInstCode, courseCode });
        }

        private async Task<bool> SaveEnrichment(string instCode, string courseCode, ICourseEnrichmentViewModel viewModel)
        {
            var course = await _manageApi.GetCourseEnrichment(instCode, courseCode);

            if (course == null && viewModel.IsEmpty())
            {
                // Draft state is "New" and no changes have been made - don't insert a draft
                return false;
            }

            var enrichmentModel = course?.EnrichmentModel ?? new CourseEnrichmentModel();
            viewModel.MapInto(ref enrichmentModel);

            await _manageApi.SaveCourseEnrichment(instCode, courseCode, enrichmentModel);
            return true;
        }

        private void Validate(string instCode, string accreditingInstCode, string courseCode)
        {
            if (string.IsNullOrEmpty(instCode)) { throw new ArgumentNullException(instCode, "instCode cannot be null or empty"); }
            if (string.IsNullOrEmpty(accreditingInstCode)) { throw new ArgumentNullException(accreditingInstCode, "accreditingInstCode cannot be null or empty"); }
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

        private CourseViewModel LoadViewModel(UserOrganisation org, ApiClient.Course course, bool multipleOrganisations, UcasCourseEnrichmentGetModel ucasCourseEnrichmentGetModel, CourseRouteDataViewModel routeData)
        {
            var courseVariant =
                new ViewModels.CourseDetailsViewModel
                {
                    CourseTitle = course.Name,
                    Type = course.TypeDescription,
                    AccreditingInstName = course.AccreditingProviderName,
                    AccreditingInstCode = course.AccreditingProviderId,
                    CourseCode = course.CourseCode,
                    InstCode = course.InstCode,
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
                            .Where(line => !string.IsNullOrEmpty(line));

                        var address = addressLines.Count() > 0 ? addressLines.Where(line => !string.IsNullOrEmpty(line))
                            .Aggregate((string current, string next) => current + ", " + next) : "";

                        return new SchoolViewModel
                        {
                            ApplicationsAcceptedFrom = campus.ApplicationsAcceptedFrom,
                            CampusCode = campus.Code,
                            LocationName = campus.LocationName,
                            Address = address,
                            Status = campus.Status,
                            VacStatus = campus.VacStatus
                        };
                    })
                };

            var isSalary = course.ProgramType.Equals("SS", StringComparison.InvariantCultureIgnoreCase)
                        || course.ProgramType.Equals("TA", StringComparison.InvariantCultureIgnoreCase);
            var courseEnrichmentViewModel = GetCourseEnrichmentViewModel(ucasCourseEnrichmentGetModel, isSalary, routeData);
            var viewModel = new CourseViewModel
            {
                OrganisationName = org.OrganisationName,
                OrganisationId = org.OrganisationId,
                CourseTitle = course.Name,
                AccreditingProviderId = course.AccreditingProviderId,
                MultipleOrganisations = multipleOrganisations,
                Course = courseVariant,
                CourseEnrichment = courseEnrichmentViewModel,
                LiveSearchUrl = searchAndCompareUrlService.GetCoursePageUri(org.UcasCode, courseVariant.CourseCode),
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

        private CourseRouteDataViewModel GetCourseRouteDataViewModel(string instCode, string courseCode)
        {
            return new CourseRouteDataViewModel
            {
                InstCode = instCode,
                CourseCode = courseCode
            };
        }
    }
}
