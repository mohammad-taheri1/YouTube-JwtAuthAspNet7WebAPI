using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace JwtAuthAspNet7WebAPI.Core.Entities;

public class ApplicationRole : IdentityRole<Guid>
{
    #region Constructor
    private ApplicationRole() { }

    public ApplicationRole(string role, string faTitle) : base(role)
    {
        FaTitle = faTitle;
    }
    #endregion

    #region Properties

    [MaxLength(50)]
    public string FaTitle { get; set; }

    #endregion
}
