using AspNetCoreIdentityApp.Repository.Models;
using Microsoft.AspNetCore.Identity;

namespace AspNetCoreIdentityApp.Web.CustomValidations
{
    public class PasswordValidator : IPasswordValidator<AppUser>
    {
        // Asenkron bir görev olarak kimlik doğrulama işlemi yapan metot. 
        // Bu metot, 'user' adlı kullanıcı için bir şifreyi doğrulamak amacıyla kullanılıyor.
        public Task<IdentityResult> ValidateAsync(UserManager<AppUser> manager, AppUser user, string password)
        {
            // Hata mesajlarını saklamak için bir liste oluşturuluyor.
            var errors = new List<IdentityError>();

            // Şifrenin kullanıcı adıyla aynı olup olmadığını kontrol ediyor.
            // Eğer şifre, kullanıcı adı içeriyorsa hata ekleniyor.
            if (password!.ToLower().Contains(user.UserName.ToLower()))
            {
                errors.Add(new IdentityError() { Code = "PasswordContainUserName", Description = "Şifre alanı kullanıcı adı içeremez." });
            }

            // Şifrenin başında '1234' olup olmadığını kontrol ediyor.
            // Eğer şifre bu ardışık sayıları içeriyorsa hata ekleniyor.
            if (password!.ToLower().StartsWith("1234"))
            {
                errors.Add(new IdentityError() { Code = "PasswordContain1234", Description = "Şifre alanı ardışık sayı içeremez." });
            }

            // Eğer herhangi bir hata varsa, sonuç olarak başarılı bir kimlik doğrulama sonucu döndürülüyor.
            // (Ancak burada hatalı bir durum var, çünkü 'errors' listesinde hata varsa
            // sonuç 'IdentityResult.Success' değil, 'IdentityResult.Failed' olmalı.)
            if (errors.Any())
            {
                return Task.FromResult(IdentityResult.Failed(errors.ToArray())); 
            }
            return Task.FromResult(IdentityResult.Success);
        }

    }
}
