using BookingService.API.Features.Services.Models;
using FluentValidation;

namespace BookingService.API.Features.Services.Validators;

public sealed class CreateServiceValidator : AbstractValidator<CreateServiceRequest>
{
    public CreateServiceValidator()
    {
        RuleFor(s => s.TrainerId)
                    .NotEmpty().WithMessage("Trainer ID is required");

        RuleFor(s => s.End)
            .NotEmpty().WithMessage("End time is required");

        RuleFor(s => s.Start)
            .NotEmpty().WithMessage("Start time is required")
            .LessThan(s => s.End).WithMessage("Start time must be before End time");

        RuleFor(s => s.Price)
            .GreaterThan(0).WithMessage("Price must be greater than zero");

        RuleFor(s => s.ServiceName)
            .NotEmpty().WithMessage("Name not provided");

        RuleFor(s => s.RoomId)
            .NotEmpty().WithMessage("Room ID is required");
    }
}
