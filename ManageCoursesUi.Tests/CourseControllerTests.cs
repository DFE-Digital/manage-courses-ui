using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GovUk.Education.ManageCourses.ApiClient;
using GovUk.Education.ManageCourses.Ui;
using GovUk.Education.ManageCourses.Ui.Controllers;
using GovUk.Education.ManageCourses.Ui.Services;
using GovUk.Education.ManageCourses.Ui.ViewModels;
using GovUk.Education.ManageCourses.Ui.ViewModels.Enums;
using ManageCoursesUi.Tests.Enums;
using ManageCoursesUi.Tests.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
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
        public async Task TestController_Variants_should_return_matched_model(EnumDataType type)
        {
            var manageApi = new Mock<IManageApi>();
            var testData = TestHelper.GetTestData(type, null, null);
            var testOrgs = new List<UserOrganisation>
            {
                new UserOrganisation
                {
                OrganisationId = testData.InstitutionCode,
                OrganisationName = testData.InstitutionName,
                UcasCode = TestHelper.InstitutionCode,
                TotalCourses = testData.Courses.Count
                }
            };

            var testCourse = testData.Courses.FirstOrDefault(x => x.Name == TestHelper.TargetedCourseTitle);

            var enrichmentModel = new CourseEnrichmentModel { AboutCourse = "AboutCourse", InterviewProcess = "InterviewProcess", HowSchoolPlacementsWork = "HowSchoolPlacementsWork" };
            var ucasCourseEnrichmentGetModel = new UcasCourseEnrichmentGetModel { EnrichmentModel = enrichmentModel };

            manageApi.Setup(x => x.GetOrganisations()).ReturnsAsync(testOrgs);
            manageApi.Setup(x => x.GetCourseByUcasCode(TestHelper.InstitutionCode, TestHelper.TargetedUcasCode)).ReturnsAsync(testCourse);

            manageApi.Setup(x => x.GetEnrichmentCourse(TestHelper.InstitutionCode, TestHelper.TargetedUcasCode)).ReturnsAsync(ucasCourseEnrichmentGetModel);

            manageApi.Setup(x => x.GetEnrichmentCourse(TestHelper.InstitutionCode, TestHelper.TargetedUcasCode)).ReturnsAsync(ucasCourseEnrichmentGetModel);

            var controller = new CourseController(manageApi.Object, new SearchAndCompareUrlService("http://www.example.com"), new MockFeatureFlags());
            var result = await controller.Variants(TestHelper.InstitutionCode, TestHelper.AccreditedProviderId, TestHelper.TargetedUcasCode);

            var viewResult = result as ViewResult;
            Assert.IsNotNull(viewResult);

            var model = viewResult.Model as VariantViewModel;

            Assert.IsNotNull(model);
            Assert.AreEqual(TestHelper.TargetedCourseTitle, model.CourseTitle);
            Assert.AreEqual(TestHelper.OrganisationId, model.OrganisationId);

            Assert.AreEqual(TestHelper.OrganisationName, model.OrganisationName);

            Assert.AreEqual(TestHelper.InstitutionCode, model.RouteData.InstCode);
            Assert.AreEqual(TestHelper.AccreditedProviderId, model.RouteData.AccreditingProviderId);
            Assert.AreEqual(TestHelper.TargetedUcasCode, model.RouteData.UcasCode);

            Assert.AreEqual(enrichmentModel.AboutCourse, model.CourseEnrichment.AboutCourse);
            Assert.AreEqual(enrichmentModel.InterviewProcess, model.CourseEnrichment.InterviewProcess);
            Assert.AreEqual(enrichmentModel.HowSchoolPlacementsWork, model.CourseEnrichment.HowSchoolPlacementsWork);
        }

        [Test]
        [TestCase(EnumDataType.SingleVariantNoMatch)]
        [TestCase(EnumDataType.MultiVariantNoMatch)]
        public void TestController_Variants_should_return_not_found(EnumDataType type)
        {
            var manageApi = new Mock<IManageApi>();
            var testData = TestHelper.GetTestData(type, null, null);

            var testOrgs = new List<UserOrganisation>
            {
                new UserOrganisation
                {
                OrganisationId = testData.InstitutionCode,
                OrganisationName = testData.InstitutionName,
                UcasCode = "123",
                TotalCourses = testData.Courses.Count
                }
            };

            var testCourse = testData.Courses.FirstOrDefault(x => x.Name == TestHelper.TargetedCourseTitle);
            manageApi.Setup(x => x.GetOrganisations()).ReturnsAsync(testOrgs);
            manageApi.Setup(x => x.GetCourseByUcasCode(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(testCourse);

            var controller = new CourseController(manageApi.Object, new SearchAndCompareUrlService("http://www.example.com"), new MockFeatureFlags());

            var res = controller.Variants(TestHelper.InstitutionCode, TestHelper.AccreditedProviderId, TestHelper.TargetedUcasCode).Result;

            Assert.That(res is NotFoundObjectResult);
            Assert.AreEqual(404, (res as NotFoundObjectResult).StatusCode);
        }

        [Test]
        public async Task TestController_should_return_not_found_status()
        {
            var manageApi = new Mock<IManageApi>();
            var testData = TestHelper.GetTestData(EnumDataType.SingleVariantOneMatch, "123", "provider Name");

            var testOrgs = new List<UserOrganisation>
            {
                new UserOrganisation
                {
                OrganisationId = testData.InstitutionCode,
                OrganisationName = testData.InstitutionName,
                UcasCode = TestHelper.InstitutionCode,
                TotalCourses = testData.Courses.Count
                }
            };

            manageApi.Setup(x => x.GetOrganisations()).ReturnsAsync(testOrgs);
            manageApi.Setup(x => x.GetCourseByUcasCode(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync((Course) null);

            var controller = new CourseController(manageApi.Object, new SearchAndCompareUrlService("http://www.example.com"), new MockFeatureFlags());

            var result = await controller.Variants(TestHelper.InstitutionCode, TestHelper.AccreditedProviderId, TestHelper.TargetedUcasCode);

            Assert.NotNull(result);
            Assert.IsInstanceOf(typeof(NotFoundResult), result);
        }

        [Test]
        [TestCase("", "xxx", "xxx")]
        [TestCase(null, "self", "35L6")]
        [TestCase("2AT", "", "35L6")]
        [TestCase("2AT", null, "35L6")]
        [TestCase("2AT", "self", "")]
        [TestCase("2AT", "self", null)]
        public void TestController_Variants_with_null_or_empty_parameters_should_return_exception(string institutionCode, string accreditedProviderId, string ucasCode)
        {
            var manageApi = new Mock<IManageApi>();
            var testData = TestHelper.GetTestData(EnumDataType.SingleVariantOneMatch, null, null);

            var testOrgs = new List<UserOrganisation>
            {
                new UserOrganisation
                {
                OrganisationId = testData.InstitutionCode,
                OrganisationName = testData.InstitutionName,
                UcasCode = TestHelper.InstitutionCode,
                TotalCourses = testData.Courses.Count
                }
            };

            var testCourse = testData.Courses.FirstOrDefault(x => x.Name == TestHelper.TargetedCourseTitle);
            manageApi.Setup(x => x.GetOrganisations()).ReturnsAsync(testOrgs);
            manageApi.Setup(x => x.GetCourseByUcasCode(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(testCourse);

            var controller = new CourseController(manageApi.Object, new SearchAndCompareUrlService("http://www.example.com"), new MockFeatureFlags());
            Assert.ThrowsAsync<ArgumentNullException>(async() => await controller.Variants(institutionCode, accreditedProviderId, ucasCode));
        }

        [Test]
        public void TestController_with_api_exception_on_first_call_should_return_exception()
        {
            var manageApi = new Mock<IManageApi>();

            manageApi.Setup(x => x.GetOrganisations()).ThrowsAsync(new Exception());

            var controller = new CourseController(manageApi.Object, new SearchAndCompareUrlService("http://www.example.com"), new MockFeatureFlags());

            Assert.ThrowsAsync<Exception>(async() => await controller.Variants(TestHelper.InstitutionCode, TestHelper.AccreditedProviderId, TestHelper.TargetedUcasCode));
        }

        [Test]
        [TestCase(EnumDataType.SingleVariantOneMatch)]
        [TestCase(EnumDataType.MultiVariantOneMatch)]
        public void TestController_with_api_exception_on_second_call_should_return_exception(EnumDataType type)
        {
            var manageApi = new Mock<IManageApi>();
            var testData = TestHelper.GetTestData(type, null, null);
            var testOrgs = new List<UserOrganisation>
            {
                new UserOrganisation
                {
                OrganisationId = testData.InstitutionCode,
                OrganisationName = testData.InstitutionName,
                UcasCode = TestHelper.InstitutionCode,
                TotalCourses = testData.Courses.Count
                }
            };

            manageApi.Setup(x => x.GetOrganisations()).ReturnsAsync(testOrgs);
            manageApi.Setup(x => x.GetCourseByUcasCode(It.IsAny<string>(), It.IsAny<string>())).ThrowsAsync(new Exception());

            var controller = new CourseController(manageApi.Object, new SearchAndCompareUrlService("http://www.example.com"), new MockFeatureFlags());

            Assert.ThrowsAsync<Exception>(async() => await controller.Variants(TestHelper.InstitutionCode, TestHelper.AccreditedProviderId, TestHelper.TargetedUcasCode));
        }

        [Test]
        public void Preview()
        {
            var courseController = new CourseController(new Mock<IManageApi>().Object, new Mock<ISearchAndCompareUrlService>().Object, new MockFeatureFlags());
            var viewModel = (courseController.Preview("abc", "def", "ghi") as ViewResult)?.Model as CourseReferenceViewModel;

            Assert.NotNull(viewModel);
            Assert.AreEqual("abc", viewModel.InstCode);
            Assert.AreEqual("ghi", viewModel.CourseCode);
        }

        [Test]
        public void Preview_RespectsFeatureFlag()
        {
            var flags = new Mock<IFeatureFlags>();
            flags.Setup(x => x.ShowCoursePreview).Returns(false).Verifiable();

            var courseController = new CourseController(new Mock<IManageApi>().Object, new Mock<ISearchAndCompareUrlService>().Object, flags.Object);
            var redirectResult = courseController.Preview("abc", "def", "ghi") as RedirectToActionResult;

            flags.VerifyAll();
            Assert.NotNull(redirectResult);
            Assert.AreEqual("Variants", redirectResult.ActionName);
            Assert.AreEqual("abc", redirectResult.RouteValues["instCode"]);
            Assert.AreEqual("def", redirectResult.RouteValues["accreditingProviderId"]);
            Assert.AreEqual("ghi", redirectResult.RouteValues["ucasCode"]);
        }

        [Test]
        public void VariantsPublish()
        {
            var enrichmentModel = new CourseEnrichmentModel {
                AboutCourse = "AboutCourse",
                InterviewProcess = "InterviewProcess",
                HowSchoolPlacementsWork = "HowSchoolPlacementsWork"
            };

            var ucasCourseEnrichmentGetModel = new UcasCourseEnrichmentGetModel { EnrichmentModel = enrichmentModel };

            var mockApi = new Mock<IManageApi>();
            mockApi.Setup(x => x.GetEnrichmentCourse(TestHelper.InstitutionCode, TestHelper.TargetedUcasCode)).ReturnsAsync(ucasCourseEnrichmentGetModel).Verifiable();
            mockApi.Setup(x => x.PublishEnrichmentCourse(TestHelper.InstitutionCode, TestHelper.TargetedUcasCode)).ReturnsAsync(true).Verifiable();


            var objectValidator = new Mock<IObjectModelValidator>();
            CourseEnrichmentViewModel objectToVerify = null;

            objectValidator.Setup(o => o.Validate(It.IsAny<ActionContext>(),
                                              It.IsAny<ValidationStateDictionary>(),
                                              It.IsAny<string>(),
                                              It.IsAny<Object>()))
                            .Callback<ActionContext, ValidationStateDictionary, string, Object>((a,b,c,d) => {
                                objectToVerify = d as CourseEnrichmentViewModel;
                            })
                            .Verifiable();

            var courseController = new CourseController(mockApi.Object, new Mock<ISearchAndCompareUrlService>().Object, new MockFeatureFlags());
            courseController.ObjectValidator = objectValidator.Object;
            courseController.TempData = new TempDataDictionary(new DefaultHttpContext(), Mock.Of<ITempDataProvider>());

            var res = courseController.VariantsPublish(TestHelper.InstitutionCode, "def", TestHelper.TargetedUcasCode).Result;

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
        public void VariantsPublish_RespectsFeatureFlag()
        {
            var flags = new Mock<IFeatureFlags>();
            flags.Setup(x => x.ShowCoursePublish).Returns(false).Verifiable();

            var courseController = new CourseController(new Mock<IManageApi>().Object, new Mock<ISearchAndCompareUrlService>().Object, flags.Object);
            var redirectResult = courseController.VariantsPublish("abc", "def", "ghi").Result as RedirectToActionResult;

            flags.VerifyAll();
            Assert.NotNull(redirectResult);
            Assert.AreEqual("Variants", redirectResult.ActionName);
            Assert.AreEqual("abc", redirectResult.RouteValues["instCode"]);
            Assert.AreEqual("def", redirectResult.RouteValues["accreditingProviderId"]);
            Assert.AreEqual("ghi", redirectResult.RouteValues["ucasCode"]);
        }

        [Test]
        public async Task About()
        {
            var manageApi = new Mock<IManageApi>();

            var enrichmentModel = new CourseEnrichmentModel { AboutCourse = "AboutCourse", InterviewProcess = "InterviewProcess", HowSchoolPlacementsWork = "HowSchoolPlacementsWork" };
            var ucasCourseEnrichmentGetModel = new UcasCourseEnrichmentGetModel { EnrichmentModel = enrichmentModel };

            manageApi.Setup(x => x.GetEnrichmentCourse(TestHelper.InstitutionCode, TestHelper.TargetedUcasCode)).ReturnsAsync(ucasCourseEnrichmentGetModel);

            var testCourse = new Course() { Name = "Name",  CourseCode = "CourseCode" };

            manageApi.Setup(x => x.GetCourseByUcasCode(TestHelper.InstitutionCode, TestHelper.TargetedUcasCode)).ReturnsAsync(testCourse);

            var controller = new CourseController(manageApi.Object, new SearchAndCompareUrlService("http://www.example.com"), new MockFeatureFlags());
            var result = await controller.About(TestHelper.InstitutionCode, TestHelper.AccreditedProviderId, TestHelper.TargetedUcasCode);

            var viewResult = result as ViewResult;
            Assert.IsNotNull(viewResult);

            var model = viewResult.Model as AboutCourseEnrichmentViewModel;

            Assert.IsNotNull(model);

            Assert.AreEqual(TestHelper.InstitutionCode, model.RouteData.InstCode);
            Assert.AreEqual(TestHelper.AccreditedProviderId, model.RouteData.AccreditingProviderId);
            Assert.AreEqual(TestHelper.TargetedUcasCode, model.RouteData.UcasCode);

            Assert.AreEqual(enrichmentModel.AboutCourse, model.AboutCourse);
            Assert.AreEqual(enrichmentModel.InterviewProcess, model.InterviewProcess);
            Assert.AreEqual(enrichmentModel.HowSchoolPlacementsWork, model.HowSchoolPlacementsWork);
        }

        [Test]
        public async Task AboutPost_Invalid()
        {
            var manageApi = new Mock<IManageApi>();

            var viewModel = new AboutCourseEnrichmentViewModel { AboutCourse = "AboutCourse", InterviewProcess = "InterviewProcess", HowSchoolPlacementsWork = "HowSchoolPlacementsWork" };
            var testCourse = new Course() { Name = "Name",  CourseCode = "CourseCode" };

            manageApi.Setup(x => x.GetCourseByUcasCode(TestHelper.InstitutionCode, TestHelper.TargetedUcasCode)).ReturnsAsync(testCourse);

            var controller = new CourseController(manageApi.Object, new SearchAndCompareUrlService("http://www.example.com"), new MockFeatureFlags());

            controller.ModelState.AddModelError("you", "failed");

            var result = await controller.AboutPost(TestHelper.InstitutionCode, TestHelper.AccreditedProviderId, TestHelper.TargetedUcasCode, viewModel);

            var viewResult = result as ViewResult;
            Assert.IsNotNull(viewResult);
            Assert.AreEqual("About", viewResult.ViewName);

            var model = viewResult.Model as AboutCourseEnrichmentViewModel;

            Assert.IsNotNull(model);

            Assert.AreEqual(TestHelper.InstitutionCode, model.RouteData.InstCode);
            Assert.AreEqual(TestHelper.AccreditedProviderId, model.RouteData.AccreditingProviderId);
            Assert.AreEqual(TestHelper.TargetedUcasCode, model.RouteData.UcasCode);

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

            manageApi.Setup(x => x.GetEnrichmentCourse(TestHelper.InstitutionCode, TestHelper.TargetedUcasCode)).ReturnsAsync(ucasCourseEnrichmentGetModel);

            var tempDataMock = new Mock<ITempDataDictionary>();

            var controller = new CourseController(manageApi.Object, new SearchAndCompareUrlService("http://www.example.com"), new MockFeatureFlags());
            controller.TempData = tempDataMock.Object;
            var result = await controller.AboutPost(TestHelper.InstitutionCode, TestHelper.AccreditedProviderId, TestHelper.TargetedUcasCode, viewModel);

            var redirectToActionResult = result as RedirectToActionResult;
            Assert.IsNotNull(redirectToActionResult);
            Assert.AreEqual("Variants", redirectToActionResult.ActionName);

            var tempData = controller.TempData;

            tempDataMock.Verify(x => x.Add("MessageType", "success"), Times.Once);
            tempDataMock.Verify(x => x.Add("MessageTitle", "Your changes have been saved"), Times.Once);

            var routeValues = redirectToActionResult.RouteValues;

            Assert.AreEqual(TestHelper.InstitutionCode, routeValues["instCode"]);
            Assert.AreEqual(TestHelper.AccreditedProviderId, routeValues["accreditingProviderId"]);
            Assert.AreEqual(TestHelper.TargetedUcasCode, routeValues["ucasCode"]);

            manageApi.Verify(x => x.SaveEnrichmentCourse(TestHelper.InstitutionCode, TestHelper.TargetedUcasCode, It.Is<CourseEnrichmentModel>(c => Check(c, viewModel))), Times.Once());
        }

        [Test]
        public async Task Requirements()
        {
            var manageApi = new Mock<IManageApi>();

            var enrichmentModel = new CourseEnrichmentModel { Qualifications = "Qualifications", PersonalQualities = "PersonalQualities", OtherRequirements = "OtherRequirements" };
            var ucasCourseEnrichmentGetModel = new UcasCourseEnrichmentGetModel { EnrichmentModel = enrichmentModel };

            manageApi.Setup(x => x.GetEnrichmentCourse(TestHelper.InstitutionCode, TestHelper.TargetedUcasCode)).ReturnsAsync(ucasCourseEnrichmentGetModel);

            var testCourse = new Course() { Name = "Name",  CourseCode = "CourseCode" };

            manageApi.Setup(x => x.GetCourseByUcasCode(TestHelper.InstitutionCode, TestHelper.TargetedUcasCode)).ReturnsAsync(testCourse);

            var controller = new CourseController(manageApi.Object, new SearchAndCompareUrlService("http://www.example.com"), new MockFeatureFlags());
            var result = await controller.Requirements(TestHelper.InstitutionCode, TestHelper.AccreditedProviderId, TestHelper.TargetedUcasCode);

            var viewResult = result as ViewResult;
            Assert.IsNotNull(viewResult);

            var model = viewResult.Model as CourseRequirementsEnrichmentViewModel;

            Assert.IsNotNull(model);

            Assert.AreEqual(TestHelper.InstitutionCode, model.RouteData.InstCode);
            Assert.AreEqual(TestHelper.AccreditedProviderId, model.RouteData.AccreditingProviderId);
            Assert.AreEqual(TestHelper.TargetedUcasCode, model.RouteData.UcasCode);

            Assert.AreEqual(enrichmentModel.Qualifications, model.Qualifications);
            Assert.AreEqual(enrichmentModel.PersonalQualities, model.PersonalQualities);
            Assert.AreEqual(enrichmentModel.OtherRequirements, model.OtherRequirements);
        }

        [Test]
        public async Task RequirementsPost_Invalid()
        {
            var manageApi = new Mock<IManageApi>();

            var viewModel = new CourseRequirementsEnrichmentViewModel { Qualifications = "Qualifications", PersonalQualities = "PersonalQualities", OtherRequirements = "OtherRequirements" };

            var testCourse = new Course() { Name = "Name",  CourseCode = "CourseCode" };

            manageApi.Setup(x => x.GetCourseByUcasCode(TestHelper.InstitutionCode, TestHelper.TargetedUcasCode)).ReturnsAsync(testCourse);

            var controller = new CourseController(manageApi.Object, new SearchAndCompareUrlService("http://www.example.com"), new MockFeatureFlags());

            controller.ModelState.AddModelError("you", "failed");

            var result = await controller.RequirementsPost(TestHelper.InstitutionCode, TestHelper.AccreditedProviderId, TestHelper.TargetedUcasCode, viewModel);

            var viewResult = result as ViewResult;
            Assert.IsNotNull(viewResult);
            Assert.AreEqual("Requirements", viewResult.ViewName);

            var model = viewResult.Model as CourseRequirementsEnrichmentViewModel;

            Assert.IsNotNull(model);

            Assert.AreEqual(TestHelper.InstitutionCode, model.RouteData.InstCode);
            Assert.AreEqual(TestHelper.AccreditedProviderId, model.RouteData.AccreditingProviderId);
            Assert.AreEqual(TestHelper.TargetedUcasCode, model.RouteData.UcasCode);

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

            manageApi.Setup(x => x.GetEnrichmentCourse(TestHelper.InstitutionCode, TestHelper.TargetedUcasCode)).ReturnsAsync(ucasCourseEnrichmentGetModel);

            var tempDataMock = new Mock<ITempDataDictionary>();

            var controller = new CourseController(manageApi.Object, new SearchAndCompareUrlService("http://www.example.com"), new MockFeatureFlags());
            controller.TempData = tempDataMock.Object;
            var result = await controller.RequirementsPost(TestHelper.InstitutionCode, TestHelper.AccreditedProviderId, TestHelper.TargetedUcasCode, viewModel);

            var redirectToActionResult = result as RedirectToActionResult;
            Assert.IsNotNull(redirectToActionResult);
            Assert.AreEqual("Variants", redirectToActionResult.ActionName);

            var tempData = controller.TempData;

            tempDataMock.Verify(x => x.Add("MessageType", "success"), Times.Once);
            tempDataMock.Verify(x => x.Add("MessageTitle", "Your changes have been saved"), Times.Once);

            var routeValues = redirectToActionResult.RouteValues;

            Assert.AreEqual(TestHelper.InstitutionCode, routeValues["instCode"]);
            Assert.AreEqual(TestHelper.AccreditedProviderId, routeValues["accreditingProviderId"]);
            Assert.AreEqual(TestHelper.TargetedUcasCode, routeValues["ucasCode"]);

            manageApi.Verify(x => x.SaveEnrichmentCourse(TestHelper.InstitutionCode, TestHelper.TargetedUcasCode, It.Is<CourseEnrichmentModel>(c => Check(c, viewModel))), Times.Once());
        }


        [Test]
        public async Task Fees()
        {
            var manageApi = new Mock<IManageApi>();

            var enrichmentModel = new CourseEnrichmentModel {
                FeeUkEu = 123.45m, FeeInternational = 543.21m, FeeDetails = "FeeDetails", CourseLength = null, FinancialSupport = "FinancialSupport"
            };
            var ucasCourseEnrichmentGetModel = new UcasCourseEnrichmentGetModel { EnrichmentModel = enrichmentModel };

            manageApi.Setup(x => x.GetEnrichmentCourse(TestHelper.InstitutionCode, TestHelper.TargetedUcasCode)).ReturnsAsync(ucasCourseEnrichmentGetModel);

            var testCourse = new Course() { Name = "Name", CourseCode = "CourseCode" };

            manageApi.Setup(x => x.GetCourseByUcasCode(TestHelper.InstitutionCode, TestHelper.TargetedUcasCode)).ReturnsAsync(testCourse);

            var controller = new CourseController(manageApi.Object, new SearchAndCompareUrlService("http://www.example.com"), new MockFeatureFlags());
            var result = await controller.Fees(TestHelper.InstitutionCode, TestHelper.AccreditedProviderId, TestHelper.TargetedUcasCode);

            var viewResult = result as ViewResult;
            Assert.IsNotNull(viewResult);

            var model = viewResult.Model as CourseFeesEnrichmentViewModel;

            Assert.IsNotNull(model);

            Assert.AreEqual(TestHelper.InstitutionCode, model.RouteData.InstCode);
            Assert.AreEqual(TestHelper.AccreditedProviderId, model.RouteData.AccreditingProviderId);
            Assert.AreEqual(TestHelper.TargetedUcasCode, model.RouteData.UcasCode);

            Assert.AreEqual(enrichmentModel.CourseLength, model.CourseLength);
            Assert.AreEqual(enrichmentModel.FeeDetails, model.FeeDetails);
            Assert.AreEqual(enrichmentModel.FeeInternational.ToString(), model.FeeInternational);
            Assert.AreEqual(enrichmentModel.FeeUkEu.ToString(), model.FeeUkEu);
            Assert.AreEqual(enrichmentModel.FinancialSupport, model.FinancialSupport);
        }

        [Test]
        public async Task FeesPost_Invalid()
        {
            var manageApi = new Mock<IManageApi>();

            var viewModel = new CourseFeesEnrichmentViewModel { FeeUkEu = "123.45", FeeInternational = "543.21", FeeDetails = "FeeDetails", CourseLength = CourseLength.OneYear, FinancialSupport = "FinancialSupport" };

            var testCourse = new Course() { Name = "Name", CourseCode = "CourseCode" };

            manageApi.Setup(x => x.GetCourseByUcasCode(TestHelper.InstitutionCode, TestHelper.TargetedUcasCode)).ReturnsAsync(testCourse);

            var controller = new CourseController(manageApi.Object, new SearchAndCompareUrlService("http://www.example.com"), new MockFeatureFlags());

            controller.ModelState.AddModelError("you", "failed");

            var result = await controller.FeesPost(TestHelper.InstitutionCode, TestHelper.AccreditedProviderId, TestHelper.TargetedUcasCode, viewModel);

            var viewResult = result as ViewResult;
            Assert.IsNotNull(viewResult);
            Assert.AreEqual("Fees", viewResult.ViewName);

            var model = viewResult.Model as CourseFeesEnrichmentViewModel;

            Assert.IsNotNull(model);

            Assert.AreEqual(TestHelper.InstitutionCode, model.RouteData.InstCode);
            Assert.AreEqual(TestHelper.AccreditedProviderId, model.RouteData.AccreditingProviderId);
            Assert.AreEqual(TestHelper.TargetedUcasCode, model.RouteData.UcasCode);

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

            var viewModel = new CourseFeesEnrichmentViewModel { FeeUkEu = "123.45", FeeInternational = "543.21", FeeDetails = "FeeDetails", CourseLength = CourseLength.TwoYears, FinancialSupport = "FinancialSupport" };

            var enrichmentModel = new CourseEnrichmentModel { FeeUkEu = 123.45m, FeeInternational = 543.21m, FeeDetails = "FeeDetails", CourseLength = null, FinancialSupport = "FinancialSupport" };

            var ucasCourseEnrichmentGetModel = new UcasCourseEnrichmentGetModel { EnrichmentModel = enrichmentModel };

            manageApi.Setup(x => x.GetEnrichmentCourse(TestHelper.InstitutionCode, TestHelper.TargetedUcasCode)).ReturnsAsync(ucasCourseEnrichmentGetModel);

            var tempDataMock = new Mock<ITempDataDictionary>();


            var controller = new CourseController(manageApi.Object, new SearchAndCompareUrlService("http://www.example.com"), new MockFeatureFlags());

            controller.TempData = tempDataMock.Object;
            var result = await controller.FeesPost(TestHelper.InstitutionCode, TestHelper.AccreditedProviderId, TestHelper.TargetedUcasCode, viewModel);

            var redirectToActionResult = result as RedirectToActionResult;
            Assert.IsNotNull(redirectToActionResult);
            Assert.AreEqual("Variants", redirectToActionResult.ActionName);

            var tempData = controller.TempData;

            tempDataMock.Verify(x => x.Add("MessageType", "success"), Times.Once);
            tempDataMock.Verify(x => x.Add("MessageTitle", "Your changes have been saved"), Times.Once);

            var routeValues = redirectToActionResult.RouteValues;

            Assert.AreEqual(TestHelper.InstitutionCode, routeValues["instCode"]);
            Assert.AreEqual(TestHelper.AccreditedProviderId, routeValues["accreditingProviderId"]);
            Assert.AreEqual(TestHelper.TargetedUcasCode, routeValues["ucasCode"]);

            manageApi.Verify(x => x.SaveEnrichmentCourse(TestHelper.InstitutionCode, TestHelper.TargetedUcasCode, It.Is<CourseEnrichmentModel>(c => Check(c, viewModel))), Times.Once());
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
                    model.FeeInternational.ToString() == courseFeesEnrichmentViewModel.FeeInternational &&
                    model.FeeUkEu.ToString() == courseFeesEnrichmentViewModel.FeeUkEu &&
                    model.FinancialSupport == courseFeesEnrichmentViewModel.FinancialSupport &&
                    model.CourseLength == courseLength;
            }
            return result;
        }

        private class MockFeatureFlags : IFeatureFlags
        {
            public bool ShowCoursePreview => true;

            public bool ShowCoursePublish => true;
            
            public bool ShowCourseLiveView => true;
        }
    }
}
