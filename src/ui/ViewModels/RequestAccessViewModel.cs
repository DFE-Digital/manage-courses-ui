using System.ComponentModel.DataAnnotations;

namespace GovUk.Education.ManageCourses.Ui.ViewModels
{
    public class RequestAccessViewModel
    {
        [Required(ErrorMessage = "Enter your first name")]
        [Display(Name = "First name")]
        public string FirstName { get; set; }
        [Required(ErrorMessage = "Enter your last name")]
        [Display(Name = "Last name")]
        public string LastName { get; set; }
        [Required(ErrorMessage = "Enter your email address")]
        [Display(Name = "Email address")]
        public string EmailAddress { get; set; }
        [Required(ErrorMessage = "Enter their organisation")]
        [Display(Name = "Their organisation")]
        public string Organisation { get; set; }
        [Required(ErrorMessage = "Why do they need access?")]
        [Display(Name = "Reason they need access")]
        public string Reason { get; set; }
    }
}
