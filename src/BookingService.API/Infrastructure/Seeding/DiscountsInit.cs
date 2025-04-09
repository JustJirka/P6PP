using BookingService.API.Domain.Models;

namespace BookingService.API.Infrastructure.Seeding;

internal static class DiscountsInit
{
    public static IEnumerable<Discount> GetDiscounts()
    {
        return
        [
            new Discount
            {
                Id = 1,
                Percentage = 10,
                ValidFrom = DateTime.Now,
                ValidTo = DateTime.Now.AddMonths(1),
                IsValid = true
            },
            new Discount
            {
                Id = 2,
                Percentage = 20,
                ValidFrom = DateTime.Now,
                ValidTo = DateTime.Now.AddMonths(1),
                IsValid = true
            }
        ];
    }
}