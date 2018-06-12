using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace GovUk.Education.ManageCourses.ApiClient
{
    public class ManageApi
    {
        public void SendToManageCoursesApi(List<Course> courses)
        {
            Console.WriteLine("Posting to api...");
            var client = new ManageCoursesApiClient();
            var payload = new Payload { Courses = new ObservableCollection<Course>(courses) };
            client.ImportAsync(payload).Wait();
            Console.WriteLine("Done.");
        }
    }
}
