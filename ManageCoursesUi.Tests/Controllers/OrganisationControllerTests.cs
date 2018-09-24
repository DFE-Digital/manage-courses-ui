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
using ManageCoursesUi.Tests.Mocks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Moq;
using NUnit.Framework;

namespace ManageCoursesUi.Tests
{
    [TestFixture]
    public class OrganisationControllerTests
    {
        [Test]
        public async Task Show()
        {
            var ucasCode = "UCASCODE";
            var organisationName = "organisationName";
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

            var controller = new OrganisationController(apiMock.Object, new MockFeatureFlags());

            var result = await controller.Show(ucasCode);

            var viewResult = result as ViewResult;

            Assert.IsNotNull(viewResult);
            var model = viewResult.ViewData.Model as CourseListViewModel;

            Assert.NotNull(model);
            Assert.AreEqual(1, model.Providers.Count);

            Assert.IsTrue(model.MultipleOrganisations);
        }

        [Test]
        public async Task RequestAccess()
        {
            var ucasCode = "UCASCODE";
            var organisationName = "organisationName";

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

            var controller = new OrganisationController(apiMock.Object, new MockFeatureFlags());

            var result = await controller.RequestAccess(ucasCode);

            var viewResult = result as ViewResult;

            Assert.IsNotNull(viewResult);
            var model = viewResult.ViewData.Model as RequestAccessViewModel;

            Assert.AreEqual(ucasCode, controller.ViewBag.UcasCode);
        }

        [Test]
        public async Task RequestAccessPost_invalid()
        {
            var ucasCode = "UCASCODE";
            var organisationName = "organisationName";

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

            var controller = new OrganisationController(apiMock.Object, new MockFeatureFlags());
            controller.ModelState.AddModelError("you", "failed");
            var result = await controller.RequestAccessPost(ucasCode, new RequestAccessViewModel());

            var viewResult = result as ViewResult;

            Assert.IsNotNull(viewResult);
            var model = viewResult.ViewData.Model as RequestAccessViewModel;

            Assert.AreEqual(ucasCode, controller.ViewBag.UcasCode);
        }

        [Test]
        public async Task RequestAccessPost()
        {
            var ucasCode = "UCASCODE";
            var viewModel = new RequestAccessViewModel { FirstName = "FirstName", LastName = "LastName" };
            var tempKey = "RequestAccess_To_Name";

            var apiMock = new Mock<IManageApi>();

            var tempDataMock = new Mock<ITempDataDictionary>();

            var controller = new OrganisationController(apiMock.Object, new MockFeatureFlags());
            controller.TempData = tempDataMock.Object;

            var result = await controller.RequestAccessPost(ucasCode, viewModel);

            apiMock.Verify(x => x.GetOrganisations(), Times.Never);
            apiMock.Verify(x => x.LogAccessRequest(It.IsAny<AccessRequest>()), Times.Once);

            tempDataMock.Verify(x => x.Add(tempKey, $"{viewModel.FirstName} {viewModel.LastName}"));

            var actionResult = result as RedirectToActionResult;

            Assert.IsNotNull(actionResult);
            Assert.AreEqual("Show", actionResult.ActionName);
            Assert.AreEqual("Organisation", actionResult.ControllerName);
            Assert.AreEqual(ucasCode, actionResult.RouteValues[ucasCode]);
        }

