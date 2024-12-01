using System.Text;

namespace Markdown.Converters;

public class HtmlConverter : IConverter
{
    public string Convert(List<Token> tokens)
    {
        var html = new StringBuilder();
        var isClosed = new Dictionary<TokenType, bool>
        {
            {TokenType.Italic, true},
            {TokenType.Text, true},
            {TokenType.Strong, true},
        };

        foreach (var token in tokens)
        {
            html.Append(token.Type switch
            {
                TokenType.Text => token.Content,
                TokenType.Italic when token.Pair != null =>
                    isClosed[token.Type] ? Tag.Open("em") : Tag.Close("em"),
                TokenType.Strong when token.Pair != null  =>
                    isClosed[token.Type] ? Tag.Open("strong") : Tag.Close("strong"),
                _ => token.Content
            });

            if (token.Pair != null)
            {
                isClosed[token.Type] = !isClosed[token.Type];
            }
        }

        return html.ToString();
    }
}