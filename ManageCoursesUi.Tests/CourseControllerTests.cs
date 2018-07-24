using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
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
        public async Task TestController_Variants_should_return_model()
        {
            var manageApi = new Mock<IManageApi>();
            var testData = TestHelper.GetTestData(EnumDataType.HappyPath);

            manageApi.Setup(x => x.GetCoursesByOrganisation(It.IsAny<string>())).ReturnsAsync(testData);

            var controller = new CourseController(manageApi.Object);
            var result = await controller.Variants(TestHelper.InstitutionCode, TestHelper.AccreditedProviderId, TestHelper.UcasCode);
            
            Assert.IsInstanceOf(typeof(ViewResult), result);

            Assert.IsTrue((result as ViewResult)?.Model is FromUcasViewModel model && model.CourseTitle == TestHelper.CourseTitle && model.OrganisationId == TestHelper.OrganisationId && model.OrganisationName == TestHelper.OrganisationName);
        }
        [Test]
        public async Task TestController_Variants_should_return_exception()
        {
            var manageApi = new Mock<IManageApi>();
            var testData = TestHelper.GetTestData(EnumDataType.ExceptionPath);

            manageApi.Setup(x => x.GetCoursesByOrganisation(It.IsAny<string>())).ReturnsAsync(testData);

            var controller = new CourseController(manageApi.Object);

            try
            {
                await controller.Variants(TestHelper.InstitutionCode, TestHelper.AccreditedProviderId, TestHelper.UcasCode);
            }
            catch (Exception e)
            {
                Assert.IsTrue(e.Message == "Unexpected error: course should not be null");
            }
            
        }
    }
}
