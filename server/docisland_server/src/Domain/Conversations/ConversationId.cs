namespace Domain.Conversations;

public record ConversationId(Guid Value)
{
    public static ConversationId Empty() => new(Guid.Empty);
    public static ConversationId New() => new(Guid.NewGuid());
    public override string ToString() => Value.ToString();
}