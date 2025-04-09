using BookingService.API.Features.Rooms.Models;
using FluentValidation;

namespace BookingService.API.Features.Rooms.Validators;

public sealed class RoomRequestValidator : AbstractValidator<RoomRequest>
{
    public RoomRequestValidator()
    {
        RuleFor(r => r.Name)
            .NotEmpty()
            .MaximumLength(64);

        RuleFor(r => r.Capacity)
            .NotEmpty()
            .LessThan(1_000);
    }
}
