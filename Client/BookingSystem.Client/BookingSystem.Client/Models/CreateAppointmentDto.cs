namespace BookingSystem.Client.Models;

public class CreateAppointmentDto
{
    public int ServiceId { get; set; }
    public DateTime StartTime { get; set; }
    public string? Note { get; set; }
}