using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using FluentAssertions;
using GovUk.Education.ManageCourses.Ui.ViewModels;
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
                HowSchoolPlacementsWork = "HowSchoolPlacementsWork"
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

            validationResults.Count.Should().Be(3);
            AssertMessageFor(validationResults, "AboutCourse", "Give details about the course");
            AssertMessageFor(validationResults, "Qualifications", "Give details about the required qualifications");
            AssertMessageFor(validationResults, "HowSchoolPlacementsWork", "Give details about school placements");
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
                FinancialSupport = Times("word ", 251)
            };

            var validationResults = Validate(input);

            validationResults.Count.Should().Be(8);

            AssertMessageFor(validationResults, "AboutCourse", "Reduce the word count for details about the course");
            AssertMessageFor(validationResults, "InterviewProcess", "Reduce the word count for details about the interview process");
            AssertMessageFor(validationResults, "HowSchoolPlacementsWork", "Reduce the word count for details about school placements");

            AssertMessageFor(validationResults, "Qualifications", "Reduce the word count for details about the required qualifications");
            AssertMessageFor(validationResults, "PersonalQualities", "Reduce the word count for details about personal qualities");
            AssertMessageFor(validationResults, "OtherRequirements", "Reduce the word count for details about other requirements");

            AssertMessageFor(validationResults, "FeeDetails", "Reduce the word count for fee details");
            AssertMessageFor(validationResults, "FinancialSupport", "Reduce the word count for financial support");

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
