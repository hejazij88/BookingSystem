using BookingSystem.Applications.Features.Payments.Commands.Records;
using BookingSystem.Applications.Payments;
using BookingSystem.Domain.Enums;
using BookingSystem.Infrastructure.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BookingSystem.Applications.Features.Payments.Commands;

public class PayForAppointmentCommandHandler : IRequestHandler<PayForAppointmentCommand, PaymentResult>
{
    private readonly BookingDbContext _context;
    private readonly IPaymentGateway _paymentGateway;

    public PayForAppointmentCommandHandler(BookingDbContext context, IPaymentGateway paymentGateway)
    {
        _context = context;
        _paymentGateway = paymentGateway;
    }

    public async Task<PaymentResult> Handle(PayForAppointmentCommand request, CancellationToken cancellationToken)
    {
        var appointment = await _context.Appointments
            .Include(a => a.Service)
            .FirstOrDefaultAsync(a => a.Id == request.AppointmentId && a.UserId == request.UserId, cancellationToken);

        if (appointment == null)
            throw new Exception("رزرو یافت نشد یا متعلق به شما نیست.");

        if (appointment.PaymentStatus == PaymentStatus.Paid)
        {
            return new PaymentResult(true, "این رزرو قبلاً پرداخت شده است.", appointment.PaymentReference, appointment.AmountPaid, appointment.Currency);
        }

        if (appointment.Service == null)
            throw new Exception("اطلاعات سرویس برای پرداخت در دسترس نیست.");

        var amount = appointment.Service.Price;
        var description = $"Payment for appointment #{appointment.Id} - {appointment.Service.Name}";

        var gatewayResult = await _paymentGateway.ChargeAsync(amount, appointment.Currency, description, cancellationToken);

        appointment.PaymentStatus = gatewayResult.Success ? PaymentStatus.Paid : PaymentStatus.Failed;
        appointment.PaymentReference = gatewayResult.Reference;
        appointment.AmountPaid = gatewayResult.Success ? amount : 0;

        await _context.SaveChangesAsync(cancellationToken);

        return new PaymentResult(gatewayResult.Success, gatewayResult.Message, gatewayResult.Reference, appointment.AmountPaid, appointment.Currency);
    }
}

