using Analytics.Application.DTOs;
using Analytics.Application.Services.Interface;
using Microsoft.AspNetCore.Mvc;

namespace Analytics.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserCreditController(
        IUserCreditService userCreditService,
        ILogger<UserCreditController> logger) : ControllerBase
    {

        // GET: api/UserCredits
        [HttpGet]
        public async Task<ActionResult<List<UserCreditDto>>> GetAllCredits()
        {
            try
            {
                var credits = await userCreditService.GetAllUserCredits();
                logger.LogInformation($"Retrieved {credits.Count} user credits");

                var dtos = credits.Select(c => new UserCreditDto
                {
                    userId = c.UserId,
                    roleId = c.RoleId,
                    creditBalance = c.CreditBalance
                }).ToList();

                return Ok(dtos);
            }
            catch (System.Exception ex)
            {
                logger.LogError(ex, "Error retrieving user credits");
                return StatusCode(500, "Internal server error");
            }
        }

        // GET: api/UserCredits/{userId}
        [HttpGet("{id:int}")]
        public async Task<ActionResult<UserCreditDto>> GetCreditByUserId(int id)
        {
            try
            {
                var credit = await userCreditService.GetUserCreditById(id);
                if (credit == null)
                    return NotFound();

                var dto = new UserCreditDto
                {
                    userId = credit.UserId,
                    roleId = credit.RoleId,
                    creditBalance = credit.CreditBalance
                };

                return Ok(dto);
            }
            catch (System.Exception ex)
            {
                logger.LogError(ex, $"Error retrieving user credit for UserId {id}");
                return StatusCode(500, "Internal server error");
            }
        }

        // POST: api/UserCredits
        [HttpPost]
        public async Task<ActionResult<UserCreditDto>> CreateUserCredit([FromBody] UserCreditDto userCreditDto)
        {
            try
            {
                logger.LogInformation("Received user credit: {@UserCredit}", userCreditDto);
                if (userCreditDto == null)
                    return BadRequest("UserCredit data is missing.");

                var created = await userCreditService.CreateUserCredit(userCreditDto);

                var dto = new UserCreditDto
                {
                    userId = created.UserId,
                    roleId = created.RoleId,
                    creditBalance = created.CreditBalance
                };

                return CreatedAtAction(nameof(GetCreditByUserId), new { Id = created.Id }, dto);
            }
            catch (System.Exception ex)
            {
                logger.LogError(ex, "Error creating user credit");
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // DELETE: api/UserCredits/{userId}
        [HttpDelete("{id:int}")]
        public async Task<ActionResult<UserCreditDto>> DeleteUserCredit(int id)
        {
            try
            {
                var deleted = await userCreditService.DeleteUserCredit(id);
                if (deleted == null)
                    return NotFound();

                var dto = new UserCreditDto
                {
                    userId = deleted.UserId,
                    roleId = deleted.RoleId,
                    creditBalance = deleted.CreditBalance
                };

                return Ok(dto);
            }
            catch (System.Exception ex)
            {
                logger.LogError(ex, $"Error deleting user credit for UserId {id}");
                return StatusCode(500, "Internal server error");
            }
        }
    }
}