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
        [TestCase("", "f", "ss", "QTS full time with salary")]
        [TestCase("", "F", "SS", "QTS full time with salary")]
        [TestCase(null, "f", "ss", "QTS full time with salary")]
        [TestCase(null, "F", "SS", "QTS full time with salary")]
        [TestCase("pg", "f", "ss", "PGCE with QTS full time with salary")]
        [TestCase("PG", "F", "SS", "PGCE with QTS full time with salary")]
        [TestCase("pg", "p", "ss", "PGCE with QTS part time with salary")]
        [TestCase("PG", "P", "SS", "PGCE with QTS part time with salary")]
        [TestCase("", "f", "sd", "QTS full time")]
        [TestCase("", "F", "SD", "QTS full time")]
        [TestCase("", "", "SD", "QTS part time")]
        [TestCase("", "", "", "QTS part time")]
        [TestCase(null, "f", "sd", "QTS full time")]
        [TestCase(null, "F", "SD", "QTS full time")]
        [TestCase("pg", "f", "sd", "PGCE with QTS full time")]
        [TestCase("PG", "F", "SD", "PGCE with QTS full time")]
        [TestCase("pg", "p", "sd", "PGCE with QTS part time")]
        [TestCase("PG", "P", "SD", "PGCE with QTS part time")]        
        public void TestGetCourseVariantType(string profpostFlag, string studyMode, string programType, string expectedResult)
        {
            var course = new Course
            {
                ProfpostFlag = profpostFlag,
                StudyMode = studyMode,
                ProgramType = programType
            };
            var result = course.GetCourseVariantType();
            result.Should().Be(expectedResult);
        }

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
            var schools = new List<School>
            {
                new School {Status = status1},
                new School {Status = status2},
                new School {Status = status3},
                new School {Status = status4}
            };
            var course = new Course
            {
                Schools = new ObservableCollection<School>(schools)
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
        public void TestGetSchoolStatus(string status, string expectedResult)
        {
            var schoolViewModel = new SchoolViewModel{Status = status};
            var result = schoolViewModel.GetSchoolStatus();
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
            var courseVariantViewModel = new CourseVariantViewModel{Route = route};
            var result = courseVariantViewModel.GetRoute();
            result.Should().Be(expectedResult);
        }
        [Test]
        [TestCase("F", "Full time")]
        [TestCase("f", "Full time")]
        [TestCase("", "Part time")]
        [TestCase("P", "Part time")]
        [TestCase("p", "Part time")]
        public void TestGetStudyMode(string studyMode, string expectedResult)
        {
            var courseVariantViewModel = new CourseVariantViewModel { StudyMode = studyMode };
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
            var courseVariantViewModel = new CourseVariantViewModel { AgeRange = ageRange };
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
            var courseVariantViewModel = new CourseVariantViewModel { Qualifications = qualification };
            var result = courseVariantViewModel.GetQualification();
            result.Should().Be(expectedResult);
        }        
    }
}
