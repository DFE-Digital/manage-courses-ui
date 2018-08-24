using System;

namespace GovUk.Education.ManageCourses.Ui.Services
{
    public class SearchAndCompareUrlService : ISearchAndCompareUrlService
    {
        private object searchAndComparBaseUrl;

        public SearchAndCompareUrlService(String searchAndComparBaseUrl)
        {
            this.searchAndComparBaseUrl = searchAndComparBaseUrl;
        }

        public Uri GetCoursePageUri(string instCode, string courseCode)
        {
            return new Uri($"{searchAndComparBaseUrl}/course/{Uri.EscapeDataString(instCode)}/{Uri.EscapeDataString(courseCode)}");           
        }
    }
}
