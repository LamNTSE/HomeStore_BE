using FluentValidation;
using HomeStore.Domain.DTOs.Feedbacks;

namespace HomeStore.API.Validators;

public class CreateFeedbackRequestValidator : AbstractValidator<CreateFeedbackRequest>
{
    public CreateFeedbackRequestValidator()
    {
        RuleFor(x => x.ProductId).GreaterThan(0);
        RuleFor(x => x.Rating).InclusiveBetween(1, 5)
            .WithMessage("Rating must be between 1 and 5.");
        RuleFor(x => x.Comment).MaximumLength(2000).When(x => x.Comment != null);
    }
}

public class UpdateFeedbackRequestValidator : AbstractValidator<UpdateFeedbackRequest>
{
    public UpdateFeedbackRequestValidator()
    {
        RuleFor(x => x.Rating).InclusiveBetween(1, 5)
            .WithMessage("Rating must be between 1 and 5.")
            .When(x => x.Rating.HasValue);
        RuleFor(x => x.Comment).MaximumLength(2000).When(x => x.Comment != null);
    }
}
