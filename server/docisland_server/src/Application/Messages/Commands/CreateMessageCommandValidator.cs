using FluentValidation;

namespace Application.Messages.Commands;

public class CreateMessageCommandValidator: AbstractValidator<CreateMessageCommand>
{
    public CreateMessageCommandValidator()
    {
        RuleFor(x => x.Content).NotEmpty();
        RuleFor(x => x.ConversationId).NotEmpty();
    }
}