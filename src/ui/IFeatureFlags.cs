namespace GovUk.Education.ManageCourses.Ui
{
    public interface IFeatureFlags
    {
        bool ShowCoursePreview { get; }
        bool ShowCoursePublish { get; }
        bool ShowCourseLiveView { get; }
    }
}
