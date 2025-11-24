using BookingSystem.Applications.DTOs;
using MediatR;

namespace BookingSystem.Applications.Features.Users.Commands;

public record LoginUserCommand(LoginDto Dto) : IRequest<string>;