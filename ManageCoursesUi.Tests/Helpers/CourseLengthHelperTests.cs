using System;
using System.Collections.Generic;
using System.Text;
using FluentAssertions;
using GovUk.Education.ManageCourses.ApiClient;
using GovUk.Education.ManageCourses.Ui.Helpers;
using GovUk.Education.ManageCourses.Ui.ViewModels;
using GovUk.Education.ManageCourses.Ui.ViewModels.Enums;
using NUnit.Framework;

namespace ManageCoursesUi.Tests.Helpers
{
    [TestFixture]
    public class CourseLengthHelperTests
    {
        private const string OtherUserEnteredText = "Other test entered by the user";
        [Test]
        public void TestCourseLengthOtherHappyPath()
        {
            var enrichmentModel = new CourseEnrichmentModel {CourseLength = OtherUserEnteredText};

            var courseLengthResult = enrichmentModel.CourseLength.GetCourseLength();
            var courseLengthOtherResult = enrichmentModel.CourseLength.GetCourseLengthOther();

            courseLengthResult.Should().BeEquivalentTo(CourseLength.Other);
            courseLengthOtherResult.Should().BeEquivalentTo(OtherUserEnteredText);
        }
        [Test]
        public void TestCourseLengthOneYearHappyPath()
        {
            var enrichmentModel = new CourseEnrichmentModel { CourseLength = "OneYear" };

            var courseLengthResult = enrichmentModel.CourseLength.GetCourseLength();
            var courseLengthOtherResult = enrichmentModel.CourseLength.GetCourseLengthOther();

            courseLengthResult.Should().BeEquivalentTo(CourseLength.OneYear);
            courseLengthOtherResult.Should().BeNullOrEmpty();
        }
        [Test]
        public void TestCourseLengthTwoYearHappyPath()
        {
            var enrichmentModel = new CourseEnrichmentModel { CourseLength = "TwoYears" };

            var courseLengthResult = enrichmentModel.CourseLength.GetCourseLength();
            var courseLengthOtherResult = enrichmentModel.CourseLength.GetCourseLengthOther();

            courseLengthResult.Should().BeEquivalentTo(CourseLength.TwoYears);
            courseLengthOtherResult.Should().BeNullOrEmpty();
        }

        [Test]
        [TestCase("")]
        [TestCase("   ")]
        [TestCase(null)]
        public void TestCourseLength1EdgeCases(string courseLength)
        {
            var enrichmentModel = new CourseEnrichmentModel { CourseLength = courseLength };

            var courseLengthResult = enrichmentModel.CourseLength.GetCourseLength();
            var courseLengthOtherResult = enrichmentModel.CourseLength.GetCourseLengthOther();

            courseLengthResult.Should().BeNull();
            courseLengthOtherResult.Should().BeNullOrEmpty();
        }
    }
}
