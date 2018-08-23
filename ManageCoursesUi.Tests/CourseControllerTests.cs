using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GovUk.Education.ManageCourses.ApiClient;
using GovUk.Education.ManageCourses.Ui;
using GovUk.Education.ManageCourses.Ui.Controllers;
using GovUk.Education.ManageCourses.Ui.ViewModels;
using ManageCoursesUi.Tests.Enums;
using ManageCoursesUi.Tests.Helpers;
using Microsoft.AspNetCore.Mvc;
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

            var ucasCourseEnrichmentGetModel = new UcasCourseEnrichmentGetModel { EnrichmentModel = new CourseEnrichmentModel { AboutCourse = "AboutCourse", InterviewProcess = "InterviewProcess", HowSchoolPlacementsWork = "HowSchoolPlacementsWork" } };

            manageApi.Setup(x => x.GetOrganisations()).ReturnsAsync(testOrgs);
            manageApi.Setup(x => x.GetCourseByUcasCode(TestHelper.InstitutionCode, TestHelper.TargetedUcasCode)).ReturnsAsync(testCourse);

            manageApi.Setup(x => x.GetEnrichmentCourse(TestHelper.InstitutionCode, TestHelper.TargetedUcasCode)).ReturnsAsync(ucasCourseEnrichmentGetModel);

            var controller = new CourseController(manageApi.Object);
            var result = await controller.Variants(TestHelper.InstitutionCode, TestHelper.AccreditedProviderId, TestHelper.TargetedUcasCode);

            var viewResult = result as ViewResult;
            Assert.IsNotNull(viewResult);

            var model = viewResult.Model as FromUcasViewModel;

            Assert.IsNotNull(model);
            Assert.IsInstanceOf(typeof(ViewResult), result);
            Assert.AreEqual(TestHelper.TargetedCourseTitle, model.CourseTitle);
            Assert.AreEqual(TestHelper.OrganisationId, model.OrganisationId);

            Assert.AreEqual(TestHelper.OrganisationName, model.OrganisationName);

            Assert.AreEqual(TestHelper.InstitutionCode, model.RouteData.InstCode);
            Assert.AreEqual(TestHelper.AccreditedProviderId, model.RouteData.AccreditingProviderId);
            Assert.AreEqual(TestHelper.TargetedUcasCode, model.RouteData.UcasCode);
        }

        [Test]
        [TestCase(EnumDataType.SingleVariantNoMatch)]
        [TestCase(EnumDataType.MultiVariantNoMatch)]
        public void TestController_Variants_should_return_exception(EnumDataType type)
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

            var testCourse = testData.Courses.FirstOrDefault(x => x.Name == TestHelper.TargetedCourseTitle);;
            manageApi.Setup(x => x.GetOrganisations()).ReturnsAsync(testOrgs);
            manageApi.Setup(x => x.GetCourseByUcasCode(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(testCourse);

            var controller = new CourseController(manageApi.Object);

            Assert.ThrowsAsync<InvalidOperationException>(async() => await controller.Variants(TestHelper.InstitutionCode, TestHelper.AccreditedProviderId, TestHelper.TargetedUcasCode));
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

            var controller = new CourseController(manageApi.Object);

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

            var controller = new CourseController(manageApi.Object);
            Assert.ThrowsAsync<ArgumentNullException>(async() => await controller.Variants(institutionCode, accreditedProviderId, ucasCode));
        }

        [Test]
        public void TestController_with_api_exception_on_first_call_should_return_exception()
        {
            var manageApi = new Mock<IManageApi>();

            manageApi.Setup(x => x.GetOrganisations()).ThrowsAsync(new Exception());

            var controller = new CourseController(manageApi.Object);

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

            var controller = new CourseController(manageApi.Object);

            Assert.ThrowsAsync<Exception>(async() => await controller.Variants(TestHelper.InstitutionCode, TestHelper.AccreditedProviderId, TestHelper.TargetedUcasCode));
        }
    }
}
