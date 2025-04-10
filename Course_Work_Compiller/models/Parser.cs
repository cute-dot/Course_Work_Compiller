﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace Course_Work_Compiller.models
{
    public class ParseError
    {
        public string Результат { get; set; }
        public string Тип { get; set; }
    }
    public class Parser
    {
        private List<Token> _tokens;
        private int _currentIndex;
        private List<ParseError> _errors = new List<ParseError>();

        public Parser(List<Token> tokens)
        {
            _tokens = tokens;
            _currentIndex = 0;
        }

        public List<ParseError> Errors
        {
            get => _errors;
            set => _errors = value ?? throw new ArgumentNullException(nameof(value));
        }
        // public List<ParseError> Errors
        // {
        //     get => _errors;
        //     set => _errors = value ?? throw new ArgumentNullException(nameof(value));
        // }

        public void Parse()
        {
            ParseFunction();
            if (Errors.Count == 0)
            {
                Errors.Add(new ParseError 
                {
                    Результат = "Парсинг успешно завершен",
                    Тип = "Информация"
                });
            }
            else
                foreach (var error in _errors) Console.WriteLine(error);
        }

        private Token CurrentToken => 
            _currentIndex < _tokens.Count ? _tokens[_currentIndex] : null;

        private void MoveNext() => _currentIndex++;

        private void AddError(string expected, Token actual)
        {
            if (actual == null) return;
            // _errors.Add($"Ошибка: Ожидалось {expected}, получено '{actual.Lexeme}' " +
            //            $"(Позиция: {actual.Position})");
            Errors.Add(new ParseError(){
                Результат = $"Ожидалось {expected}, получено '{actual.Lexeme}' (Позиция: {actual.Position})",
                Тип = "Ошибка"
            });
        }
        private bool Match(int expectedCode, string expectedName)
        {
            if (CurrentToken?.Code == expectedCode)
            {
                MoveNext();
                return true;
            }
            AddError(expectedName, CurrentToken);
            MoveNext();
            return false;
        }

        private void SkipUntil(int[] stopTokens)
        {
            if (stopTokens.Contains(CurrentToken.Code))
            {
                
            }
            else
            {
                while (CurrentToken != null &&  !stopTokens.Contains(CurrentToken.Code)  )
                {
                    MoveNext();
                }
            }
        }
        
        private void Fun()
        {
            if (Match(1, "ключевое слово 'fun '"))
            {
                Fun_Name();
            }else
            {
                // SkipUntil(new[] {10,9});
                Console.WriteLine(CurrentToken.Code);
                if (CurrentToken?.Code == 10)
                {
                    Fun_Name();
                }
                else
                {
                    Fun_Name();
                }
            }
        }
        private void Fun_Name()
        {
            if (CurrentToken?.Code == 10)
            {
                MoveNext();
                After_Fun_Name();
            }
            else
            {
                AddError("Идентификатор функции", CurrentToken);
                MoveNext();
                After_Fun_Name();
                // SkipUntil(new[] {10,9});
                // Console.WriteLine(CurrentToken.Code);
                // if (CurrentToken?.Code == 10)
                // {
                //     ParseParameters();
                // }
                // else if (CurrentToken?.Code == 9)
                // {
                //     After_Fun_Name();
                // }
                // else
                // {
                //     After_Fun_Name();
                // }
            }
        }
        private void After_Fun_Name()
        {
            if (CurrentToken?.Code == 9)
            {
                MoveNext();
                ParseParameters();
            }
            else
            {
                AddError("'('", CurrentToken);
                if (CurrentToken?.Code == 10)
                {
                    ParseParameters();
                }
                else
                {
                    ParseParameters();
                }
                // SkipUntil(new[] {10,8});
                // if (CurrentToken?.Code == 10)
                // {
                //     ParseParameters();
                // }
            }
        }
        
        private void After_Params()
        {
            if (Match(11, "')'"))
            {
                Set_Return_Type();
            }
            else
            {
                SkipUntil(new[] {8,2,3,4,5});
                if (CurrentToken?.Code == 8)
                {
                    Set_Return_Type();
                }
                else if(CurrentToken?.Code >= 2 && CurrentToken.Code <= 5){
                    Fun_Return_Type();
                }
            }
        }
        private void Set_Return_Type()
        {
            if (CurrentToken?.Code == 8 )
            {
                MoveNext();
                Fun_Return_Type();
            }else
            {
                AddError("Ожидалось : ", CurrentToken);
                Fun_Return_Type();
            }
        }

        private void Start_Body_Fun()
        {
            
            if (CurrentToken.Code == 12)
            {
                MoveNext();
                ParseReturnStatement();
            }
            else
            {
                if (!_tokens.Any(token => token.Code == 12))
                {
                    AddError("'{'", CurrentToken);
                    ParseReturnStatement();
                }
            }
        }
        private void Fun_Return_Type()
        {
            if (CurrentToken?.Code >= 2 && CurrentToken?.Code <= 5)
            {
                Console.WriteLine($" Тип: {CurrentToken.Lexeme}");
                MoveNext();
                Start_Body_Fun();
            }
            else
            {
                AddError("тип данных (Int, Double и т.д.)", CurrentToken);
                MoveNext();
                Start_Body_Fun();
            }
        }
        
        private void Set_Type()
        {
            if (CurrentToken?.Code == 8)
            {
                MoveNext();
                Param_Type();
            } else
            {
                AddError(":", CurrentToken);
                Param_Type();
                
            }
        }
        private void Comma()
        {
            if (CurrentToken?.Code == 17)
            {
                ParseParameter();
            }else
            {
                if (CurrentToken?.Code == 11)
                {
                    After_Params();
                    return;
                }
                AddError("','", CurrentToken);
                ParseParameter();
                // SkipUntil(new[] {10});
                // if (CurrentToken.Code == 10)
                // {
                //     ParseParameter();
                // }
            }
        }
        private void Param_Type()
        {
            if (CurrentToken?.Code >= 2 && CurrentToken?.Code <= 5)
            {
                MoveNext();
                Comma();
            }
            else
            {
                AddError("тип данных (Int, Double и т.д.)", CurrentToken);
                MoveNext();
                Comma();
            }
        }
        private void ParseParameters()
        {
            while (true)
            {
                if (CurrentToken?.Code == 10) 
                {
                    ParseParameter();
                    if (CurrentToken?.Code != 17) break;
                    MoveNext();
                }
                else
                {
                    AddError("Ожидался идентификатор или скобка", CurrentToken);
                    ParseParameter();
                    break;
                }
            }
        }
        private void ParseParameter()
        {
            if (CurrentToken?.Code == 10)
            {
                MoveNext();
                Set_Type();
            }
            else
            {
                SkipUntil(new []{8,2,3,4,5});
                if (CurrentToken?.Code == 8)
                {
                    Set_Type();
                } else if (CurrentToken?.Code >= 2 && CurrentToken?.Code <= 5)
                {
                    Param_Type();
                }
            }
        }
        private void ParseReturnStatement()
        {
            if (Match(6, "'return'"))
            {
                Expression_Block();
            }
            else
            {
                // SkipUntil(new []{10,9, 16,18,19,20});
                Expression_Block();
            }
        }
        private void Expression_Block()
        {
            ParseExpression();
            if (!_tokens.Any(token => token.Code == 13))
            {
                Token endToken = new Token();
                endToken.Code = 13;
                endToken.Lexeme = " ";
                endToken.Position = $"{_currentIndex}";
                AddError("Ожидался конец объявления функции '}'", endToken);
            }
            if (!_tokens.Any(token => token.Code == 14))
            {
                Token endToken = new Token();
                endToken.Code = 14;
                endToken.Lexeme = " ";
                endToken.Position = $"{_currentIndex}";
                AddError("Ожидался конец объявления функции ';'", endToken);
            }
            
        }
        // CurrentToken.Code != 13 && CurrentToken.Code != 14
        private void ParseExpression()
    {
        while (CurrentToken != null && ( CurrentToken.Code == 11 
                                         || CurrentToken.Code == 15 
                                         || CurrentToken.Code == 9 
                                         || CurrentToken.Code == 10 
                                         || CurrentToken.Code == 16 
                                         || CurrentToken.Code == 18 
                                         || CurrentToken.Code == 19 
                                         ||CurrentToken.Code == 20 ) ) // '}' или ';'
        {
            // Проверка на неожиданный токен (code == 15)
            if (CurrentToken.Code == 15)
            {
                AddError("Неожиданный токен в выражении", CurrentToken);
                MoveNext();
                continue;
            }

            switch (CurrentToken.Code)
            {
                case 9: // '('
                    MoveNext();
                    ParseExpression();
                    if (!Match(11, "')'")) return;
                    MoveNext();
                    break;

                case 10: // IDENT
                         // Сохраняем предыдущий токен
                    var prevToken = _currentIndex > 0 ? _tokens[_currentIndex - 1] : null;
                    MoveNext();

                    // Пропускаем проверку если следующий токен неожиданный (code == 15)
                    if (CurrentToken != null && CurrentToken.Code != 15 &&
                        (CurrentToken.Code == 10 || CurrentToken.Code == 9))
                    {
                        // Между двумя идентификаторами/числами/скобкой должен быть оператор
                        AddError("Ожидался оператор между выражениями", CurrentToken);
                    }
                    break;

                case 16:
                case 18:
                case 19:
                case 20: // Операторы
                    MoveNext();
                    // Пропускаем проверку если следующий токен неожиданный (code == 15)
                    if (CurrentToken != null && CurrentToken.Code != 15 &&
                        CurrentToken.Code != 10 && CurrentToken.Code != 9)
                    {
                        AddError("Ожидалось выражение после оператора", CurrentToken);
                    }
                    break;

                case 11: // ')'
                    return;

                default:
                    AddError("Ожидалась часть выражения", CurrentToken);
                    MoveNext();
                    break;
            }
            
        }
        
    }
        private void ParseFunction()
        {
           Fun();
        }
    }
}


