using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using NUnit.Framework;
using Moq;

using GovUk.Education.ManageCourses.Ui.Helpers;
using GovUk.Education.ManageCourses.Ui.ViewModels;
using GovUk.Education.ManageCourses.Ui.Controllers;
using GovUk.Education.ManageCourses.ApiClient;
using GovUk.Education.ManageCourses.Ui;

namespace ManageCoursesUi.Tests
{
    [TestFixture]
    public class OrganisationControllerTests
    {
        [Test]
        public async Task Courses()
        {
            var ucasCode = "ucasCode";
            var organisationName = "organisationName";
            var currentTab = "courses";
            // Todo: fix this ObservableCollection.
            var orgCourses = new OrganisationCourses
            {
                UcasCode = ucasCode,
                ProviderCourses = new ObservableCollection<ProviderCourse>
                { new ProviderCourse
                { CourseDetails = new ObservableCollection<CourseDetail>
                { new CourseDetail
                { Variants = new ObservableCollection<CourseVariant>
                { new CourseVariant
                { UcasCode = ucasCode } } } } } }
            };
            var orgs = new List<UserOrganisation> {
                new UserOrganisation(), new UserOrganisation {
                    UcasCode = ucasCode,
                    OrganisationName = organisationName }
            };

            var apiMock = new Mock<IManageApi>();
            apiMock.Setup(x => x.GetCoursesByOrganisation(ucasCode))
                .ReturnsAsync(orgCourses);

            apiMock.Setup(x => x.GetOrganisations())
                .ReturnsAsync(orgs);

            var controller = new OrganisationController(apiMock.Object);

            var result = await controller.Courses(ucasCode);

            var viewResult = result as ViewResult;

            Assert.IsNotNull(viewResult);
            var model = viewResult.ViewData.Model as CourseListViewModel;
            Assert.AreEqual(orgCourses, model.Courses);
            Assert.AreEqual(1, model.TotalCount);

            var tabViewModel = model.TabViewModel;
            Assert.AreEqual(currentTab, tabViewModel.CurrentTab);
            Assert.AreEqual(organisationName, tabViewModel.OrganisationName);
            Assert.AreEqual(ucasCode, tabViewModel.UcasCode);
            Assert.IsTrue(tabViewModel.MultipleOrganisations);
        }

        [Test]
        public async Task RequestAccess()
        {
            var ucasCode = "ucasCode";
            var organisationName = "organisationName";
            var currentTab = "request-access";

            var orgs = new List<UserOrganisation> {
                 new UserOrganisation {
                    UcasCode = ucasCode,
                    OrganisationName = organisationName }
            };

            var apiMock = new Mock<IManageApi>();

            apiMock.Setup(x => x.GetOrganisations())
                .ReturnsAsync(orgs);

            var controller = new OrganisationController(apiMock.Object);

            var result = await controller.RequestAccess(ucasCode);

            var viewResult = result as ViewResult;

            Assert.IsNotNull(viewResult);
            var model = viewResult.ViewData.Model as RequestAccessViewModel;

            var tabViewModel = model.TabViewModel;
            Assert.AreEqual(currentTab, tabViewModel.CurrentTab);
            Assert.AreEqual(organisationName, tabViewModel.OrganisationName);
            Assert.AreEqual(ucasCode, tabViewModel.UcasCode);
            Assert.IsFalse(tabViewModel.MultipleOrganisations);
        }

        [Test]
        public async Task RequestAccessPost_invalid()
        {
            var ucasCode = "ucasCode";
            var organisationName = "organisationName";
            var currentTab = "request-access";

            var orgs = new List<UserOrganisation> {
                 new UserOrganisation {
                    UcasCode = ucasCode,
                    OrganisationName = organisationName }
            };

            var apiMock = new Mock<IManageApi>();

            apiMock.Setup(x => x.GetOrganisations())
                .ReturnsAsync(orgs);

            var controller = new OrganisationController(apiMock.Object);
            controller.ModelState.AddModelError("you", "failed");
            var result = await controller.RequestAccessPost(ucasCode, new RequestAccessViewModel());

            var viewResult = result as ViewResult;

            Assert.IsNotNull(viewResult);
            var model = viewResult.ViewData.Model as RequestAccessViewModel;

            var tabViewModel = model.TabViewModel;
            Assert.AreEqual(currentTab, tabViewModel.CurrentTab);
            Assert.AreEqual(organisationName, tabViewModel.OrganisationName);
            Assert.AreEqual(ucasCode, tabViewModel.UcasCode);
            Assert.IsFalse(tabViewModel.MultipleOrganisations);
        }

