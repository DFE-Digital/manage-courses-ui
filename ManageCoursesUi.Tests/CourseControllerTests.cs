using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using GovUk.Education.ManageCourses.Api.Model;
using GovUk.Education.ManageCourses.Domain.Models;
using GovUk.Education.ManageCourses.Ui;
using GovUk.Education.ManageCourses.Ui.Helpers;
using GovUk.Education.ManageCourses.Ui.Controllers;
using GovUk.Education.ManageCourses.Ui.Services;
using GovUk.Education.ManageCourses.Ui.ViewModels;
using GovUk.Education.ManageCourses.Ui.ViewModels.Enums;
using ManageCoursesUi.Tests.Enums;
using ManageCoursesUi.Tests.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Moq;
using NUnit.Framework;


namespace ManageCoursesUi.Tests
{
    [TestFixture]
    public class CourseControllerTests
    {
        [Test]
        [TestCase(EnumDataType.SingleVariantOneMatch)]
        [TestCase(EnumDataType.MultiVariantOneMatch)]
        public async Task TestController_Show_should_be_redirected(EnumDataType type)
        {
            var manageApi = new Mock<IManageApi>();
            var frontendUrlMock = new Mock<IFrontendUrlService>();
            frontendUrlMock.Setup(x => x.RedirectToFrontend("/organisations/" + TestHelper.ProviderCode + "/courses/" + TestHelper.TargetedProviderCode)).Returns(new RedirectResult("frontend"));

            var controller = new CourseController(manageApi.Object, new SearchAndCompareUrlService("http://www.example.com"), frontendUrlMock.Object);
            var result = await controller.Show(TestHelper.ProviderCode, TestHelper.AccreditingProviderCode, TestHelper.TargetedProviderCode);

            Assert.IsTrue(result is RedirectResult);
        }


        [Test]
        public void ShowPublish()
        {
            var enrichmentModel = new CourseEnrichmentModel
            {
                AboutCourse = "AboutCourse",
                InterviewProcess = "InterviewProcess",
                HowSchoolPlacementsWork = "HowSchoolPlacementsWork"
            };

            var ucasCourseEnrichmentGetModel = new UcasCourseEnrichmentGetModel { EnrichmentModel = enrichmentModel };

            var mockApi = new Mock<IManageApi>();
            mockApi.Setup(x => x.GetCourseEnrichment(TestHelper.ProviderCode, TestHelper.TargetedProviderCode)).ReturnsAsync(ucasCourseEnrichmentGetModel).Verifiable();
            mockApi.Setup(x => x.PublishCourseToSearchAndCompare(TestHelper.ProviderCode, TestHelper.TargetedProviderCode)).ReturnsAsync(true).Verifiable();

            mockApi.Setup(x => x.GetCourse(TestHelper.ProviderCode, TestHelper.TargetedProviderCode)).ReturnsAsync(new Course { ProgramType = "", Provider = new GovUk.Education.ManageCourses.Domain.Models.Provider() }).Verifiable();

            var objectValidator = new Mock<IObjectModelValidator>();
            BaseCourseEnrichmentViewModel objectToVerify = null;

            objectValidator.Setup(o => o.Validate(It.IsAny<ActionContext>(),
                    It.IsAny<ValidationStateDictionary>(),
                    It.IsAny<string>(),
                    It.IsAny<Object>()))
                .Callback<ActionContext, ValidationStateDictionary, string, Object>((a, b, c, d) =>
                {
                    objectToVerify = d as BaseCourseEnrichmentViewModel;
                })
                .Verifiable();

            var frontendUrlMock = new Mock<IFrontendUrlService>();
            var courseController = new CourseController(mockApi.Object, new Mock<ISearchAndCompareUrlService>().Object, frontendUrlMock.Object);
            courseController.ObjectValidator = objectValidator.Object;
            courseController.TempData = new TempDataDictionary(new DefaultHttpContext(), Mock.Of<ITempDataProvider>());

            var res = courseController.ShowPublish(TestHelper.ProviderCode, "def", TestHelper.TargetedProviderCode).Result;

            mockApi.VerifyAll();
            objectValidator.VerifyAll();

            Assert.AreEqual("success", courseController.TempData["MessageType"]);
            Assert.AreEqual("Your course has been published", courseController.TempData["MessageTitle"]);

            Assert.IsNotNull(objectToVerify);
            Assert.AreEqual("AboutCourse", objectToVerify.AboutCourse);
            Assert.AreEqual("InterviewProcess", objectToVerify.InterviewProcess);
            Assert.AreEqual("HowSchoolPlacementsWork", objectToVerify.HowSchoolPlacementsWork);
        }