        [Test]
        public async Task About()
        {
            var ucasCode = "UCASCODE";
            var organisationName = "OrganisationName";

            var userOrganisations = new List<UserOrganisation>
            {
                new UserOrganisation
                {
                UcasCode = ucasCode,
                OrganisationName = organisationName
                }
            };

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

            apiMock.Setup(x => x.GetUcasInstitution(ucasCode)).ReturnsAsync(
                new UcasInstitution { InstCode = ucasCode, InstFull = organisationName }
            );

            apiMock.Setup(x => x.GetCoursesByOrganisation(ucasCode))
                .ReturnsAsync(institutionCourses);

            apiMock.Setup(x => x.GetEnrichmentOrganisation(ucasCode))
                .ReturnsAsync(ucasInstitutionEnrichmentGetModel);

            var controller = new OrganisationController(apiMock.Object, new MockFeatureFlags());

            var result = await controller.About(ucasCode);

            var viewResult = result as ViewResult;

            Assert.IsNotNull(viewResult);
            var organisationViewModel = viewResult.ViewData.Model as OrganisationViewModelForAbout;

            Assert.AreEqual(organisationName, organisationViewModel.InstitutionName);
            Assert.AreEqual(trainWithUs, organisationViewModel.TrainWithUs);
            Assert.AreEqual(2, organisationViewModel.AboutTrainingProviders.Count);
            Assert.AreEqual(description, organisationViewModel.AboutTrainingProviders.First(x => x.InstitutionCode == ucasCode + 2).Description);
            Assert.AreEqual(institutionName, organisationViewModel.AboutTrainingProviders.First(x => x.InstitutionCode == ucasCode + 1).InstitutionName);
            Assert.AreEqual(trainWithDisability, organisationViewModel.TrainWithDisability);
            Assert.AreEqual(now, organisationViewModel.LastPublishedTimestampUtc);
            Assert.AreEqual(EnumStatus.Published, organisationViewModel.Status);
        }

        [Test]
        public async Task AboutPost_SaveOrganisation()
        {
            var ucasCode = "UCASCODE";
            var viewModel = new OrganisationViewModelForAbout
            {
                AboutTrainingProviders = new List<TrainingProviderViewModel>()
            };

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

            var apiMock = new Mock<IManageApi>();
            apiMock.Setup(x => x.GetCoursesByOrganisation(ucasCode))
                .ReturnsAsync(institutionCourses);

            var enrichmentModel = new InstitutionEnrichmentModel { AccreditingProviderEnrichments = new ObservableCollection<AccreditingProviderEnrichment> { } };

            var ucasInstitutionEnrichmentGetModel = new UcasInstitutionEnrichmentGetModel()
            {
                EnrichmentModel = enrichmentModel
            };

            apiMock.Setup(x => x.GetEnrichmentOrganisation(ucasCode))
                .ReturnsAsync(ucasInstitutionEnrichmentGetModel);

            var objectValidator = new Mock<IObjectModelValidator>();
            objectValidator.Setup(o => o.Validate(It.IsAny<ActionContext>(),
                It.IsAny<ValidationStateDictionary>(),
                It.IsAny<string>(),
                It.IsAny<Object>()));

            var controller = new OrganisationController(apiMock.Object, new MockFeatureFlags());

            controller.ObjectValidator = objectValidator.Object;

            controller.TempData = new Mock<ITempDataDictionary>().Object;

            var result = await controller.AboutPost(ucasCode, viewModel);

            apiMock.Verify(x => x.SaveEnrichmentOrganisation(ucasCode, It.IsAny<UcasInstitutionEnrichmentPostModel>()), Times.Once);

            var actionResult = result as RedirectToActionResult;

            Assert.IsNotNull(actionResult);
            Assert.AreEqual("Details", actionResult.ActionName);
            Assert.AreEqual("Organisation", actionResult.ControllerName);
            Assert.AreEqual(ucasCode, actionResult.RouteValues[ucasCode]);
        }

        [Test]
        public async Task DetailsPost_PublishOrganisation_WhenApiReturnsFalse()
        {
            var ucasCode = "UCASCODE";
            var institutionName = "InstitutionName";

            var viewModel = new OrganisationViewModel
            {
                AboutTrainingProviders = new List<TrainingProviderViewModel>()
            };

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

            var apiMock = new Mock<IManageApi>();

            apiMock.Setup(x => x.GetUcasInstitution(ucasCode))
                .ReturnsAsync(new UcasInstitution { InstCode = ucasCode, InstFull = institutionName });

            apiMock.Setup(x => x.GetCoursesByOrganisation(ucasCode))
                .ReturnsAsync(institutionCourses);

            var enrichmentModel = new InstitutionEnrichmentModel { AccreditingProviderEnrichments = new ObservableCollection<AccreditingProviderEnrichment> { } };

            var ucasInstitutionEnrichmentGetModel = new UcasInstitutionEnrichmentGetModel()
            {
                EnrichmentModel = enrichmentModel
            };

            apiMock.Setup(x => x.GetEnrichmentOrganisation(ucasCode))
                .ReturnsAsync(ucasInstitutionEnrichmentGetModel);

            var objectValidator = new Mock<IObjectModelValidator>();
            objectValidator.Setup(o => o.Validate(It.IsAny<ActionContext>(),
                It.IsAny<ValidationStateDictionary>(),
                It.IsAny<string>(),
                It.IsAny<Object>()));

            var controller = new OrganisationController(apiMock.Object, new MockFeatureFlags());
            controller.ObjectValidator = objectValidator.Object;

            controller.TempData = new Mock<ITempDataDictionary>().Object;

            Assert.ThrowsAsync<InvalidOperationException>( async () => await controller.DetailsPost(ucasCode, viewModel));
        }

