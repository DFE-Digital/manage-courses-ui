using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using GovUk.Education.ManageCourses.Api.Model;
using GovUk.Education.ManageCourses.Domain.Models;

namespace GovUk.Education.ManageCourses.Ui.ViewModels
{
  public class AcceptTermsViewModel
  {
    [Range(typeof(bool), "true", "true", ErrorMessage = "Please agree to the terms and conditions before continuing")]
    public bool TermsAccepted { get; set; }
  }
}
