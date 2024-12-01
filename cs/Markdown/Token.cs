namespace Markdown;

public class Token
{
    public TokenType Type { get; init; }
    public required string Content { get; init; }
    public bool IsClosing { get; init; }
    public bool IsOpening { get; init; }
    public int Position { get; init; }
    public Token? Pair { get; set; }
}