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
    }

    public class Organisation
    {
        public string TrainWithUs { get; set; }

        public string DomainName { get; set; }

        public string AboutTrainingProvider { get; set; }

        public string TrainWithDisability { get; set; }
    }
}
