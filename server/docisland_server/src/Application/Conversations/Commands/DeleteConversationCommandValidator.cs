using FluentValidation;

namespace Application.Conversations.Commands;

public class DeleteConversationCommandValidator : AbstractValidator<DeleteConversationCommand>
{
    public DeleteConversationCommandValidator()
    {
        RuleFor(x => x.ConversationId).NotEmpty();
    }
}