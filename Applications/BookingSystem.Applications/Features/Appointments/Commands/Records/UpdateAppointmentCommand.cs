using BookingSystem.Applications.DTOs;
using MediatR;

namespace BookingSystem.Applications.Features.Appointments.Commands.Records;

public record UpdateAppointmentCommand(UpdateAppointmentDto Dto, string UserId) : IRequest<int>;