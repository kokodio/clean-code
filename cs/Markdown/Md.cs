namespace Markdown;

public class Md
{
    public string Render(string text)
    {
        var tokenizer = new Tokenizer();
        
        var tokens = tokenizer.Tokenize(text);
        var html = HtmlConverter.ConvertToHtml(tokens);
        
        return html;
    }
}