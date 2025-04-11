using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace AspNetCoreIdentityApp.Web.Requirements
{
    public class ExchangeExpireRequirement : IAuthorizationRequirement
    {
    }
    public class ExchangeExpireRequirementHandler : AuthorizationHandler<ExchangeExpireRequirement>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, ExchangeExpireRequirement requirement)
        {
            // Kullanıcının "ExchangeExpireDate" claim'ine sahip olup olmadığını kontrol et
            Claim? exchangeExpireDateClaim = context.User.FindFirst("ExchangeExpireDate");

            if (exchangeExpireDateClaim == null)
            {
                // Eğer ilgili claim yoksa yetkilendirme başarısız
                context.Fail();
                return Task.CompletedTask;
            }

            // Tarih dönüşümü güvenli bir şekilde yapılmalı
            if (!DateTime.TryParse(exchangeExpireDateClaim.Value, out DateTime exchangeExpireDate))
            {
                // Geçersiz tarih formatı varsa yetkilendirme başarısız
                context.Fail();
                return Task.CompletedTask;
            }

            // Eğer tarih geçmişse yetkilendirme başarısız
            if (DateTime.Now > exchangeExpireDate)
            {
                context.Fail();
                return Task.CompletedTask;
            }

            // Eğer tüm kontroller geçilirse yetkilendirme başarılı
            context.Succeed(requirement);
            return Task.CompletedTask;
        }

    }
}
