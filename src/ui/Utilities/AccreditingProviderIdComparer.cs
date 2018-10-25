using System;
using System.Collections.Generic;
using GovUk.Education.ManageCourses.ApiClient;

namespace GovUk.Education.ManageCourses.Ui.Utilities
{
    ///<summary>
    ///  Considers two providers equal if the ID (AKA the UCAS Code) are equivalent.
    ///</summary>
    public class AccreditingProviderIdComparer : IEqualityComparer<Course>
    {
        public bool Equals(Course x, Course y)
        {
            return string.Equals(x.AccreditingInstitution?.InstCode, y.AccreditingInstitution?.InstCode, StringComparison.InvariantCultureIgnoreCase);
        }

        public int GetHashCode(Course obj)
        {
            return (obj.AccreditingInstitution?.InstCode ?? "").ToLowerInvariant().GetHashCode();
        }
    }
}
