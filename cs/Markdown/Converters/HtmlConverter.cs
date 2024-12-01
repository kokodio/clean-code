using System.Text;

namespace Markdown.Converters;

public class HtmlConverter : IConverter
{
    private static readonly Dictionary<TokenType, string> HtmlTag = new()
    {
        { TokenType.Italic, "em" },
        { TokenType.Strong, "strong" },
    };
    
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
                TokenType.Italic or TokenType.Strong when token.Pair != null =>
                    isClosed[token.Type] ? Tag.Open(HtmlTag[token.Type]) : Tag.Close(HtmlTag[token.Type]),
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