
using Microsoft.Extensions.Configuration;

namespace GovUk.Education.ManageCourses.Ui
{
    public interface IFeatureFlags 
    {
        bool ShowOrgEnrichment { get; }
    }

    public class FeatureFlags : IFeatureFlags
    {
        private readonly IConfigurationSection _config;

        private const string FEATURE_ORG_ENRICHMENT = "FEATURE_ORG_ENRICHEMENT";

        public FeatureFlags(IConfigurationSection config)
        {
            _config = config;
        } 

        public bool ShowOrgEnrichment => ShouldShow(FEATURE_ORG_ENRICHMENT);

        private bool ShouldShow(string key) => _config.GetValue(key, false);
    }
}