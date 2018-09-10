using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using FluentAssertions;
using GovUk.Education.ManageCourses.ApiClient;
using GovUk.Education.ManageCourses.Ui.ViewModels;
using GovUk.Education.ManageCourses.Ui.ViewModels.Enums;
using NUnit.Framework;

namespace ManageCoursesUi.Tests.ViewModels
{
    [TestFixture]
    public class CourseEnrichmentViewModelTests
    {
        [Test]
        public void FeeBased_Validation_Full()
        {
            var input = new FeeBasedCourseEnrichmentViewModel
            {
                AboutCourse = "AboutCourse",
                Qualifications = "Qualifications",
                HowSchoolPlacementsWork = "HowSchoolPlacementsWork",
                CourseLength = CourseLength.Other,
                FeeUkEu = 10000000
            };

            var validationResults = Validate(input);

            validationResults.Count.Should().Be(0);
        }

        [Test]
        public void SalaryBased_Validation_Full()
        {
            var input = new SalaryBasedCourseEnrichmentViewModel
            {
                AboutCourse = "AboutCourse",
                Qualifications = "Qualifications",
                HowSchoolPlacementsWork = "HowSchoolPlacementsWork",
                SalaryDetails = "SalaryDetails",
                CourseLength = CourseLength.Other,
            };

            var validationResults = Validate(input);

            validationResults.Count.Should().Be(0);
        }

        [Test]
        public void FeeBased_Validation_Empty()
        {
            var input = new FeeBasedCourseEnrichmentViewModel
            {

            };

            var validationResults = Validate(input);

            validationResults.Count.Should().Be(5);
            AssertMessageFor(validationResults, "AboutCourse", "Enter details about this course");
            AssertMessageFor(validationResults, "Qualifications", "Enter details for about qualifications needed");
            AssertMessageFor(validationResults, "HowSchoolPlacementsWork", "Enter details about school placements");

            AssertMessageFor(validationResults, "CourseLength", "Give details about course length");
            AssertMessageFor(validationResults, "FeeUkEu", "Give details about the fee for UK and EU students");
        }

        [Test]
        public void SalaryBased_Validation_Empty()
        {
            var input = new SalaryBasedCourseEnrichmentViewModel
            {

            };

            var validationResults = Validate(input);

            validationResults.Count.Should().Be(5);
            AssertMessageFor(validationResults, "AboutCourse", "Enter details about this course");
            AssertMessageFor(validationResults, "Qualifications", "Enter details for about qualifications needed");
            AssertMessageFor(validationResults, "HowSchoolPlacementsWork", "Enter details about school placements");
            AssertMessageFor(validationResults, "SalaryDetails", "Give details about salary");
            AssertMessageFor(validationResults, "CourseLength", "Give details about course length");
        }

        [Test]
        public void FeeBased_Validation_Excess()
        {
            var input = new FeeBasedCourseEnrichmentViewModel
            {
                AboutCourse = Times("word ", 401),
                InterviewProcess = Times("word ", 251),
                HowSchoolPlacementsWork = Times("word ", 351),

                Qualifications = Times("word ", 101),
                PersonalQualities = Times("word ", 101),
                OtherRequirements = Times("word ", 101),
                FeeDetails = Times("word ", 251),
                FinancialSupport = Times("word ", 251),
                CourseLength = CourseLength.OneYear,
                FeeUkEu = 12321

            };

            var validationResults = Validate(input);

            validationResults.Count.Should().Be(8);

            AssertMessageFor(validationResults, "AboutCourse", "Reduce the word count for about this course");
            AssertMessageFor(validationResults, "InterviewProcess", "Reduce the word count for interview process");
            AssertMessageFor(validationResults, "HowSchoolPlacementsWork", "Reduce the word count for how school placements work");

            AssertMessageFor(validationResults, "Qualifications", "Reduce the word count for qualifications needed");
            AssertMessageFor(validationResults, "PersonalQualities", "Reduce the word count for personal qualities");
            AssertMessageFor(validationResults, "OtherRequirements", "Reduce the word count for other requirements");

            AssertMessageFor(validationResults, "FeeDetails", "Reduce the word count for fee details");
            AssertMessageFor(validationResults, "FinancialSupport", "Reduce the word count for financial support");
        }

