using BookingSystem.API.Services;
using BookingSystem.Applications.DTOs;
using BookingSystem.Applications.Features.Services.Commands;
using BookingSystem.Applications.Features.Services.Queries;
using BookingSystem.Domain.Constants;
using BookingSystem.Domain.Models;
using BookingSystem.Infrastructure.Data;
using MediatR;
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
        private readonly IMediator _mediator;
        private readonly AvailabilityService _availabilityService; 

        public ServicesController(AvailabilityService availabilityService, IMediator mediator)
        {
            _availabilityService = availabilityService;
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ServiceDto>>> GetServices()
        {
            // فقط درخواست را به لایه Application می‌فرستد و منتظر پاسخ می‌ماند
            var result = await _mediator.Send(new GetServicesQuery());
            return Ok(result);
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
        public async Task<ActionResult<ServiceDto>> CreateService([FromBody] CreateServiceCommand command)
        {
            // فقط دستور را می‌فرستد. Controller نمی‌داند چگونه این کار انجام می‌شود
            var result = await _mediator.Send(command);
            return CreatedAtAction(nameof(GetServices), new { id = result.Id }, result);
        }
    }
}
