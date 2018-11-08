using System;

namespace GovUk.Education.ManageCourses.Ui.Services
{
    public interface ISearchAndCompareUrlService
    {
        Uri GetCoursePageUri(string providerCode, string courseCode);
    }
}