using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using FluentAssertions;
using GovUk.Education.ManageCourses.Ui.ViewModels;
using GovUk.Education.ManageCourses.Ui.ViewModels.Enums;
using NUnit.Framework;

namespace ManageCoursesUi.Tests.ViewModels
{
    [TestFixture]
    public class CourseEnrichmentViewModelTests
    {
        [Test]
        public void Validation_Full()
        {
            var input = new CourseEnrichmentViewModel
            {
                AboutCourse = "AboutCourse",
                Qualifications = "Qualifications",
                HowSchoolPlacementsWork = "HowSchoolPlacementsWork",
                SalaryDetails = "SalaryDetails",
                CourseLength = CourseLength.Other,
                FeeUkEu = 10000000
            };

            var validationResults = Validate(input);

            validationResults.Count.Should().Be(0);
        }

        [Test]
        public void Validation_Empty()
        {
            var input = new CourseEnrichmentViewModel
            {

            };

            var validationResults = Validate(input);

            validationResults.Count.Should().Be(6);
            AssertMessageFor(validationResults, "AboutCourse", "Enter details about this course");
            AssertMessageFor(validationResults, "Qualifications", "Enter details for about qualifications needed");
            AssertMessageFor(validationResults, "HowSchoolPlacementsWork", "Enter details about school placements");
            AssertMessageFor(validationResults, "SalaryDetails", "Give details about salary");
            AssertMessageFor(validationResults, "CourseLength", "Give details about course length");
            AssertMessageFor(validationResults, "FeeUkEu", "Give details about the fee for UK and EU students");
        }

        [Test]
        public void Validation_Excess()
        {
            var input = new CourseEnrichmentViewModel
            {
                AboutCourse = Times("word ", 401),
                InterviewProcess = Times("word ", 251),
                HowSchoolPlacementsWork = Times("word ", 351),

                Qualifications = Times("word ", 101),
                PersonalQualities = Times("word ", 101),
                OtherRequirements = Times("word ", 101),
                FeeDetails = Times("word ", 251),
                FinancialSupport = Times("word ", 251),
                SalaryDetails = Times("word ", 251),
                CourseLength = CourseLength.OneYear,
                FeeUkEu = 12321

            };

            var validationResults = Validate(input);

            validationResults.Count.Should().Be(9);

            AssertMessageFor(validationResults, "AboutCourse", "Reduce the word count for about this course");
            AssertMessageFor(validationResults, "InterviewProcess", "Reduce the word count for interview process");
            AssertMessageFor(validationResults, "HowSchoolPlacementsWork", "Reduce the word count for how school placements work");

            AssertMessageFor(validationResults, "Qualifications", "Reduce the word count for qualifications needed");
            AssertMessageFor(validationResults, "PersonalQualities", "Reduce the word count for personal qualities");
            AssertMessageFor(validationResults, "OtherRequirements", "Reduce the word count for other requirements");

            AssertMessageFor(validationResults, "FeeDetails", "Reduce the word count for fee details");
            AssertMessageFor(validationResults, "FinancialSupport", "Reduce the word count for financial support");

            AssertMessageFor(validationResults, "SalaryDetails", "Reduce the word count for salary");

        }

        private static List<ValidationResult> Validate(CourseEnrichmentViewModel input)
        {
            var validationContext = new ValidationContext(input, null, null);
            var validationResults = new List<ValidationResult>();
            Validator.TryValidateObject(input, validationContext, validationResults, true);
            return validationResults;
        }

        private static void AssertMessageFor(List<ValidationResult> validationResults, string propertyName, string message)
        {
            validationResults.Single(x => x.MemberNames.Single() == propertyName).ErrorMessage.Should().Be(message);
        }

        private static string Times(string word, int times)
        {
            return new StringBuilder(word.Length * times).Insert(0, word, times).ToString();
        }

    }
}