        [Test]
        public async Task AboutPost_Invalid()
        {
            var manageApi = new Mock<IManageApi>();

            var viewModel = new AboutCourseEnrichmentViewModel { AboutCourse = "AboutCourse", InterviewProcess = "InterviewProcess", HowSchoolPlacementsWork = "HowSchoolPlacementsWork" };
            var testCourse = new Course() { Name = "Name", CourseCode = "CourseCode" };

            manageApi.Setup(x => x.GetCourse(TestHelper.ProviderCode, TestHelper.TargetedProviderCode)).ReturnsAsync(testCourse);

            var frontendUrlMock = new Mock<IFrontendUrlService>();
            var controller = new CourseController(manageApi.Object, new SearchAndCompareUrlService("http://www.example.com"), frontendUrlMock.Object);

            controller.ModelState.AddModelError("you", "failed");

            var result = await controller.AboutPost(TestHelper.ProviderCode, TestHelper.AccreditingProviderCode, TestHelper.TargetedProviderCode, viewModel);

            var viewResult = result as ViewResult;
            Assert.IsNotNull(viewResult);
            Assert.AreEqual("About", viewResult.ViewName);

            var model = viewResult.Model as AboutCourseEnrichmentViewModel;

            Assert.IsNotNull(model);

            Assert.AreEqual(TestHelper.ProviderCode, model.RouteData.ProviderCode);
            Assert.AreEqual(TestHelper.TargetedProviderCode, model.RouteData.CourseCode);

            Assert.AreEqual(viewModel.AboutCourse, model.AboutCourse);
            Assert.AreEqual(viewModel.InterviewProcess, model.InterviewProcess);
            Assert.AreEqual(viewModel.HowSchoolPlacementsWork, model.HowSchoolPlacementsWork);
        }

        [Test]
        public async Task AboutPost()
        {
            var manageApi = new Mock<IManageApi>();

            var viewModel = new AboutCourseEnrichmentViewModel { AboutCourse = "AboutCourse", InterviewProcess = "InterviewProcess", HowSchoolPlacementsWork = "HowSchoolPlacementsWork" };

            var enrichmentModel = new CourseEnrichmentModel { AboutCourse = "", InterviewProcess = "", HowSchoolPlacementsWork = "" };
            var ucasCourseEnrichmentGetModel = new UcasCourseEnrichmentGetModel { EnrichmentModel = enrichmentModel };

            manageApi.Setup(x => x.GetCourseEnrichment(TestHelper.ProviderCode, TestHelper.TargetedProviderCode)).ReturnsAsync(ucasCourseEnrichmentGetModel);

            var tempDataMock = new Mock<ITempDataDictionary>();
            var urlHelperMock = new Mock<IUrlHelper>();

            var previewLink = "preview-link";

            Expression<Func<IUrlHelper, string>> urlSetup = url => url.Action(It.Is<UrlActionContext>(uac => uac.Action == "Preview"));
            urlHelperMock.Setup(urlSetup).Returns(previewLink);

            var frontendUrlMock = new Mock<IFrontendUrlService>();
            var controller = new CourseController(manageApi.Object, new SearchAndCompareUrlService("http://www.example.com"), frontendUrlMock.Object);
            controller.TempData = tempDataMock.Object;
            controller.Url = urlHelperMock.Object;
            var result = await controller.AboutPost(TestHelper.ProviderCode, TestHelper.AccreditingProviderCode, TestHelper.TargetedProviderCode, viewModel);

            var redirectToActionResult = result as RedirectToActionResult;
            Assert.IsNotNull(redirectToActionResult);
            Assert.AreEqual("Show", redirectToActionResult.ActionName);

            var tempData = controller.TempData;

            VerifyTempDataIsSet(tempDataMock, previewLink);

            var routeValues = redirectToActionResult.RouteValues;

            Assert.AreEqual(TestHelper.ProviderCode, routeValues["providerCode"]);
            Assert.AreEqual(TestHelper.AccreditingProviderCode, routeValues["accreditingProviderCode"]);
            Assert.AreEqual(TestHelper.TargetedProviderCode, routeValues["courseCode"]);

            manageApi.Verify(x => x.SaveCourseEnrichment(TestHelper.ProviderCode, TestHelper.TargetedProviderCode, It.Is<CourseEnrichmentModel>(c => Check(c, viewModel))), Times.Once());
        }

