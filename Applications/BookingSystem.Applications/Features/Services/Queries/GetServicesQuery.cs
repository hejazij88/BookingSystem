using BookingSystem.Applications.DTOs;
using MediatR;

namespace BookingSystem.Applications.Features.Services.Queries;

public record GetServicesQuery : IRequest<List<ServiceDto>>;