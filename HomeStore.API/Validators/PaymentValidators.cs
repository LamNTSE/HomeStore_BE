using FluentValidation;
using HomeStore.Domain.DTOs.Payments;

namespace HomeStore.API.Validators;

public class PaymentRequestValidator : AbstractValidator<PaymentRequest>
{
    public PaymentRequestValidator()
    {
        RuleFor(x => x.OrderId).GreaterThan(0);
        RuleFor(x => x.PaymentMethod).NotEmpty().Must(x => x == "COD" || x == "VNPay")
            .WithMessage("PaymentMethod must be COD or VNPay.");
    }
}
