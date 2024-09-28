using JwtAuthAspNet7WebAPI.Core.DbContext;
using JwtAuthAspNet7WebAPI.Core.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

namespace JwtAuthAspNet7WebAPI.Core.Identity.Stores;

public class ApplicationRoleStore(ApplicationDbContext context, IdentityErrorDescriber? describer = null)
    : RoleStore<ApplicationRole, ApplicationDbContext, Guid>(context, describer)
{
}
