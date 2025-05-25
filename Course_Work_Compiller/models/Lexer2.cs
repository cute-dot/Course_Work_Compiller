using System;
using System.Collections.Generic;

namespace RecursiveDescentParser
{
    public enum TokenType
    {
        KEYWORD, ID, CONST, AO, AS, SEMICOLON, EOF, ERROR, WHITESPACE
    }
    public class Token
    {
        public TokenType Type { get; }
        public string Value { get; }
        public string Position { get; }

        public Token(TokenType type, string value, string position)
        {
            Type = type;
            Value = value;
            Position = position;
        }

        public override string ToString() => $"('{Type}', '{Value}')";
    }

    public class Lexer2
    {
        private readonly Dictionary<string, TokenType> _keywords = new Dictionary<string, TokenType>
        {
            { "for", TokenType.KEYWORD },
            { "to", TokenType.KEYWORD },
            { "do", TokenType.KEYWORD },
            { "and", TokenType.KEYWORD },
            { "or", TokenType.KEYWORD }
        };

        private bool _expectSpace = false;

        public List<Token> Analyze(string input)
        {
            var tokens = new List<Token>();
            int pos = 0;
            _expectSpace = false;

            while (pos < input.Length)
            {
                char current = input[pos];

                if (char.IsWhiteSpace(current))
                {
                    if (_expectSpace)
                    {
                        tokens.Add(new Token(TokenType.WHITESPACE, " ", $"{pos + 1}"));
                        _expectSpace = false;
                    }
                    pos++;
                    continue;
                }
                if (char.IsLetter(current))
                {
                    int start = pos;
                    while (pos < input.Length && (char.IsLetterOrDigit(input[pos])))
                        pos++;

                    string value = input.Substring(start, pos - start);

                    if (_keywords.TryGetValue(value, out TokenType type))
                    {
                        tokens.Add(new Token(type, value, $"{start + 1}-{pos}"));
                        _expectSpace = (value == "for" || value == "to" || value == "do");
                    }
                    else if (IsValidIdentifier(value))
                    {
                        tokens.Add(new Token(TokenType.ID, value, $"{start + 1}-{pos}"));
                    }
                    else
                    {
                        tokens.Add(new Token(TokenType.ERROR, value, $"{start + 1}-{pos}"));
                    }
                    continue;
                }
                if (char.IsDigit(current))
                {
                    int start = pos;
                    while (pos < input.Length && char.IsDigit(input[pos]))
                        pos++;

                    string value = input.Substring(start, pos - start);
                    tokens.Add(new Token(TokenType.CONST, value, $"{start + 1}-{pos}"));
                    continue;
                }
                if (pos + 1 < input.Length && input[pos] == ':' && input[pos + 1] == '=')
                {
                    tokens.Add(new Token(TokenType.AS, ":=", $"{pos + 1}-{pos + 2}"));
                    pos += 2;
                    _expectSpace = true;
                    continue;
                }
                switch (current)
                {
                    case '+':
                    case '-':
                        tokens.Add(new Token(TokenType.AO, current.ToString(), $"{pos + 1}"));
                        pos++;
                        break;
                    case '=':
                        tokens.Add(new Token(TokenType.AS, "=", $"{pos + 1}"));
                        pos++;
                        break;
                    case ';':
                        tokens.Add(new Token(TokenType.SEMICOLON, ";", $"{pos + 1}"));
                        pos++;
                        break;
                    default:
                        tokens.Add(new Token(TokenType.ERROR, current.ToString(), $"{pos + 1}"));
                        pos++;
                        break;
                }
                _expectSpace = false;
            }

            tokens.Add(new Token(TokenType.EOF, "", $"{pos + 1}"));
            return tokens;
        }
        private bool IsValidIdentifier(string value)
        {
            if (string.IsNullOrEmpty(value) || !char.IsLetter(value[0]))
                return false;
            foreach (char c in value)
            {
                if (!char.IsLetterOrDigit(c))
                    return false;
            }
            return true;
        }
    }
}