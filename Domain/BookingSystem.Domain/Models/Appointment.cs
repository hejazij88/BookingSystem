using BookingSystem.Domain.Enums;

namespace BookingSystem.Domain.Models;

public class Appointment:BaseEntity
{
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }

    // وضعیت رزرو
    public BookingStatus Status { get; set; } = BookingStatus.Pending;

    // کلید خارجی برای سرویس
    public int ServiceId { get; set; }
    public Service? Service { get; set; }

    public required string UserId { get; set; }
    public ApplicationUser? User { get; set; }

    // یادداشت کاربر
    public string? Note { get; set; }

    // پرداخت
    public PaymentStatus PaymentStatus { get; set; } = PaymentStatus.Pending;
    public string? PaymentReference { get; set; }
    public decimal AmountPaid { get; set; }
    public string Currency { get; set; } = "usd";
}