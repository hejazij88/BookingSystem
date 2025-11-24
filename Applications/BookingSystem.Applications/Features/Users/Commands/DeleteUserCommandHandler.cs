using BookingSystem.Domain.Models;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace BookingSystem.Applications.Features.Users.Commands;

public class DeleteUserCommandHandler:IRequestHandler<DeleteUserCommand>
{
    private readonly UserManager<ApplicationUser> _userManager;

    public DeleteUserCommandHandler(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }

    public async Task<Unit> Handle(DeleteUserCommand request, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByIdAsync(request.id);
        if (user == null) throw new Exception("User not found");

        var result = await _userManager.DeleteAsync(user);
        if (!result.Succeeded)
            throw new Exception(string.Join(", ", result.Errors.Select(e => e.Description)));

        return Unit.Value;
    }
}