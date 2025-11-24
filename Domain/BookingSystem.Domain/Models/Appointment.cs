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

    // شناسه کاربر (فعلا به صورت String نگه می‌داریم تا بعدا Identity را اضافه کنیم)
    public required string UserId { get; set; }

    // یادداشت کاربر
    public string? Note { get; set; }
}