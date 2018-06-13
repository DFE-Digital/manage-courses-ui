using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ManageCoursesUi.ViewModels
{
    public class AboutCourseViewModel
    {
        public string CourseWebpage { get; set; }
        public string AboutCourse { get; set; }
    }

    public class AboutOrganisationViewModel
    {
        public string AboutOrganisation { get; set; }
        public string TrainingWithDisability { get; set; }
        public string InterviewProcess { get; set; }
        public string SchoolPlacements { get; set; }
    }
    public class CourseRequirementsViewModel
    {
        public string QualicationsNeeded { get; set; }
        public string PersonalQualities { get; set; }
        public string OtherRequirements { get; set; }
    }
    public class SalaryViewModel
    {
        public string EarnSalaryTraining { get; set; }
    }
    public class SubjectViewModel
    {
        public string Subject { get; set; }
        public string Type { get; set; }
        public string Code { get; set; }
    }

    public class CourseDetailsViewModel
    {
        public string CourseTitle { get; set; }
        public IList<SubjectViewModel> Subjects { get; set; }

        public AboutCourseViewModel AboutCourse { get; set; }
        public AboutOrganisationViewModel AboutOrganisation { get; set; }
        public CourseRequirementsViewModel CourseRequirements { get; set; }
        public SalaryViewModel Salary { get; set; }
    }
}
