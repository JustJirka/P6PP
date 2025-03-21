﻿using AdminSettings.Persistence.Entities;
using Microsoft.EntityFrameworkCore;

namespace AdminSettings.Data;

public class AdminSettingsDbContext : DbContext
{
    public AdminSettingsDbContext(DbContextOptions<AdminSettingsDbContext> options)
        : base(options) { }

    public DbSet<AuditLog> AuditLogs { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<SystemSetting> SystemSettings { get; set; }
    public DbSet<Timezone> Timezones { get; set; }
    public DbSet<Currency> Currencies { get; set; }
    public DbSet<Language> Languages { get; set; }
}