using AspNetCoreIdentityApp.Repository.Models;
using Microsoft.AspNetCore.Identity;

namespace AspNetCoreIdentityApp.Web.CustomValidations
{
    public  class UserValidator : IUserValidator<AppUser>
    {
        public Task<IdentityResult> ValidateAsync(UserManager<AppUser> manager, AppUser user)
        {
            var errors = new List<IdentityError>();
            // Kullanıcı adının ilk karakterini alıyoruz ve bunun bir rakam olup olmadığını kontrol ediyoruz.
            var isDigit = int.TryParse(user!.UserName[0].ToString(), out _);
            // 'int.TryParse', ilk karakterin bir tamsayıya dönüştürülüp dönüştürülemeyeceğini kontrol eder.
            // Eğer dönüşüm başarılıysa 'isNumeric' true olur, aksi takdirde false olur.
            // 'out _' kısmı, dönüşüm sonucunu almadığımızı ve sadece başarılı olup olmadığını kontrol ettiğimizi gösterir.

            if (isDigit)
            {
                errors.Add(new IdentityError() { Code = "UserNameContainFirstLetterDigit", Description = "Kullanıcı Adının ilk karekteri sayısal bir ifade içeremez" });
            }

            if (errors.Any())
            {
                return Task.FromResult(IdentityResult.Failed(errors.ToArray()));
            }
            return Task.FromResult(IdentityResult.Success);
        }
    }
}
