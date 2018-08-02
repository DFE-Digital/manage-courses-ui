using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Threading.Tasks;
using GovUk.Education.ManageCourses.ApiClient;


namespace GovUk.Education.ManageCourses.Ui
{
    public interface IManageApi
    {
        Task<OrganisationCourses> GetCoursesByOrganisation(string ucasCode);
        Task<IEnumerable<UserOrganisation>> GetOrganisations();
        Task LogAccessRequest(AccessRequest accessRequest);
        Task SaveOrganisationDetails(Organisation organisation);
        Task<Organisation> GetOrganisationDetails(string ucasCode);
        Task<Course> GetCourseDetails(string ucasCode);
    }

    public class Organisation
    {
        public string TrainWithUs { get; set; }

        public string DomainName { get; set; }

        public string AboutTrainingProvider { get; set; }

        public string TrainWithDisability { get; set; }
    }

    public class Course
    {
        public string AboutCourse { get; set; }
        public string InterviewProcess { get; set; }
        public string SchoolPlacement { get; set; }
        public string CourseLength { get; set; }
        public string CourseFees { get; set; }
        public string InternationalCourseFees { get; set; }
        public string FeeDetails { get; set; }
        public string FinancialSupport { get; set; }
        public string Qualifications { get; set; }
        public string PersonalQualities { get; set; }
        public string OtherRequirements { get; set; }
    }
}
