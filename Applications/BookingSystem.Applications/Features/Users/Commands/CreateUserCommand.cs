using BookingSystem.Applications.DTOs;
using MediatR;

namespace BookingSystem.Applications.Features.Users.Commands;

public record CreateUserCommand(RegisterDto RegisterDto):IRequest<UserDto>;