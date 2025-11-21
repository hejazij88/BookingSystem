using BookingSystem.API.DTOs;
using BookingSystem.Domain.Models;
using BookingSystem.Infrastructure.Data;
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

        public ServicesController(BookingDbContext context)
        {
            _context = context;
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



        [HttpPost]
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
