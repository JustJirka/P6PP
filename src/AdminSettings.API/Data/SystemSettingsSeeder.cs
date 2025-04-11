using AdminSettings.Data;
using AdminSettings.Persistence.Entities;
using Microsoft.EntityFrameworkCore;

namespace AdminSettings.Data;

public class SystemSettingsSeeder
{
    private readonly AdminSettingsDbContext _context;
    private readonly ILogger<SystemSettingsSeeder> _logger;

    public SystemSettingsSeeder(AdminSettingsDbContext context, ILogger<SystemSettingsSeeder> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task SeedAsync()
    {
        _logger.LogInformation("Seeding default data for system settings...");

        // Seed Timezone
        if (!await _context.Timezones.AnyAsync())
        {
            _context.Timezones.AddRange(
                new Timezone { Name = "UTC", UtcOffset = "+00:00" },
                new Timezone { Name = "CET", UtcOffset = "+01:00" }
            );
        }

        // Seed Currency
        if (!await _context.Currencies.AnyAsync())
        {
            _context.Currencies.AddRange(
                new Currency { Name = "Euro", Symbol = "€" },
                new Currency { Name = "Dollar", Symbol = "$" }
            );
        }

        await _context.SaveChangesAsync();

        // Seed SystemSettings
        if (!await _context.SystemSettings.AnyAsync())
        {
            var timezone = await _context.Timezones.FirstAsync();
            var currency = await _context.Currencies.FirstAsync();

            _context.SystemSettings.Add(new SystemSetting
            {
                TimezoneId = timezone.Id,
                CurrencyId = currency.Id,
            });

            await _context.SaveChangesAsync();
        }

        _logger.LogInformation("Default system settings data seeded.");
    }
}