using CarRentalSystem.Application.DTOs;
using FluentValidation;

namespace CarRentalSystem.Application.Validators;

public class CreateVehicleValidator : AbstractValidator<CreateVehicleDto>
{
    public CreateVehicleValidator()
    {
        RuleFor(x => x.Brand)
            .NotEmpty().WithMessage("Brand is required")
            .MaximumLength(50).WithMessage("Brand cannot exceed 50 characters");

        RuleFor(x => x.Model)
            .NotEmpty().WithMessage("Model is required")
            .MaximumLength(50).WithMessage("Model cannot exceed 50 characters");

        RuleFor(x => x.Year)
            .GreaterThanOrEqualTo(2000).WithMessage("Year must be 2000 or later")
            .LessThanOrEqualTo(DateTime.Now.Year + 1).WithMessage($"Year cannot be later than {DateTime.Now.Year + 1}");

        RuleFor(x => x.PricePerDay)
            .GreaterThan(0).WithMessage("Price per day must be greater than 0");

        RuleFor(x => x.LicensePlate)
            .MaximumLength(20).WithMessage("License plate cannot exceed 20 characters")
            .When(x => !string.IsNullOrEmpty(x.LicensePlate));
    }
}
