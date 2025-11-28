using BookingSystem.Applications.DTOs.ResponseDTOs;
using BookingSystem.Applications.JWT;
using BookingSystem.Domain.Models;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;

namespace BookingSystem.Applications.Features.Users.Commands;

public class LoginUserCommandHandler : IRequestHandler<LoginUserCommand, LoginResponseDto>
{
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;


    public LoginUserCommandHandler(SignInManager<ApplicationUser> signInManager,
        UserManager<ApplicationUser> userManager, IJwtTokenGenerator jwtTokenGenerator)
    {
        _signInManager = signInManager;
        _userManager = userManager;
        _jwtTokenGenerator = jwtTokenGenerator;
    }

    public async Task<LoginResponseDto> Handle(LoginUserCommand request, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByNameAsync(request.Dto.Email);
        if (user == null) throw new Exception("Invalid username or password");

        var result = await _signInManager.CheckPasswordSignInAsync(user, request.Dto.Password, false);
        if (!result.Succeeded) throw new Exception("Invalid username or password");


        var roles = await _userManager.GetRolesAsync(user);
        user.Roles = roles;

        // ایجاد JWT
        var token = _jwtTokenGenerator.GenerateJwtToken(user);
        var refreshToken = _jwtTokenGenerator.GenerateRefreshToken();
        refreshToken.ApplicationUserId = user.Id;

        user.RefreshTokens.Add(refreshToken);

        await _userManager.UpdateAsync(user);


        return new LoginResponseDto
        {
            AccessToken = token,
            RefreshToken = refreshToken.Token,
            AccessTokenExpiration = refreshToken.Expires
        };
    }
}