        [Test]
        public async Task RequirementsPost_Invalid()
        {
            var manageApi = new Mock<IManageApi>();

            var viewModel = new CourseRequirementsEnrichmentViewModel { Qualifications = "Qualifications", PersonalQualities = "PersonalQualities", OtherRequirements = "OtherRequirements" };

            var testCourse = new Course() { Name = "Name", CourseCode = "CourseCode" };

            manageApi.Setup(x => x.GetCourse(TestHelper.ProviderCode, TestHelper.TargetedProviderCode)).ReturnsAsync(testCourse);

            var frontendUrlMock = new Mock<IFrontendUrlService>();
            var controller = new CourseController(manageApi.Object, new SearchAndCompareUrlService("http://www.example.com"), frontendUrlMock.Object);

            controller.ModelState.AddModelError("you", "failed");

            var result = await controller.RequirementsPost(TestHelper.ProviderCode, TestHelper.AccreditingProviderCode, TestHelper.TargetedProviderCode, viewModel);

            var viewResult = result as ViewResult;
            Assert.IsNotNull(viewResult);
            Assert.AreEqual("Requirements", viewResult.ViewName);

            var model = viewResult.Model as CourseRequirementsEnrichmentViewModel;

            Assert.IsNotNull(model);

            Assert.AreEqual(TestHelper.ProviderCode, model.RouteData.ProviderCode);
            Assert.AreEqual(TestHelper.TargetedProviderCode, model.RouteData.CourseCode);

            Assert.AreEqual(viewModel.Qualifications, model.Qualifications);
            Assert.AreEqual(viewModel.PersonalQualities, model.PersonalQualities);
            Assert.AreEqual(viewModel.OtherRequirements, model.OtherRequirements);
        }

        [Test]
        public async Task RequirementsPost()
        {
            var manageApi = new Mock<IManageApi>();

            var viewModel = new CourseRequirementsEnrichmentViewModel { Qualifications = "Qualifications", PersonalQualities = "PersonalQualities", OtherRequirements = "OtherRequirements" };

            var enrichmentModel = new CourseEnrichmentModel { Qualifications = "", PersonalQualities = "", OtherRequirements = "" };
            var ucasCourseEnrichmentGetModel = new UcasCourseEnrichmentGetModel { EnrichmentModel = enrichmentModel };

            manageApi.Setup(x => x.GetCourseEnrichment(TestHelper.ProviderCode, TestHelper.TargetedProviderCode)).ReturnsAsync(ucasCourseEnrichmentGetModel);

            var tempDataMock = new Mock<ITempDataDictionary>();
            var urlHelperMock = new Mock<IUrlHelper>();

            var previewLink = "preview-link";

            Expression<Func<IUrlHelper, string>> urlSetup = url => url.Action(It.Is<UrlActionContext>(uac => uac.Action == "Preview"));
            urlHelperMock.Setup(urlSetup).Returns(previewLink);

            var frontendUrlMock = new Mock<IFrontendUrlService>();
            var controller = new CourseController(manageApi.Object, new SearchAndCompareUrlService("http://www.example.com"), frontendUrlMock.Object);

            controller.TempData = tempDataMock.Object;
            controller.Url = urlHelperMock.Object;

            var result = await controller.RequirementsPost(TestHelper.ProviderCode, TestHelper.AccreditingProviderCode, TestHelper.TargetedProviderCode, viewModel);

            var redirectToActionResult = result as RedirectToActionResult;
            Assert.IsNotNull(redirectToActionResult);
            Assert.AreEqual("Show", redirectToActionResult.ActionName);

            var tempData = controller.TempData;

            VerifyTempDataIsSet(tempDataMock, previewLink);

            var routeValues = redirectToActionResult.RouteValues;

            Assert.AreEqual(TestHelper.ProviderCode, routeValues["providerCode"]);
            Assert.AreEqual(TestHelper.AccreditingProviderCode, routeValues["accreditingProviderCode"]);
            Assert.AreEqual(TestHelper.TargetedProviderCode, routeValues["courseCode"]);

            manageApi.Verify(x => x.SaveCourseEnrichment(TestHelper.ProviderCode, TestHelper.TargetedProviderCode, It.Is<CourseEnrichmentModel>(c => Check(c, viewModel))), Times.Once());
        }

