using Microsoft.Extensions.Configuration;

namespace GovUk.Education.ManageCourses.Ui
{
    public interface IFeatureFlags
    {
        bool ShowCoursePreview { get; }
        bool ShowCoursePublish { get; }
    }

    public class FeatureFlags : IFeatureFlags
    {
        private readonly IConfigurationSection _config;

        private const string FEATURE_COURSE_PREVIEW = "FEATURE_COURSE_PREVIEW";
        private const string FEATURE_COURSE_PUBLISH = "FEATURE_COURSE_PUBLISH";

        public FeatureFlags(IConfigurationSection config)
        {
            _config = config;
        }

        public bool ShowCoursePreview => ShouldShow(FEATURE_COURSE_PREVIEW);

        public bool ShowCoursePublish => ShouldShow(FEATURE_COURSE_PUBLISH);

        private bool ShouldShow(string key) => _config.GetValue(key, false);
    }
}
