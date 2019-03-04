using GovUk.Education.ManageCourses.Ui.Services;
using NUnit.Framework;
using FluentAssertions;
using System;
using Microsoft.AspNetCore.Mvc;

namespace ManageCoursesUi.Tests.Services
{
    [TestFixture]
    public class FrontendUrlServiceTests
    {
        [Test]
        public void ReturnsRedirectResultToFrontend()
        {
            var searchAndCompareUrlService = new FrontendUrlService("http://frontend:123");
            searchAndCompareUrlService.RedirectToFrontend("/organisation/47").Url.Should().Be("http://frontend:123/organisation/47");
        }
    }
}
