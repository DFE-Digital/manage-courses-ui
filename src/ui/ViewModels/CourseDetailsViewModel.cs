using System.Collections.Generic;

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
        public string AboutCourse { get; set; }
        public string InterviewProcess { get; set; }
        public string HowSchoolPlacementsWork { get; set; }
    }
}
