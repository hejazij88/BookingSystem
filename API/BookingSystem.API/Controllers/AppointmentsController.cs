using Azure.Core;
using BookingSystem.Applications.DTOs;
using BookingSystem.Applications.Features.Appointments.Commands.Records;
using BookingSystem.Applications.Features.Appointments.Queries.Records;
using BookingSystem.Domain.Enums;
using BookingSystem.Domain.Models;
using BookingSystem.Infrastructure.Data;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace BookingSystem.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class AppointmentsController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IHttpContextAccessor _httpContextAccessor;


        public AppointmentsController(IMediator mediator, IHttpContextAccessor httpContextAccessor)
        {
            _mediator = mediator;
            _httpContextAccessor = httpContextAccessor;
        }

        [HttpGet]
        public async Task<ActionResult<List<AppointmentDto>>> GetAppointments()
        {
            var list = await _mediator.Send(new GetAppointmentsQuery());
            return Ok(list);
        }

        [HttpGet("GetAppointmentsByUserId")]
        public async Task<ActionResult<List<AppointmentDto>>> GetAppointmentsByUserId([FromQuery]bool isActive)
        {
            var userId = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var list = await _mediator.Send(new GetAppointmentsByUserIdQuery(userId,isActive));
            return Ok(list);
        }


        [HttpPost]
        public async Task<ActionResult<AppointmentDto>> CreateAppointment([FromBody] CreateAppointmentDto dto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var appointmentId = await _mediator.Send(new CreateAppointmentCommand(dto, userId));
            return Ok(new { Message = "رزرو با موفقیت انجام شد", AppointmentId = appointmentId });
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<AppointmentDto>> UpdateAppointment(int id, [FromBody] UpdateAppointmentDto dto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            if (id != dto.Id)
                return BadRequest("Id رزرو با پارامتر مسیر مطابقت ندارد.");

            try
            {
                var updatedId = await _mediator.Send(new UpdateAppointmentCommand(dto, userId));
                return Ok(new { Message = "رزرو با موفقیت بروزرسانی شد", AppointmentId = updatedId });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Error = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAppointment(int id)
        {
            await _mediator.Send(new DeleteAppointmentCommand(id));
            return NoContent();
        }
    }
}
