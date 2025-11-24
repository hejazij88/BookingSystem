using BookingSystem.Domain.Models;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace BookingSystem.Applications.Features.Users.Commands;

public class LoginUserCommandHandler : IRequestHandler<LoginUserCommand, string>
{
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly UserManager<ApplicationUser> _userManager;

    public LoginUserCommandHandler(SignInManager<ApplicationUser> signInManager,
        UserManager<ApplicationUser> userManager)
    {
        _signInManager = signInManager;
        _userManager = userManager;
    }

    public async Task<string> Handle(LoginUserCommand request, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByNameAsync(request.Dto.Email);
        if (user == null) throw new Exception("Invalid username or password");

        var result = await _signInManager.CheckPasswordSignInAsync(user, request.Dto.Password, false);
        if (!result.Succeeded) throw new Exception("Invalid username or password");

        // ایجاد JWT
        var token = _jwtTokenGenerator.GenerateToken(user);
        return token;
    }
}