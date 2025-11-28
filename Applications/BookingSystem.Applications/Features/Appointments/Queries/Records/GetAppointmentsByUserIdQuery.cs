using BookingSystem.Applications.DTOs;
using MediatR;

namespace BookingSystem.Applications.Features.Appointments.Queries.Records;

public record GetAppointmentsByUserIdQuery(string Id,bool isActive) : IRequest<List<AppointmentDto>>;