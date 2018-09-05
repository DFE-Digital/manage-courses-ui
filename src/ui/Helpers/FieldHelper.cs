using System;
using System.Globalization;
using GovUk.Education.ManageCourses.Ui.ViewModels.Enums;
namespace GovUk.Education.ManageCourses.Ui.Helpers
{
    public static class FieldHelper
    {
        public const int FeeMin = 0;
        public const int FeeMax = 100000;

        public static string DisplayText(this string value, int maxLength = 100, string defaultEmpty = "This field is empty")
        {
            var result = string.IsNullOrWhiteSpace(value) ? defaultEmpty : value.Length <= maxLength ? value : value.Substring(0, maxLength);
            return result;
        }

        public static string DisplayText(this int? value, int maxLength = 100, string defaultEmpty = "This field is empty")
        {
            var text = value.HasValue ? string.Format(CultureInfo.InvariantCulture, "£{0:n0}", value.Value) : "";
            var result = DisplayText(text, maxLength, defaultEmpty);
            return result;
        }

        public static string InputText(this int? value)
        {
            var text = value.HasValue ? string.Format(CultureInfo.InvariantCulture, "{0:G0}", value.Value) : "";
            var result = DisplayText(text, 100, "");
            return result;
        }

        public static string DisplayText(this CourseLength? value, int maxLength = 100, string defaultEmpty = "This field is empty")
        {
            var text = value.GetDisplayText();
            var result = DisplayText(text, maxLength, defaultEmpty);
            return result;
        }

        public static int? GetFeeValue(this decimal? value)
        {
            return (value.HasValue && value.Value <= FeeMax && value.Value >= FeeMin) ? (int?) value.Value : null;

        }
    }
}
