﻿using AspNetCoreIdentityApp.Core.Models;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace AspNetCoreIdentityApp.Core.ViewModels
{
    public class UserEditViewModel
    {
        [Required(ErrorMessage = "Kullanıcı adı boş bırakılamaz")]
        [Display(Name = "Kullanıcı Adı :")]
        public string UserName { get; set; }
        [EmailAddress(ErrorMessage = "Email formatı yanlış")]
        [Display(Name = "Email :")]
        public string Email { get; set; }
        [Required(ErrorMessage = "Telefon alanı boş bırakılamaz")]
        [Display(Name = "Telefon :")]
        public string Phone { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Doğum Tarihi :")]
        public DateTime? BirthDate { get; set; }

        [Display(Name = "Şehir :")]
        public string? City { get; set; }

        [Display(Name = "Profil resmi :")]
        public IFormFile? Picture { get; set; }


        [Display(Name = "Cinsiyet :")]
        public Gender? Gender { get; set; }



    }
}
