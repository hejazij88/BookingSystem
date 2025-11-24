using BookingSystem.Applications.DTOs;
using MediatR;

namespace BookingSystem.Applications.Features.Users.Commands;

public record UpdateUserCommand(UpdateUserDto UserDto):IRequest<UserDto>;