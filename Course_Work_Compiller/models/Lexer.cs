// using System.Collections.Generic;
//
// namespace Course_Work_Compiller.models;
//
// public class Lexer
// {
//     private readonly Dictionary<string, int> _keywords = new Dictionary<string, int>
//     {
//         { "fun", 1 }, { "Int", 2 }, { "Double", 3 }, { "Bool", 4 }, 
//         { "String", 5 }, { "return", 6 }
//     };
//
//     public List<Token> Analyze(string input)
//     {
//         var tokens = new List<Token>();
//         int pos = 0;
//         int line = 1;
//
//         while (pos < input.Length)
//         {
//             char current = input[pos];
//
//             // Пропуск пробелов
//             if (char.IsWhiteSpace(current))
//             {
//                 pos++;
//                 continue;
//             }
//
//             // Ключевые слова и идентификаторы
//             if (char.IsLetter(current))
//             {
//                 int start = pos;
//                 while (pos < input.Length && (char.IsLetterOrDigit(input[pos]) || input[pos] == '_') )
//                     pos++;
//
//                 string value = input.Substring(start, pos - start);
//                 if (_keywords.TryGetValue(value, out int code))
//                 {
//                     tokens.Add(new Token { 
//                         Code = code,
//                         Type = "Ключевое слово",
//                         Lexeme = value,
//                         Position = $"{start + 1}-{pos}"
//                     });
//                 }
//                 else
//                 {
//                     tokens.Add(new Token { 
//                         Code = 10,
//                         Type = "Идентификатор",
//                         Lexeme = value,
//                         Position = $"{start + 1}-{pos}"
//                     });
//                 }
//                 continue;
//             }
//
//             // Разделители и символы
//             switch (current)
//             {
//                 case '(':
//                     tokens.Add(new Token { Code = 9, Type = "Начало параметров", Lexeme = "(", Position = $"{pos + 1}" });
//                     break;
//                 case ')':
//                     tokens.Add(new Token { Code = 11, Type = "Конец параметров", Lexeme = ")", Position = $"{pos + 1}" });
//                     break;
//                 case '{':
//                     tokens.Add(new Token { Code = 12, Type = "Начало тела", Lexeme = "{", Position = $"{pos + 1}" });
//                     break;
//                 case '}':
//                     tokens.Add(new Token { Code = 13, Type = "Конец тела", Lexeme = "}", Position = $"{pos + 1}" });
//                     break;
//                 case ':':
//                     tokens.Add(new Token { Code = 8, Type = "Определение типа", Lexeme = ":", Position = $"{pos + 1}" });
//                     break;
//                 case ',':
//                     tokens.Add(new Token { Code = 7, Type = "Разделитель", Lexeme = ",", Position = $"{pos + 1}" });
//                     break;
//                 case ';':
//                     tokens.Add(new Token { Code = 14, Type = "Конец объявления", Lexeme = ";", Position = $"{pos + 1}" });
//                     break;
//                 default:
//                     if (!char.IsWhiteSpace(current))
//                     {
//                         tokens.Add(new Token { 
//                             Code = 15, 
//                             Type = "Ошибка", 
//                             Lexeme = current.ToString(), 
//                             Position = $"{pos + 1}" 
//                         });
//                     }
//                     break;
//             }
//             pos++;
//         }
//
//         return tokens;
//     }
// }


using System.Collections.Generic;

namespace Course_Work_Compiller.models;

public class Lexer
{
    private readonly Dictionary<string, int> _keywords = new Dictionary<string, int>
    {
        { "fun", 1 }, { "Int", 2 }, { "Double", 3 }, { "Bool", 4 }, 
        { "String", 5 }, { "return", 6 }
    };

    public List<Token> Analyze(string input)
    {
        var tokens = new List<Token>();
        int pos = 0;
        int line = 1;

        while (pos < input.Length)
        {
            char current = input[pos];

            // Обработка пробелов (code 7)
            if (char.IsWhiteSpace(current))
            {
                tokens.Add(new Token { 
                    Code = 7,
                    Type = "Разделитель",
                    Lexeme = " ",
                    Position = $"{pos + 1}"
                });
                pos++;
                continue;
            }

            // Ключевые слова и идентификаторы
            if (char.IsLetter(current))
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
                continue;
            }

            // Операторы и символы
            switch (current)
            {
                case '+':
                case '-':
                case '*':
                    tokens.Add(new Token { 
                        Code = 16, 
                        Type = "Оператор", 
                        Lexeme = current.ToString(), 
                        Position = $"{pos + 1}" 
                    });
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
                    tokens.Add(new Token { Code = 7, Type = "Разделитель", Lexeme = ",", Position = $"{pos + 1}" });
                    break;
                case ';':
                    tokens.Add(new Token { Code = 14, Type = "Конец объявления", Lexeme = ";", Position = $"{pos + 1}" });
                    break;
                default:
                    tokens.Add(new Token { 
                        Code = 15, 
                        Type = "Ошибка", 
                        Lexeme = current.ToString(), 
                        Position = $"{pos + 1}" 
                    });
                    break;
            }
            pos++;
        }

        return tokens;
    }
}