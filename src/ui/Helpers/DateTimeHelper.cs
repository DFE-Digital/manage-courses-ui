using System;

namespace GovUk.Education.ManageCourses.Ui.Helpers
{
    public static class DateTimeHelper
    {
        public static string DateString(this DateTime? date)
        {
            var result = "";

            if (date.HasValue && date > DateTime.MinValue)
            {
                result = date.Value.ToString("dd MMMM yyyy");
            }

            return result;
        }
    }
}