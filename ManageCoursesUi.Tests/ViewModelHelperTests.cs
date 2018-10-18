using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using FluentAssertions;
using GovUk.Education.ManageCourses.ApiClient;
using GovUk.Education.ManageCourses.Ui.Helpers;
using GovUk.Education.ManageCourses.Ui.ViewModels;
using NUnit.Framework;

namespace ManageCoursesUi.Tests
{
    [TestFixture]
    class ViewModelHelperTests
    {
        [Test]
        [TestCase("Y", "N", "N", "N", "N", "New – not yet running")]
        [TestCase("Y", "N", "N", "N", "N", "New – not yet running")]
        [TestCase("Y", "N", "N", "N", "N", "New – not yet running")]
        [TestCase("Y", "N", "N", "S", "S", "New – not yet running")]
        [TestCase("Y", "N", "S", "S", "S", "New – not yet running")]
        [TestCase("y", "S", "N", "S", "S", "New – not yet running")]
        [TestCase("y", "S", "S", "N", "S", "New – not yet running")]
        [TestCase("y", "S", "S", "S", "N", "New – not yet running")]
        [TestCase("y", "n", "n", "n", "n", "New – not yet running")]
        [TestCase("y", "n", "n", "n", "s", "New – not yet running")]
        [TestCase("y", "n", "s", "s", "s", "New – not yet running")]
        [TestCase("y", "s", "n", "s", "s", "New – not yet running")]
        [TestCase("y", "s", "s", "n", "s", "New – not yet running")]
        [TestCase("y", "s", "s", "s", "n", "New – not yet running")]
        [TestCase("N", "N", "N", "N", "N", "New – not yet running")]
        [TestCase("N", "N", "N", "N", "N", "New – not yet running")]
        [TestCase("N", "N", "N", "N", "N", "New – not yet running")]
        [TestCase("N", "N", "N", "S", "S", "New – not yet running")]
        [TestCase("N", "N", "S", "S", "S", "New – not yet running")]
        [TestCase("n", "S", "N", "S", "S", "New – not yet running")]
        [TestCase("n", "S", "S", "N", "S", "New – not yet running")]
        [TestCase("n", "S", "S", "S", "N", "New – not yet running")]
        [TestCase("n", "n", "n", "n", "n", "New – not yet running")]
        [TestCase("n", "n", "n", "n", "s", "New – not yet running")]
        [TestCase("n", "n", "s", "s", "s", "New – not yet running")]
        [TestCase("n", "s", "n", "s", "s", "New – not yet running")]
        [TestCase("n", "s", "s", "n", "s", "New – not yet running")]
        [TestCase("n", "s", "s", "s", "n", "New – not yet running")]
        [TestCase("N", "N", "", "", "N", "New – not yet running")]

        [TestCase("Y", "R", "N", "N", "N", "Running")]
        [TestCase("Y", "N", "R", "N", "N", "Running")]
        [TestCase("Y", "N", "N", "R", "N", "Running")]
        [TestCase("Y", "N", "N", "N", "R", "Running")]
        [TestCase("Y", "r", "n", "n", "n", "Running")]
        [TestCase("Y", "n", "r", "n", "n", "Running")]
        [TestCase("Y", "n", "n", "r", "n", "Running")]
        [TestCase("Y", "n", "n", "n", "r", "Running")]
        [TestCase("Y", "R", "", "", "N", "Running")]
        [TestCase("Y", "R", "S", "D", "D", "Running")]
        [TestCase("Y", "R", "S", "D", "N", "Running")]
        [TestCase("Y", "r", "s", "d", "d", "Running")]
        [TestCase("Y", "r", "s", "d", "n", "Running")]

        [TestCase("y", "R", "N", "N", "N", "Running")]
        [TestCase("y", "N", "R", "N", "N", "Running")]
        [TestCase("y", "N", "N", "R", "N", "Running")]
        [TestCase("y", "N", "N", "N", "R", "Running")]
        [TestCase("y", "r", "n", "n", "n", "Running")]
        [TestCase("y", "n", "r", "n", "n", "Running")]
        [TestCase("y", "n", "n", "r", "n", "Running")]
        [TestCase("y", "n", "n", "n", "r", "Running")]
        [TestCase("y", "R", "", "", "N", "Running")]
        [TestCase("y", "R", "S", "D", "D", "Running")]
        [TestCase("y", "R", "S", "D", "N", "Running")]
        [TestCase("y", "r", "s", "d", "d", "Running")]
        [TestCase("y", "r", "s", "d", "n", "Running")]

