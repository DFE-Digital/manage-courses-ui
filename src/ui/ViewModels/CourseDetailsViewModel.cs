using System.Collections.Generic;

namespace GovUk.Education.ManageCourses.Ui.ViewModels
{
    public class CourseDetailsViewModel
    {
        public string CourseTitle { get; set; }
        public string UcasCode { get; set; }
        public IEnumerable<SubjectViewModel> Subjects { get; set; }
    }
}
