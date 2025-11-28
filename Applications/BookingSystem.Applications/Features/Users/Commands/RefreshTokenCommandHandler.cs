using BookingSystem.Applications.DTOs.ResponseDTOs;
using BookingSystem.Applications.JWT;
using BookingSystem.Domain.Models;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace BookingSystem.Applications.Features.Users.Commands;


public class RefreshTokenCommand : IRequest<LoginResponseDto>
{
    public string Token { get; }
    public RefreshTokenCommand(string token)
    {
        Token = token;
    }
}

public class RefreshTokenCommandHandler : IRequestHandler<RefreshTokenCommand, LoginResponseDto>
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;

    public RefreshTokenCommandHandler(UserManager<ApplicationUser> userManager, IJwtTokenGenerator jwtTokenGenerator)
    {
        _userManager = userManager;
        _jwtTokenGenerator = jwtTokenGenerator;
    }

    public async Task<LoginResponseDto> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
    {
        // پیدا کردن کاربر با Refresh Token معتبر
        var user = await _userManager.Users
            .Include(u => u.RefreshTokens)
            .FirstOrDefaultAsync(u => u.RefreshTokens.Any(t => t.Token == request.Token));

        if (user == null) throw new Exception("Invalid refresh token");

        var refreshToken = user.RefreshTokens.First(t => t.Token == request.Token);

        if (refreshToken.IsExpired || refreshToken.IsRevoked)
            throw new Exception("Refresh token is expired or revoked");

        // لغو توکن قبلی
        refreshToken.Revoked = DateTime.UtcNow;

        // ایجاد توکن جدید
        var newRefreshToken = _jwtTokenGenerator.GenerateRefreshToken();
        newRefreshToken.ApplicationUserId = user.Id;

        user.RefreshTokens.Add(newRefreshToken);

        // ایجاد Access Token جدید
        var roles = await _userManager.GetRolesAsync(user);
        user.Roles = roles;
        var accessToken = _jwtTokenGenerator.GenerateJwtToken(user);

        await _userManager.UpdateAsync(user);

        return new LoginResponseDto
        (
            
            accessToken,
            refreshToken.Token,
            refreshToken.Expires
        );
    }
}