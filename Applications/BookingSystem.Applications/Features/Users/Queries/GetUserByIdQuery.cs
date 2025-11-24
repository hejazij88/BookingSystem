using BookingSystem.Applications.DTOs;
using MediatR;

namespace BookingSystem.Applications.Features.Users.Queries;

public record GetUserByIdQuery(string id):IRequest<UserDto>;