        [TestCase("N", "R", "N", "N", "N", "Running but incomplete")]
        [TestCase("N", "N", "R", "N", "N", "Running but incomplete")]
        [TestCase("N", "N", "N", "R", "N", "Running but incomplete")]
        [TestCase("N", "N", "N", "N", "R", "Running but incomplete")]
        [TestCase("N", "r", "n", "n", "n", "Running but incomplete")]
        [TestCase("N", "n", "r", "n", "n", "Running but incomplete")]
        [TestCase("N", "n", "n", "r", "n", "Running but incomplete")]
        [TestCase("N", "n", "n", "n", "r", "Running but incomplete")]
        [TestCase("N", "R", "", "", "N", "Running but incomplete")]
        [TestCase("N", "R", "S", "D", "D", "Running but incomplete")]
        [TestCase("N", "R", "S", "D", "N", "Running but incomplete")]
        [TestCase("N", "r", "s", "d", "d", "Running but incomplete")]
        [TestCase("N", "r", "s", "d", "n", "Running but incomplete")]
        [TestCase("n", "R", "N", "N", "N", "Running but incomplete")]
        [TestCase("n", "N", "R", "N", "N", "Running but incomplete")]
        [TestCase("n", "N", "N", "R", "N", "Running but incomplete")]
        [TestCase("n", "N", "N", "N", "R", "Running but incomplete")]
        [TestCase("n", "r", "n", "n", "n", "Running but incomplete")]
        [TestCase("n", "n", "r", "n", "n", "Running but incomplete")]
        [TestCase("n", "n", "n", "r", "n", "Running but incomplete")]
        [TestCase("n", "n", "n", "n", "r", "Running but incomplete")]
        [TestCase("n", "R", "", "", "N", "Running but incomplete")]
        [TestCase("n", "R", "S", "D", "D", "Running but incomplete")]
        [TestCase("n", "R", "S", "D", "N", "Running but incomplete")]
        [TestCase("n", "r", "s", "d", "d", "Running but incomplete")]
        [TestCase("n", "r", "s", "d", "n", "Running but incomplete")]

        [TestCase("Y", "S", "S", "S", "S", "Not running")]
        [TestCase("Y", "D", "S", "S", "S", "Not running")]
        [TestCase("Y", "S", "D", "S", "S", "Not running")]
        [TestCase("Y", "S", "S", "D", "S", "Not running")]
        [TestCase("Y", "S", "S", "S", "D", "Not running")]
        [TestCase("Y", "S", "", "", "S", "Not running")]
        [TestCase("y", "s", "s", "s", "s", "Not running")]
        [TestCase("y", "d", "s", "s", "s", "Not running")]
        [TestCase("y", "s", "d", "s", "s", "Not running")]
        [TestCase("y", "s", "s", "d", "s", "Not running")]
        [TestCase("y", "s", "s", "s", "d", "Not running")]
        [TestCase("y", "s", "", "", "s", "Not running")]

        [TestCase("N", "S", "S", "S", "S", "Not running")]
        [TestCase("N", "D", "S", "S", "S", "Not running")]
        [TestCase("N", "S", "D", "S", "S", "Not running")]
        [TestCase("N", "S", "S", "D", "S", "Not running")]
        [TestCase("N", "S", "S", "S", "D", "Not running")]
        [TestCase("N", "S", "", "", "S", "Not running")]
        [TestCase("n", "s", "s", "s", "s", "Not running")]
        [TestCase("n", "d", "s", "s", "s", "Not running")]
        [TestCase("n", "s", "d", "s", "s", "Not running")]
        [TestCase("n", "s", "s", "d", "s", "Not running")]
        [TestCase("n", "s", "s", "s", "d", "Not running")]
        [TestCase("n", "s", "", "", "s", "Not running")]

