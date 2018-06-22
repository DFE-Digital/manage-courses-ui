using System.Collections.Generic;

namespace GovUk.Education.ManageCourses.Ui.ViewModels
{
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
    }
}
