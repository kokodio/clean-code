namespace Markdown;

public class Strong(int position) : IToken
{
    public string Text { get; set; }
    public int Position { get; init; } = position;
}