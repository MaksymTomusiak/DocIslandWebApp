using Api.Dtos;
using FluentValidation;

namespace Api.Modules.Validators.Conversations;

public class ConversationDtoValidator : AbstractValidator<ConversationDto>
{
    public ConversationDtoValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.UserId).NotEmpty();
        RuleFor(x => x.FileId).NotEmpty();
        RuleFor(x => x.CreatedAt).NotEmpty();
    }
}