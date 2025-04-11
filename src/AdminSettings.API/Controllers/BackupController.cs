using Microsoft.AspNetCore.Mvc;
using AdminSettings.Services;

namespace AdminSettings.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BackupController : ControllerBase
{
    private readonly DatabaseBackupService _backupService;

    public BackupController(DatabaseBackupService backupService)
    {
        _backupService = backupService;
    }

    [HttpPost("run")]
    public async Task<IActionResult> RunBackup()
    {
        await _backupService.BackupAllAsync();
        return Ok("Zálohování dokončeno.");
    }
}