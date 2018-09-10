
using Microsoft.Extensions.Configuration;

namespace GovUk.Education.ManageCourses.Ui
{
    public class FeatureFlags : IFeatureFlags
    {
        private readonly IConfigurationSection _config;

        private const string FEATURE_COURSE_LIVEVIEW = "FEATURE_COURSE_LIVEVIEW";

        public FeatureFlags(IConfigurationSection config)
        {
            _config = config;
        }

        public bool ShowCourseLiveView => ShouldShow(FEATURE_COURSE_LIVEVIEW);

        private bool ShouldShow(string key) => _config.GetValue(key, false);
    }
}