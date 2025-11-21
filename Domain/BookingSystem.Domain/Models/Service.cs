namespace BookingSystem.Domain.Models;

public class Service:BaseEntity
{
    public required string Name { get; set; } // نام خدمت: کارواش، اصلاح مو
    public int DurationMinutes { get; set; } // طول مدت انجام کار (مثلا 30 دقیقه)
    public decimal Price { get; set; } // قیمت
    public bool IsActive { get; set; } = true; // فعال/غیرفعال بودن خدمت

    // ارتباط با رزروها (یک خدمت می‌تواند چندین رزرو داشته باشد)
    //public ICollection<Appointment>? Appointments { get; set; }
}