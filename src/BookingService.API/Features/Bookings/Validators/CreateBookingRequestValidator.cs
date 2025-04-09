using BookingService.API.Features.Bookings.Models;
using FluentValidation;

namespace BookingService.API.Features.Bookings.Validators;

public sealed class CreateBookingRequestValidator : AbstractValidator<CreateBookingRequest>
{
    public CreateBookingRequestValidator()
    {
        RuleFor(b => b.ServiceId)
            .NotEmpty();
    }
}
