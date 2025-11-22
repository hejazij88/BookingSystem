using BookingSystem.API.Services;
using BookingSystem.Applications.DTOs;
using BookingSystem.Domain.Constants;
using BookingSystem.Domain.Models;
using BookingSystem.Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BookingSystem.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ServicesController : ControllerBase
    {
        public readonly BookingDbContext _context;
        private readonly AvailabilityService _availabilityService; 

        public ServicesController(BookingDbContext context, AvailabilityService availabilityService)
        {
            _context = context;
            _availabilityService = availabilityService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ServiceDto>>> GetServices() 
        {
            var services =await _context.Services.Select(s => new ServiceDto
            {
                Id = s.Id,
                Name = s.Name,
                DurationMinutes = s.DurationMinutes,
                Price = s.Price,
            }).ToListAsync();

            return Ok(services);
        }

        [HttpGet("{id}/slots")]
        public async Task<ActionResult<IEnumerable<TimeSlotDto>>> GetSlots(int id, [FromQuery] DateTime date)
        {
            // تاریخ اگر گذشته باشد، منطقا نباید سانس بدهد (این شرط را می‌توان بعدا افزود)
            var slots = await _availabilityService.GenerateSlots(id, date);
            return Ok(slots);
        }

        [HttpPost]
        [Authorize(Roles = ApplicationRoles.Admin)]
        public async Task<ActionResult<ServiceDto>> CreateService(CreateServiceDto request)
        {
            // تبدیل DTO ورودی به موجودیت دیتابیس
            var service = new Service
            {
                Name = request.Name,
                DurationMinutes = request.DurationMinutes,
                Price = request.Price,
                IsActive = true
            };

            // اضافه کردن به دیتابیس
            _context.Services.Add(service);
            await _context.SaveChangesAsync(); // ذخیره نهایی (Commit)

            // ساختن خروجی برای نمایش به کاربر
            var response = new ServiceDto
            {
                Id = service.Id,
                Name = service.Name,
                DurationMinutes = service.DurationMinutes,
                Price = service.Price
            };

            // برگرداندن کد 201 Created
            return CreatedAtAction(nameof(GetServices), new { id = service.Id }, response);
        }
    }
}
