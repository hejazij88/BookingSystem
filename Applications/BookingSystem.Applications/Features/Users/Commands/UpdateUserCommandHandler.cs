using BookingSystem.Applications.DTOs;
using BookingSystem.Domain.Models;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace BookingSystem.Applications.Features.Users.Commands;

public class UpdateUserCommandHandler:IRequestHandler<UpdateUserCommand,UserDto>
{
    private readonly UserManager<ApplicationUser> _userManager;

    public UpdateUserCommandHandler(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }


    public async Task<UserDto> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByIdAsync(request.UserDto.Id);
        if (user == null) return null;

        user.Email = request.UserDto.Email;
        user.FirstName = request.UserDto.FirstName;
        user.LastName = request.UserDto.LastName;
        

        var result = await _userManager.UpdateAsync(user);
        if (!result.Succeeded)
            throw new Exception(string.Join(", ", result.Errors.Select(e => e.Description)));

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