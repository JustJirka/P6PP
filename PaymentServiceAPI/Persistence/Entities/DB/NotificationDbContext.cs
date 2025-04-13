using System;
using Microsoft.EntityFrameworkCore;
using PaymentService.API.Persistence.Entities.DB.Models;
using PaymentService.API.Persistence.Entities.DB.Seeding;

namespace PaymentService.API.Persistence.Entities.DB;

public class NotificationDbContext : DbContext
{
    public DbSet<Payment> Payments { get; set; }
    public NotificationDbContext(DbContextOptions options)
        : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        var paymentInit = new PaymentInit();
        var userCreditInit = new UserCreditInit();
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Payment>().HasData(paymentInit.GetPayments());
        modelBuilder.Entity<Payment>().HasData(userCreditInit.GetUserCredit());
    }
}
