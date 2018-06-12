using System.Collections.Generic;
using System.Threading.Tasks;

namespace GovUk.Education.ManageCourses.ApiClient
{
    public class ManageApi
    {
        public async Task<IEnumerable<Course>> GetCourses()
        {
            var client = new ManageCoursesApiClient();
            var response = await client.ExportAsync();
            // todo: return actual course list
            return new List<Course>()
            {
                new Course {Title = "todo-course-title"},
            };
        }
    }
}
