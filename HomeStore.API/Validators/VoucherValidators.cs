using FluentValidation;
using HomeStore.Domain.DTOs.Vouchers;

namespace HomeStore.API.Validators;

public class CreateVoucherRequestValidator : AbstractValidator<CreateVoucherRequest>
{
    public CreateVoucherRequestValidator()
    {
        RuleFor(x => x.Code).NotEmpty().MaximumLength(50);
        RuleFor(x => x.DiscountType).Must(t => t == "Percent" || t == "Fixed")
            .WithMessage("DiscountType must be Percent or Fixed.");
        RuleFor(x => x.DiscountValue).GreaterThan(0);
        RuleFor(x => x.DiscountValue).LessThanOrEqualTo(100)
            .When(x => x.DiscountType == "Percent")
            .WithMessage("Percent discount must be <= 100.");
        RuleFor(x => x.MinOrderValue).GreaterThanOrEqualTo(0);
        RuleFor(x => x.MaxUsageCount).GreaterThan(0);
        RuleFor(x => x.ExpiryDate)
            .GreaterThan(x => x.StartDate ?? DateTime.UtcNow)
            .When(x => x.ExpiryDate.HasValue)
            .WithMessage("ExpiryDate must be after StartDate.");
    }
}

public class UpdateVoucherRequestValidator : AbstractValidator<UpdateVoucherRequest>
{
    public UpdateVoucherRequestValidator()
    {
        RuleFor(x => x.Code).MaximumLength(50).When(x => x.Code != null);
        RuleFor(x => x.DiscountType).Must(t => t == "Percent" || t == "Fixed")
            .WithMessage("DiscountType must be Percent or Fixed.")
            .When(x => x.DiscountType != null);
        RuleFor(x => x.DiscountValue).GreaterThan(0)
            .When(x => x.DiscountValue.HasValue);
        RuleFor(x => x.MinOrderValue).GreaterThanOrEqualTo(0)
            .When(x => x.MinOrderValue.HasValue);
        RuleFor(x => x.MaxUsageCount).GreaterThan(0)
            .When(x => x.MaxUsageCount.HasValue);
        RuleFor(x => x.ExpiryDate)
            .GreaterThan(x => x.StartDate ?? DateTime.UtcNow)
            .When(x => x.ExpiryDate.HasValue)
            .WithMessage("ExpiryDate must be after StartDate.");
    }
}
