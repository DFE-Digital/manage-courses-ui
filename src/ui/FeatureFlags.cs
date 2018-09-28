
using Microsoft.Extensions.Configuration;

namespace GovUk.Education.ManageCourses.Ui
{
    public class FeatureFlags : IFeatureFlags
    {
        private readonly IConfigurationSection _config;

        private const string FEATURE_COURSE_LIVEVIEW = "FEATURE_COURSE_LIVEVIEW";
        private const string FEATURE_SEND_TO_SEARCH_AND_COMPARE = "FEATURE_SEND_TO_SEARCH_AND_COMPARE";

        public FeatureFlags(IConfigurationSection config)
        {
            _config = config;
        }

        public bool ShowCourseLiveView => ShouldShow(FEATURE_COURSE_LIVEVIEW);
        public bool SendToSearchAndCompare => ShouldShow(FEATURE_SEND_TO_SEARCH_AND_COMPARE);

        private bool ShouldShow(string key) => _config.GetValue(key, false);

    }
}