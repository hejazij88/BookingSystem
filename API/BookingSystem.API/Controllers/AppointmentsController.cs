using BookingSystem.Domain.Enums;
using BookingSystem.Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using BookingSystem.Applications.DTOs;
using BookingSystem.Domain.Models;

namespace BookingSystem.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class AppointmentsController : ControllerBase
    {
        private readonly BookingDbContext _context;

        public AppointmentsController(BookingDbContext context)
        {
            _context = context;
        }

        // ثبت رزرو جدید
        // POST: api/appointments
        [HttpPost]
        public async Task<IActionResult> CreateBooking(CreateAppointmentDto request)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // 1. آیا سرویس وجود دارد؟
            var service = await _context.Services.FindAsync(request.ServiceId);
            if (service == null)
                return NotFound("سرویس مورد نظر یافت نشد.");

            // 2. محاسبه زمان پایان
            var endTime = request.StartTime.AddMinutes(service.DurationMinutes);

            // 3. بررسی حیاتی: آیا این بازه قبلاً رزرو شده؟ (Double Booking Check)
            var conflict = await _context.Appointments.AnyAsync(a =>
                a.Status != BookingStatus.Cancelled && // اگر کنسل شده بود، مهم نیست
                a.StartTime < endTime &&
                a.EndTime > request.StartTime); // فرمول ریاضی تداخل زمانی

            if (conflict)
            {
                return BadRequest("متاسفانه این زمان لحظاتی پیش رزرو شد. لطفاً زمان دیگری را انتخاب کنید.");
            }

            // 4. ساخت موجودیت نهایی
            var appointment = new Appointment
            {
                ServiceId = request.ServiceId,
                UserId = userId,
                StartTime = request.StartTime,
                EndTime = endTime,
                Status = BookingStatus.Pending, // وضعیت اولیه: در انتظار تایید/پرداخت
                Note = request.Note,
                CreatedAt = DateTime.UtcNow
            };

            // 5. ذخیره در دیتابیس
            _context.Appointments.Add(appointment);
            await _context.SaveChangesAsync();

            return Ok(new { Message = "رزرو با موفقیت انجام شد", AppointmentId = appointment.Id });
        }

        // متدی برای دیدن رزروها (برای تست)
        [HttpGet]
        public async Task<ActionResult> GetAll()
        {
            return Ok(await _context.Appointments.ToListAsync());
        }
    }
}