        [Test]
        public async Task SalaryPost_Invalid()
        {
            var manageApi = new Mock<IManageApi>();

            var viewModel = new CourseSalaryEnrichmentViewModel { SalaryDetails = "SalaryDetails", CourseLength = CourseLength.Other };

            var testCourse = new Course() { Name = "Name", CourseCode = "CourseCode" };

            manageApi.Setup(x => x.GetCourse(TestHelper.ProviderCode, TestHelper.TargetedProviderCode)).ReturnsAsync(testCourse);

            var frontendUrlMock = new Mock<IFrontendUrlService>();
            var controller = new CourseController(manageApi.Object, new SearchAndCompareUrlService("http://www.example.com"), frontendUrlMock.Object);

            controller.ModelState.AddModelError("you", "failed");

            var result = await controller.SalaryPost(TestHelper.ProviderCode, TestHelper.AccreditingProviderCode, TestHelper.TargetedProviderCode, viewModel);

            var viewResult = result as ViewResult;
            Assert.IsNotNull(viewResult);
            Assert.AreEqual("Salary", viewResult.ViewName);

            var model = viewResult.Model as CourseSalaryEnrichmentViewModel;

            Assert.IsNotNull(model);

            Assert.AreEqual(TestHelper.ProviderCode, model.RouteData.ProviderCode);
            Assert.AreEqual(TestHelper.TargetedProviderCode, model.RouteData.CourseCode);

            Assert.AreEqual(viewModel.CourseLength, model.CourseLength);
            Assert.AreEqual(viewModel.SalaryDetails, model.SalaryDetails);
        }

        [Test]
        public async Task SalaryPost()
        {
            var manageApi = new Mock<IManageApi>();

            var viewModel = new CourseSalaryEnrichmentViewModel { SalaryDetails = "SalaryDetails", CourseLength = CourseLength.TwoYears };

            var enrichmentModel = new CourseEnrichmentModel { SalaryDetails = "SalaryDetails2", CourseLength = null };

            var ucasCourseEnrichmentGetModel = new UcasCourseEnrichmentGetModel { EnrichmentModel = enrichmentModel };

            manageApi.Setup(x => x.GetCourseEnrichment(TestHelper.ProviderCode, TestHelper.TargetedProviderCode)).ReturnsAsync(ucasCourseEnrichmentGetModel);

            var tempDataMock = new Mock<ITempDataDictionary>();
            var urlHelperMock = new Mock<IUrlHelper>();

            var previewLink = "preview-link";

            Expression<Func<IUrlHelper, string>> urlSetup = url => url.Action(It.Is<UrlActionContext>(uac => uac.Action == "Preview"));
            urlHelperMock.Setup(urlSetup).Returns(previewLink);

            var frontendUrlMock = new Mock<IFrontendUrlService>();
            var controller = new CourseController(manageApi.Object, new SearchAndCompareUrlService("http://www.example.com"), frontendUrlMock.Object);

            controller.TempData = tempDataMock.Object;
            controller.Url = urlHelperMock.Object;

            var result = await controller.SalaryPost(TestHelper.ProviderCode, TestHelper.AccreditingProviderCode, TestHelper.TargetedProviderCode, viewModel);

            var redirectToActionResult = result as RedirectToActionResult;
            Assert.IsNotNull(redirectToActionResult);
            Assert.AreEqual("Show", redirectToActionResult.ActionName);

            var tempData = controller.TempData;

            VerifyTempDataIsSet(tempDataMock, previewLink);

            var routeValues = redirectToActionResult.RouteValues;

            Assert.AreEqual(TestHelper.ProviderCode, routeValues["providerCode"]);
            Assert.AreEqual(TestHelper.AccreditingProviderCode, routeValues["accreditingProviderCode"]);
            Assert.AreEqual(TestHelper.TargetedProviderCode, routeValues["courseCode"]);

            manageApi.Verify(x => x.SaveCourseEnrichment(TestHelper.ProviderCode, TestHelper.TargetedProviderCode, It.Is<CourseEnrichmentModel>(c => Check(c, viewModel))), Times.Once());
        }

