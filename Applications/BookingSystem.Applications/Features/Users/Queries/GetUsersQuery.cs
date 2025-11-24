using BookingSystem.Applications.DTOs;
using MediatR;

namespace BookingSystem.Applications.Features.Users.Queries;

public record GetUsersQuery : IRequest<List<UserDto>>;