        [Test]
        public void SalaryBased_Validation_Excess()
        {
            var input = new SalaryBasedCourseEnrichmentViewModel
            {
                AboutCourse = Times("word ", 401),
                InterviewProcess = Times("word ", 251),
                HowSchoolPlacementsWork = Times("word ", 351),

                Qualifications = Times("word ", 101),
                PersonalQualities = Times("word ", 101),
                OtherRequirements = Times("word ", 101),
                SalaryDetails = Times("word ", 251),
                CourseLength = CourseLength.OneYear,
            };

            var validationResults = Validate(input);

            validationResults.Count.Should().Be(7);

            AssertMessageFor(validationResults, "AboutCourse", "Reduce the word count for about this course");
            AssertMessageFor(validationResults, "InterviewProcess", "Reduce the word count for interview process");
            AssertMessageFor(validationResults, "HowSchoolPlacementsWork", "Reduce the word count for how school placements work");

            AssertMessageFor(validationResults, "Qualifications", "Reduce the word count for qualifications needed");
            AssertMessageFor(validationResults, "PersonalQualities", "Reduce the word count for personal qualities");
            AssertMessageFor(validationResults, "OtherRequirements", "Reduce the word count for other requirements");

            AssertMessageFor(validationResults, "SalaryDetails", "Reduce the word count for salary");
        }
        [Test]
        public void TestCourseLengthCopyFromForOtherTextEntry_CourseSalaryEnrichmentViewModel()
        {
            const string textEnteredByUser = "text entered by users";

            var viewModel = new CourseSalaryEnrichmentViewModel();
            var enrichmentModel = new CourseEnrichmentModel {CourseLength = textEnteredByUser};

            viewModel.CopyFrom(enrichmentModel);

            viewModel.CourseLength.Should().Be(CourseLength.Other);
            viewModel.CourseLengthInput.Should().BeEquivalentTo(textEnteredByUser);
        }
        [Test]
        public void TestCourseLengthOneYearCopyFrom_CourseSalaryEnrichmentViewModel()
        {
            var viewModel = new CourseSalaryEnrichmentViewModel();
            var enrichmentModel = new CourseEnrichmentModel { CourseLength = "OneYear" };

            viewModel.CopyFrom(enrichmentModel);

            viewModel.CourseLength.Should().Be(CourseLength.OneYear);
            viewModel.CourseLengthInput.Should().BeNullOrEmpty();
        }
        [Test]
        public void TestCourseLengthTwoYearsCopyFrom_CourseSalaryEnrichmentViewModel()
        {
            var viewModel = new CourseSalaryEnrichmentViewModel();
            var enrichmentModel = new CourseEnrichmentModel { CourseLength = "TwoYears" };

            viewModel.CopyFrom(enrichmentModel);

            viewModel.CourseLength.Should().Be(CourseLength.TwoYears);
            viewModel.CourseLengthInput.Should().BeNullOrEmpty();
        }

