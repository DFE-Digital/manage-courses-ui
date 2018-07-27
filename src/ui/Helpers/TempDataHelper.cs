using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace GovUk.Education.ManageCourses.Ui.Helpers
{
    public static class TempDataHelper
    {
        public static void Add(this ITempDataDictionary data, string key, string value)
        {
            data.Add(key, value);
        }

        public static string Get(this ITempDataDictionary data, string key)
        {
            return data.Get(key);
        }
    }
}
