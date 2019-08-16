using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using GovUk.Education.ManageCourses.Api.Model;
using GovUk.Education.ManageCourses.Domain.Models;
using GovUk.Education.ManageCourses.Ui;
using GovUk.Education.ManageCourses.Ui.Controllers;
using GovUk.Education.ManageCourses.Ui.Services;
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
        public async Task Show_Redirects_To_FrontEnd()
        {
            var providerCode = "PROVIDERCODE";

            var frontendUrlMock = new Mock<IFrontendUrlService>();
            frontendUrlMock.Setup(x => x.ShouldRedirectOrganisationShow()).Returns(true);
            frontendUrlMock.Setup(x => x.RedirectToFrontend("/organisations/" + providerCode)).Returns(new RedirectResult("frontend"));
            var controller = new OrganisationController(null, frontendUrlMock.Object);

            var result = await controller.Show(providerCode);

            Assert.IsTrue(result is RedirectResult);
        }

        [Test]
        public async Task Index_Redirects_To_FrontEnd()
        {
            var frontendUrlMock = new Mock<IFrontendUrlService>();
            frontendUrlMock.Setup(x => x.ShouldRedirectOrganisationShow()).Returns(true);
            frontendUrlMock.Setup(x => x.RedirectToFrontend("/organisations" )).Returns(new RedirectResult("frontend"));
            var controller = new OrganisationController(null, frontendUrlMock.Object);

            var result = await controller.Index();

            Assert.IsTrue(result is RedirectResult);
        }

        [Test]
        public async Task Show()
        {
            var providerCode = "PROVIDERCODE";
            var organisationName = "organisationName";
            // Todo: fix this ObservableCollection.
            var providerCourses = new List<Course>
                {
                    new Course
                    {
                        Provider = new GovUk.Education.ManageCourses.Domain.Models.Provider { ProviderCode = providerCode, ProviderName = organisationName }
                    }
                };

            var orgs = new List<ProviderSummary>
            {
                new ProviderSummary(),
                new ProviderSummary
                {
                    ProviderCode = providerCode,
                    ProviderName = organisationName
                }
            };

            var apiMock = new Mock<IManageApi>();
            apiMock.Setup(x => x.GetCoursesOfProvider(providerCode)).ReturnsAsync(providerCourses);

            apiMock.Setup(x => x.GetProviderSummaries()).ReturnsAsync(orgs);

            apiMock.Setup(x => x.GetProviderSummary(It.IsAny<string>())).ReturnsAsync(orgs[1]);

            var frontendUrlMock = new Mock<IFrontendUrlService>();

            var controller = new OrganisationController(apiMock.Object, frontendUrlMock.Object);

            var result = await controller.Show(providerCode);

            var viewResult = result as ViewResult;

            Assert.IsNotNull(viewResult);
            var model = viewResult.ViewData.Model as CourseListViewModel;

            Assert.NotNull(model);

            Assert.IsTrue(model.MultipleOrganisations);
        }

        [Test]
        public void RequestAccess()
        {
            var providerCode = "PROVIDERCODE";

            var frontendUrlMock = new Mock<IFrontendUrlService>();
            frontendUrlMock.Setup(x => x.ShouldRedirectOrganisationShow()).Returns(true);
            frontendUrlMock.Setup(x => x.RedirectToFrontend("/organisations/" + $"{providerCode}/request-access")).Returns(new RedirectResult("frontend"));
            var controller = new OrganisationController(null, frontendUrlMock.Object);

            var result = controller.RequestAccess(providerCode);

            Assert.IsTrue(result is RedirectResult);
        }

        [Test]
        public async Task RequestAccessPost_invalid()
        {
            var providerCode = "PROVIDERCODE";
            var organisationName = "organisationName";

            var orgs = new List<ProviderSummary>
            {
                new ProviderSummary
                {
                ProviderCode = providerCode,
                ProviderName = organisationName
                }
            };

            var apiMock = new Mock<IManageApi>();

            apiMock.Setup(x => x.GetProviderSummaries())
                .ReturnsAsync(orgs);

            var frontendUrlMock = new Mock<IFrontendUrlService>();

            var controller = new OrganisationController(apiMock.Object, frontendUrlMock.Object);
            controller.ModelState.AddModelError("you", "failed");
            var result = await controller.RequestAccessPost(providerCode, new RequestAccessViewModel());

            var viewResult = result as ViewResult;

            Assert.IsNotNull(viewResult);
            var model = viewResult.ViewData.Model as RequestAccessViewModel;

            Assert.AreEqual(providerCode, controller.ViewBag.ProviderCode);
        }

        [Test]
        public async Task RequestAccessPost()
        {
            var providerCode = "PROVIDERCODE";
            var viewModel = new RequestAccessViewModel { FirstName = "FirstName", LastName = "LastName" };
            var tempKey = "RequestAccess_To_Name";

            var apiMock = new Mock<IManageApi>();

            var tempDataMock = new Mock<ITempDataDictionary>();

            var frontendUrlMock = new Mock<IFrontendUrlService>();

            var controller = new OrganisationController(apiMock.Object, frontendUrlMock.Object);
            controller.TempData = tempDataMock.Object;

            var result = await controller.RequestAccessPost(providerCode, viewModel);

            apiMock.Verify(x => x.GetProviderSummaries(), Times.Never);
            apiMock.Verify(x => x.CreateAccessRequest(It.IsAny<GovUk.Education.ManageCourses.Api.Model.AccessRequest>()), Times.Once);

            tempDataMock.Verify(x => x.Add(tempKey, $"{viewModel.FirstName} {viewModel.LastName}"));

            var actionResult = result as RedirectToActionResult;

            Assert.IsNotNull(actionResult);
            Assert.AreEqual("Show", actionResult.ActionName);
            Assert.AreEqual("Organisation", actionResult.ControllerName);
            Assert.AreEqual(providerCode, actionResult.RouteValues[providerCode]);
        }

        [Test]
        public async Task Details()
        {
            var providerCode = "PROVIDERCODE";

            var frontendUrlMock = new Mock<IFrontendUrlService>();
            frontendUrlMock.Setup(x => x.ShouldRedirectOrganisationShow()).Returns(true);
            frontendUrlMock.Setup(x => x.RedirectToFrontend("/organisations/" + $"{providerCode}/2019/details")).Returns(new RedirectResult("frontend"));
            var controller = new OrganisationController(null, frontendUrlMock.Object);

            var result = await controller.Details(providerCode);

            Assert.IsTrue(result is RedirectResult);
        }

        [Test]
        public async Task About()
        {
            var providerCode = "PROVIDERCODE";

            var frontendUrlMock = new Mock<IFrontendUrlService>();
            frontendUrlMock.Setup(x => x.ShouldRedirectOrganisationShow()).Returns(true);
            frontendUrlMock.Setup(x => x.RedirectToFrontend("/organisations/" + $"{providerCode}/2019/about")).Returns(new RedirectResult("frontend"));
            var controller = new OrganisationController(null, frontendUrlMock.Object);

            var result = await controller.About(providerCode);

            Assert.IsTrue(result is RedirectResult);
        }

        [Test]
        public async Task AboutPost_SaveOrganisation()
        {
            var providerCode = "PROVIDERCODE";
            var viewModel = new OrganisationViewModelForAbout
            {
                AboutTrainingProviders = new List<TrainingProviderViewModel>()
            };

            var providerName = "ProviderName";

            var providerCourses = new List<Course>
                {
                    new Course { },
                    new Course { AccreditingProvider = new GovUk.Education.ManageCourses.Domain.Models.Provider { ProviderCode = providerCode.ToUpperInvariant() }},
                    new Course { AccreditingProvider = new GovUk.Education.ManageCourses.Domain.Models.Provider { ProviderCode = providerCode.ToLowerInvariant() }},
                    new Course { AccreditingProvider = new GovUk.Education.ManageCourses.Domain.Models.Provider { ProviderCode = providerCode }},
                    new Course { AccreditingProvider = new GovUk.Education.ManageCourses.Domain.Models.Provider { ProviderCode = providerCode + 1, ProviderName = providerName }},
                    new Course { AccreditingProvider = new GovUk.Education.ManageCourses.Domain.Models.Provider { ProviderCode = providerCode + 2 }},
                };

            var apiMock = new Mock<IManageApi>();
            apiMock.Setup(x => x.GetCoursesOfProvider(providerCode))
                .ReturnsAsync(providerCourses);

            var enrichmentModel = new ProviderEnrichmentModel { AccreditingProviderEnrichments = new List<AccreditingProviderEnrichment> { } };

            var ucasProviderEnrichmentGetModel = new UcasProviderEnrichmentGetModel()
            {
                EnrichmentModel = enrichmentModel
            };

            apiMock.Setup(x => x.GetProviderEnrichment(providerCode))
                .ReturnsAsync(ucasProviderEnrichmentGetModel);

            var objectValidator = new Mock<IObjectModelValidator>();
            objectValidator.Setup(o => o.Validate(It.IsAny<ActionContext>(),
                It.IsAny<ValidationStateDictionary>(),
                It.IsAny<string>(),
                It.IsAny<Object>()));

            var frontendUrlMock = new Mock<IFrontendUrlService>();

            var controller = new OrganisationController(apiMock.Object, frontendUrlMock.Object);

            controller.ObjectValidator = objectValidator.Object;

            controller.TempData = new Mock<ITempDataDictionary>().Object;

            var result = await controller.AboutPost(providerCode, viewModel);

            apiMock.Verify(x => x.SaveProviderEnrichment(providerCode, It.IsAny<UcasProviderEnrichmentPostModel>()), Times.Once);

            var actionResult = result as RedirectToActionResult;

            Assert.IsNotNull(actionResult);
            Assert.AreEqual("Details", actionResult.ActionName);
            Assert.AreEqual("Organisation", actionResult.ControllerName);
            Assert.AreEqual(providerCode, actionResult.RouteValues[providerCode]);
        }

        [Test]
        public void DetailsPost_PublishOrganisation_WhenApiReturnsFalse()
        {
            var providerCode = "PROVIDERCODE";
            var providerName = "ProviderName";

            var viewModel = new OrganisationViewModel
            {
                AboutTrainingProviders = new List<TrainingProviderViewModel>()
            };

            var providerCourses = new List<Course>
                {
                    new Course { },
                    new Course { AccreditingProvider = new GovUk.Education.ManageCourses.Domain.Models.Provider { ProviderCode = providerCode.ToUpperInvariant() }},
                    new Course { AccreditingProvider = new GovUk.Education.ManageCourses.Domain.Models.Provider { ProviderCode = providerCode.ToLowerInvariant() }},
                    new Course { AccreditingProvider = new GovUk.Education.ManageCourses.Domain.Models.Provider { ProviderCode = providerCode }},
                    new Course { AccreditingProvider = new GovUk.Education.ManageCourses.Domain.Models.Provider { ProviderCode = providerCode + 1, ProviderName = providerName }},
                    new Course { AccreditingProvider = new GovUk.Education.ManageCourses.Domain.Models.Provider { ProviderCode = providerCode + 2 }},
                };

            var apiMock = new Mock<IManageApi>();

            apiMock.Setup(x => x.GetProviderSummary(providerCode))
                .ReturnsAsync(new ProviderSummary { ProviderCode = providerCode, ProviderName = providerName });

            apiMock.Setup(x => x.GetCoursesOfProvider(providerCode))
                .ReturnsAsync(providerCourses);

            var enrichmentModel = new ProviderEnrichmentModel { AccreditingProviderEnrichments = new List<AccreditingProviderEnrichment> { } };

            var ucasProviderEnrichmentGetModel = new UcasProviderEnrichmentGetModel()
            {
                EnrichmentModel = enrichmentModel
            };

            apiMock.Setup(x => x.GetProviderEnrichment(providerCode))
                .ReturnsAsync(ucasProviderEnrichmentGetModel);

            var objectValidator = new Mock<IObjectModelValidator>();
            objectValidator.Setup(o => o.Validate(It.IsAny<ActionContext>(),
                It.IsAny<ValidationStateDictionary>(),
                It.IsAny<string>(),
                It.IsAny<Object>()));

            var frontendUrlMock = new Mock<IFrontendUrlService>();

            var controller = new OrganisationController(apiMock.Object, frontendUrlMock.Object);
            controller.ObjectValidator = objectValidator.Object;

            controller.TempData = new Mock<ITempDataDictionary>().Object;

            Assert.ThrowsAsync<InvalidOperationException>( async () => await controller.DetailsPost(providerCode, viewModel));
        }

        [Test]
        public async Task DetailsPost_PublishOrganisation_WhenApiReturnsTrue()
        {
            var providerCode = "PROVIDERCODE";
            var viewModel = new OrganisationViewModel
            {
                AboutTrainingProviders = new List<TrainingProviderViewModel>()
            };

            var providerName = "ProviderName";

            var providerCourses = new List<Course>
                {
                    new Course { },
                    new Course { AccreditingProvider = new GovUk.Education.ManageCourses.Domain.Models.Provider { ProviderCode = providerCode.ToUpperInvariant() }},
                    new Course { AccreditingProvider = new GovUk.Education.ManageCourses.Domain.Models.Provider { ProviderCode = providerCode.ToLowerInvariant() }},
                    new Course { AccreditingProvider = new GovUk.Education.ManageCourses.Domain.Models.Provider { ProviderCode = providerCode }},
                    new Course { AccreditingProvider = new GovUk.Education.ManageCourses.Domain.Models.Provider { ProviderCode = providerCode + 1, ProviderName = providerName }},
                    new Course { AccreditingProvider = new GovUk.Education.ManageCourses.Domain.Models.Provider { ProviderCode = providerCode + 2 }},
                };
            var apiMock = new Mock<IManageApi>();

            apiMock.Setup(x => x.GetProviderSummary(providerCode))
                .ReturnsAsync(new ProviderSummary { ProviderCode = providerCode, ProviderName = providerName });

            apiMock.Setup(x => x.GetCoursesOfProvider(providerCode))
                .ReturnsAsync(providerCourses);

            var enrichmentModel = new ProviderEnrichmentModel { AccreditingProviderEnrichments = new List<AccreditingProviderEnrichment>() };

            var ucasProviderEnrichmentGetModel = new UcasProviderEnrichmentGetModel()
            {
                EnrichmentModel = enrichmentModel
            };

            apiMock.Setup(x => x.GetProviderEnrichment(providerCode))
                .ReturnsAsync(ucasProviderEnrichmentGetModel);

            var objectValidator = new Mock<IObjectModelValidator>();
            objectValidator.Setup(o => o.Validate(It.IsAny<ActionContext>(),
                It.IsAny<ValidationStateDictionary>(),
                It.IsAny<string>(),
                It.IsAny<Object>()));

            apiMock.Setup(x => x.PublishAllCoursesOfProviderToSearchAndCompare(providerCode))
                .ReturnsAsync(true);
            var frontendUrlMock = new Mock<IFrontendUrlService>();
            var controller = new OrganisationController(apiMock.Object, frontendUrlMock.Object);
            controller.ObjectValidator = objectValidator.Object;

            controller.TempData = new Mock<ITempDataDictionary>().Object;

            var result = await controller.DetailsPost(providerCode, viewModel);

            var actionResult = result as RedirectToActionResult;

            Assert.IsNotNull(actionResult);
            Assert.AreEqual("Details", actionResult.ActionName);
            Assert.AreEqual(providerCode, actionResult.RouteValues[providerCode]);
        }

        [Test]
        public async Task DetailsPost_PublishOrganisation_invalid()
        {
            var providerCode = "PROVIDERCODE";

            var organisationName = "OrganisationName";

            var providerSummaries = new List<ProviderSummary>
            {
                new ProviderSummary
                {
                ProviderCode = providerCode,
                ProviderName = organisationName
                }
            };

            var trainWithUs = "TrainWithUs";
            var trainWithDisability = "TrainWithDisability";

            var description = "Description";
            var providerName = "ProviderName";
            var providerCourses = new List<Course>
                {
                    new Course { },
                    new Course { AccreditingProvider = new GovUk.Education.ManageCourses.Domain.Models.Provider { ProviderCode = providerCode.ToUpperInvariant() }},
                    new Course { AccreditingProvider = new GovUk.Education.ManageCourses.Domain.Models.Provider { ProviderCode = providerCode.ToLowerInvariant() }},
                    new Course { AccreditingProvider = new GovUk.Education.ManageCourses.Domain.Models.Provider { ProviderCode = providerCode }},
                    new Course { AccreditingProvider = new GovUk.Education.ManageCourses.Domain.Models.Provider { ProviderCode = providerCode + 1, ProviderName = providerName }},
                    new Course { AccreditingProvider = new GovUk.Education.ManageCourses.Domain.Models.Provider { ProviderCode = providerCode + 2 }},
                };

            var now = DateTime.Now;
            var ucasProviderEnrichmentGetModel = new UcasProviderEnrichmentGetModel()
            {
                LastPublishedTimestampUtc = now,
                Status = EnumStatus.Published,
                EnrichmentModel = new ProviderEnrichmentModel
                {
                AccreditingProviderEnrichments = new List<AccreditingProviderEnrichment>
                {
                new AccreditingProviderEnrichment { UcasProviderCode = providerCode + 2, Description = description }
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

            apiMock.Setup(x => x.GetProviderSummary(providerCode))
                .ReturnsAsync(new ProviderSummary { ProviderCode = providerCode, ProviderName = organisationName });

            apiMock.Setup(x => x.GetProviderSummaries())
                .ReturnsAsync(providerSummaries);

            apiMock.Setup(x => x.GetCoursesOfProvider(providerCode))
                .ReturnsAsync(providerCourses);

            apiMock.Setup(x => x.GetProviderEnrichment(providerCode))
                .ReturnsAsync(ucasProviderEnrichmentGetModel);

            apiMock.Setup(x => x.PublishAllCoursesOfProviderToSearchAndCompare(providerCode))
                .ReturnsAsync(true);
            var frontendUrlMock = new Mock<IFrontendUrlService>();
            var controller = new OrganisationControllerMockedValidation(apiMock.Object, frontendUrlMock.Object);

            var result = await controller.DetailsPost(providerCode, viewModel);

            var viewResult = result as ViewResult;

            Assert.IsNotNull(viewResult);
            var organisationViewModel = viewResult.ViewData.Model as OrganisationViewModel;

            Assert.AreEqual(organisationName, organisationViewModel.ProviderName);
            Assert.AreEqual(trainWithUs, organisationViewModel.TrainWithUs);
            Assert.AreEqual(2, organisationViewModel.AboutTrainingProviders.Count);
            Assert.AreEqual(description, organisationViewModel.AboutTrainingProviders.First(x => x.ProviderCode == providerCode + 2).Description);
            Assert.AreEqual(providerName, organisationViewModel.AboutTrainingProviders.First(x => x.ProviderCode == providerCode + 1).ProviderName);
            Assert.AreEqual(trainWithDisability, organisationViewModel.TrainWithDisability);
            Assert.AreEqual(now, organisationViewModel.LastPublishedTimestampUtc);
            Assert.AreEqual(EnumStatus.Published, organisationViewModel.Status);
        }

        [Test]
        public async Task AboutPost_SetAccreditingProviderToEmpty()
        {
            var existingEnrichment = new UcasProviderEnrichmentGetModel()
            {
                EnrichmentModel = new ProviderEnrichmentModel()
                {
                    AccreditingProviderEnrichments = new List<AccreditingProviderEnrichment>
                    {
                        new AccreditingProviderEnrichment
                        {
                            UcasProviderCode = "ACC",
                            Description = "foo"
                        }
                    }
                }
            };

            var apiMock = new Mock<IManageApi>();
            apiMock.Setup(x => x.GetProviderEnrichment("ABC")).ReturnsAsync(existingEnrichment);

            UcasProviderEnrichmentPostModel result = null;

            apiMock.Setup(x => x.SaveProviderEnrichment("ABC", It.IsAny<UcasProviderEnrichmentPostModel>()))
                .Callback((string a, UcasProviderEnrichmentPostModel b) => result = b)
                .Returns(Task.CompletedTask);

            apiMock.Setup(x => x.GetCoursesOfProvider("ABC")).ReturnsAsync(new List<Course> { new Course { AccreditingProvider = new GovUk.Education.ManageCourses.Domain.Models.Provider { ProviderCode = "ACC" } }});

            var objectValidator = new Mock<IObjectModelValidator>();
            objectValidator.Setup(o => o.Validate(It.IsAny<ActionContext>(),
                It.IsAny<ValidationStateDictionary>(),
                It.IsAny<string>(),
                It.IsAny<Object>()));

            var frontendUrlMock = new Mock<IFrontendUrlService>();

            var controller = new OrganisationController(apiMock.Object, frontendUrlMock.Object);
            controller.ObjectValidator = objectValidator.Object;
            controller.TempData = new Mock<ITempDataDictionary>().Object;

            var res = await controller.AboutPost("ABC", new OrganisationViewModelForAbout {
                AboutTrainingProviders = new List<TrainingProviderViewModel>
                {
                    new TrainingProviderViewModel
                    {
                        ProviderCode = "ACC",
                        Description = null // not an empty string... this is how MVC model binding behaves
                    }
                }
            });


            result.Should().NotBeNull();
            result.EnrichmentModel.AccreditingProviderEnrichments[0].UcasProviderCode.Should().Be("ACC");
            result.EnrichmentModel.AccreditingProviderEnrichments[0].Description.Should().BeNullOrEmpty();
        }

        [Test]
        public void AboutPost_ModelState_WordCount()
        {
            var providerCode = "PROVIDERCODE";
            var exceed100Words = "";
            for (int i = 0; i < 101; i++)
            {
                exceed100Words += i + " ";
            }
            var providerName = "ProviderName";

            var viewModel = new OrganisationViewModelForAbout
            {
                AboutTrainingProviders = new List<TrainingProviderViewModel>() {
                    new TrainingProviderViewModel{ Description = exceed100Words,
                    ProviderName = providerName,
                    ProviderCode = providerCode + 1}
                }
            };

            var providerCourses = new List<Course>
                {
                    new Course { },
                    new Course { AccreditingProvider = new GovUk.Education.ManageCourses.Domain.Models.Provider { ProviderCode = providerCode.ToUpperInvariant() }},
                    new Course { AccreditingProvider = new GovUk.Education.ManageCourses.Domain.Models.Provider { ProviderCode = providerCode.ToLowerInvariant() }},
                    new Course { AccreditingProvider = new GovUk.Education.ManageCourses.Domain.Models.Provider { ProviderCode = providerCode }},
                    new Course { AccreditingProvider = new GovUk.Education.ManageCourses.Domain.Models.Provider { ProviderCode = providerCode + 1, ProviderName = providerName }},
                    new Course { AccreditingProvider = new GovUk.Education.ManageCourses.Domain.Models.Provider { ProviderCode = providerCode + 2 }},
                };

            var apiMock = new Mock<IManageApi>();
            apiMock.Setup(x => x.GetCoursesOfProvider(providerCode))
                .ReturnsAsync(providerCourses);


            var enrichmentModel = new ProviderEnrichmentModel { AccreditingProviderEnrichments = new List<AccreditingProviderEnrichment> { } };

            var ucasProviderEnrichmentGetModel = new UcasProviderEnrichmentGetModel()
            {
                EnrichmentModel = enrichmentModel
            };

            apiMock.Setup(x => x.GetProviderSummaries())
                .ReturnsAsync(new List<ProviderSummary> {new ProviderSummary()});

            apiMock.Setup(x => x.GetProviderEnrichment(providerCode))
                .ReturnsAsync(ucasProviderEnrichmentGetModel);

            var objectValidator = new Mock<IObjectModelValidator>();
            objectValidator.Setup(o => o.Validate(It.IsAny<ActionContext>(),
                It.IsAny<ValidationStateDictionary>(),
                It.IsAny<string>(),
                It.IsAny<Object>()));

            var frontendUrlMock = new Mock<IFrontendUrlService>();

            var controller = new OrganisationController(apiMock.Object, frontendUrlMock.Object);

            controller.ObjectValidator = objectValidator.Object;

            controller.TempData = new Mock<ITempDataDictionary>().Object;

            Assert.IsFalse(controller.ModelState.Any());
            Assert.IsTrue(controller.ModelState.IsValid);
            var result = controller.AboutPost(providerCode, viewModel).Result;


            Assert.IsTrue(controller.ModelState.Any());
            Assert.AreEqual($"Reduce word count for {providerName}", controller.ModelState["AboutTrainingProviders_0__Description"].Errors.First().ErrorMessage);
            Assert.IsFalse(controller.ModelState.IsValid);

            var viewResult = result as ViewResult;

            var organisationViewModel = viewResult.ViewData.Model as OrganisationViewModelForAbout;

            Assert.IsNotNull(viewResult);
            Assert.AreEqual(exceed100Words, organisationViewModel.AboutTrainingProviders.First(x => x.ProviderCode == providerCode + 1).Description);
            Assert.AreEqual(providerName, organisationViewModel.AboutTrainingProviders.First(x => x.ProviderCode == providerCode + 1).ProviderName);

        }
    }
}
