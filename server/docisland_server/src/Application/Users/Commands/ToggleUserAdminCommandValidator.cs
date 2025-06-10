using FluentValidation;

namespace Application.Users.Commands;

public class ToggleUserAdminCommandValidator: AbstractValidator<ToggleUserAdminCommand>
{
    public ToggleUserAdminCommandValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();
    }
}