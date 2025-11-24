using BookingSystem.Domain.Enums;

namespace BookingSystem.Applications.DTOs;

public class PaymentResultDto
{
    public bool Success { get; set; }
    public PaymentStatus Status { get; set; }
    public string Message { get; set; } = string.Empty;
    public string? Reference { get; set; }
}

