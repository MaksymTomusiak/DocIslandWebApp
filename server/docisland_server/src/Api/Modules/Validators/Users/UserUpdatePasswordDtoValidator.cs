using Api.Dtos;
using FluentValidation;

namespace Api.Modules.Validators.Users;

public class UserUpdatePasswordDtoValidator : AbstractValidator<UserUpdatePasswordDto>
{
    public UserUpdatePasswordDtoValidator()
    {
        RuleFor(x => x.NewPassword)
            .NotEmpty()
            .Matches(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^a-zA-Z\d]).{8,}$")
            .MaximumLength(255);
        RuleFor(x => x.OldPassword)
            .NotEmpty()
            .Matches(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^a-zA-Z\d]).{8,}$")
            .MaximumLength(255);
    }
}