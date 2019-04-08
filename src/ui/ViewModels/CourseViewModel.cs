using System;

namespace GovUk.Education.ManageCourses.Ui.ViewModels
{
    public class CourseViewModel
    {
        public string ProviderName { get; set; }
        public string ProviderCode { get; set; }
        public bool OptedIn { get; set; }
        public bool MultipleOrganisations { get; set; }
        public string CourseTitle { get; set; }
        public string AccreditingProviderCode { get; set; }
        public CourseDetailsViewModel Course { get; set; }
        public BaseCourseEnrichmentViewModel CourseEnrichment { get; set; }
        public Uri LiveSearchUrl { get; set; }
        public bool IsSalary { get; set; }
    }
}
