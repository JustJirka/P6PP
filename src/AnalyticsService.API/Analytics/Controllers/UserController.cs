using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Analytics.Application.Services.Interface;
using Analytics.Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using Analytics.Application.DTOs;

namespace Analytics.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IDatabaseSyncService _databaseSyncService;
        private readonly ILogger<UsersController> _logger;

        public UsersController(IUserService userService, IDatabaseSyncService databaseSyncService)
        {
            _userService = userService;
            _databaseSyncService = databaseSyncService;
            _logger = new LoggerFactory().CreateLogger<UsersController>();
        }

        // GET: api/Users
        [HttpGet]
        public async Task<ActionResult<List<UserDto>>> GetAllUsers()
        {
            var users = await _userService.GetAllUsers();
            return Ok(users);
        }

        // GET: api/Users/{id}
        [HttpGet("{id:int}")]
        public async Task<ActionResult<UserDto>> GetUserById(int id)
        {
            var user = await _userService.GetUserById(id);
            if (user == null)
            {
                return NotFound();
            }
            return Ok(user);
        }

        // POST: api/Users
        [HttpPost]
        public async Task<ActionResult<UserDto>> CreateUser([FromBody] UserDto user)
        {
            _logger.LogInformation("Received user: {@User}", user);
            if (user == null)
            {
                return BadRequest("User data is missing.");
            }

            var createdUser = await _userService.CreateUser(user);
            // CreatedAtAction returns a 201 response with a Location header.
            return CreatedAtAction(nameof(GetUserById), new { id = createdUser.Id }, createdUser);
        }

        // DELETE: api/Users/{id}
        [HttpDelete("{id:int}")]
        public async Task<ActionResult<UserDto>> DeleteUser(int id)
        {
            var deletedUser = await _userService.DeleteUser(id);
            if (deletedUser == null)
            {
                return NotFound();
            }
            return Ok(deletedUser);
        }

        [HttpGet("triggerSync")]
        public async Task<IActionResult> TriggerSync()
        {
            _logger.LogInformation("Sync triggered.");
            await _databaseSyncService.SyncDatabase();
            return Ok("Sync triggered successfully.");
        }
    }
}
