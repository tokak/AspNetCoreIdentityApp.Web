using System.ComponentModel.DataAnnotations;

namespace AspNetCoreIdentityApp.Web.ViewModels
{
    public class ResetPassordViewModel
    {
        [EmailAddress(ErrorMessage = "Email formatı yanlış")]
        [Required(ErrorMessage = "Email alanı boş bırakılamaz")]
        [Display(Name = "Email :")]
        public string Email { get; set; }
    }
}
