using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using GovUk.Education.ManageCourses.ApiClient;

namespace GovUk.Education.ManageCourses.Ui.ViewModels
{
    public class CourseDetailsViewModel
    {
        public string OrganisationName { get; set; }
        public string CourseTitle { get; set; }
        public string UcasCode { get; set; }
        public IEnumerable<SubjectViewModel> Subjects { get; set; }
    }

    public class CourseAboutViewModel
    {
        [Required(ErrorMessage = "Enter about this course")]
        [Display(Name = "About this course")]
        public string AboutCourse { get; set; }

        [Display(Name = "Interview process (optional)")]
        public string InterviewProcess { get; set; }

        [Required(ErrorMessage = "Enter how school placements work")]
        [Display(Name = "How school placements work")]
        public string SchoolPlacement { get; set; }
    }

    public class CourseFeesLengthViewModel
    {
        [Required(ErrorMessage = "Enter course length")]
        [Display(Name = "Course length")]
        public string CourseLength { get; set; }

        [Required(ErrorMessage = "Enter fee for UK and EU students")]
        [Display(Name = "Fee for UK and EU students")]
        public string CourseFees { get; set; }

        [Display(Name = "Fee for international students (optional)")]
        public string InternationalCourseFees { get; set; }

        [Display(Name = "Fee details (optional)")]
        public string FeeDetails { get; set; }

        [Display(Name = "Financial support you offer (optional)")]
        public string FinancialSupport { get; set; }
    }

    public class CourseRequirementsEligbilityViewModel
    {
        [Required(ErrorMessage = "Enter qualifications needed")]
        [Display(Name = "Qualifications needed")]
        public string Qualifications { get; set; }

        [Display(Name = "Personal qualities (optional)")]
        public string PersonalQualities { get; set; }

        [Display(Name = "Other requirements (optional)")]
        public string OtherRequirements { get; set; }
    }
}
