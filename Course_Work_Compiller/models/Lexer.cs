using System.Collections.Generic;

namespace Course_Work_Compiller.models;

public class Lexer
{
    private readonly Dictionary<string, int> _keywords = new Dictionary<string, int>
    {
        { "fun", 1 }, { "Int", 2 }, { "Double", 3 }, { "Bool", 4 }, 
        { "String", 5 }, { "return", 6 }
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

            // Обработка пробелов
            if (char.IsWhiteSpace(current))
            {
                if (_expectSpace)
                {
                    tokens.Add(new Token { 
                        Code = 7,
                        Type = "Пробел",
                        Lexeme = " ",
                        Position = $"{pos + 1}"
                    });
                    _expectSpace = false;
                }
                pos++;
                continue;
            }

            // Ключевые слова и идентификаторы
            if (char.IsLetter(current) || current == '_')
            {
                int start = pos;
                while (pos < input.Length && (char.IsLetterOrDigit(input[pos]) || input[pos] == '_'))
                    pos++;

                string value = input.Substring(start, pos - start);

                if (_keywords.TryGetValue(value, out int code))
                {
                    tokens.Add(new Token { 
                        Code = code,
                        Type = "Ключевое слово",
                        Lexeme = value,
                        Position = $"{start + 1}-{pos}"
                    });
                    
                    // Устанавливаем флаг ожидания пробела для fun и return
                    if (code == 1 || code == 6) _expectSpace = true;
                }
                else
                {
                    if (ContainsCyrillic(value))
                    {
                        tokens.Add(new Token { 
                            Code = 15, 
                            Type = "Ошибка", 
                            Lexeme = value, 
                            Position = $"{start + 1}-{pos}" 
                        });
                    }
                    else
                    {
                        tokens.Add(new Token { 
                            Code = 10,
                            Type = "Идентификатор",
                            Lexeme = value,
                            Position = $"{start + 1}-{pos}"
                        });
                    }
                }
                continue;
            }

            // Проверка на русские буквы
            if (IsCyrillic(current))
            {
                int start = pos;
                while (pos < input.Length && IsCyrillic(input[pos]))
                    pos++;

                string value = input.Substring(start, pos - start);
                tokens.Add(new Token { 
                    Code = 15, 
                    Type = "Ошибка", 
                    Lexeme = value, 
                    Position = $"{start + 1}-{pos}" 
                });
                continue;
            }

            // Операторы и символы
            switch (current)
            {
                case '+':
                    tokens.Add(new Token { Code = 16, Type = "Оператор", Lexeme = "+", Position = $"{pos + 1}" });
                    break;
                case '-':
                    tokens.Add(new Token { Code = 18, Type = "Оператор", Lexeme = "-", Position = $"{pos + 1}" });
                    break;
                case '*':
                    tokens.Add(new Token { Code = 19, Type = "Оператор", Lexeme = "*", Position = $"{pos + 1}" });
                    break;
                case '/':
                    tokens.Add(new Token { Code = 20, Type = "Оператор", Lexeme = "/", Position = $"{pos + 1}" });
                    break;
                case '(':
                    tokens.Add(new Token { Code = 9, Type = "Начало параметров", Lexeme = "(", Position = $"{pos + 1}" });
                    break;
                case ')':
                    tokens.Add(new Token { Code = 11, Type = "Конец параметров", Lexeme = ")", Position = $"{pos + 1}" });
                    break;
                case '{':
                    tokens.Add(new Token { Code = 12, Type = "Начало тела", Lexeme = "{", Position = $"{pos + 1}" });
                    break;
                case '}':
                    tokens.Add(new Token { Code = 13, Type = "Конец тела", Lexeme = "}", Position = $"{pos + 1}" });
                    break;
                case ':':
                    tokens.Add(new Token { Code = 8, Type = "Определение типа", Lexeme = ":", Position = $"{pos + 1}" });
                    break;
                case ',':
                    tokens.Add(new Token { Code = 17, Type = "Запятая", Lexeme = ",", Position = $"{pos + 1}" });
                    break;
                case ';':
                    tokens.Add(new Token { Code = 14, Type = "Конец объявления", Lexeme = ";", Position = $"{pos + 1}" });
                    break;
                default:
                    if (char.IsDigit(current))
                    {
                        int start = pos;
                        while (pos < input.Length && char.IsDigit(input[pos]))
                            pos++;
                        tokens.Add(new Token {
                            Code = 21,
                            Type = "Ошибка",
                            Lexeme = input.Substring(start, pos - start),
                            Position = $"{start + 1}-{pos}"
                        });
                        pos--;
                    }
                    else
                    {
                        tokens.Add(new Token { 
                            Code = 15, 
                            Type = "Ошибка", 
                            Lexeme = current.ToString(), 
                            Position = $"{pos + 1}" 
                        });
                    }
                    break;
            }
            pos++;
            _expectSpace = false; // Сбрасываем флаг после любого символа
        }

        return tokens;
    }

    private bool IsCyrillic(char c)
    {
        return (c >= 'А' && c <= 'я') || (c >= 'Ё' && c <= 'ё');
    }

    private bool ContainsCyrillic(string value)
    {
        foreach (char c in value)
        {
            if (IsCyrillic(c)) return true;
        }
        return false;
    }
}