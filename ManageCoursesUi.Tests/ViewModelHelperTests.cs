using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using FluentAssertions;
using GovUk.Education.ManageCourses.Api.Model;
using GovUk.Education.ManageCourses.Domain.Models;
using GovUk.Education.ManageCourses.Ui.Helpers;
using GovUk.Education.ManageCourses.Ui.ViewModels;
using NUnit.Framework;

namespace ManageCoursesUi.Tests
{
    [TestFixture]
    class ViewModelHelperTests
    {
        [Test]
        [TestCase("N", "N", "N", "N", "New – not yet running")]
        [TestCase("N", "N", "S", "S", "New – not yet running")]
        [TestCase("N", "S", "S", "S", "New – not yet running")]
        [TestCase("S", "N", "S", "S", "New – not yet running")]
        [TestCase("S", "S", "N", "S", "New – not yet running")]
        [TestCase("S", "S", "S", "N", "New – not yet running")]
        [TestCase("n", "n", "n", "n", "New – not yet running")]
        [TestCase("n", "n", "n", "s", "New – not yet running")]
        [TestCase("n", "s", "s", "s", "New – not yet running")]
        [TestCase("s", "n", "s", "s", "New – not yet running")]
        [TestCase("s", "s", "n", "s", "New – not yet running")]
        [TestCase("s", "s", "s", "n", "New – not yet running")]
        [TestCase("N", "", "", "N", "New – not yet running")]
        [TestCase("R", "N", "N", "N", "Running")]
        [TestCase("N", "R", "N", "N", "Running")]
        [TestCase("N", "N", "R", "N", "Running")]
        [TestCase("N", "N", "N", "R", "Running")]
        [TestCase("r", "n", "n", "n", "Running")]
        [TestCase("n", "r", "n", "n", "Running")]
        [TestCase("n", "n", "r", "n", "Running")]
        [TestCase("n", "n", "n", "r", "Running")]
        [TestCase("R", "", "", "N", "Running")]
        [TestCase("R", "S", "D", "D", "Running")]
        [TestCase("R", "S", "D", "N", "Running")]
        [TestCase("r", "s", "d", "d", "Running")]
        [TestCase("r", "s", "d", "n", "Running")]
        [TestCase("S", "S", "S", "S", "Not running")]
        [TestCase("D", "S", "S", "S", "Not running")]
        [TestCase("S", "D", "S", "S", "Not running")]
        [TestCase("S", "S", "D", "S", "Not running")]
        [TestCase("S", "S", "S", "D", "Not running")]
        [TestCase("s", "s", "s", "s", "Not running")]
        [TestCase("d", "s", "s", "s", "Not running")]
        [TestCase("s", "d", "s", "s", "Not running")]
        [TestCase("s", "s", "d", "s", "Not running")]
        [TestCase("s", "s", "s", "d", "Not running")]
        [TestCase("S", "", "", "S", "Not running")]
        [TestCase("", "", "", "", "")]
        public void TestGetCourseStatus(string status1, string status2, string status3, string status4, string expectedResult)
        {
            var schools = new List<CourseSite>
            {
                new CourseSite {Status = status1},
                new CourseSite {Status = status2},
                new CourseSite {Status = status3},
                new CourseSite {Status = status4}
            };
            var course = new Course
            {
                CourseSites = new List<CourseSite>(schools)
            };
            var result = course.GetCourseStatus();
            result.Should().Be(expectedResult);
        }
        [Test]
        [TestCase("D", "Discontinued")]
        [TestCase("d", "Discontinued")]
        [TestCase("R", "Running")]
        [TestCase("r", "Running")]
        [TestCase("N", "New")]
        [TestCase("n", "New")]
        [TestCase("S", "Suspended")]
        [TestCase("s", "Suspended")]
        [TestCase("", "")]
        public void TestGetSiteStatus(string status, string expectedResult)
        {
            var schoolViewModel = new SiteViewModel { Status = status };
            var result = schoolViewModel.GetSiteStatus();
            result.Should().Be(expectedResult);
        }

