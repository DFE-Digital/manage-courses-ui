using System;
using System.Collections.Generic;
using GovUk.Education.ManageCourses.Api.Model;
using GovUk.Education.ManageCourses.Domain.Models;

namespace GovUk.Education.ManageCourses.Ui.Utilities
{
    ///<summary>
    ///  Considers two providers equal if the ID (AKA the UCAS Code) are equivalent.
    ///</summary>
    public class AccreditingProviderCodeComparer : IEqualityComparer<Course>
    {
        public bool Equals(Course x, Course y)
        {
            return string.Equals(x.AccreditingProvider?.ProviderCode, y.AccreditingProvider?.ProviderCode, StringComparison.InvariantCultureIgnoreCase);
        }

        public int GetHashCode(Course obj)
        {
            return (obj.AccreditingProvider?.ProviderCode ?? "").ToLowerInvariant().GetHashCode();
        }
    }
}
