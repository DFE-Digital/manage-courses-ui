using System.ComponentModel.DataAnnotations;

namespace GovUk.Education.ManageCourses.Ui.ViewModels
{
    public class RequestAccessViewModel : TabbedViewModel
    {
        [Required(ErrorMessage = "Enter your first name")]
        public string FirstName { get; set; }
        [Required(ErrorMessage = "Enter your last name")]
        public string LastName { get; set; }
        [Required(ErrorMessage = "Enter your email address")]
        public string EmailAddress { get; set; }
        [Required(ErrorMessage = "Enter their organisation")]
        public string Organisation { get; set; }
        [Required(ErrorMessage = "Why do they need access?")]
        public string Reason { get; set; }
    }
}
