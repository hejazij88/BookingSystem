using BookingSystem.Applications.DTOs;
using BookingSystem.Domain.Models;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace BookingSystem.Applications.Features.Users.Queries;

public class GetUsersQueryHandler:IRequestHandler<GetUsersQuery,List<UserDto>>
{
    private readonly UserManager<ApplicationUser> _userManager;

    public GetUsersQueryHandler(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }

    public async Task<List<UserDto>> Handle(GetUsersQuery request, CancellationToken cancellationToken)
    {
        var users = _userManager.Users.ToList();
        return users.Select(u => new UserDto
        {
            Id = u.Id,
            UserName = u.UserName,
            Email = u.Email,
            FirstName = u.FirstName,
            LastName = u.LastName
        }).ToList();
    }
}