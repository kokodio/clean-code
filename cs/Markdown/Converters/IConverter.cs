namespace Markdown.Converters;

public interface IConverter
{
    public string Convert(List<Token> tokens);
}