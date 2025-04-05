namespace Domain.Messages;

public record MessageId(Guid Value)
{
    public static MessageId Empty() => new(Guid.Empty);
    public static MessageId New() => new(Guid.NewGuid());
    public override string ToString() => Value.ToString();
}