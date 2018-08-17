using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GovUk.Education.ManageCourses.ApiClient;
using GovUk.Education.ManageCourses.Ui;
using GovUk.Education.ManageCourses.Ui.Controllers;
using GovUk.Education.ManageCourses.Ui.Helpers;
using GovUk.Education.ManageCourses.Ui.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Moq;
using NUnit.Framework;

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
            var instCourses = new InstitutionCourses
            {
                InstitutionCode = ucasCode,
                InstitutionName = organisationName,
                Courses = new ObservableCollection<Course>
                {
                new Course
                {
                InstCode = ucasCode
                }
                }
            };
            var orgs = new List<UserOrganisation>
            {
                new UserOrganisation(),
                new UserOrganisation
                {
                UcasCode = ucasCode,
                OrganisationName = organisationName
                }
            };

            var apiMock = new Mock<IManageApi>();
            apiMock.Setup(x => x.GetCoursesByOrganisation(ucasCode)).ReturnsAsync(instCourses);

            apiMock.Setup(x => x.GetOrganisations()).ReturnsAsync(orgs);

            var controller = new OrganisationController(apiMock.Object, GetFeatureFlagMock().Object);

            var result = await controller.Courses(ucasCode);

            var viewResult = result as ViewResult;

            Assert.IsNotNull(viewResult);
            var model = viewResult.ViewData.Model as CourseListViewModel;

            Assert.NotNull(model);
            Assert.AreEqual(1, model.Providers.Count);

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

            var orgs = new List<UserOrganisation>
            {
                new UserOrganisation
                {
                UcasCode = ucasCode,
                OrganisationName = organisationName
                }
            };

            var apiMock = new Mock<IManageApi>();

            apiMock.Setup(x => x.GetOrganisations())
                .ReturnsAsync(orgs);

            var controller = new OrganisationController(apiMock.Object, GetFeatureFlagMock().Object);

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

            var orgs = new List<UserOrganisation>
            {
                new UserOrganisation
                {
                UcasCode = ucasCode,
                OrganisationName = organisationName
                }
            };

            var apiMock = new Mock<IManageApi>();

            apiMock.Setup(x => x.GetOrganisations())
                .ReturnsAsync(orgs);

            var controller = new OrganisationController(apiMock.Object, GetFeatureFlagMock().Object);
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

            var controller = new OrganisationController(apiMock.Object, GetFeatureFlagMock().Object);
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

            var userOrganisations = new List<UserOrganisation>
            {
                new UserOrganisation
                {
                UcasCode = ucasCode,
                OrganisationName = organisationName
                }
            };

            var currentTab = "about";
            var trainWithUs = "TrainWithUs";
            var trainWithDisability = "TrainWithDisability";

            var description = "Description";
            var institutionName = "InstitutionName";
            var institutionCourses = new InstitutionCourses
            {
                Courses = new ObservableCollection<Course>
                {
                new Course { },
                new Course { AccreditingProviderId = ucasCode.ToUpperInvariant() },
                new Course { AccreditingProviderId = ucasCode.ToLowerInvariant() },
                new Course { AccreditingProviderId = ucasCode },
                new Course { AccreditingProviderId = ucasCode + 1, AccreditingProviderName = institutionName },
                new Course { AccreditingProviderId = ucasCode + 2 },
                }
            };
            var now = DateTime.Now;
            var ucasInstitutionEnrichmentGetModel = new UcasInstitutionEnrichmentGetModel()
            {
                LastPublishedTimestampUtc = now,
                Status = EnumStatus.Published,
                EnrichmentModel = new InstitutionEnrichmentModel
                {
                AccreditingProviderEnrichments = new ObservableCollection<AccreditingProviderEnrichment>
                {
                new AccreditingProviderEnrichment { UcasInstitutionCode = ucasCode + 2, Description = description }
                },
                TrainWithUs = trainWithUs,
                TrainWithDisability = trainWithDisability
                }
            };

            var apiMock = new Mock<IManageApi>();

            apiMock.Setup(x => x.GetOrganisations())
                .ReturnsAsync(userOrganisations);

            apiMock.Setup(x => x.GetCoursesByOrganisation(ucasCode))
                .ReturnsAsync(institutionCourses);

            apiMock.Setup(x => x.GetEnrichmentOrganisation(ucasCode))
                .ReturnsAsync(ucasInstitutionEnrichmentGetModel);

            var controller = new OrganisationController(apiMock.Object, GetFeatureFlagMock().Object);

            var result = await controller.About(ucasCode);

            var viewResult = result as ViewResult;

            Assert.IsNotNull(viewResult);
            var organisationViewModel = viewResult.ViewData.Model as OrganisationViewModel;

            var tabViewModel = organisationViewModel.TabViewModel;
            Assert.AreEqual(currentTab, tabViewModel.CurrentTab);
            Assert.AreEqual(organisationName, tabViewModel.OrganisationName);
            Assert.AreEqual(ucasCode, tabViewModel.UcasCode);
            Assert.AreEqual(trainWithUs, organisationViewModel.TrainWithUs);
            Assert.AreEqual(2, organisationViewModel.AboutTrainingProviders.Count);
            Assert.AreEqual(description, organisationViewModel.AboutTrainingProviders.First(x => x.InstitutionCode == ucasCode + 2).Description);
            Assert.AreEqual(institutionName, organisationViewModel.AboutTrainingProviders.First(x => x.InstitutionCode == ucasCode + 1).InstitutionName);
            Assert.AreEqual(trainWithDisability, organisationViewModel.TrainWithDisability);
            Assert.IsFalse(tabViewModel.MultipleOrganisations);
            Assert.AreEqual(now, organisationViewModel.LastPublishedTimestampUtc);
            Assert.AreEqual(EnumStatus.Published, organisationViewModel.Status);
        }

        [Test]
        public void EnrichmentFeatureFlag()
        {
            // ARRANGE

            var flagMock = new Mock<IFeatureFlags>();
            flagMock.SetupGet(x => x.ShowOrgEnrichment).Returns(false);
            var controller = new OrganisationController(new Mock<IManageApi>().Object, flagMock.Object);

            // ACT
            
            var res = controller.About("123").Result;

            // ASSERT

            Assert.That(res is RedirectToActionResult, "About page should redirect");    
            Assert.AreEqual(false, (res as RedirectToActionResult).Permanent, "Redirect shouldn't be permanent");
            Assert.AreEqual("Courses", (res as RedirectToActionResult).ActionName, "Redirect should go to courses action");            
            Assert.AreEqual("123", (res as RedirectToActionResult).RouteValues["ucasCode"], "Redirect should use same UCAS Code");
        }

        [Test]
        public async Task AboutPost()
        {
            var ucasCode = "ucasCode";
            var viewModel = new OrganisationViewModel
            {
                AboutTrainingProviders = new List<TrainingProviderViewModel>()
            };

            var apiMock = new Mock<IManageApi>();

            var enrichmentModel = new InstitutionEnrichmentModel { };

            var ucasInstitutionEnrichmentGetModel = new UcasInstitutionEnrichmentGetModel()
            {
                EnrichmentModel = enrichmentModel
            };

            apiMock.Setup(x => x.GetEnrichmentOrganisation(ucasCode))
                .ReturnsAsync(ucasInstitutionEnrichmentGetModel);

            var controller = new OrganisationController(apiMock.Object, GetFeatureFlagMock().Object);

            controller.TempData = new Mock<ITempDataDictionary>().Object;

            var result = await controller.AboutPost(ucasCode, viewModel);

            apiMock.Verify(x => x.SaveEnrichmentOrganisation(ucasCode, It.IsAny<UcasInstitutionEnrichmentPostModel>()), Times.Once);

            var actionResult = result as RedirectToActionResult;

            Assert.IsNotNull(actionResult);
            Assert.AreEqual("About", actionResult.ActionName);
            Assert.AreEqual("Organisation", actionResult.ControllerName);
            Assert.AreEqual(ucasCode, actionResult.RouteValues[ucasCode]);
        }

        
        private Mock<IFeatureFlags> GetFeatureFlagMock()
        {
            var mock = new Mock<IFeatureFlags>();
            mock.SetupGet(x => x.ShowOrgEnrichment).Returns(true);
            return mock;
        }
    }
}