        [Test]
        [TestCase("d", "B", "")]
        [TestCase("d", "F", "")]
        [TestCase("d", "P", "")]
        [TestCase("s", "B", "")]
        [TestCase("s", "F", "")]
        [TestCase("s", "P", "")]

        [TestCase("", "", "No")]
        [TestCase("b", "B", "Yes")]
        [TestCase("b", "F", "Yes")]
        [TestCase("b", "P", "Yes")]
        [TestCase("b", "", "No")]

        // These are probably wrong
        [TestCase("", "B", "Yes")]
        [TestCase("", "F", "Yes")]
        [TestCase("", "P", "Yes")]
        public void TestGetHasVacancies(string status, string vacStatus, string expectedResult)
        {
            var schools = new List<CourseSite>
            {
                new CourseSite {Status = status, VacStatus = vacStatus},
            };
            var course = new Course
            {
                CourseSites = new List<CourseSite>(schools),

            };
            var result = course.GetHasVacancies();
            result.Should().Be(expectedResult);
        }

        [Test]
        [TestCase("HE", "Higher education programme")]
        [TestCase("he", "Higher education programme")]
        [TestCase("SD", "School Direct training programme")]
        [TestCase("sd", "School Direct training programme")]
        [TestCase("SS", "School Direct (salaried) training programme")]
        [TestCase("ss", "School Direct (salaried) training programme")]
        [TestCase("TA", "PG Teaching Apprenticeship")]
        [TestCase("ta", "PG Teaching Apprenticeship")]
        [TestCase("", "")]
        public void TestGetRoute(string route, string expectedResult)
        {
            var courseVariantViewModel = new CourseDetailsViewModel { Route = route };
            var result = courseVariantViewModel.GetRoute();
            result.Should().Be(expectedResult);
        }
        [Test]
        [TestCase("F", "Full time")]
        [TestCase("f", "Full time")]
        [TestCase("P", "Part time")]
        [TestCase("p", "Part time")]
        [TestCase("B", "Full time or part time")]
        [TestCase("b", "Full time or part time")]
        public void TestGetStudyMode(string studyMode, string expectedResult)
        {
            var courseVariantViewModel = new CourseDetailsViewModel { StudyMode = studyMode };
            var result = courseVariantViewModel.GetStudyMode();
            result.Should().Be(expectedResult);
        }
        [Test]
        [TestCase("S", "Secondary (11+ years)")]
        [TestCase("s", "Secondary (11+ years)")]
        [TestCase("M", "Middle years (7 - 14 years)")]
        [TestCase("m", "Middle years (7 - 14 years)")]
        [TestCase("P", "Primary (3 - 11/12 years)")]
        [TestCase("p", "Primary (3 - 11/12 years)")]
        [TestCase("", "")]
        [TestCase(null, "")]
        public void TestGetAgeRange(string ageRange, string expectedResult)
        {
            var courseVariantViewModel = new CourseDetailsViewModel { AgeRange = ageRange };
            var result = courseVariantViewModel.GetAgeRange();
            result.Should().Be(expectedResult);
        }
        [Test]
        [TestCase("PF", "Professional")]
        [TestCase("pf", "Professional")]
        [TestCase("PG", "Postgraduate")]
        [TestCase("pg", "Postgraduate")]
        [TestCase("BO", "Professional/Postgraduate")]
        [TestCase("bo", "Professional/Postgraduate")]
        [TestCase("", "Recommendation for QTS")]
        [TestCase(null, "")]
        public void TestGetQualification(string qualification, string expectedResult)
        {
            var courseVariantViewModel = new CourseDetailsViewModel { Qualifications = qualification };
            var result = courseVariantViewModel.GetQualification();
            result.Should().Be(expectedResult);
        }
    }
}
