using BookingSystem.Applications.DTOs;

namespace BookingSystem.Applications.Payments;

public interface IPaymentGateway
{
    Task<PaymentResultDto> ChargeAsync(decimal amount, string currency, string description, CancellationToken cancellationToken);
}

