using AdminSettings.Persistence.Entities;
using AdminSettings.Persistence.Interface;
using AdminSettings.Services;
using Microsoft.AspNetCore.Mvc;

namespace AdminSettings.Controllers;

[Route("api/audit")]
[ApiController]
public class AuditController : ControllerBase
{
    private readonly AuditLogService _auditLogService;
    private readonly IUserServiceClient _userServiceClient;
    
    public AuditController(AuditLogService auditLogService, IUserServiceClient userServiceClient)
    {
        _auditLogService = auditLogService;
        _userServiceClient = userServiceClient;
    }

    [HttpGet]
    public async Task<IActionResult> GetAuditLogs([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10, [FromQuery] DateTime? fromDate = null, [FromQuery] DateTime? toDate = null)
    {
        var logs = await _auditLogService.GetAllAsync(pageNumber, pageSize, fromDate, toDate);
        return Ok(logs);
    }


    [HttpPost]
    public async Task<IActionResult> AddAuditLog([FromBody] AuditLog auditLog)
    {
        if (auditLog == null || string.IsNullOrEmpty(auditLog.Action))
            return BadRequest("Invalid data");

        if (!int.TryParse(auditLog.UserId, out int userId) || !await _userServiceClient.UserExistsAsync(userId))
            return NotFound($"Uživatel s ID {auditLog.UserId} neexistuje.");

        var id = await _auditLogService.CreateAsync(auditLog);
        return CreatedAtAction(nameof(GetAuditLogs), new { id }, auditLog);
    }


    [HttpGet("user/{userId}")]
    public async Task<IActionResult> GetUserAuditLogs(string userId, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10, [FromQuery] DateTime? fromDate = null, [FromQuery] DateTime? toDate = null)
    {
        if (!int.TryParse(userId, out int id) || !await _userServiceClient.UserExistsAsync(id))
            return NotFound("Uživatel neexistuje");

        var logs = await _auditLogService.GetByUserAsync(userId, pageNumber, pageSize, fromDate, toDate);
        return Ok(logs);
    }



    [HttpGet("action/{action}")]
    public async Task<IActionResult> GetAuditLogsByAction(string action)
    {
        var logs = await _auditLogService.GetByActionAsync(action);
        return Ok(logs);
    }

    [HttpPut("{id}/archive")]
    public async Task<IActionResult> ArchiveAuditLog(int id)
    {
        await _auditLogService.ArchiveAsync(id);
        return Ok();
    }
}
