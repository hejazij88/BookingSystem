using BookingSystem.Applications.DTOs;
using BookingSystem.Applications.DTOs.ResponseDTOs;
using MediatR;

namespace BookingSystem.Applications.Features.Users.Commands;

public record LoginUserCommand(LoginDto Dto) : IRequest<LoginResponseDto>;