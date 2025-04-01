using System;
using System.Collections.Generic;
using System.Linq;

namespace Course_Work_Compiller.models
{
    public class Parser
    {
        private List<Token> _tokens;
        private int _currentIndex;
        private List<string> _errors = new List<string>();

        public Parser(List<Token> tokens)
        {
            _tokens = tokens;
            _currentIndex = 0;
        }

        public void Parse()
        {
            ParseFunction();
            if (_errors.Count == 0)
                Console.WriteLine("Парсинг завершен успешно");
            else
                foreach (var error in _errors) Console.WriteLine(error);
        }

        private Token CurrentToken => 
            _currentIndex < _tokens.Count ? _tokens[_currentIndex] : null;

        private void MoveNext() => _currentIndex++;

        private void AddError(string expected, Token actual)
        {
            if (actual == null) return;
            _errors.Add($"Ошибка: Ожидалось {expected}, получено '{actual.Lexeme}' " +
                       $"(Позиция: {actual.Position})");
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
                SkipUntil(new[] {10,9});
                Console.WriteLine(CurrentToken.Code);
                if (CurrentToken?.Code == 10)
                {
                    Fun_Name();
                }
                else if (CurrentToken?.Code == 9)
                {
                    After_Fun_Name();
                }
            }
        }
        private void Fun_Name()
        {
            if (Match(10, "Идентификатор функции"))
            {
                After_Fun_Name();
            }
            else
            {
                SkipUntil(new[] {10,9});
                Console.WriteLine(CurrentToken.Code);
                if (CurrentToken?.Code == 10)
                {
                    ParseParameters();
                }
                else if (CurrentToken?.Code == 9)
                {
                    After_Fun_Name();
                }
            }
        }
        private void After_Fun_Name()
        {
            if (Match(9, "'('"))
            {
                ParseParameters();
            }
            else
            {
                SkipUntil(new[] {10,8});
                if (CurrentToken?.Code == 10)
                {
                    ParseParameters();
                }
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
            if (Match(8, "':'"))
            {
                Fun_Return_Type();
            }else
            {
                SkipUntil(new[] {2,3,4,5,12});
                if (CurrentToken?.Code >= 2 && CurrentToken?.Code <= 5)
                {
                    Fun_Return_Type();
                }
                else if (CurrentToken?.Code == 12)
                {
                    Start_Body_Fun();
                }
            }
        }

        private void Start_Body_Fun()
        {
            if (Match(12, "'{'"))
            {
                ParseReturnStatement();
            }
            else
            {
                SkipUntil(new[] {6});
                if (CurrentToken?.Code == 6)
                {
                    ParseReturnStatement();
                }
            }
        }

        private void End()
        {
            int lengthHistory = _errors.Count;
            Boolean result = Match(14, "';'");
            
            Console.WriteLine(_currentIndex);
            if (result == true)
            {
                
            }
            else if (result == false && (lengthHistory == _errors.Count) )
            {
                Token endToken = new Token();
                endToken.Code = 14;
                endToken.Lexeme = "Пробел";
                endToken.Position = $"{_currentIndex}";
                AddError("Ожидался конец объявления функции ';'", endToken);
            }
        }
        private void End_Body_Fun()
        {
            if (Match(13, "'}'"))
            {
                End();
            }
            else
            {
                SkipUntil(new[] {14});
                if (CurrentToken?.Code == 14)
                {
                    End();
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
                SkipUntil(new []{12});
                if (CurrentToken?.Code == 12)
                {
                    Start_Body_Fun();
                }
            }
        }
        
        private void Set_Type()
        {
            if (Match(8, "':'"))
            {
                Param_Type();
            }else
            {
                SkipUntil(new[] {2,3,4,5, 17, 11});
                if (CurrentToken?.Code >= 2 && CurrentToken?.Code <= 5)
                {
                    ParseParameters();
                }
                else if (CurrentToken?.Code == 17)
                {
                    Comma();
                }
                else if (CurrentToken?.Code == 11)
                {
                    After_Params();
                }
            }
        }

        private void Comma()
        {
            if (Match(17, "','"))
            {
                ParseParameter();
            }else
            {
                SkipUntil(new[] {10});
                if (CurrentToken.Code == 10)
                {
                    ParseParameter();
                }
            }
        }

        private void Param_Type()
        {
            if (CurrentToken?.Code >= 2 && CurrentToken?.Code <= 5)
            {
                Console.WriteLine($" Тип: {CurrentToken.Lexeme}");
                MoveNext();
                if (CurrentToken?.Code == 17)
                {
                    Comma();
                }
                else if (CurrentToken?.Code == 11)
                {
                    After_Params();
                }
            }
            else
            {
                AddError("тип данных (Int, Double и т.д.)", CurrentToken);
                SkipUntil(new []{17, 11, 8 });
                if (CurrentToken?.Code == 17)
                {
                    Comma();
                }
                else if (CurrentToken?.Code == 11)
                {
                    After_Params();
                }
                else if (CurrentToken?.Code == 8)
                {
                    Set_Type();
                }
            }
        }
        
        private void ParseParameters()
        {
            while (true)
            {
                if (CurrentToken?.Code == 10) // IDENT
                {
                    ParseParameter();
                    if (CurrentToken?.Code != 17) break;
                    MoveNext();
                }
                else
                {
                    After_Params();
                    break;
                }
            }
        }
        
        private void ParseParameter()
        {
            if (Match(10, "Параметр"))
            {
                Set_Type();
            }
            else
            {
                SkipUntil(new []{8});
                if (CurrentToken?.Code == 8)
                {
                    Set_Type();
                }
            }
        }
        
        private void ParseReturnStatement()
        {
            if (Match(6, "'return'"))
            {
                // Space_After_Return();
                Expression_Block();
            }
            else
            {
                SkipUntil(new []{10,9, 16,18,19,20});
                if (CurrentToken?.Code == 10)
                {
                    Expression_Block();
                }
                else if (CurrentToken?.Code == 9)
                {
                    Expression_Start_Bracket();
                }
                else if (CurrentToken?.Code != 17 && CurrentToken?.Code >= 16 && CurrentToken?.Code <= 20)
                {
                    AddError("Ожидался идентификатор или скобка", CurrentToken);
                    Expression_Operator();
                }
            }
            
        }
        private void Expression_Operator()
        {
            if (CurrentToken?.Code >= 16 && CurrentToken?.Code <= 20 && CurrentToken?.Code != 17)
            {
                MoveNext();
                if (CurrentToken?.Code == 9)
                {
                    Expression_Start_Bracket();
                }
                else if (CurrentToken?.Code == 10)
                {
                    Expression_Parameter();
                }
                else
                {
                    AddError("начало приритетного выражение или идентификатор", CurrentToken);
                }
            }
            else
            {
                AddError("оператор", CurrentToken);
                SkipUntil(new[] {16,18,19,13,20,10});
            }
        }

        private void Expression_Start_Bracket()
        {
            if(Match(9, "Начало приоритетного выражения"))
            {
                Expression_Parameter();
            }
            else
            {
                SkipUntil(new []{10});
                if (CurrentToken?.Code == 10)
                {
                    Expression_Parameter();
                }
            }
        }
        private void Expression_End_Bracket()
        {
            if(Match(11, "Конец приоритетного выражения"))
            {
                if (CurrentToken?.Code >= 16 && CurrentToken?.Code <= 20 && CurrentToken?.Code != 17)
                {
                    Expression_Operator();
                }
                else if (CurrentToken?.Code == 13)
                {
                    End_Body_Fun();
                }
                else
                {
                    AddError("Ожидался завершение тела функции или оператор", CurrentToken);
                }
            }
            else
            {
                SkipUntil(new []{13,16,18,19,20});
            }
        }
        private void Expression_Parameter()
        {
            if (Match(10, "Идентификатор переменной"))
            {
                if (CurrentToken?.Code == 13)
                {
                    End_Body_Fun();
                }
                else if (CurrentToken?.Code >= 16 && CurrentToken?.Code <= 20 && CurrentToken?.Code != 17)
                {
                    Expression_Operator();
                }
                else if (CurrentToken?.Code == 11)
                {
                    Expression_End_Bracket();
                }
                else
                {
                    AddError("Ожидался оператор, завершение тела или скобка", CurrentToken);
                }
            }
            else
            {
                SkipUntil(new []{16,18,19,20,13,11});
            }
        }
        private void Expression_Block()
        {
            if (CurrentToken?.Code == 9)
            {
                Expression_Start_Bracket();
            }
            else if (CurrentToken?.Code == 10)
            {
                Expression_Parameter();
            }
            else
            {   
                AddError("Ожидался идентификатор или скобка", CurrentToken);
                SkipUntil(new []{9,10});
                if (CurrentToken?.Code == 9)
                {
                    Expression_Start_Bracket();
                }
                else if (CurrentToken?.Code == 10)
                {
                    Expression_Parameter();
                }
            }
        }
        
        // private void Space_After_Return()
        // {
        //     if (Match(7, "пробел после 'return'"))
        //     {
        //         Expression_Block();
        //     }
        //     else
        //     {
        //         SkipUntil(new []{10});
        //         
        //     }
        // }
        
        private void ParseFunction()
        {
           Fun();
        }

        

        

        

        

    }
}