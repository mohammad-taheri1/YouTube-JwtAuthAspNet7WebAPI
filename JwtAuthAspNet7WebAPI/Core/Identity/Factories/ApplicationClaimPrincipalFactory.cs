using JwtAuthAspNet7WebAPI.Core.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using System.Security.Claims;

namespace JwtAuthAspNet7WebAPI.Core.Identity.Factories;

public class ApplicationClaimPrincipalFactory : UserClaimsPrincipalFactory<ApplicationUser, ApplicationRole>
{
    #region Constructor
    public ApplicationClaimPrincipalFactory(
        UserManager<ApplicationUser> userManager,
        RoleManager<ApplicationRole> roleManager,
        IOptions<IdentityOptions> options) : base(userManager, roleManager, options)
    {
    }
    #endregion

    public override Task<ClaimsPrincipal> CreateAsync(ApplicationUser user)
    {
        return base.CreateAsync(user);
    }

    protected override async Task<ClaimsIdentity> GenerateClaimsAsync(ApplicationUser user)
    {
        var claims = await base.GenerateClaimsAsync(user);

        claims.AddClaim(new Claim("App", "JwtAuthAspNet7WebAPI"));

        return claims;
    }
}
