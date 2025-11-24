namespace BookingSystem.Client.Models;

public class PaymentResponseModel
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public string? Reference { get; set; }
    public decimal Amount { get; set; }
    public string Currency { get; set; } = "usd";
}

