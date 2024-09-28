using JwtAuthAspNet7WebAPI.Core.DbContext;
using JwtAuthAspNet7WebAPI.Core.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace JwtAuthAspNet7WebAPI.Core.Identity.Stores;

public class ApplicationUserStore(ApplicationDbContext context, IdentityErrorDescriber? describer = null)
    : UserStore<
        ApplicationUser,
        ApplicationRole,
        ApplicationDbContext,
        Guid,
        IdentityUserClaim<Guid>,
        IdentityUserRole<Guid>,
        IdentityUserLogin<Guid>,
        IdentityUserToken<Guid>,
        IdentityRoleClaim<Guid>
        >(context, describer)
{
}
