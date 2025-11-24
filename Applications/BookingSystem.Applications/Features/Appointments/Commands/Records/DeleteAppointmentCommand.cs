using MediatR;

namespace BookingSystem.Applications.Features.Appointments.Commands.Records;

public record DeleteAppointmentCommand(int Id) : IRequest;