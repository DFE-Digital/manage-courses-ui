namespace GovUk.Education.ManageCourses.ApiClient
{
    public class ManageCoursesApiClientBase
    {
        protected readonly IManageCoursesApiClientConfiguration ApiClientConfiguration;

        public ManageCoursesApiClientBase(IManageCoursesApiClientConfiguration apiClientConfiguration)
        {
            ApiClientConfiguration = apiClientConfiguration;
        }
    }
}
