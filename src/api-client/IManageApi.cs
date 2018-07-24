using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace GovUk.Education.ManageCourses.ApiClient
{
    public interface IManageApi
    {
        Task<OrganisationCourses> GetCoursesByOrganisation(string ucasCode);
        Task<IEnumerable<UserOrganisation>> GetOrganisations();
        Task LogAccessRequest(AccessRequest accessRequest);
    }
}
