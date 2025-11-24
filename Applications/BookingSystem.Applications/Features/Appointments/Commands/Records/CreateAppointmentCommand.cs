using BookingSystem.Applications.DTOs;
using MediatR;

namespace BookingSystem.Applications.Features.Appointments.Commands.Records;

public record CreateAppointmentCommand(CreateAppointmentDto Dto,string UserId) : IRequest<int>;