using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using GovUk.Education.ManageCourses.Api.Model;
using GovUk.Education.ManageCourses.Domain.Models;
using GovUk.Education.ManageCourses.Ui;
using GovUk.Education.ManageCourses.Ui.Controllers;
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
            var instCode = "INSTCODE";
            var organisationName = "organisationName";
            // Todo: fix this ObservableCollection.
            var instCourses = new List<Course>
                {
                    new Course
                    {
                        Institution = new Institution { InstCode = instCode, InstName = organisationName }
                    }
                };

            var orgs = new List<InstitutionSummary>
            {
                new InstitutionSummary(),
                new InstitutionSummary
                {
                    InstCode = instCode,
                    InstName = organisationName
                }
            };

            var apiMock = new Mock<IManageApi>();
            apiMock.Setup(x => x.GetCoursesOfInstitution(instCode)).ReturnsAsync(instCourses);

            apiMock.Setup(x => x.GetInstitutionSummaries()).ReturnsAsync(orgs);

            apiMock.Setup(x => x.GetInstitutionSummary(It.IsAny<string>())).ReturnsAsync(orgs[1]);

            var controller = new OrganisationController(apiMock.Object);

            var result = await controller.Show(instCode);

            var viewResult = result as ViewResult;

            Assert.IsNotNull(viewResult);
            var model = viewResult.ViewData.Model as CourseListViewModel;

            Assert.NotNull(model);
            Assert.AreEqual(1, model.Providers.Count);

            Assert.IsTrue(model.MultipleOrganisations);
        }

        [Test]
        public void RequestAccess()
        {
            var instCode = "INSTCODE";
            var organisationName = "organisationName";

            var orgs = new List<InstitutionSummary>
            {
                new InstitutionSummary
                {
                InstCode = instCode,
                InstName = organisationName
                }
            };

            var apiMock = new Mock<IManageApi>();

            apiMock.Setup(x => x.GetInstitutionSummaries())
                .ReturnsAsync(orgs);

            var controller = new OrganisationController(apiMock.Object);

            var result = controller.RequestAccess(instCode);

            var viewResult = result as ViewResult;

            Assert.IsNotNull(viewResult);
            var model = viewResult.ViewData.Model as RequestAccessViewModel;

            Assert.AreEqual(instCode, controller.ViewBag.InstCode);
        }

        [Test]
        public async Task RequestAccessPost_invalid()
        {
            var instCode = "INSTCODE";
            var organisationName = "organisationName";

            var orgs = new List<InstitutionSummary>
            {
                new InstitutionSummary
                {
                InstCode = instCode,
                InstName = organisationName
                }
            };

            var apiMock = new Mock<IManageApi>();

            apiMock.Setup(x => x.GetInstitutionSummaries())
                .ReturnsAsync(orgs);

            var controller = new OrganisationController(apiMock.Object);
            controller.ModelState.AddModelError("you", "failed");
            var result = await controller.RequestAccessPost(instCode, new RequestAccessViewModel());

            var viewResult = result as ViewResult;

            Assert.IsNotNull(viewResult);
            var model = viewResult.ViewData.Model as RequestAccessViewModel;

            Assert.AreEqual(instCode, controller.ViewBag.InstCode);
        }

        [Test]
        public async Task RequestAccessPost()
        {
            var instCode = "INSTCODE";
            var viewModel = new RequestAccessViewModel { FirstName = "FirstName", LastName = "LastName" };
            var tempKey = "RequestAccess_To_Name";

            var apiMock = new Mock<IManageApi>();

            var tempDataMock = new Mock<ITempDataDictionary>();

            var controller = new OrganisationController(apiMock.Object);
            controller.TempData = tempDataMock.Object;

            var result = await controller.RequestAccessPost(instCode, viewModel);

            apiMock.Verify(x => x.GetInstitutionSummaries(), Times.Never);
            apiMock.Verify(x => x.CreateAccessRequest(It.IsAny<GovUk.Education.ManageCourses.Api.Model.AccessRequest>()), Times.Once);

            tempDataMock.Verify(x => x.Add(tempKey, $"{viewModel.FirstName} {viewModel.LastName}"));

            var actionResult = result as RedirectToActionResult;

            Assert.IsNotNull(actionResult);
            Assert.AreEqual("Show", actionResult.ActionName);
            Assert.AreEqual("Organisation", actionResult.ControllerName);
            Assert.AreEqual(instCode, actionResult.RouteValues[instCode]);
        }

        [Test]
        public async Task About()
        {
            var instCode = "INSTCODE";
            var organisationName = "OrganisationName";

            var InstitutionSummaries = new List<InstitutionSummary>
            {
                new InstitutionSummary
                {
                InstCode = instCode,
                InstName = organisationName
                }
            };

            var trainWithUs = "TrainWithUs";
            var trainWithDisability = "TrainWithDisability";

            var description = "Description";
            var institutionName = "InstitutionName";
            var institutionCourses =  new List<Course>
                {
                    new Course { },
                    new Course { AccreditingInstitution = new Institution { InstCode = instCode.ToUpperInvariant() }},
                    new Course { AccreditingInstitution = new Institution { InstCode = instCode.ToLowerInvariant() }},
                    new Course { AccreditingInstitution = new Institution { InstCode = instCode }},
                    new Course { AccreditingInstitution = new Institution { InstCode = instCode + 1, InstName = institutionName }},
                    new Course { AccreditingInstitution = new Institution { InstCode = instCode + 2 }},
                };

            var now = DateTime.Now;
            var ucasInstitutionEnrichmentGetModel = new UcasInstitutionEnrichmentGetModel()
            {
                LastPublishedTimestampUtc = now,
                Status = EnumStatus.Published,
                EnrichmentModel = new InstitutionEnrichmentModel
                {
                AccreditingProviderEnrichments = new List<AccreditingProviderEnrichment>
                {
                new AccreditingProviderEnrichment { UcasInstitutionCode = instCode + 2, Description = description }
                },
                TrainWithUs = trainWithUs,
                TrainWithDisability = trainWithDisability
                }
            };

            var apiMock = new Mock<IManageApi>();

            apiMock.Setup(x => x.GetInstitutionSummary(instCode)).ReturnsAsync(
                new InstitutionSummary { InstCode = instCode, InstName = organisationName }
            );

            apiMock.Setup(x => x.GetCoursesOfInstitution(instCode))
                .ReturnsAsync(institutionCourses);

            apiMock.Setup(x => x.GetInstitutitionEnrichment(instCode))
                .ReturnsAsync(ucasInstitutionEnrichmentGetModel);

            var controller = new OrganisationController(apiMock.Object);

            var result = await controller.About(instCode);

            var viewResult = result as ViewResult;

            Assert.IsNotNull(viewResult);
            var organisationViewModel = viewResult.ViewData.Model as OrganisationViewModelForAbout;

            Assert.AreEqual(organisationName, organisationViewModel.InstName);
            Assert.AreEqual(trainWithUs, organisationViewModel.TrainWithUs);
            Assert.AreEqual(2, organisationViewModel.AboutTrainingProviders.Count);
            Assert.AreEqual(description, organisationViewModel.AboutTrainingProviders.First(x => x.InstCode == instCode + 2).Description);
            Assert.AreEqual(institutionName, organisationViewModel.AboutTrainingProviders.First(x => x.InstCode == instCode + 1).InstName);
            Assert.AreEqual(trainWithDisability, organisationViewModel.TrainWithDisability);
            Assert.AreEqual(now, organisationViewModel.LastPublishedTimestampUtc);
            Assert.AreEqual(EnumStatus.Published, organisationViewModel.Status);
        }

        [Test]
        public async Task AboutPost_SaveOrganisation()
        {
            var instCode = "INSTCODE";
            var viewModel = new OrganisationViewModelForAbout
            {
                AboutTrainingProviders = new List<TrainingProviderViewModel>()
            };

            var institutionName = "InstitutionName";

            var institutionCourses = new List<Course>
                {
                    new Course { },
                    new Course { AccreditingInstitution = new Institution { InstCode = instCode.ToUpperInvariant() }},
                    new Course { AccreditingInstitution = new Institution { InstCode = instCode.ToLowerInvariant() }},
                    new Course { AccreditingInstitution = new Institution { InstCode = instCode }},
                    new Course { AccreditingInstitution = new Institution { InstCode = instCode + 1, InstName = institutionName }},
                    new Course { AccreditingInstitution = new Institution { InstCode = instCode + 2 }},
                };

            var apiMock = new Mock<IManageApi>();
            apiMock.Setup(x => x.GetCoursesOfInstitution(instCode))
                .ReturnsAsync(institutionCourses);

            var enrichmentModel = new InstitutionEnrichmentModel { AccreditingProviderEnrichments = new List<AccreditingProviderEnrichment> { } };

            var ucasInstitutionEnrichmentGetModel = new UcasInstitutionEnrichmentGetModel()
            {
                EnrichmentModel = enrichmentModel
            };

            apiMock.Setup(x => x.GetInstitutitionEnrichment(instCode))
                .ReturnsAsync(ucasInstitutionEnrichmentGetModel);

            var objectValidator = new Mock<IObjectModelValidator>();
            objectValidator.Setup(o => o.Validate(It.IsAny<ActionContext>(),
                It.IsAny<ValidationStateDictionary>(),
                It.IsAny<string>(),
                It.IsAny<Object>()));

            var controller = new OrganisationController(apiMock.Object);

            controller.ObjectValidator = objectValidator.Object;

            controller.TempData = new Mock<ITempDataDictionary>().Object;

            var result = await controller.AboutPost(instCode, viewModel);

            apiMock.Verify(x => x.SaveInstitutionEnrichment(instCode, It.IsAny<UcasInstitutionEnrichmentPostModel>()), Times.Once);

            var actionResult = result as RedirectToActionResult;

            Assert.IsNotNull(actionResult);
            Assert.AreEqual("Details", actionResult.ActionName);
            Assert.AreEqual("Organisation", actionResult.ControllerName);
            Assert.AreEqual(instCode, actionResult.RouteValues[instCode]);
        }

        [Test]
        public void DetailsPost_PublishOrganisation_WhenApiReturnsFalse()
        {
            var instCode = "INSTCODE";
            var institutionName = "InstitutionName";

            var viewModel = new OrganisationViewModel
            {
                AboutTrainingProviders = new List<TrainingProviderViewModel>()
            };

            var institutionCourses = new List<Course>
                {
                    new Course { },
                    new Course { AccreditingInstitution = new Institution { InstCode = instCode.ToUpperInvariant() }},
                    new Course { AccreditingInstitution = new Institution { InstCode = instCode.ToLowerInvariant() }},
                    new Course { AccreditingInstitution = new Institution { InstCode = instCode }},
                    new Course { AccreditingInstitution = new Institution { InstCode = instCode + 1, InstName = institutionName }},
                    new Course { AccreditingInstitution = new Institution { InstCode = instCode + 2 }},
                };

            var apiMock = new Mock<IManageApi>();

            apiMock.Setup(x => x.GetInstitutionSummary(instCode))
                .ReturnsAsync(new InstitutionSummary { InstCode = instCode, InstName = institutionName });

            apiMock.Setup(x => x.GetCoursesOfInstitution(instCode))
                .ReturnsAsync(institutionCourses);

            var enrichmentModel = new InstitutionEnrichmentModel { AccreditingProviderEnrichments = new List<AccreditingProviderEnrichment> { } };

            var ucasInstitutionEnrichmentGetModel = new UcasInstitutionEnrichmentGetModel()
            {
                EnrichmentModel = enrichmentModel
            };

            apiMock.Setup(x => x.GetInstitutitionEnrichment(instCode))
                .ReturnsAsync(ucasInstitutionEnrichmentGetModel);

            var objectValidator = new Mock<IObjectModelValidator>();
            objectValidator.Setup(o => o.Validate(It.IsAny<ActionContext>(),
                It.IsAny<ValidationStateDictionary>(),
                It.IsAny<string>(),
                It.IsAny<Object>()));

            var controller = new OrganisationController(apiMock.Object);
            controller.ObjectValidator = objectValidator.Object;

            controller.TempData = new Mock<ITempDataDictionary>().Object;

            Assert.ThrowsAsync<InvalidOperationException>( async () => await controller.DetailsPost(instCode, viewModel));
        }

        [Test]
        public async Task DetailsPost_PublishOrganisation_WhenApiReturnsTrue()
        {
            var instCode = "INSTCODE";
            var viewModel = new OrganisationViewModel
            {
                AboutTrainingProviders = new List<TrainingProviderViewModel>()
            };

            var institutionName = "InstitutionName";

            var institutionCourses = new List<Course>
                {
                    new Course { },
                    new Course { AccreditingInstitution = new Institution { InstCode = instCode.ToUpperInvariant() }},
                    new Course { AccreditingInstitution = new Institution { InstCode = instCode.ToLowerInvariant() }},
                    new Course { AccreditingInstitution = new Institution { InstCode = instCode }},
                    new Course { AccreditingInstitution = new Institution { InstCode = instCode + 1, InstName = institutionName }},
                    new Course { AccreditingInstitution = new Institution { InstCode = instCode + 2 }},
                };
            var apiMock = new Mock<IManageApi>();

            apiMock.Setup(x => x.GetInstitutionSummary(instCode))
                .ReturnsAsync(new InstitutionSummary { InstCode = instCode, InstName = institutionName });

            apiMock.Setup(x => x.GetCoursesOfInstitution(instCode))
                .ReturnsAsync(institutionCourses);

            var enrichmentModel = new InstitutionEnrichmentModel { AccreditingProviderEnrichments = new List<AccreditingProviderEnrichment>() };

            var ucasInstitutionEnrichmentGetModel = new UcasInstitutionEnrichmentGetModel()
            {
                EnrichmentModel = enrichmentModel
            };

            apiMock.Setup(x => x.GetInstitutitionEnrichment(instCode))
                .ReturnsAsync(ucasInstitutionEnrichmentGetModel);

            var objectValidator = new Mock<IObjectModelValidator>();
            objectValidator.Setup(o => o.Validate(It.IsAny<ActionContext>(),
                It.IsAny<ValidationStateDictionary>(),
                It.IsAny<string>(),
                It.IsAny<Object>()));

            apiMock.Setup(x => x.PublishAllCoursesOfInstitutionToSearchAndCompare(instCode))
                .ReturnsAsync(true);
            var controller = new OrganisationController(apiMock.Object);
            controller.ObjectValidator = objectValidator.Object;

            controller.TempData = new Mock<ITempDataDictionary>().Object;

            var result = await controller.DetailsPost(instCode, viewModel);

            var actionResult = result as RedirectToActionResult;

            Assert.IsNotNull(actionResult);
            Assert.AreEqual("Details", actionResult.ActionName);
            Assert.AreEqual(instCode, actionResult.RouteValues[instCode]);
        }

        [Test]
        public async Task DetailsPost_PublishOrganisation_invalid()
        {
            var instCode = "INSTCODE";

            var organisationName = "OrganisationName";

            var InstitutionSummaries = new List<InstitutionSummary>
            {
                new InstitutionSummary
                {
                InstCode = instCode,
                InstName = organisationName
                }
            };

            var trainWithUs = "TrainWithUs";
            var trainWithDisability = "TrainWithDisability";

            var description = "Description";
            var institutionName = "InstitutionName";
            var institutionCourses = new List<Course>
                {
                    new Course { },
                    new Course { AccreditingInstitution = new Institution { InstCode = instCode.ToUpperInvariant() }},
                    new Course { AccreditingInstitution = new Institution { InstCode = instCode.ToLowerInvariant() }},
                    new Course { AccreditingInstitution = new Institution { InstCode = instCode }},
                    new Course { AccreditingInstitution = new Institution { InstCode = instCode + 1, InstName = institutionName }},
                    new Course { AccreditingInstitution = new Institution { InstCode = instCode + 2 }},
                };

            var now = DateTime.Now;
            var ucasInstitutionEnrichmentGetModel = new UcasInstitutionEnrichmentGetModel()
            {
                LastPublishedTimestampUtc = now,
                Status = EnumStatus.Published,
                EnrichmentModel = new InstitutionEnrichmentModel
                {
                AccreditingProviderEnrichments = new List<AccreditingProviderEnrichment>
                {
                new AccreditingProviderEnrichment { UcasInstitutionCode = instCode + 2, Description = description }
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

            apiMock.Setup(x => x.GetInstitutionSummary(instCode))
                .ReturnsAsync(new InstitutionSummary { InstCode = instCode, InstName = organisationName });

            apiMock.Setup(x => x.GetInstitutionSummaries())
                .ReturnsAsync(InstitutionSummaries);

            apiMock.Setup(x => x.GetCoursesOfInstitution(instCode))
                .ReturnsAsync(institutionCourses);

            apiMock.Setup(x => x.GetInstitutitionEnrichment(instCode))
                .ReturnsAsync(ucasInstitutionEnrichmentGetModel);

            apiMock.Setup(x => x.PublishAllCoursesOfInstitutionToSearchAndCompare(instCode))
                .ReturnsAsync(true);
            var controller = new OrganisationControllerMockedValidation(apiMock.Object);

            var result = await controller.DetailsPost(instCode, viewModel);

            var viewResult = result as ViewResult;

            Assert.IsNotNull(viewResult);
            var organisationViewModel = viewResult.ViewData.Model as OrganisationViewModel;

            Assert.AreEqual(organisationName, organisationViewModel.InstName);
            Assert.AreEqual(trainWithUs, organisationViewModel.TrainWithUs);
            Assert.AreEqual(2, organisationViewModel.AboutTrainingProviders.Count);
            Assert.AreEqual(description, organisationViewModel.AboutTrainingProviders.First(x => x.InstCode == instCode + 2).Description);
            Assert.AreEqual(institutionName, organisationViewModel.AboutTrainingProviders.First(x => x.InstCode == instCode + 1).InstName);
            Assert.AreEqual(trainWithDisability, organisationViewModel.TrainWithDisability);
            Assert.AreEqual(now, organisationViewModel.LastPublishedTimestampUtc);
            Assert.AreEqual(EnumStatus.Published, organisationViewModel.Status);
        }

        [Test]
        public async Task AboutPost_SetAccreditingProviderToEmpty()
        {
            var existingEnrichment = new UcasInstitutionEnrichmentGetModel()
            {
                EnrichmentModel = new InstitutionEnrichmentModel()
                {
                    AccreditingProviderEnrichments = new List<AccreditingProviderEnrichment>
                    {
                        new AccreditingProviderEnrichment
                        {
                            UcasInstitutionCode = "ACC",
                            Description = "foo"
                        }
                    }
                }
            };

            var apiMock = new Mock<IManageApi>();
            apiMock.Setup(x => x.GetInstitutitionEnrichment("ABC")).ReturnsAsync(existingEnrichment);

            UcasInstitutionEnrichmentPostModel result = null;

            apiMock.Setup(x => x.SaveInstitutionEnrichment("ABC", It.IsAny<UcasInstitutionEnrichmentPostModel>()))
                .Callback((string a, UcasInstitutionEnrichmentPostModel b) => result = b)
                .Returns(Task.CompletedTask);

            apiMock.Setup(x => x.GetCoursesOfInstitution("ABC")).ReturnsAsync(new List<Course> { new Course { AccreditingInstitution = new Institution { InstCode = "ACC" } }});

            var objectValidator = new Mock<IObjectModelValidator>();
            objectValidator.Setup(o => o.Validate(It.IsAny<ActionContext>(),
                It.IsAny<ValidationStateDictionary>(),
                It.IsAny<string>(),
                It.IsAny<Object>()));

            var controller = new OrganisationController(apiMock.Object);
            controller.ObjectValidator = objectValidator.Object;
            controller.TempData = new Mock<ITempDataDictionary>().Object;

            var res = await controller.AboutPost("ABC", new OrganisationViewModelForAbout {
                AboutTrainingProviders = new List<TrainingProviderViewModel>
                {
                    new TrainingProviderViewModel
                    {
                        InstCode = "ACC",
                        Description = null // not an empty string... this is how MVC model binding behaves
                    }
                }
            });


            result.Should().NotBeNull();
            result.EnrichmentModel.AccreditingProviderEnrichments[0].UcasInstitutionCode.Should().Be("ACC");
            result.EnrichmentModel.AccreditingProviderEnrichments[0].Description.Should().BeNullOrEmpty();
        }

        [Test]
        public void AboutPost_ModelState_WordCount()
        {
            var instCode = "INSTCODE";
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
                    InstName = institutionName,
                    InstCode = instCode + 1}
                }
            };

            var institutionCourses = new List<Course>
                {
                    new Course { },
                    new Course { AccreditingInstitution = new Institution { InstCode = instCode.ToUpperInvariant() }},
                    new Course { AccreditingInstitution = new Institution { InstCode = instCode.ToLowerInvariant() }},
                    new Course { AccreditingInstitution = new Institution { InstCode = instCode }},
                    new Course { AccreditingInstitution = new Institution { InstCode = instCode + 1, InstName = institutionName }},
                    new Course { AccreditingInstitution = new Institution { InstCode = instCode + 2 }},
                };

            var apiMock = new Mock<IManageApi>();
            apiMock.Setup(x => x.GetCoursesOfInstitution(instCode))
                .ReturnsAsync(institutionCourses);


            var enrichmentModel = new InstitutionEnrichmentModel { AccreditingProviderEnrichments = new List<AccreditingProviderEnrichment> { } };

            var ucasInstitutionEnrichmentGetModel = new UcasInstitutionEnrichmentGetModel()
            {
                EnrichmentModel = enrichmentModel
            };

            apiMock.Setup(x => x.GetInstitutionSummaries())
                .ReturnsAsync(new List<InstitutionSummary> {new InstitutionSummary()});

            apiMock.Setup(x => x.GetInstitutitionEnrichment(instCode))
                .ReturnsAsync(ucasInstitutionEnrichmentGetModel);

            var objectValidator = new Mock<IObjectModelValidator>();
            objectValidator.Setup(o => o.Validate(It.IsAny<ActionContext>(),
                It.IsAny<ValidationStateDictionary>(),
                It.IsAny<string>(),
                It.IsAny<Object>()));

            var controller = new OrganisationController(apiMock.Object);

            controller.ObjectValidator = objectValidator.Object;

            controller.TempData = new Mock<ITempDataDictionary>().Object;

            Assert.IsFalse(controller.ModelState.Any());
            Assert.IsTrue(controller.ModelState.IsValid);
            var result = controller.AboutPost(instCode, viewModel).Result;


            Assert.IsTrue(controller.ModelState.Any());
            Assert.AreEqual($"Reduce word count for {institutionName}", controller.ModelState["AboutTrainingProviders_0__Description"].Errors.First().ErrorMessage);
            Assert.IsFalse(controller.ModelState.IsValid);

            var viewResult = result as ViewResult;

            var organisationViewModel = viewResult.ViewData.Model as OrganisationViewModelForAbout;

            Assert.IsNotNull(viewResult);
            Assert.AreEqual(exceed100Words, organisationViewModel.AboutTrainingProviders.First(x => x.InstCode == instCode + 1).Description);
            Assert.AreEqual(institutionName, organisationViewModel.AboutTrainingProviders.First(x => x.InstCode == instCode + 1).InstName);

        }
    }
}