        [TestCase("", "", "", "", "", "")]
        public void TestGetCourseStatus(string publish, string status1, string status2, string status3, string status4, string expectedResult)
        {
            var schools = new List<CourseSite>
            {
                new CourseSite {Status = status1, Publish = publish},
                new CourseSite {Status = status2, Publish = publish},
                new CourseSite {Status = status3, Publish = publish},
                new CourseSite {Status = status4, Publish = publish}
            };
            var course = new Course
            {
                CourseSites = new ObservableCollection<CourseSite>(schools)
            };
            var result = course.GetCourseStatus();
            result.Should().Be(expectedResult);
        }

        [TestCase("Y", "D", "Discontinued")]
        [TestCase("Y", "d", "Discontinued")]
        [TestCase("Y", "N", "New")]
        [TestCase("Y", "n", "New")]
        [TestCase("Y", "S", "Suspended")]
        [TestCase("Y", "s", "Suspended")]
        [TestCase("Y", "", "")]

        [TestCase("y", "D", "Discontinued")]
        [TestCase("y", "d", "Discontinued")]
        [TestCase("y", "N", "New")]
        [TestCase("y", "n", "New")]
        [TestCase("y", "S", "Suspended")]
        [TestCase("y", "s", "Suspended")]
        [TestCase("y", "", "")]

        [TestCase("N", "D", "Discontinued")]
        [TestCase("N", "d", "Discontinued")]
        [TestCase("N", "N", "New")]
        [TestCase("N", "n", "New")]
        [TestCase("N", "S", "Suspended")]
        [TestCase("N", "s", "Suspended")]
        [TestCase("N", "", "")]
        [TestCase("n", "D", "Discontinued")]
        [TestCase("n", "d", "Discontinued")]
        [TestCase("n", "N", "New")]
        [TestCase("n", "n", "New")]
        [TestCase("n", "S", "Suspended")]
        [TestCase("n", "s", "Suspended")]
        [TestCase("n", "", "")]
        [TestCase("", "D", "Discontinued")]
        [TestCase("", "d", "Discontinued")]
        [TestCase("", "N", "New")]
        [TestCase("", "n", "New")]
        [TestCase("", "S", "Suspended")]
        [TestCase("", "s", "Suspended")]
        [TestCase("", "", "")]
        [TestCase("", "D", "Discontinued")]
        [TestCase("", "d", "Discontinued")]
        [TestCase("", "N", "New")]
        [TestCase("", "n", "New")]
        [TestCase("", "S", "Suspended")]
        [TestCase("", "s", "Suspended")]
        [TestCase("", "", "")]

        [TestCase("Y", "R", "Running")]
        [TestCase("y", "R", "Running")]
        [TestCase("Y", "r", "Running")]
        [TestCase("y", "r", "Running")]
        [TestCase("N", "R", "Running but incomplete")]
        [TestCase("n", "R", "Running but incomplete")]
        [TestCase("N", "r", "Running but incomplete")]
        [TestCase("n", "r", "Running but incomplete")]
        [TestCase("", "R", "Running but incomplete")]
        [TestCase("", "R", "Running but incomplete")]
        [TestCase("", "r", "Running but incomplete")]
        [TestCase("", "r", "Running but incomplete")]
        public void TestGetSchoolStatus(string publish, string status, string expectedResult)
        {
            var schoolViewModel = new SiteViewModel{Status = status, Publish = publish};
            var result = schoolViewModel.GetSiteStatus();
            result.Should().Be(expectedResult);
        }

        [Test]
        [TestCase("d", true, "")]
        [TestCase("d", false, "")]
        [TestCase("s", true, "")]
        [TestCase("s", false, "")]
        [TestCase("b", true, "Yes")]
        [TestCase("b", false, "No")]
        public void TestGetHasVacancies(string status, bool hasVacancies, string expectedResult)
        {
            var schools = new List<CourseSite>
            {
                new CourseSite {Status = status},
            };
            var course = new Course
            {
                CourseSites = new ObservableCollection<CourseSite>(schools),
                HasVacancies = hasVacancies
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
