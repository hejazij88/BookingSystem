using BookingSystem.Applications.DTOs;

namespace BookingSystem.Applications.Payments;

public class FakePaymentGateway : IPaymentGateway
{
    private static readonly Random Random = new();

    public Task<PaymentResultDto> ChargeAsync(decimal amount, string currency, string description, CancellationToken cancellationToken)
    {
        // شبیه‌سازی تماس با درگاه پرداخت
        var reference = $"PAY-{DateTime.UtcNow:yyyyMMddHHmmss}-{Random.Next(1000, 9999)}";

        return Task.FromResult(new PaymentResultDto
        {
            Success = true,
            Status = Domain.Enums.PaymentStatus.Paid,
            Reference = reference,
            Message = "پرداخت با موفقیت انجام شد."
        });
    }
}

