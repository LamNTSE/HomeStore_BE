using FluentValidation;
using HomeStore.Domain.DTOs.Chat;

namespace HomeStore.API.Validators;

public class SendMessageRequestValidator : AbstractValidator<SendMessageRequest>
{
    public SendMessageRequestValidator()
    {
        RuleFor(x => x.ReceiverId).GreaterThan(0);
        RuleFor(x => x.Content).NotEmpty().MaximumLength(2000);
    }
}