        [Test]
        public async Task DetailsPost_PublishOrganisation_WhenApiReturnsTrue()
        {
            var ucasCode = "UCASCODE";
            var viewModel = new OrganisationViewModel
            {
                AboutTrainingProviders = new List<TrainingProviderViewModel>()  
            };
                
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
            var apiMock = new Mock<IManageApi>();
            
            apiMock.Setup(x => x.GetUcasInstitution(ucasCode))
                .ReturnsAsync(new UcasInstitution { InstCode = ucasCode, InstFull = institutionName });

            apiMock.Setup(x => x.GetCoursesByOrganisation(ucasCode))
                .ReturnsAsync(institutionCourses);

            var enrichmentModel = new InstitutionEnrichmentModel { AccreditingProviderEnrichments = new ObservableCollection<AccreditingProviderEnrichment>() };

            var ucasInstitutionEnrichmentGetModel = new UcasInstitutionEnrichmentGetModel()
            {
                EnrichmentModel = enrichmentModel
            };

            apiMock.Setup(x => x.GetEnrichmentOrganisation(ucasCode))
                .ReturnsAsync(ucasInstitutionEnrichmentGetModel);

            var objectValidator = new Mock<IObjectModelValidator>();
            objectValidator.Setup(o => o.Validate(It.IsAny<ActionContext>(),
                It.IsAny<ValidationStateDictionary>(),
                It.IsAny<string>(),
                It.IsAny<Object>()));

            apiMock.Setup(x => x.PublishEnrichmentOrganisation(ucasCode))
                .ReturnsAsync(true);
            var controller = new OrganisationController(apiMock.Object, new MockFeatureFlags());
            controller.ObjectValidator = objectValidator.Object;

            controller.TempData = new Mock<ITempDataDictionary>().Object;

            var result = await controller.DetailsPost(ucasCode, viewModel);

            var actionResult = result as RedirectToActionResult;

            Assert.IsNotNull(actionResult);
            Assert.AreEqual("Details", actionResult.ActionName);
            Assert.AreEqual(ucasCode, actionResult.RouteValues[ucasCode]);
        }

        [Test]
        public async Task DetailsPost_PublishOrganisation_invalid()
        {
            var ucasCode = "UCASCODE";

            var organisationName = "OrganisationName";

            var userOrganisations = new List<UserOrganisation>
            {
                new UserOrganisation
                {
                UcasCode = ucasCode,
                OrganisationName = organisationName
                }
            };

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

            var viewModel = new OrganisationViewModel
            {
                AboutTrainingProviders = new List<TrainingProviderViewModel>()
            };

            var apiMock = new Mock<IManageApi>();

            apiMock.Setup(x => x.GetUcasInstitution(ucasCode))
                .ReturnsAsync(new UcasInstitution { InstCode = ucasCode, InstFull = organisationName });

            apiMock.Setup(x => x.GetOrganisations())
                .ReturnsAsync(userOrganisations);

            apiMock.Setup(x => x.GetCoursesByOrganisation(ucasCode))
                .ReturnsAsync(institutionCourses);

            apiMock.Setup(x => x.GetEnrichmentOrganisation(ucasCode))
                .ReturnsAsync(ucasInstitutionEnrichmentGetModel);

            apiMock.Setup(x => x.PublishEnrichmentOrganisation(ucasCode))
                .ReturnsAsync(true);
            var controller = new OrganisationControllerMockedValidation(apiMock.Object, new MockFeatureFlags());

            var result = await controller.DetailsPost(ucasCode, viewModel);

            var viewResult = result as ViewResult;

            Assert.IsNotNull(viewResult);
            var organisationViewModel = viewResult.ViewData.Model as OrganisationViewModel;

            Assert.AreEqual(organisationName, organisationViewModel.InstitutionName);
            Assert.AreEqual(trainWithUs, organisationViewModel.TrainWithUs);
            Assert.AreEqual(2, organisationViewModel.AboutTrainingProviders.Count);
            Assert.AreEqual(description, organisationViewModel.AboutTrainingProviders.First(x => x.InstitutionCode == ucasCode + 2).Description);
            Assert.AreEqual(institutionName, organisationViewModel.AboutTrainingProviders.First(x => x.InstitutionCode == ucasCode + 1).InstitutionName);
            Assert.AreEqual(trainWithDisability, organisationViewModel.TrainWithDisability);
            Assert.AreEqual(now, organisationViewModel.LastPublishedTimestampUtc);
            Assert.AreEqual(EnumStatus.Published, organisationViewModel.Status);
        }

