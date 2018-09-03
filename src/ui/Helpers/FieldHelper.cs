using System;
using System.Globalization;
using GovUk.Education.ManageCourses.Ui.ViewModels.Enums;
namespace GovUk.Education.ManageCourses.Ui.Helpers
{
    public static class FieldHelper
    {
        public static string DisplayText(this string value, int maxLength = 100, string defaultEmpty = "This field is empty")
        {
            var result = string.IsNullOrWhiteSpace(value) ? defaultEmpty : value.Length <= maxLength ? value : value.Substring(0, maxLength);
            return result;
        }

        public static string DisplayText(this decimal? value, int maxLength = 100, string defaultEmpty = "This field is empty")
        {
            var text = value.HasValue ? string.Format(CultureInfo.InvariantCulture, "£{0:n0}", (int) value.Value) : "";
            var result = DisplayText(text, maxLength, defaultEmpty);
            return result;
        }

        public static string DisplayText(this CourseLength? value, int maxLength = 100, string defaultEmpty = "This field is empty")
        {
            var text = value.GetDisplayText();
            var result = DisplayText(text, maxLength, defaultEmpty);
            return result;
        }
    }
}
