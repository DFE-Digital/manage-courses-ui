using GovUk.Education.ManageCourses.Ui.Services;
using NUnit.Framework;
using FluentAssertions;
using System;

namespace ManageCoursesUi.Tests.Services
{
    [TestFixture]
    public class SearchAndCompareUrlServiceTests
    {
        [Test]
        public void GeneratesUrls()
        {
            var searchAndCompareUrlService = new SearchAndCompareUrlService("http://www.example.com");
            searchAndCompareUrlService.GetCoursePageUri("aBc", "DeF").Should().Be(new Uri("http://www.example.com/course/aBc/DeF"));
        }

        [Test]
        public void EscapesUriStuff()
        {            
            var searchAndCompareUrlService = new SearchAndCompareUrlService("http://www.example.com");
            searchAndCompareUrlService.GetCoursePageUri(" / ", "%").Should().Be(new Uri("http://www.example.com/course/ %2F /%25"));
        }

    }
}