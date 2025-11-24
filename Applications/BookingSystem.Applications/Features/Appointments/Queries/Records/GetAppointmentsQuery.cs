using BookingSystem.Applications.DTOs;
using MediatR;

namespace BookingSystem.Applications.Features.Appointments.Queries.Records;

public record GetAppointmentsQuery():IRequest<List<AppointmentDto>>;