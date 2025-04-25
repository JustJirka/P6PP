using Microsoft.AspNetCore.Mvc;
using AdminSettings.Services;
using System.Threading.Tasks;
using AdminSettings.Persistence.Entities;

namespace AdminSettings.Controllers
{
    [ApiController]
    [Route("api")]
    public class SystemSettingsController : ControllerBase
    {
        private readonly SystemSettingsService _systemSettingsService;

        public SystemSettingsController(SystemSettingsService systemSettingsService)
        {
            _systemSettingsService = systemSettingsService;
        }

        [HttpGet("system-settings")]
        public async Task<IActionResult> GetSystemSettings()
        {
            var settings = await _systemSettingsService.GetSystemSettingsAsync();
            return settings == null ? NotFound("System settings not found.") : Ok(settings);
        }

        [HttpPut("system-settings")]
        public async Task<IActionResult> UpdateSystemSettings([FromBody] SystemSetting settings)
        {
            if (!ModelState.IsValid || settings == null)
                return BadRequest("Invalid data.");

            var result = await _systemSettingsService.UpdateSystemSettingsAsync(settings);
            return result ? NoContent() : NotFound("System settings not found.");
        }

        [HttpGet("system-settings/timezones")]
        public async Task<IActionResult> GetTimezones()
        {
            var timezones = await _systemSettingsService.GetTimezonesAsync();
            return timezones.Any() ? Ok(timezones) : NotFound("No timezones available.");
        }

        [HttpPut("system-settings/timezones")]
        public async Task<IActionResult> UpdateTimezone([FromBody] Timezone timezone)
        {
            if (!ModelState.IsValid || timezone == null)
                return BadRequest("Invalid timezone data.");

            var result = await _systemSettingsService.UpdateTimezoneAsync(timezone);
            return result ? NoContent() : NotFound("Timezone not found.");
        }

        [HttpGet("system-settings/currencies")]
        public async Task<IActionResult> GetCurrencies()
        {
            var currencies = await _systemSettingsService.GetCurrenciesAsync();
            return currencies.Any() ? Ok(currencies) : NotFound("No currencies available.");
        }

        [HttpPut("system-settings/currencies")]
        public async Task<IActionResult> UpdateCurrency([FromBody] Currency currency)
        {
            if (!ModelState.IsValid || currency == null)
                return BadRequest("Invalid currency data.");

            var result = await _systemSettingsService.UpdateCurrencyAsync(currency);
            return result ? NoContent() : NotFound("Currency not found.");
        }
    }
}
