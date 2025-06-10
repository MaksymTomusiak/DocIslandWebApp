using FluentValidation;

namespace Application.Users.Commands;

public class ToggleUserBanCommandValidator: AbstractValidator<ToggleUserBanCommand>
{
    public ToggleUserBanCommandValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();
    }
}