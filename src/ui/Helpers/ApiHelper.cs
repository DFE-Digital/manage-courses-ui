using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace GovUk.Education.ManageCourses.Ui.Helpers
{
    public static class ApiHelper
    {
        private static JsonSerializerSettings _jsonSerializerSettings;
        static ApiHelper()
        {
            _jsonSerializerSettings = new JsonSerializerSettings
            {
                MissingMemberHandling = MissingMemberHandling.Ignore,
                NullValueHandling = NullValueHandling.Ignore
            };
        }
        
        public static SearchAndCompare.Domain.Models.Course Convert(ApiClient.Course2 course)//TODO sort this Course2 its pants
        {
            var jsonCourse = JsonConvert.SerializeObject(course, _jsonSerializerSettings);
            SearchAndCompare.Domain.Models.Course deserializedCourse = JsonConvert.DeserializeObject<SearchAndCompare.Domain.Models.Course>(jsonCourse);

            return deserializedCourse;
        }
    }
}
