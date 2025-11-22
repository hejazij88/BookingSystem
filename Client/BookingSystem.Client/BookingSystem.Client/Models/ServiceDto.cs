namespace BookingSystem.Client.Models;

public class ServiceDto
{
    public int Id { get; set; }
    public string Name { get; set; }
    public int DurationMinutes { get; set; }
    public decimal Price { get; set; }
}