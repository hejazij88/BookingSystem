using BookingSystem.Applications.DTOs;
using BookingSystem.Domain.Models;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace BookingSystem.Applications.Features.Users.Queries;

public class GetUserQueryHandler:IRequestHandler<GetUserByIdQuery,UserDto>
{
    private readonly UserManager<ApplicationUser> _userManager;

    public GetUserQueryHandler(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }

    public async Task<UserDto> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByIdAsync(request.id);
        if (user == null) return null;

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