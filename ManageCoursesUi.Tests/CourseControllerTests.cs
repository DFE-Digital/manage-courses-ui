using System;
using System.Threading.Tasks;
using GovUk.Education.ManageCourses.ApiClient;
using GovUk.Education.ManageCourses.Ui.Controllers;
using GovUk.Education.ManageCourses.Ui.ViewModels;
using ManageCoursesUi.Tests.Helpers;
using Microsoft.AspNetCore.Mvc;
using NUnit.Framework;
using Moq;

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
            var testData = TestHelper.GetTestData(type);

            manageApi.Setup(x => x.GetCoursesByOrganisation(It.IsAny<string>())).ReturnsAsync(testData);

            var controller = new CourseController(manageApi.Object);
            var result = await controller.Variants(TestHelper.InstitutionCode, TestHelper.AccreditedProviderId, TestHelper.TargetedUcasCode);
            
            Assert.IsInstanceOf(typeof(ViewResult), result);

            Assert.IsTrue((result as ViewResult)?.Model is FromUcasViewModel model &&
                          model.CourseTitle == TestHelper.TargetedCourseTitle &&
                          model.OrganisationId == TestHelper.OrganisationId &&
                          model.OrganisationName == TestHelper.OrganisationName);
        }
        [Test]
        [TestCase(EnumDataType.SingleVariantNoMatch)]
        [TestCase(EnumDataType.MultiVariantNoMatch)]
        public async Task TestController_Variants_should_return_exception(EnumDataType type)
        {
            var exceptionCalled = false;
            var manageApi = new Mock<IManageApi>();
            var testData = TestHelper.GetTestData(type);

            manageApi.Setup(x => x.GetCoursesByOrganisation(It.IsAny<string>())).ReturnsAsync(testData);

            var controller = new CourseController(manageApi.Object);

            try
            {
                await controller.Variants(TestHelper.InstitutionCode, TestHelper.AccreditedProviderId, TestHelper.TargetedUcasCode);
            }
            catch (Exception e)
            {
                exceptionCalled = (e.Message == "Unexpected error: course should not be null");
            }            
            Assert.IsTrue(exceptionCalled);
        }
        [Test]
        [TestCase("xxx", "xxx", "xxx")]
        [TestCase("xxx", "xxx", "35L6")]
        [TestCase("xxx", "self", "xxx")]
        [TestCase("2AT", "xxx", "xxx")]
        [TestCase("2AT", "self", "xxx")]
        [TestCase("2AT", "xxx", "35L6")]
        [TestCase("xxx", "self", "35L6")]
        public async Task TestController_Variants_with_incorrect_parameters_should_return_exception(string institutionCode, string accreditedProviderId, string ucasCode)
        {
            var exceptionCalled = false;
            var manageApi = new Mock<IManageApi>();
            var testData = TestHelper.GetTestData(EnumDataType.SingleVariantOneMatch);

            manageApi.Setup(x => x.GetCoursesByOrganisation("2AT")).ReturnsAsync(testData);

            var controller = new CourseController(manageApi.Object);

            try
            {
                await controller.Variants(institutionCode, accreditedProviderId, ucasCode);
            }
            catch
            {
                exceptionCalled = true;
            }
            Assert.IsTrue(exceptionCalled);
        }
        [Test]
        [TestCase("", "xxx", "xxx", "instCode cannot be null or empty")]
        [TestCase(null, "self", "35L6", "instCode cannot be null or empty")]
        [TestCase("2AT", "", "35L6", "accreditingProviderId cannot be null or empty")]
        [TestCase("2AT", null, "35L6", "accreditingProviderId cannot be null or empty")]
        [TestCase("2AT", "self", "", "ucasCode cannot be null or empty")]
        [TestCase("2AT", "self", null, "ucasCode cannot be null or empty")]
        public async Task TestController_Variants_with_null_or_empty_parameters_should_return_exception(string institutionCode, string accreditedProviderId, string ucasCode, string expectedMessage)
        {
            var exceptionCalled = false;
            var manageApi = new Mock<IManageApi>();
            var testData = TestHelper.GetTestData(EnumDataType.SingleVariantOneMatch);

            manageApi.Setup(x => x.GetCoursesByOrganisation(It.IsAny<string>())).ReturnsAsync(testData);

            var controller = new CourseController(manageApi.Object);

            try
            {
                await controller.Variants(institutionCode, accreditedProviderId, ucasCode);
            }
            catch (ArgumentNullException e)
            {
                exceptionCalled = (e.Message == expectedMessage);
            }
            Assert.IsTrue(exceptionCalled);
        }
        [Test]
        public async Task TestController_Variants__with_api_exception_should_return_exception()
        {
            var exceptionCalled = false;
            var manageApi = new Mock<IManageApi>();

            manageApi.Setup(x => x.GetCoursesByOrganisation(It.IsAny<string>())).ThrowsAsync(new Exception());

            var controller = new CourseController(manageApi.Object);

            try
            {
                await controller.Variants(TestHelper.InstitutionCode, TestHelper.AccreditedProviderId, TestHelper.TargetedUcasCode);
            }
            catch
            {
                exceptionCalled = true;
            }
            Assert.IsTrue(exceptionCalled);
        }
    }
}
