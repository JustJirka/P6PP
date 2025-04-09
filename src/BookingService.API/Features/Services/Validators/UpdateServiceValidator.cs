using BookingService.API.Features.Services.Models;
using FluentValidation;

namespace BookingService.API.Features.Services.Validators;

public sealed class UpdateServiceValidator : AbstractValidator<UpdateServiceRequest>
{
    public UpdateServiceValidator()
    {
        RuleFor(s => s.Id)
                    .NotEmpty().WithMessage("ID is required");

        RuleFor(s => s.End)
            .NotEmpty().WithMessage("End time is required");

        RuleFor(s => s.Start)
            .NotEmpty().WithMessage("Start time is required")
            .LessThan(s => s.End).WithMessage("Start time must be before End time");

        RuleFor(s => s.ServiceName)
            .NotEmpty().WithMessage("Name not provided");

        RuleFor(s => s.RoomId)
            .NotEmpty().WithMessage("Room ID is required");
    }
}
