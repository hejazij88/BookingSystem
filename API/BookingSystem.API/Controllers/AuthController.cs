using BookingSystem.Applications.DTOs;
using BookingSystem.Applications.Features.Users.Commands;
using BookingSystem.Applications.Features.Users.Queries;
using BookingSystem.Domain.Models;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BookingSystem.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IMediator _mediator;

        public AuthController(IMediator mediator)
        {
            _mediator = mediator;
        }


        /// <summary>
        /// دریافت Access Token جدید با Refresh Token
        /// </summary>
        [HttpPost("refresh")]
        public async Task<IActionResult> Refresh([FromBody] string token)
        {
            var command = new RefreshTokenCommand(token);
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<List<UserDto>>> GetUsers()
        {
            var users = await _mediator.Send(new GetUsersQuery());
            return Ok(users);
        }

        // GET: api/Users/{id}
        [HttpGet("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<UserDto>> GetUserById(string id)
        {
            var user = await _mediator.Send(new GetUserByIdQuery(id));
            if (user == null) return NotFound();
            return Ok(user);
        }

        // POST: api/Users
        [HttpPost]
        [AllowAnonymous] 
        public async Task<ActionResult<UserDto>> CreateUser([FromBody] RegisterDto dto)
        {
            var user = await _mediator.Send(new CreateUserCommand(dto));
            return CreatedAtAction(nameof(GetUserById), new { id = user.Id }, user);
        }

        // PUT: api/Users/{id}
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<UserDto>> UpdateUser(string id, [FromBody] UpdateUserDto dto)
        {
            if (id != dto.Id) return BadRequest("Id mismatch");
            var user = await _mediator.Send(new UpdateUserCommand(dto));
            return Ok(user);
        }

        // DELETE: api/Users/{id}
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteUser(string id)
        {
            await _mediator.Send(new DeleteUserCommand(id));
            return NoContent();
        }

        // POST: api/Users/login
        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<ActionResult<string>> Login([FromBody] LoginDto dto)
        {
            var token = await _mediator.Send(new LoginUserCommand(dto));
            return Ok(new
            {
                AccessToken = token.AccessToken,
                RefreshToken=token.RefreshToken,
                AccessTokenExpiration=token.AccessTokenExpiration
            });
        }

    }
}
