using BookingSystem.Applications.DTOs;
using FluentValidation;
using MediatR;

namespace BookingSystem.Applications.Features.Services.Commands;

public record CreateServiceCommand(string Name, int DurationMinutes, decimal Price) : IRequest<ServiceDto>;

public class CreateServiceCommandValidator : AbstractValidator<CreateServiceCommand>
{
    public CreateServiceCommandValidator()
    {
        // 1. نام سرویس نباید خالی و طول آن بین 3 تا 100 کاراکتر باشد
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("وارد کردن نام سرویس الزامی است.")
            .MinimumLength(3).WithMessage("نام سرویس حداقل 3 کاراکتر است.")
            .MaximumLength(100).WithMessage("نام سرویس حداکثر 100 کاراکتر است.");

        // 2. مدت زمان باید حداقل 10 دقیقه باشد
        RuleFor(x => x.DurationMinutes)
            .GreaterThanOrEqualTo(10).WithMessage("مدت زمان نوبت نمی‌تواند کمتر از 10 دقیقه باشد.");

        // 3. قیمت نباید منفی باشد
        RuleFor(x => x.Price)
            .GreaterThanOrEqualTo(0).WithMessage("قیمت نمی‌تواند منفی باشد.");
    }
}