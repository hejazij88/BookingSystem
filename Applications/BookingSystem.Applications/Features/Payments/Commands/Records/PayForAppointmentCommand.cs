using MediatR;

namespace BookingSystem.Applications.Features.Payments.Commands.Records;

public record PayForAppointmentCommand(int AppointmentId, string UserId) : IRequest<PaymentResult>;

public record PaymentResult(bool Success, string Message, string? Reference, decimal Amount, string Currency);

