using System;

namespace GovUk.Education.ManageCourses.Ui.Services
{
    public class SearchAndCompareUrlService : ISearchAndCompareUrlService
    {
        private string searchAndComparBaseUrl;

        public SearchAndCompareUrlService(string searchAndComparBaseUrl)
        {
            this.searchAndComparBaseUrl = searchAndComparBaseUrl;
        }

        public Uri GetCoursePageUri(string providerCode, string courseCode)
        {
            return new Uri($"{searchAndComparBaseUrl}/course/{Uri.EscapeDataString(providerCode)}/{Uri.EscapeDataString(courseCode)}");           
        }
    }
}
