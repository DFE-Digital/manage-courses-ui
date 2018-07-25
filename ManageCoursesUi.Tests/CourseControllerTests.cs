using System;
using System.Threading.Tasks;
using GovUk.Education.ManageCourses.ApiClient;
using GovUk.Education.ManageCourses.Ui.Controllers;
using GovUk.Education.ManageCourses.Ui.ViewModels;
using ManageCoursesUi.Tests.Enums;
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
            var testData = TestHelper.GetTestData(type, null, null);

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
        public void TestController_Variants_should_return_exception(EnumDataType type)
        {        
            var manageApi = new Mock<IManageApi>();
            var testData = TestHelper.GetTestData(type, null, null);

            manageApi.Setup(x => x.GetCoursesByOrganisation(It.IsAny<string>())).ReturnsAsync(testData);

            var controller = new CourseController(manageApi.Object);

            Assert.ThrowsAsync<InvalidOperationException>(async () => await controller.Variants(TestHelper.InstitutionCode, TestHelper.AccreditedProviderId, TestHelper.TargetedUcasCode));
        }
        [Test]
        [TestCase("2AT", "xxx", "xxx")]
        [TestCase("2AT", "self", "xxx")]
        [TestCase("2AT", "xxx", "35L6")]
        [TestCase("xxx", "xxx", "xxx")]
        [TestCase("xxx", "xxx", "35L6")]
        [TestCase("xxx", "self", "xxx")]
        [TestCase("xxx", "self", "35L6")]
        public async Task TestController_Variants_with_incorrect_parameters_should_return_note_found_status(string institutionCode, string accreditedProviderId, string ucasCode)
        {
            var manageApi = new Mock<IManageApi>();
            var testData = TestHelper.GetTestData(EnumDataType.SingleVariantOneMatch, "123", "provider Name");

            manageApi.Setup(x => x.GetCoursesByOrganisation("2AT")).ReturnsAsync(testData);

            var controller = new CourseController(manageApi.Object);

            var result = await controller.Variants(institutionCode, accreditedProviderId, ucasCode);

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

            manageApi.Setup(x => x.GetCoursesByOrganisation(It.IsAny<string>())).ReturnsAsync(testData);

            var controller = new CourseController(manageApi.Object);
            Assert.ThrowsAsync<ArgumentNullException>(async () => await controller.Variants(institutionCode, accreditedProviderId, ucasCode));
        }
        [Test]
        public void TestController_Variants__with_api_exception_should_return_exception()
        {
            var manageApi = new Mock<IManageApi>();

            manageApi.Setup(x => x.GetCoursesByOrganisation(It.IsAny<string>())).ThrowsAsync(new Exception());

            var controller = new CourseController(manageApi.Object);
            Assert.ThrowsAsync<Exception>(async () => await controller.Variants(TestHelper.InstitutionCode, TestHelper.AccreditedProviderId, TestHelper.TargetedUcasCode));
        }
    }
}
