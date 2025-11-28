using BookingSystem.Domain.Enums;

namespace BookingSystem.Applications.DTOs;

public class AppointmentDto
{
    public int Id { get; set; }
    public string ServiceTitle { get; set; }
    public string UserId { get; set; }
    public string UserName { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public string Description { get; set; }
    public BookingStatus Status { get; set; }
}