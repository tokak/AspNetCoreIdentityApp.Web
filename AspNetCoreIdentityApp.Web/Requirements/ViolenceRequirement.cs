using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace AspNetCoreIdentityApp.Web.Requirements
{
    public class ViolenceRequirement : IAuthorizationRequirement
    {
        public int ThresholAge { get; set; }
    }

    public class ViolenceRequirementHandler : AuthorizationHandler<ViolenceRequirement>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, ViolenceRequirement requirement)
        {
            if (!context.User.HasClaim(x => x.Type == "birthdate"))
            {
                context.Fail();
                return Task.CompletedTask;
            }

            Claim? birthDateClaim = context.User.FindFirst("birthdate");
            if (birthDateClaim == null)
            {
                context.Fail();
                return Task.CompletedTask;
            }

            var today = DateTime.Now;
            var birthDate = Convert.ToDateTime(birthDateClaim.Value);
            var age = today.Year - birthDate.Year;

            if (birthDate > today.AddYears(-age))
                age--;

            if (age >= requirement.ThresholAge)
            {
                context.Succeed(requirement);
                return Task.CompletedTask;
            }

            context.Fail();
            return Task.CompletedTask;
        }

    }
}
