using System;

namespace GovUk.Education.ManageCourses.Ui.Helpers
{
    public static class FieldHelper
    {
        public static string DisplayText(this string value, int maxLength = 100, string defaultEmpty = "This field is empty")
        {
            var result = string.IsNullOrWhiteSpace(value) ? defaultEmpty : value.Length <= maxLength ? value : value.Substring(0, maxLength);

            return result;
        }
    }
}
