using BookingSystem.Applications.DTOs;
using MediatR;

namespace BookingSystem.Applications.Features.Services.Commands;

public record CreateServiceCommand(string Name, int DurationMinutes, decimal Price) : IRequest<ServiceDto>;