using Api.Dtos;
using FluentValidation;

namespace Api.Modules.Validators.Users;

public class UserUpdateUserNameDtoValidator : AbstractValidator<UserUpdateUserNameDto>
{
    public UserUpdateUserNameDtoValidator()
    {
        RuleFor(x => x.UserName)
            .NotEmpty()
            .MinimumLength(5)
            .MaximumLength(255);
    }
}