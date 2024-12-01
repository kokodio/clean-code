using Markdown;
using Markdown.Converters;
using Markdown.Tokenizers;

var tokenizer = new KonturMdTokenizer();
var converter = new HtmlConverter();
var markdown = new Md(tokenizer, converter);

const string input = """
                     Подчерки внутри текста c цифрами_12_3 не считаются выделением и должны оставаться символами подчерка.
                     """;

var result = markdown.Render(input);

Console.WriteLine(result);