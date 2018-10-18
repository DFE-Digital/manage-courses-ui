using System;

namespace GovUk.Education.ManageCourses.Ui.ViewModels
{
    public class VariantViewModel
    {
        public string OrganisationName { get; set; }
        public string OrganisationId { get; set; }
        public bool MultipleOrganisations { get; set; }
        public string CourseTitle { get; set; }
        public string AccreditingProviderId { get; set; }
        public CourseVariantViewModel Course { get; set; }
        public BaseCourseEnrichmentViewModel CourseEnrichment { get; set; }
        public Uri LiveSearchUrl { get; set; }
        public bool IsSalary { get; set; }
    }
}
