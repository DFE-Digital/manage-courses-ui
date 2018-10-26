using System;

namespace GovUk.Education.ManageCourses.Ui.ViewModels
{
    public class CourseViewModel
    {
        public string InstName { get; set; }
        public string InstCode { get; set; }
        public bool MultipleOrganisations { get; set; }
        public string CourseTitle { get; set; }
        public string AccreditingInstCode { get; set; }
        public CourseDetailsViewModel Course { get; set; }
        public BaseCourseEnrichmentViewModel CourseEnrichment { get; set; }
        public Uri LiveSearchUrl { get; set; }
        public bool IsSalary { get; set; }
    }
}
