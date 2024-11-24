namespace Markdown;

public class NewLineToken(int position) : IToken
{
    public string Text { get; set; } = "\n";
    public int Position { get; init; } = position;
}