﻿using JwtAuthAspNet7WebAPI.Core.Dtos;
using JwtAuthAspNet7WebAPI.Core.Entities;
using JwtAuthAspNet7WebAPI.Core.Interfaces;
using JwtAuthAspNet7WebAPI.Core.OtherObjects;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace JwtAuthAspNet7WebAPI.Core.Services;

public class AuthService : IAuthService
{
    #region Constructor & DI
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<ApplicationRole> _roleManager;
    private readonly IConfiguration _configuration;

    public AuthService(UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager, IConfiguration configuration)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _configuration = configuration;
    }
    #endregion

    public async Task<AuthServiceResponseDto> LoginAsync(LoginDto loginDto)
    {
        var user = await _userManager.FindByNameAsync(loginDto.UserName);

        if (user is null)
            return new AuthServiceResponseDto(false, "Invalid Credentials");

        var isPasswordCorrect = await _userManager.CheckPasswordAsync(user, loginDto.Password);

        if (!isPasswordCorrect)
            return new AuthServiceResponseDto(false, "Invalid Credentials");

        var userRoles = await _userManager.GetRolesAsync(user);

        var authClaims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, user.UserName),
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim("JWTID", Guid.NewGuid().ToString()),
            new Claim("FirstName", user.FirstName),
            new Claim("LastName", user.LastName),
        };

        foreach (var userRole in userRoles)
        {
            authClaims.Add(new Claim(ClaimTypes.Role, userRole));
        }

        var token = GenerateNewJsonWebToken(authClaims);

        return new AuthServiceResponseDto(true, token);
    }

    public async Task<AuthServiceResponseDto> MakeAdminAsync(UpdatePermissionDto updatePermissionDto)
    {
        var user = await _userManager.FindByNameAsync(updatePermissionDto.UserName);

        if (user is null)
            return new AuthServiceResponseDto(false, "Invalid User name!!!!!!!!");

        await _userManager.AddToRoleAsync(user, StaticUserRoles.ADMIN);

        return new AuthServiceResponseDto(true, "User is now an ADMIN");
    }

    public async Task<AuthServiceResponseDto> MakeOwnerAsync(UpdatePermissionDto updatePermissionDto)
    {
        var user = await _userManager.FindByNameAsync(updatePermissionDto.UserName);

        if (user is null)
            return new AuthServiceResponseDto(false, "Invalid User name!!!!!!!!");

        await _userManager.AddToRoleAsync(user, StaticUserRoles.OWNER);

        return new AuthServiceResponseDto(true, "User is now an OWNER");
    }

    public async Task<AuthServiceResponseDto> RegisterAsync(RegisterDto registerDto)
    {
        var isExistsUser = await _userManager.FindByNameAsync(registerDto.UserName);

        if (isExistsUser != null)
            return new AuthServiceResponseDto(false, "UserName Already Exists");

        ApplicationUser newUser = new ApplicationUser()
        {
            FirstName = registerDto.FirstName,
            LastName = registerDto.LastName,
            Mobile = registerDto.Mobile,
            Email = registerDto.Email,
            UserName = registerDto.UserName,
            SecurityStamp = Guid.NewGuid().ToString(),
        };

        var createUserResult = await _userManager.CreateAsync(newUser, registerDto.Password);

        if (!createUserResult.Succeeded)
        {
            var errorString = "User Creation Failed Beacause: ";
            foreach (var error in createUserResult.Errors)
            {
                errorString += " # " + error.Description;
            }
            return new AuthServiceResponseDto(false, errorString);
        }

        // Add a Default USER Role to all users
        await _userManager.AddToRoleAsync(newUser, StaticUserRoles.USER);

        return new AuthServiceResponseDto(true, "User Created Successfully");
    }

    public async Task<AuthServiceResponseDto> SeedRolesAsync()
    {
        bool isOwnerRoleExists = await _roleManager.RoleExistsAsync(StaticUserRoles.OWNER);
        bool isAdminRoleExists = await _roleManager.RoleExistsAsync(StaticUserRoles.ADMIN);
        bool isUserRoleExists = await _roleManager.RoleExistsAsync(StaticUserRoles.USER);

        if (isOwnerRoleExists && isAdminRoleExists && isUserRoleExists)
            return new AuthServiceResponseDto(false, "Roles Seeding is Already Done");

        await _roleManager.CreateAsync(new ApplicationRole(StaticUserRoles.OWNER, StaticUserRoles.OWNER_TITLE));
        await _roleManager.CreateAsync(new ApplicationRole(StaticUserRoles.ADMIN, StaticUserRoles.ADMIN_TITLE));
        await _roleManager.CreateAsync(new ApplicationRole(StaticUserRoles.USER, StaticUserRoles.USER_TITLE));

        return new AuthServiceResponseDto(true, "Role Seeding Done Successfully");
    }

    private string GenerateNewJsonWebToken(List<Claim> claims)
    {
        var authSecret = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"]));

        var tokenObject = new JwtSecurityToken(
                issuer: _configuration["JWT:ValidIssuer"],
                audience: _configuration["JWT:ValidAudience"],
                expires: DateTime.Now.AddHours(1),
                claims: claims,
                signingCredentials: new SigningCredentials(authSecret, SecurityAlgorithms.HmacSha256)
            );

        string token = new JwtSecurityTokenHandler().WriteToken(tokenObject);

        return token;
    }
}
