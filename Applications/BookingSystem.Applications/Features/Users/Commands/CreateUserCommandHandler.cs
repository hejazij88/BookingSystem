using BookingSystem.Applications.DTOs;
using BookingSystem.Domain.Constants;
using BookingSystem.Domain.Models;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace BookingSystem.Applications.Features.Users.Commands;

public class CreateUserCommandHandler:IRequestHandler<CreateUserCommand,UserDto>
{
    private readonly UserManager<ApplicationUser> _userManager;

    public CreateUserCommandHandler(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }


    public async Task<UserDto> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        var user = new ApplicationUser
        {
            UserName = request.RegisterDto.Email,
            Email = request.RegisterDto.Email,
            FirstName = request.RegisterDto.FirstName,
            LastName = request.RegisterDto.LastName
        };



        var result = await _userManager.CreateAsync(user, request.RegisterDto.Password);
        if (!result.Succeeded)
            throw new Exception(string.Join(", ", result.Errors.Select(e => e.Description)));

        await _userManager.AddToRoleAsync(user, ApplicationRoles.Client);


        return new UserDto
        {
            Id = user.Id,
            UserName = user.UserName,
            Email = user.Email,
            FirstName = user.FirstName,
            LastName = user.LastName
        };
    }
}