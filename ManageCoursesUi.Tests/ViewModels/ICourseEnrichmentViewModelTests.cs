using FluentAssertions;
using GovUk.Education.ManageCourses.ApiClient;
using GovUk.Education.ManageCourses.Ui.ViewModels;
using GovUk.Education.ManageCourses.Ui.ViewModels.Enums;
using NUnit.Framework;

namespace ManageCoursesUi.Tests.ViewModels
{
    [TestFixture]
    public class ICourseEnrichmentViewModelTests
    {
        [Test]
        public void AboutCourseEnrichmentViewModel_CopyFrom()
        {
            var viewModel = new AboutCourseEnrichmentViewModel
            {
                AboutCourse = "VM.AboutCourse",
                InterviewProcess = "VM.InterviewProcess",
                HowSchoolPlacementsWork = "VM.HowSchoolPlacementsWork"
            };
            CourseEnrichmentModel model = GetAnEnrichmentModel();
            viewModel.CopyFrom(model);

            viewModel.AboutCourse.Should().Be("M.AboutCourse");
            viewModel.InterviewProcess.Should().Be("VM.InterviewProcess");
            viewModel.HowSchoolPlacementsWork.Should().Be("M.HowSchoolPlacementsWork");
        }

        [Test]
        public void CourseRequirementsEnrichmentViewModel_CopyFrom()
        {
            var viewModel = new CourseRequirementsEnrichmentViewModel
            {
                Qualifications = "VM.Qualifications",
                PersonalQualities = "VM.PersonalQualities",
                OtherRequirements = "VM.OtherRequirements"
            };
            CourseEnrichmentModel model = GetAnEnrichmentModel();
            viewModel.CopyFrom(model);

            viewModel.Qualifications.Should().Be("M.Qualifications");
            viewModel.PersonalQualities.Should().Be("VM.PersonalQualities");
            viewModel.OtherRequirements.Should().Be("M.OtherRequirements");
        }

        [Test]
        public void CourseFeesEnrichmentViewModel_CopyFrom()
        {
            var viewModel = new CourseFeesEnrichmentViewModel
            {
                FeeInternational = 123,
                FinancialSupport = null // also empty
            };

            CourseEnrichmentModel model = GetAnEnrichmentModel();
            viewModel.CopyFrom(model);

            viewModel.CourseLength.Should().Be(CourseLength.OneYear);
            viewModel.FeeUkEu.Should().Be(321);
            viewModel.FeeInternational.Should().Be(123);
            viewModel.FeeDetails.Should().Be("M.FeeDetails");
            viewModel.FinancialSupport.Should().BeNull();
        }
             
        [Test]
        public void CourseSalaryEnrichmentViewModel_CopyFrom()
        {
            var viewModel = new CourseSalaryEnrichmentViewModel
            {
                SalaryDetails = "VM.SalaryDetails"
            };

            CourseEnrichmentModel model = GetAnEnrichmentModel();
            viewModel.CopyFrom(model);

            viewModel.CourseLength.Should().Be(CourseLength.OneYear);
            viewModel.SalaryDetails.Should().Be("VM.SalaryDetails");
        }


        private static CourseEnrichmentModel GetAnEnrichmentModel()
        {
            CourseEnrichmentModel model = new CourseEnrichmentModel
            {
                AboutCourse = "M.AboutCourse",
                InterviewProcess = "", //empty!!
                HowSchoolPlacementsWork = "M.HowSchoolPlacementsWork",

                Qualifications = "M.Qualifications",
                PersonalQualities = "", //empty!!
                OtherRequirements = "M.OtherRequirements",

                CourseLength = "OneYear",
                FeeUkEu = 321,
                FeeInternational = null, //empty!!
                FeeDetails = "M.FeeDetails",
                FinancialSupport = null, //empty!!
                
            };
            return model;
        }
    }
}