        [Test]
        public async Task FeesPost_Invalid()
        {
            var manageApi = new Mock<IManageApi>();

            var viewModel = new CourseFeesEnrichmentViewModel { FeeUkEu = 123, FeeInternational = 543, FeeDetails = "FeeDetails", CourseLength = CourseLength.OneYear, FinancialSupport = "FinancialSupport" };

            var testCourse = new Course() { Name = "Name", CourseCode = "CourseCode" };

            manageApi.Setup(x => x.GetCourse(TestHelper.ProviderCode, TestHelper.TargetedProviderCode)).ReturnsAsync(testCourse);

            var frontendUrlMock = new Mock<IFrontendUrlService>();
            var controller = new CourseController(manageApi.Object, new SearchAndCompareUrlService("http://www.example.com"), frontendUrlMock.Object);

            controller.ModelState.AddModelError("you", "failed");

            var result = await controller.FeesPost(TestHelper.ProviderCode, TestHelper.AccreditingProviderCode, TestHelper.TargetedProviderCode, viewModel);

            var viewResult = result as ViewResult;
            Assert.IsNotNull(viewResult);
            Assert.AreEqual("Fees", viewResult.ViewName);

            var model = viewResult.Model as CourseFeesEnrichmentViewModel;

            Assert.IsNotNull(model);

            Assert.AreEqual(TestHelper.ProviderCode, model.RouteData.ProviderCode);
            Assert.AreEqual(TestHelper.TargetedProviderCode, model.RouteData.CourseCode);

            Assert.AreEqual(viewModel.CourseLength, model.CourseLength);
            Assert.AreEqual(viewModel.FeeDetails, model.FeeDetails);
            Assert.AreEqual(viewModel.FeeInternational, model.FeeInternational);
            Assert.AreEqual(viewModel.FeeUkEu, model.FeeUkEu);
            Assert.AreEqual(viewModel.FinancialSupport, model.FinancialSupport);
        }

        [Test]
        public async Task FeesPost()
        {
            var manageApi = new Mock<IManageApi>();

            var viewModel = new CourseFeesEnrichmentViewModel { FeeUkEu = 123, FeeInternational = 543, FeeDetails = "FeeDetails", CourseLength = CourseLength.TwoYears, FinancialSupport = "FinancialSupport" };

            var enrichmentModel = new CourseEnrichmentModel { FeeUkEu = 123.45m, FeeInternational = 543.21m, FeeDetails = "FeeDetails", CourseLength = null, FinancialSupport = "FinancialSupport" };

            var ucasCourseEnrichmentGetModel = new UcasCourseEnrichmentGetModel { EnrichmentModel = enrichmentModel };

            manageApi.Setup(x => x.GetCourseEnrichment(TestHelper.ProviderCode, TestHelper.TargetedProviderCode)).ReturnsAsync(ucasCourseEnrichmentGetModel);

            var tempDataMock = new Mock<ITempDataDictionary>();
            var urlHelperMock = new Mock<IUrlHelper>();

            var previewLink = "preview-link";

            Expression<Func<IUrlHelper, string>> urlSetup = url => url.Action(It.Is<UrlActionContext>(uac => uac.Action == "Preview"));
            urlHelperMock.Setup(urlSetup).Returns(previewLink);

            var frontendUrlMock = new Mock<IFrontendUrlService>();
            var controller = new CourseController(manageApi.Object, new SearchAndCompareUrlService("http://www.example.com"), frontendUrlMock.Object);

            controller.TempData = tempDataMock.Object;
            controller.Url = urlHelperMock.Object;

            var result = await controller.FeesPost(TestHelper.ProviderCode, TestHelper.AccreditingProviderCode, TestHelper.TargetedProviderCode, viewModel);

            var redirectToActionResult = result as RedirectToActionResult;
            Assert.IsNotNull(redirectToActionResult);
            Assert.AreEqual("Show", redirectToActionResult.ActionName);

            var tempData = controller.TempData;

            VerifyTempDataIsSet(tempDataMock, previewLink);

            var routeValues = redirectToActionResult.RouteValues;

            Assert.AreEqual(TestHelper.ProviderCode, routeValues["providerCode"]);
            Assert.AreEqual(TestHelper.AccreditingProviderCode, routeValues["accreditingProviderCode"]);
            Assert.AreEqual(TestHelper.TargetedProviderCode, routeValues["courseCode"]);

            manageApi.Verify(x => x.SaveCourseEnrichment(TestHelper.ProviderCode, TestHelper.TargetedProviderCode, It.Is<CourseEnrichmentModel>(c => Check(c, viewModel))), Times.Once());
        }

