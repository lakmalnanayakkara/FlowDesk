using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using FlowDesk.Core.Common;
using FlowDesk.Core.DTOs.Auth;
using FlowDesk.Core.Entities;
using FlowDesk.Core.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;

namespace FlowDesk.API.Services;

public class AuthService : IAuthService
{
    private readonly UserManager<AppUser> _userManager;
    private readonly IConfiguration _config;

    public AuthService(UserManager<AppUser> userManager, IConfiguration config)
    {
        _userManager = userManager;
        _config = config;
    }

    public async Task<ServiceResult<AuthResponseDto>> RegisterAsync(RegisterDto dto)
    {
        var existingUser = await _userManager.FindByEmailAsync(dto.Email);
        if (existingUser is not null)
            return ServiceResult<AuthResponseDto>.Failure("Email already registered.", 409);

        var user = new AppUser { FullName = dto.FullName, Email = dto.Email, UserName = dto.Email };
        var result = await _userManager.CreateAsync(user, dto.Password);

        if (!result.Succeeded)
        {
            var errors = string.Join(", ", result.Errors.Select(e => e.Description));
            return ServiceResult<AuthResponseDto>.Failure(errors);
        }

        await _userManager.AddToRoleAsync(user, "Member");
        return ServiceResult<AuthResponseDto>.Success(await BuildAuthResponse(user), 201);
    }

    public async Task<ServiceResult<AuthResponseDto>> LoginAsync(LoginDto dto)
    {
        var user = await _userManager.FindByEmailAsync(dto.Email);
        if (user is null || !await _userManager.CheckPasswordAsync(user, dto.Password))
            return ServiceResult<AuthResponseDto>.Failure("Invalid credentials.", 401);

        return ServiceResult<AuthResponseDto>.Success(await BuildAuthResponse(user));
    }

    private async Task<AuthResponseDto> BuildAuthResponse(AppUser user)
    {
        var roles = await _userManager.GetRolesAsync(user);
        var expiry = DateTime.UtcNow.AddDays(7);
        var token = GenerateToken(user, roles, expiry);

        return new AuthResponseDto
        {
            Token = token,
            UserId = user.Id,
            Email = user.Email!,
            FullName = user.FullName,
            Roles = roles,
            ExpiresAt = expiry
        };
    }

    private string GenerateToken(AppUser user, IList<string> roles, DateTime expiry)
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id),
            new(ClaimTypes.Email, user.Email!),
            new(ClaimTypes.Name, user.FullName),
        };
        claims.AddRange(roles.Select(r => new Claim(ClaimTypes.Role, r)));

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _config["Jwt:Issuer"],
            audience: _config["Jwt:Audience"],
            claims: claims,
            expires: expiry,
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}