using System.Collections.Generic;

namespace GovUk.Education.ManageCourses.Ui.ViewModels
{
    public class FromUcasViewModel
    {
        public string OrganisationName { get; set; }
        public string CourseTitle { get; set; }
        public string UcasCode { get; set; }
        public IEnumerable<CourseVariantViewModel> Courses { get; set; }
    }

    public class CourseVariantViewModel
    {
        public string Name { get; set; }
        public string ProgrammeCode { get; set; }
        public string Options { get; set; }
        public string ProviderCode { get; set; }
        public string Accrediting { get; set; }
        public string Route { get; set; }
        public string Subjects { get; set; }
        public string AgeRange { get; set; }
        public string Qualifications { get; set; }
        public string Regions { get; set; }
        public IEnumerable<SchoolViewModel> Schools { get; set; }

    }

    public class SchoolViewModel
    {
        public string LocationName { get; set; }
        public string Address { get; set; }
        public string Code { get; set; }
        public string FullTimeVacancies { get; set; }
        public string PartTimeVacancies { get; set; }
        public string ApplicationsAcceptedFrom { get; set; }
    }
}
