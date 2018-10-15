namespace GovUk.Education.ManageCourses.Ui
{
    public interface IFeatureFlags
    {
        bool ShowCourseLiveView { get; }

        /// <summary>
        /// Just for unit testing.
        /// </summary>
        /// <value></value>
        bool TestFlagEnabled { get; }
    }
}
