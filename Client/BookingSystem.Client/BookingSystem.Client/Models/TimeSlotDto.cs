namespace BookingSystem.Client.Models;

public class TimeSlotDto
{
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }

    // خاصیت کمکی برای نمایش زمان به صورت خوانا
    public string DisplayTime => StartTime.ToShortTimeString();
}