        [Test]
        public void AboutPost_ModelState_WordCount()
        {
            var ucasCode = "UCASCODE";
            var exceed100Words = "";
            for (int i = 0; i < 101; i++)
            {
                exceed100Words += i + " ";
            }
            var institutionName = "InstitutionName";

            var viewModel = new OrganisationViewModelForAbout
            {
                AboutTrainingProviders = new List<TrainingProviderViewModel>() {
                    new TrainingProviderViewModel{ Description = exceed100Words,
                    InstitutionName = institutionName,
                    InstitutionCode = ucasCode + 1}
                }
            };

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

            var apiMock = new Mock<IManageApi>();
            apiMock.Setup(x => x.GetCoursesByOrganisation(ucasCode))
                .ReturnsAsync(institutionCourses);


            var enrichmentModel = new InstitutionEnrichmentModel { AccreditingProviderEnrichments = new ObservableCollection<AccreditingProviderEnrichment> { } };

            var ucasInstitutionEnrichmentGetModel = new UcasInstitutionEnrichmentGetModel()
            {
                EnrichmentModel = enrichmentModel
            };

            apiMock.Setup(x => x.GetOrganisations())
                .ReturnsAsync(new List<UserOrganisation> {new UserOrganisation()});

            apiMock.Setup(x => x.GetEnrichmentOrganisation(ucasCode))
                .ReturnsAsync(ucasInstitutionEnrichmentGetModel);

            var objectValidator = new Mock<IObjectModelValidator>();
            objectValidator.Setup(o => o.Validate(It.IsAny<ActionContext>(),
                It.IsAny<ValidationStateDictionary>(),
                It.IsAny<string>(),
                It.IsAny<Object>()));

            var controller = new OrganisationController(apiMock.Object, new MockFeatureFlags());

            controller.ObjectValidator = objectValidator.Object;

            controller.TempData = new Mock<ITempDataDictionary>().Object;

            Assert.IsFalse(controller.ModelState.Any());
            Assert.IsTrue(controller.ModelState.IsValid);
            var result = controller.AboutPost(ucasCode, viewModel).Result;


            Assert.IsTrue(controller.ModelState.Any());
            Assert.AreEqual($"Reduce word count for {institutionName}", controller.ModelState["AboutTrainingProviders_0__Description"].Errors.First().ErrorMessage);
            Assert.IsFalse(controller.ModelState.IsValid);

            var viewResult = result as ViewResult;

            var organisationViewModel = viewResult.ViewData.Model as OrganisationViewModelForAbout;

            Assert.IsNotNull(viewResult);
            Assert.AreEqual(exceed100Words, organisationViewModel.AboutTrainingProviders.First(x => x.InstitutionCode == ucasCode + 1).Description);
            Assert.AreEqual(institutionName, organisationViewModel.AboutTrainingProviders.First(x => x.InstitutionCode == ucasCode + 1).InstitutionName);

        }

        private class MockFeatureFlags : IFeatureFlags
        {
            public bool ShowCourseLiveView => true;
        }
    }
}
