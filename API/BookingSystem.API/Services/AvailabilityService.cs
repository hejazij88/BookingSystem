using BookingSystem.Applications.DTOs;
using BookingSystem.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace BookingSystem.API.Services;

public class AvailabilityService
{

    private readonly BookingDbContext _context;

    private readonly TimeSpan _startHour = new TimeSpan(9, 0, 0); // ۹ صبح
    private readonly TimeSpan _endHour = new TimeSpan(17, 0, 0);  // ۵ عصر

    public AvailabilityService(BookingDbContext context)
    {
        _context = context;
    }

    public async Task<List<TimeSlotDto>> GenerateSlots(int serviceId, DateTime date)
    {
        var service = await _context.Services.FindAsync(serviceId);
        if (service == null) return new List<TimeSlotDto>();


        // 2. دریافت تمام رزروهای موجود در آن روز (برای بررسی تداخل)
        // نکته: فقط رزروهایی را می‌گیریم که کنسل نشده باشند
        var existingBookings = await _context.Appointments
            .Where(a => a.StartTime.Date == date.Date && a.Status != Domain.Enums.BookingStatus.Cancelled)
            .ToListAsync();

        var availableSlots = new List<TimeSlotDto>();


        // شروع از ساعت ۹ صبح همان روز
        var currentTime = date.Date.Add(_startHour);
        var endTime = date.Date.Add(_endHour);


        while (currentTime.AddMinutes(service.DurationMinutes) <= endTime)
        {
            var slotEnd = currentTime.AddMinutes(service.DurationMinutes);

            // 4. بررسی تداخل: آیا در این بازه زمانی، رزروی وجود دارد؟
            bool isBooked = existingBookings.Any(b =>
                (currentTime < b.EndTime && slotEnd > b.StartTime)); // فرمول استاندارد بررسی تداخل زمانی

            if (!isBooked)
            {
                availableSlots.Add(new TimeSlotDto
                {
                    StartTime = currentTime,
                    EndTime = slotEnd
                });
            }

            // رفتن به بازه بعدی (فعلا پشت سر هم می‌چینیم)
            currentTime = currentTime.AddMinutes(service.DurationMinutes);
        }

        return availableSlots;
    }

}