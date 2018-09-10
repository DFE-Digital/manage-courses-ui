using System;
using System.Collections.Generic;
using GovUk.Education.ManageCourses.Ui.ViewModels.Enums;

namespace GovUk.Education.ManageCourses.Ui.Helpers
{
    public static class CourseLengthHelper
    {
        public static readonly Dictionary<CourseLength, string>  Lookup = new Dictionary<CourseLength, string>()
                {
                  { CourseLength.OneYear, "1 year" },
                  { CourseLength.TwoYears, "Up to 2 years" },
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
            if(result == null && !string.IsNullOrWhiteSpace(value))
            {
                result = CourseLength.Other;
            }
            return result;
        }
        public static string GetCourseLengthInput(this string value)
        {
            if (GetCourseLength(value) == CourseLength.Other)
            {
                return value;
            }

            return null;
        }

        public static string GetDisplayText(this CourseLength? value)
        {
            return value.HasValue ? Lookup[value.Value] : "";
        }

    }
}
