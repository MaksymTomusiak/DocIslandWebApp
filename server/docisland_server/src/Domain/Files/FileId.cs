namespace Domain.Files;

public record FileId(Guid Value)
{
    public static FileId Empty() => new(Guid.Empty);
    public static FileId New() => new(Guid.NewGuid());
    public override string ToString() => Value.ToString();
}