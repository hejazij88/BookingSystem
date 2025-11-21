using System.ComponentModel.DataAnnotations;

namespace BookingSystem.API.DTOs;

public class CreateAppointmentDto
{
    [Required]
    public int ServiceId { get; set; }

    [Required]
    public DateTime StartTime { get; set; }

    // فعلاً که سیستم لاگین نداریم، شناسه کاربر را دستی می‌گیریم
    // بعداً این را از توکن JWT کاربر استخراج می‌کنیم
    [Required]
    public string UserId { get; set; }

    public string? Note { get; set; }
}