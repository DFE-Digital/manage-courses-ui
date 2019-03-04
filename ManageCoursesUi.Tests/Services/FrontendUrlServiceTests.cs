using GovUk.Education.ManageCourses.Ui.Services;
using NUnit.Framework;
using FluentAssertions;
using System;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Microsoft.Extensions.Configuration;

namespace ManageCoursesUi.Tests.Services
{
    [TestFixture]
    public class FrontendUrlServiceTests
    {
        [Test]
        public void ReturnsRedirectResultToFrontend()
        {
            var configMock = new Mock<IConfiguration>();
            configMock.SetupGet(c => c["url:frontend"]).Returns("http://frontend:123");

            var searchAndCompareUrlService = new FrontendUrlService(configMock.Object);

            searchAndCompareUrlService.RedirectToFrontend("/organisation/47").Url.Should().Be("http://frontend:123/organisation/47");
        }

        [Test]
        public void ShouldRedirectOrganisationShow()
        {
            var configMock = new Mock<IConfiguration>();
            configMock.SetupGet(c => c["FEATURE_FRONTEND_ORGANISATION_SHOW"]).Returns("true");

            var searchAndCompareUrlService = new FrontendUrlService(configMock.Object);

            searchAndCompareUrlService.ShouldRedirectOrganisationShow().Should().Be(true);
        }
    }
}
