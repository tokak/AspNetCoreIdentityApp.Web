using AspNetCoreIdentityApp.Core.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AspNetCoreIdentityApp.Core.ViewModels
{
    public class AuthenticationViewModel
    {
        public string Sharedkey { get; set; }
        public string AuthenticationUri { get; set; }
        [Display(Name ="Doğrulama Kodunuz")]
        [Required(ErrorMessage ="Doğrulama kodu gereklidir.")]
        public string VerificationCode { get; set; }
        [Display(Name = "İki adımlı kimlik doğrulama tipi")]
        public TwoFactor TwoFactorType { get; set; }
    }
}
