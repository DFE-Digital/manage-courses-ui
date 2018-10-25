using System;

namespace GovUk.Education.ManageCourses.Ui.ViewModels
{
    public class SiteViewModel
    {
        public string LocationName { get; set; }
        public string Address { get; set; }
        public string Code { get; set; }
        public string Status { get; set; }
        public string VacStatus { get; set; }

        private string _applicationsAcceptedFrom;
        public string ApplicationsAcceptedFrom
        {
            get => FormatDate(_applicationsAcceptedFrom);
            set => _applicationsAcceptedFrom = value;
        }

        private static string FormatDate(string dateToFormat)
        {
            return DateTime.TryParse(dateToFormat, out var date) ? date.ToString("dd MMM yyyy") : dateToFormat;
        }
    }
}
