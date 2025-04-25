using System;
using Microsoft.EntityFrameworkCore;
using NotificationService.API.Persistence.Entities.DB.Models;
using NotificationService.API.Persistence.Entities.DB.Seeding;
using src.NotificationService.API.Persistence.Entities.DB.Models;

namespace NotificationService.API.Persistence.Entities.DB;

public class NotificationDbContext : DbContext
{
    public DbSet<Template> Templates { get; set; }
    public DbSet<NotificationLog> NotificationLogs { get; set; }
    public DbSet<Booking> Bookings { get; set; }
    public NotificationDbContext(DbContextOptions options)
        : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        var templateInit = new TemplateInit();
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Template>().HasData(templateInit.GetTemplates());
    }
}
