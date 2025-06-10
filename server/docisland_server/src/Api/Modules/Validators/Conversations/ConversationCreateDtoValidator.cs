using Api.Dtos;
using FluentValidation;

namespace Api.Modules.Validators.Conversations;

public class ConversationCreateDtoValidator : AbstractValidator<ConversationCreateDto>
{
    public ConversationCreateDtoValidator()
    {
        RuleFor(x => x.File).NotEmpty();
    }
}