        private bool Check(CourseEnrichmentModel model, ICourseEnrichmentViewModel viewModel)
        {
            var result = false;
            var aboutCourseEnrichmentViewModel = viewModel as AboutCourseEnrichmentViewModel;

            if (aboutCourseEnrichmentViewModel != null)
            {
                result = aboutCourseEnrichmentViewModel.AboutCourse == model.AboutCourse &&
                    aboutCourseEnrichmentViewModel.InterviewProcess == model.InterviewProcess &&
                    aboutCourseEnrichmentViewModel.HowSchoolPlacementsWork == model.HowSchoolPlacementsWork;
            }

            var courseRequirementsEnrichmentViewModel = viewModel as CourseRequirementsEnrichmentViewModel;

            if (courseRequirementsEnrichmentViewModel != null)
            {
                result =
                    model.Qualifications == courseRequirementsEnrichmentViewModel.Qualifications &&
                    model.PersonalQualities == courseRequirementsEnrichmentViewModel.PersonalQualities &&
                    model.OtherRequirements == courseRequirementsEnrichmentViewModel.OtherRequirements;
            }

            var courseFeesEnrichmentViewModel = viewModel as CourseFeesEnrichmentViewModel;

            if (courseFeesEnrichmentViewModel != null)
            {
                var courseLength = courseFeesEnrichmentViewModel.CourseLength.HasValue ? courseFeesEnrichmentViewModel.CourseLength.Value.ToString() : null;
                result =
                    model.FeeDetails == courseFeesEnrichmentViewModel.FeeDetails &&
                    model.FeeInternational == courseFeesEnrichmentViewModel.FeeInternational &&
                    model.FeeUkEu == courseFeesEnrichmentViewModel.FeeUkEu &&
                    model.FinancialSupport == courseFeesEnrichmentViewModel.FinancialSupport &&
                    model.CourseLength == courseLength;
            }

            var courseSalaryEnrichmentViewModel = viewModel as CourseSalaryEnrichmentViewModel;

            if (courseSalaryEnrichmentViewModel != null)
            {
                var courseLength = courseSalaryEnrichmentViewModel.CourseLength.HasValue ? courseSalaryEnrichmentViewModel.CourseLength.Value.ToString() : null;
                result =
                    model.SalaryDetails == courseSalaryEnrichmentViewModel.SalaryDetails &&
                    model.CourseLength == courseLength;
            }
            return result;
        }

        private void VerifyTempDataIsSet(Mock<ITempDataDictionary> tempDataMock, string previewLink)
        {
            tempDataMock.VerifySet(x => x["MessageType"] = "success", Times.Once);
            tempDataMock.VerifySet(x => x["MessageTitle"] = "Your changes have been saved", Times.Once);
            tempDataMock.VerifySet(x => x["MessageBodyHtml"] =  $@"
                <p class=""govuk-body"">
                    <a href='{previewLink}'>Preview your course</a>
                    to check for mistakes before publishing.
                </p>", Times.Once);
        }
    }
}