        [Test]
        public async Task RequestAccessPost()
        {
            var ucasCode = "ucasCode";
            var viewModel = new RequestAccessViewModel { FirstName = "FirstName", LastName = "LastName" };
            var tempKey = "RequestAccess_To_Name";

            var apiMock = new Mock<IManageApi>();

            var tempDataMock = new Mock<ITempDataDictionary>();

            var controller = new OrganisationController(apiMock.Object);
            controller.TempData = tempDataMock.Object;

            var result = await controller.RequestAccessPost(ucasCode, viewModel);

            apiMock.Verify(x => x.GetOrganisations(), Times.Never);
            apiMock.Verify(x => x.LogAccessRequest(It.IsAny<AccessRequest>()), Times.Once);

            tempDataMock.Verify(x => x.Add(tempKey, $"{viewModel.FirstName} {viewModel.LastName}"));

            var actionResult = result as RedirectToActionResult;

            Assert.IsNotNull(actionResult);
            Assert.AreEqual("RequestAccess", actionResult.ActionName);
            Assert.AreEqual("Organisation", actionResult.ControllerName);
            Assert.AreEqual(ucasCode, actionResult.RouteValues[ucasCode]);

        }

        [Test]
        public async Task About()
        {
            var ucasCode = "ucasCode";
            var organisationName = "OrganisationName";
            var currentTab = "about";
            var domainName = "DomainName";

            var orgs = new List<UserOrganisation> {
                        new UserOrganisation {
                            UcasCode = ucasCode,
                            OrganisationName = organisationName }
                    };

            var organisation = new Organisation(){
                DomainName = "DomainName"
            };

            var apiMock = new Mock<IManageApi>();

            apiMock.Setup(x => x.GetOrganisations())
                .ReturnsAsync(orgs);

            apiMock.Setup(x => x.GetOrganisationDetails(ucasCode))
                .ReturnsAsync(organisation);

            var controller = new OrganisationController(apiMock.Object);

            var result = await controller.About(ucasCode);

            var viewResult = result as ViewResult;

            Assert.IsNotNull(viewResult);
            var organisationViewModel = viewResult.ViewData.Model as OrganisationViewModel;

            var tabViewModel = organisationViewModel.TabViewModel;
            Assert.AreEqual(currentTab, tabViewModel.CurrentTab);
            Assert.AreEqual(organisationName, tabViewModel.OrganisationName);
            Assert.AreEqual(ucasCode, tabViewModel.UcasCode);
            Assert.AreEqual(domainName, organisationViewModel.DomainName);
            Assert.IsFalse(tabViewModel.MultipleOrganisations);
        }

        [Test]
        public async Task AboutPost()
        {
            var ucasCode = "ucasCode";
            var viewModel = new OrganisationViewModel{ DomainName = "DomainName" };

            var apiMock = new Mock<IManageApi>();

            var controller = new OrganisationController(apiMock.Object);

            var result = await controller.AboutPost(ucasCode, viewModel);

            apiMock.Verify(x => x.GetOrganisations(), Times.Never);
            apiMock.Verify(x => x.SaveOrganisationDetails(It.IsAny<Organisation>()), Times.Once);

            var actionResult = result as RedirectToActionResult;

            Assert.IsNotNull(actionResult);
            Assert.AreEqual("About", actionResult.ActionName);
            Assert.AreEqual("Organisation", actionResult.ControllerName);
            Assert.AreEqual(ucasCode, actionResult.RouteValues[ucasCode]);
        }
    }
}
