using FluentValidation;
using HomeStore.Domain.DTOs.Users;

namespace HomeStore.API.Validators;

public class UpdateMyProfileRequestValidator : AbstractValidator<UpdateMyProfileRequest>
{
    public UpdateMyProfileRequestValidator()
    {
        RuleFor(x => x.FullName).MaximumLength(100).When(x => x.FullName != null);
        RuleFor(x => x.Phone).MaximumLength(20).When(x => x.Phone != null);
        RuleFor(x => x.Address).MaximumLength(500).When(x => x.Address != null);
        RuleFor(x => x.NewPassword).MinimumLength(6).MaximumLength(100)
            .When(x => x.NewPassword != null);
    }
}

public class AdminCreateUserRequestValidator : AbstractValidator<AdminCreateUserRequest>
{
    public AdminCreateUserRequestValidator()
    {
        RuleFor(x => x.FullName).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Email).NotEmpty().EmailAddress().MaximumLength(255);
        RuleFor(x => x.Password).NotEmpty().MinimumLength(6).MaximumLength(100);
        RuleFor(x => x.Phone).MaximumLength(20).When(x => x.Phone != null);
        RuleFor(x => x.Role).Must(r => r == "Admin" || r == "Customer")
            .WithMessage("Role must be Admin or Customer.");
    }
}

public class AdminUpdateUserRequestValidator : AbstractValidator<AdminUpdateUserRequest>
{
    public AdminUpdateUserRequestValidator()
    {
        RuleFor(x => x.FullName).MaximumLength(100).When(x => x.FullName != null);
        RuleFor(x => x.Phone).MaximumLength(20).When(x => x.Phone != null);
        RuleFor(x => x.NewPassword).MinimumLength(6).MaximumLength(100)
            .When(x => x.NewPassword != null);
        RuleFor(x => x.Role).Must(r => r == "Admin" || r == "Customer")
            .WithMessage("Role must be Admin or Customer.")
            .When(x => x.Role != null);
    }
}
