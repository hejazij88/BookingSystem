using System.ComponentModel.DataAnnotations;

namespace BookingSystem.Applications.DTOs;

public class CreateServiceDto
{
    [Required(ErrorMessage = "نام سرویس الزامی است")]
    public string Name { get; set; }

    [Range(1, 480, ErrorMessage = "زمان سرویس باید بین ۱ تا ۴۸۰ دقیقه باشد")]
    public int DurationMinutes { get; set; }

    [Range(0, double.MaxValue, ErrorMessage = "قیمت نمی‌تواند منفی باشد")]
    public decimal Price { get; set; }
}