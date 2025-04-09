using FluentValidation;

namespace Application.Conversations.Commands;

public class CreateConversationCommandValidator : AbstractValidator<CreateConversationCommand>
{
    public CreateConversationCommandValidator()
    {
        RuleFor(x => x.File).NotEmpty();
    }
}