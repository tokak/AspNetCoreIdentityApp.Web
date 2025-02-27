using System.ComponentModel.DataAnnotations;

namespace AspNetCoreIdentityApp.Web.ViewModels
{
    public class PasswordChangeViewModel
    {
        [Required(ErrorMessage = "Eski şifre alanı boş bırakılamaz")]
        [Display(Name = "Eski Şifre :")]
        public string PasswordOld { get; set; }

        [Required(ErrorMessage = "Yeni şifre alanı boş bırakılamaz")]
        [Display(Name = "Yeni Şifre :")]
        [MinLength(6,ErrorMessage ="Şifreniz en az 6 kareker olabilir.")]
        public string PasswordNew { get; set; }

        [Compare(nameof(PasswordNew), ErrorMessage = "Şifreler uyuşmuyor tekrar deneyiniz.")]
        [Display(Name = "Yeni Şifre Tekrar :")]
        [MinLength(6, ErrorMessage = "Şifreniz en az 6 kareker olabilir.")]
        public string PasswordConfirm { get; set; }
    }
}
