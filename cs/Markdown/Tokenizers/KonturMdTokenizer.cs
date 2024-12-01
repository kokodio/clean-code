using System.Text;

namespace Markdown.Tokenizers;

public class KonturMdTokenizer : ITokenizer
{
    public List<Token> Tokenize(string markdown)
    {
        var text = new StringBuilder();
        var tokens = GetTokens(markdown, text);
        
        CreateTokenPairs(tokens, TokenType.Italic, markdown);
        CreateTokenPairs(tokens, TokenType.Strong, markdown);
        FilterStrongInsideItalic(tokens);
        FlushText(tokens, text);
        
        return tokens;
    }

    private void FlushText(List<Token> tokens, StringBuilder text)
    {
        if (text.Length > 0)
            tokens.Add(CreateTextToken(text.ToString()));
    }

    private void FilterStrongInsideItalic(List<Token> tokens)
    {
        var isItalicOpen = false;
        Token? currentItalicOpening = null;

        foreach (var token in tokens)
        {
            if (token.Pair == null) 
                continue;

            switch (token.Type)
            {
                case TokenType.Italic:
                    isItalicOpen = !isItalicOpen;
                    currentItalicOpening = isItalicOpen ? token : null;
                    break;
                case TokenType.Strong when isItalicOpen:
                    RemoveTokenPair(currentItalicOpening);
                    RemoveTokenPair(token);
                    isItalicOpen = false;
                    break;
            }
        }
    }
    
    private void RemoveTokenPair(Token? token)
    {
        if (token?.Pair == null) 
            return;

        token.Pair.Pair = null;
        token.Pair = null;
    }

    private List<Token> GetTokens(string text, StringBuilder sb)
    {
        var tokens = new List<Token>();
        
        for (var index = 0; index < text.Length;)
        {
            switch (text[index])
            {
                case '\\':
                    index = HandleEscape(text, index, sb);
                    break;

                case '_':
                    index = HandleUnderscore(text, index, sb, tokens);
                    break;

                default:
                    sb.Append(text[index++]);
                    break;
            }
        }
        
        return tokens;
    }

    private int HandleEscape(string markdown, int index, StringBuilder text)
    {
        if (index + 1 < markdown.Length)
        {
            text.Append(markdown[index + 1]);
            return index + 2;
        }

        text.Append(markdown[index]);
        return index + 1;
    }

    private int HandleUnderscore(string markdown, int index, StringBuilder text, List<Token> tokens)
    {
        var count = CountUnderscores(markdown, index);
        var tokenType = count == 2 ? TokenType.Strong : TokenType.Italic;

        if (IsValidDelimiter(markdown, index, count, out var isOpening, out var isClosing))
        {
            if (text.Length > 0)
            {
                tokens.Add(CreateTextToken(text.ToString()));
                text.Clear();
            }

            tokens.Add(new Token
            {
                Type = tokenType,
                Content = new string('_', count),
                IsOpening = isOpening,
                IsClosing = isClosing,
                Position = index
            });

            return index + count;
        }

        text.Append(new string('_', count));
        return index + count;
    }
    
    private int CountUnderscores(string text, int index)
    {
        var result = 0;
        
        if (text[index] == '_') result++;
        if (index + 1 < text.Length && text[index + 1] == '_') result++;
        
        return result;
    }

    private bool IsValidDelimiter(string markdown, int index, int length, out bool isOpening, out bool isClosing)
    {
        var before = index > 0 
            ? markdown[index - 1] 
            : '\0';
        var after = index + length < markdown.Length 
            ? markdown[index + length] 
            : '\0';

        isClosing = IsDelimiter(before);
        isOpening = IsDelimiter(after);

        return isOpening || isClosing;
    }

    private bool IsDelimiter(char value) => value != '_' && (char.IsLetter(value) || char.IsPunctuation(value));

    private Token CreateTextToken(string content) => new() { Type = TokenType.Text, Content = content };
    
    private void CreateTokenPairs(List<Token> tokens, TokenType type, string markdown)
    {
        var pairableTokens = new Stack<int>();
        
        for (var i = 0; i < tokens.Count; i++)
        {
            var token = tokens[i];
            if (type != token.Type) continue;

            switch (token.IsOpening, token.IsClosing)
            {
                case (true, false):
                    pairableTokens.Push(i);
                    break;

                case (false, true):
                    if (pairableTokens.Count > 0)
                    {
                        var openingIndex = pairableTokens.Pop();
                        tokens[openingIndex].Pair = tokens[i];
                        tokens[i].Pair = tokens[openingIndex];
                    }
                    break;

                case (true, true):
                    if (pairableTokens.Count > 0)
                    {
                        var openingIndex = pairableTokens.Peek();
                        var lenght = token.Position - tokens[openingIndex].Position;
                        var slice = markdown.AsSpan().Slice(tokens[openingIndex].Position, lenght);

                        if (!slice.Contains(' '))
                        {
                            pairableTokens.Pop();
                            tokens[openingIndex].Pair = tokens[i];
                            tokens[i].Pair = tokens[openingIndex];
                        }
                    }
                    else
                    {
                        pairableTokens.Push(i);
                    }
                    break;
            }
        }
    }
}