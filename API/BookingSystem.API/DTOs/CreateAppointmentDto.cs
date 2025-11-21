using System.ComponentModel.DataAnnotations;

namespace BookingSystem.API.DTOs;

public class CreateAppointmentDto
{
    [Required]
    public int ServiceId { get; set; }

    [Required]
    public DateTime StartTime { get; set; }

    public string? Note { get; set; }
}