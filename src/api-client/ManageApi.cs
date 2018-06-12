using System.Collections.Generic;
using System.Threading.Tasks;

namespace GovUk.Education.ManageCourses.ApiClient
{
    public class ManageApi
    {
        public async Task<IEnumerable<Course>> GetCourses()
        {
            var client = new ManageCoursesApiClient();
            var observableCollection = await client.ExportAsync();
            return observableCollection;
        }
    }
}