        [Test]
        public void TestCourseLengthMapIntoForOtherTextEntry_CourseSalaryEnrichmentViewModel()
        {
            const string TextEnteredByUser = "text entered by users";
            var viewModel = new CourseSalaryEnrichmentViewModel
            {
                CourseLength = CourseLength.Other,
                CourseLengthInput =TextEnteredByUser
            };

            var enrichmentModel = new CourseEnrichmentModel();

            viewModel.MapInto(ref enrichmentModel);

            enrichmentModel.CourseLength.Should().BeEquivalentTo(TextEnteredByUser);
        }
        [Test]
        [TestCase("")]
        [TestCase("    ")]
        [TestCase("xxxxxxxxxx")]
        [TestCase(null)]
        public void TestCourseLengthMapIntoForSelection_CourseSalaryEnrichmentViewModel(string courseLengthOther)
        {
            var viewModel = new CourseSalaryEnrichmentViewModel
            {
                CourseLength = CourseLength.OneYear,
                CourseLengthInput = courseLengthOther
            };

            var enrichmentModel = new CourseEnrichmentModel();

            viewModel.MapInto(ref enrichmentModel);

            enrichmentModel.CourseLength.Should().BeEquivalentTo("OneYear");

        }
        [Test]
        public void TestCourseLengthCopyFromForOtherTextEntry_CourseFeesEnrichmentViewModel()
        {
            const string textEnteredByUser = "text entered by users";

            var viewModel = new CourseFeesEnrichmentViewModel();
            var enrichmentModel = new CourseEnrichmentModel { CourseLength = textEnteredByUser };

            viewModel.CopyFrom(enrichmentModel);

            viewModel.CourseLength.Should().Be(CourseLength.Other);
            viewModel.CourseLengthInput.Should().BeEquivalentTo(textEnteredByUser);
        }
        [Test]
        public void TestCourseLengthOneYearCopyFrom_CourseFeesEnrichmentViewModel()
        {
            var viewModel = new CourseFeesEnrichmentViewModel();
            var enrichmentModel = new CourseEnrichmentModel { CourseLength = "OneYear" };

            viewModel.CopyFrom(enrichmentModel);

            viewModel.CourseLength.Should().Be(CourseLength.OneYear);
            viewModel.CourseLengthInput.Should().BeNullOrEmpty();
        }
        [Test]
        public void TestCourseLengthMapIntoForOtherTextEntry_CourseFeesEnrichmentViewModel()
        {
            const string TextEnteredByUser = "text entered by users";
            var viewModel = new CourseFeesEnrichmentViewModel
            {
                CourseLength = CourseLength.Other,
                CourseLengthInput = TextEnteredByUser
            };

            var enrichmentModel = new CourseEnrichmentModel();

            viewModel.MapInto(ref enrichmentModel);

            enrichmentModel.CourseLength.Should().BeEquivalentTo(TextEnteredByUser);
        }
        [Test]
        [TestCase("")]
        [TestCase("    ")]
        [TestCase("xxxxxxxxxx")]
        [TestCase(null)]
        public void TestCourseLengthMapIntoForSelection_CourseFeesEnrichmentViewModel(string courseLengthOther)
        {
            var viewModel = new CourseFeesEnrichmentViewModel()
            {
                CourseLength = CourseLength.OneYear,
                CourseLengthInput = courseLengthOther
            };

            var enrichmentModel = new CourseEnrichmentModel();

            viewModel.MapInto(ref enrichmentModel);

            enrichmentModel.CourseLength.Should().BeEquivalentTo("OneYear");

        }
        private static List<ValidationResult> Validate(BaseCourseEnrichmentViewModel input)
        {
            var feeBasedCourseEnrichmentViewModel = (input as FeeBasedCourseEnrichmentViewModel);
            var salaryBasedCourseEnrichmentViewModel = (input as SalaryBasedCourseEnrichmentViewModel);
            var validationResults = new List<ValidationResult>();

            if (feeBasedCourseEnrichmentViewModel != null)
            {
                var validationContext = new ValidationContext(feeBasedCourseEnrichmentViewModel, null, null);
                Validator.TryValidateObject(feeBasedCourseEnrichmentViewModel, validationContext, validationResults, true);
            }
            else {
                var validationContext = new ValidationContext(salaryBasedCourseEnrichmentViewModel, null, null);
                Validator.TryValidateObject(salaryBasedCourseEnrichmentViewModel, validationContext, validationResults, true);
            }

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
