using FluentValidation;
using HomeStore.Domain.DTOs.Orders;

namespace HomeStore.API.Validators;

public class CreateOrderRequestValidator : AbstractValidator<CreateOrderRequest>
{
    public CreateOrderRequestValidator()
    {
        RuleFor(x => x.ShippingAddress).NotEmpty().MaximumLength(500);
        RuleFor(x => x.Phone).NotEmpty().MaximumLength(20);
        RuleFor(x => x.ReceiverName).NotEmpty().MaximumLength(100);
        RuleFor(x => x.PaymentMethod).NotEmpty().Must(x => x == "COD" || x == "VNPay")
            .WithMessage("PaymentMethod must be COD or VNPay.");
    }
}
