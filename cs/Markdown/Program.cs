using Markdown;

var markdown = new Md();

var input =
    """
    _123_!!
    _123_
    """;

var result = markdown.Render(input);

Console.WriteLine(result);