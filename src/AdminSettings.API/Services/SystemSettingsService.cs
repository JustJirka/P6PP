using AdminSettings.Data;
using AdminSettings.Persistence.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace AdminSettings.Services
{
    public class SystemSettingsService
    {
        private readonly AdminSettingsDbContext _context;

        public SystemSettingsService(AdminSettingsDbContext context)
        {
            _context = context;
        }

        public async Task<SystemSetting?> GetSystemSettingsAsync()
        {
            return await _context.SystemSettings
                .Include(s => s.timezone)
                .Include(s => s.currency)
                .FirstOrDefaultAsync();
        }

        public async Task<bool> UpdateSystemSettingsAsync(SystemSetting settings)
        {
            var existingSettings = await _context.SystemSettings.FirstOrDefaultAsync();
            if (existingSettings == null)
                return false;

            existingSettings.TimezoneId = settings.TimezoneId;
            existingSettings.CurrencyId = settings.CurrencyId;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<List<Timezone>> GetTimezonesAsync()
        {
            return await _context.Timezones.ToListAsync();
        }

        public async Task<bool> UpdateTimezoneAsync(Timezone timezone)
        {
            var existingTimezone = await _context.Timezones.FirstOrDefaultAsync(t => t.Id == timezone.Id);
            if (existingTimezone == null)
                return false;

            existingTimezone.Name = timezone.Name;
            existingTimezone.UtcOffset = timezone.UtcOffset;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<List<Currency>> GetCurrenciesAsync()
        {
            return await _context.Currencies.ToListAsync();
        }

        public async Task<bool> UpdateCurrencyAsync(Currency currency)
        {
            var existingCurrency = await _context.Currencies.FirstOrDefaultAsync(c => c.Id == currency.Id);
            if (existingCurrency == null)
                return false;

            existingCurrency.Name = currency.Name;
            existingCurrency.Symbol = currency.Symbol;

            await _context.SaveChangesAsync();
            return true;
        }
    }
}
