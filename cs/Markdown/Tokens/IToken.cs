namespace Markdown;

public interface IToken
{
    public string Text { get; set; }
    public int Position { get; init; }
}