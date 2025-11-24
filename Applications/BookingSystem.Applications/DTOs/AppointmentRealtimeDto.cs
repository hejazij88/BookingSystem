using BookingSystem.Domain.Enums;

namespace BookingSystem.Applications.DTOs;

/// <summary>
/// Slim DTO used for pushing appointment state over SignalR.
/// </summary>
public class AppointmentRealtimeDto
{
    public int Id { get; set; }
    public int ServiceId { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public BookingStatus Status { get; set; }
}

