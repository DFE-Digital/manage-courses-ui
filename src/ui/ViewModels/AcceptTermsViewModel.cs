using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using GovUk.Education.ManageCourses.ApiClient;

namespace GovUk.Education.ManageCourses.Ui.ViewModels
{
  public class AcceptTermsViewModel
  {
    [Required(ErrorMessage = "Please agree to the terms and conditions before continuing")]
    public bool TermsAccepted { get; set; }
  }
}
