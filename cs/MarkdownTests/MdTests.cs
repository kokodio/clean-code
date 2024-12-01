using System.Diagnostics;
using System.Text;
using FluentAssertions;
using Markdown;
using Markdown.Converters;
using Markdown.Tokenizers;


namespace MarkdownTests;

[TestFixture]
public class MdTests
{
    private Md renderer;
    
    [SetUp]
    public void Setup()
    {
        var tokenizer = new KonturMdTokenizer();
        var converter = new HtmlConverter();
        renderer = new Md(tokenizer, converter);
    }

    [Test]
    public void Render_EmptyString_ReturnsEmpty()
    {
        var result = renderer.Render("");
        result.Should().Be(string.Empty);
    }
    
    [Test]
    public void Render_Italic()
    {
        var result = renderer.Render("Текст, _окруженный с двух сторон_ одинарными символами подчерка");
        result.Should().Be("Текст, <em>окруженный с двух сторон</em> одинарными символами подчерка");
    }
    
    [Test]
    public void Render_Strong()
    {
        var result = renderer.Render("__Выделенный двумя символами текст__");
        result.Should().Be("<strong>Выделенный двумя символами текст</strong>");
    }
    
    [Test]
    public void Render_Escaped()
    {
        var result = renderer.Render(@"\_Вот это\_, не должно выделиться");
        result.Should().Be("_Вот это_, не должно выделиться");
    }

    [Test]
    public void Render_ItalicInsideStrong()
    {
        var result = renderer.Render("Внутри __двойного выделения _одинарное_ тоже__ работает.");
        result.Should().Be("Внутри <strong>двойного выделения <em>одинарное</em> тоже</strong> работает.");
    }
    
    [Test]
    public void Render_DoubleNested()
    {
        var result = renderer.Render("Внутри __двойного _выделения_ _одинарное_ тоже__ работает.");
        result.Should().Be("Внутри <strong>двойного <em>выделения</em> <em>одинарное</em> тоже</strong> работает.");
    }
    
    [Test]
    public void Render_Same_StrongInsideItalic()
    {
        var result = renderer.Render("Но не наоборот — внутри _одинарного __двойное__ не_ работает.");
        result.Should().Be("Но не наоборот — внутри _одинарного __двойное__ не_ работает.");
    }
    
    [Test]
    public void Render_Same_WhenInsideDigits()
    {
        var result = renderer.Render("цифрами_12_3");
        result.Should().Be("цифрами_12_3");
    }
    
    [Test]
    public void Render_AtBeginningOfWord()
    {
        var result = renderer.Render("Однако выделять часть слова они могут: и в _нач_але,");
        result.Should().Be("Однако выделять часть слова они могут: и в <em>нач</em>але,");
    }
    
    [Test]
    public void Render_InsideWord()
    {
        var result = renderer.Render("и в сер_еди_не");
        result.Should().Be("и в сер<em>еди</em>не");
    }
    
    [Test]
    public void Render_AtEndOfWord()
    {
        var result = renderer.Render("и в кон_це._");
        result.Should().Be("и в кон<em>це.</em>");
    }
    
    [Test]
    public void Render_ItalicDifferentWords()
    {
        var result = renderer.Render(@"В то же время выделение в ра_зных сл_овах не работает.");
        result.Should().Be("В то же время выделение в ра_зных сл_овах не работает.");
    }
    
    [Test]
    public void Render_Same_NonPaired()
    {
        var result = renderer.Render(@"__Непарные_ символы в рамках одного абзаца не считаются выделением.");
        result.Should().Be("__Непарные_ символы в рамках одного абзаца не считаются выделением.");
    }
    
    [Test]
    public void Render_ItalicMustHaveNonWhitespaceAfterOpen()
    {
        var result = renderer.Render(@"За подчерками, начинающими выделение, должен следовать непробельный символ. Иначе эти_ подчерки_ не считаются выделением и остаются просто символами подчерка.");
        result.Should().Be("За подчерками, начинающими выделение, должен следовать непробельный символ. Иначе эти_ подчерки_ не считаются выделением и остаются просто символами подчерка.");
    }
    
    [Test]
    public void Render_ItalicMustHaveNonWhitespaceBeforeClose()
    {
        var result = renderer.Render(@"Подчерки, заканчивающие выделение, должны следовать за непробельным символом. Иначе эти _подчерки _не считаются_ окончанием выделения и остаются просто символами подчерка.");
        result.Should().Be("Подчерки, заканчивающие выделение, должны следовать за непробельным символом. Иначе эти _подчерки <em>не считаются</em> окончанием выделения и остаются просто символами подчерка.");
    }
    
    [Test]
    public void Render_Same_OverlappingPairs()
    {
        var result = renderer.Render(@"В случае __пересечения _двойных__ и одинарных_ подчерков ни один из них не считается выделением.");
        result.Should().Be("В случае __пересечения _двойных__ и одинарных_ подчерков ни один из них не считается выделением.");
    }

    [Test]
    public void Render_EmptyUnderscores()
    {
        var result = renderer.Render(@"Если внутри подчерков пустая строка ____, то они остаются символами подчерка.");
        result.Should().Be("Если внутри подчерков пустая строка ____, то они остаются символами подчерка.");
    }
    
    [Test]
    public void Render_Header()
    {
        var result = renderer.Render("# Header text");
        result.Should().Be("<h1>Header text</h1>");
    }
    
    [Test]
    public void Render_HeaderWithTokens()
    {
        var result = renderer.Render(@"# Заголовок __с _разными_ символами__");
        result.Should().Be("<h1>Заголовок <strong>с <em>разными</em> символами</strong></h1>");
    }
    
    [Test]
    public void Render_NewLineHeader()
    {
        var result = renderer.Render("""
                                     # Заголовок __с _разными_ символами__
                                     # Заголовок __с _разными_ символами__
                                     """);
        result.Should().Be("""
                           <h1>Заголовок <strong>с <em>разными</em> символами</strong></h1>
                           <h1>Заголовок <strong>с <em>разными</em> символами</strong></h1>
                           """);
    }
    
    [Test]
    public void Render_IsLinear()
    {
        const int tolerance = 2;
        const int firstRunCount = 100_000;
        const int secondRunCount = 1_000_000;
        const double expectedCoefficient = secondRunCount / firstRunCount + tolerance;
        
        var markdown = new StringBuilder();
        for (var i = 0; i < firstRunCount; i++)
        {
            markdown.Append("_text_");
            markdown.Append("__text__");
        }
        
        var firstRun = GetRunTime(() => renderer.Render(markdown.ToString()));;
        
        markdown.Clear();
        
        for (var i = 0; i < secondRunCount; i++)
        {
            markdown.Append("_text_");
            markdown.Append("__text__");
        }
        
        var secondRun = GetRunTime(() => renderer.Render(markdown.ToString()));

        var coefficient = secondRun / firstRun;
        
        coefficient.Should().BeLessThan(expectedCoefficient);
    }

    private static double GetRunTime(Action run)
    {
        var sw = Stopwatch.StartNew();
        run.Invoke();
        sw.Stop();
        
        return sw.ElapsedMilliseconds;
    }
}