using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GovUk.Education.ManageCourses.Api.Model;
using GovUk.Education.ManageCourses.Domain.Models;

namespace GovUk.Education.ManageCourses.Ui.ViewModels
{
    public class OrganisationListViewModel
    {
        public IEnumerable<ProviderSummary> ProviderSummaries { get; set; }
    }
}
