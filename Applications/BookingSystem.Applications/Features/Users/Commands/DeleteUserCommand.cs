using MediatR;

namespace BookingSystem.Applications.Features.Users.Commands;

public record DeleteUserCommand(string id):IRequest;