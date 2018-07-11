using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace GovUk.Education.ManageCourses.Ui.Helpers
{
    public static class TempDataHelper
    {
        public static void Set(this ITempDataDictionary data, string key, string value)
        {
            data[key] = value;
        }

        public static string Get(this ITempDataDictionary data, string key)
        {
            return data[key] as string;
        }
    }
}
