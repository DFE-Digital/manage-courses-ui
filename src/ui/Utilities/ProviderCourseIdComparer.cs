using System;
using System.Collections.Generic;
using GovUk.Education.ManageCourses.ApiClient;

namespace GovUk.Education.ManageCourses.Ui.Utilities
{
    ///<summary>
    ///  Considers two providers equal if the ID (AKA the UCAS Code) are equivalent.
    ///</summary>
    public class ProviderCourseIdComparer : IEqualityComparer<ProviderCourse>
    {
        public bool Equals(ProviderCourse x, ProviderCourse y)
        {
            return string.Equals(x.AccreditingProviderId, y.AccreditingProviderId, StringComparison.InvariantCultureIgnoreCase);
        }

        public int GetHashCode(ProviderCourse obj)
        {
            return obj.AccreditingProviderId.ToLowerInvariant().GetHashCode();
        }
    }
}