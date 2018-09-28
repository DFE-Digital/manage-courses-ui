namespace GovUk.Education.ManageCourses.Ui
{
    public interface IFeatureFlags
    {
        bool ShowCourseLiveView { get; }
        bool SendToSearchAndCompare { get; }
    }
}
