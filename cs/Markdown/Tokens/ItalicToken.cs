namespace Markdown;

public class ItalicToken(int position) : IToken
{
    public string Text { get; set; }
    public int Position { get; init; } = position;
}