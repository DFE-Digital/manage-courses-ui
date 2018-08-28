using System;
using System.Collections.Generic;
using GovUk.Education.ManageCourses.Ui.ViewModels.Enums;

namespace GovUk.Education.ManageCourses.Ui.Helpers
{
    public static class CourseLengthHelper
    {
        public static readonly Dictionary<CourseLength, string>  Lookup = new Dictionary<CourseLength, string>()
                {
                  { CourseLength.OneYear, "One year" },
                  { CourseLength.TwoYears, "Up to two years" },
                  { CourseLength.Other, "Other" },
                };

        public static CourseLength? GetCourseLength(this string value)
        {
            CourseLength parseValue;
            CourseLength? result = null;
            if (Enum.TryParse(value, out parseValue))
            {
                result = parseValue;
            }
            return result;
        }

        public static string GetDisplayText(this CourseLength? value)
        {
            return value.HasValue ? Lookup[value.Value] : "";
        }

    }
}
