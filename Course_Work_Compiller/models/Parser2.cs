using System;
using System.Collections.Generic;
using RecursiveDescentParser;

public class Parser2
    {
        private readonly Lexer2 lexer;
        private readonly string input;
        private Token currentToken;
        private readonly List<string> errors = new List<string>();
        private readonly List<Token> tokens;
        private int pos = 0;

        public List<string> Errors => errors;

        public Parser2(Lexer2 lexer, string input)
        {
            this.lexer = lexer;
            this.input = input;
            tokens = lexer.Analyze(input);
            for (int i = 0; i < tokens.Count; i++)
            {
                if (tokens[i].Type == TokenType.WHITESPACE)
                {
                    tokens.RemoveAt(i);
                }
            }
            currentToken = NextToken();
        }

        public int ErrorCount => errors.Count;

        private Token NextToken()
        {
            return pos < tokens.Count ? tokens[pos] : new Token(TokenType.EOF, "", "");
        }

        private void Consume()
        {
            pos++;
            currentToken = NextToken();
        }

        private bool Consume(TokenType expectedType, string expectedValue = null)
        {
            if (expectedValue != null)
            {
                if (currentToken.Type == expectedType && currentToken.Value == expectedValue)
                {
                    Consume();
                    return true;
                }
                errors.Add($"Ожидался {expectedType} '{expectedValue}', получен {currentToken}");
                return false;
            }
            else
            {
                if (currentToken.Type == expectedType)
                {
                    Consume();
                    return true;
                }
                errors.Add($"Ожидался {expectedType}, получен {currentToken}");
                return false;
            }
        }

        private void SkipToSemicolon()
        {
            Consume();
        }

        public bool Parse()
        {
            bool success = ParseFor();
            if (errors.Count > 0)
            {
                Console.WriteLine("Разбор завершён с ошибками:");
                foreach (var error in errors)
                {
                    Console.WriteLine($" - {error}");
                }
                return false;
            }
            return success;
        }

        private bool ParseFor()
        {
            if (!Consume(TokenType.KEYWORD, "for"))
            {
                SkipToSemicolon();
            }
            if (!Consume(TokenType.ID))
            {
                SkipToSemicolon();
                
            }
            if (!Consume(TokenType.AS, ":="))
            {
                SkipToSemicolon();
                
            }
            if (!ParseOperand())
            {
                SkipToSemicolon();
                
            }
            if (!Consume(TokenType.KEYWORD, "to"))
            {
                SkipToSemicolon();
                
            }
            if (!ParseOperand())
            {
                SkipToSemicolon();
                
            }
            if (!Consume(TokenType.KEYWORD, "do"))
            {
                SkipToSemicolon();
                
            }
            if (!ParseStmt())
            {
                SkipToSemicolon();
                
            }
            if (!Consume(TokenType.SEMICOLON))
            {
                return false;
            }
            return true;
        }

        private bool ParseOperand()
        {
            if (currentToken.Type == TokenType.ID || currentToken.Type == TokenType.CONST)
            {
                Consume();
                return true;
            }
            errors.Add($"Ожидался ID или CONST, получен {currentToken}");
            return false;
        }

        private bool ParseStmt()
        {
            if (!Consume(TokenType.ID))
            {
                SkipToSemicolon();
                return false;
            }
            if (!Consume(TokenType.AS, "="))
            {
                SkipToSemicolon();
                return false;
            }
            return ParseArithExpr();
        }

        private bool ParseArithExpr()
        {
            if (!ParseOperand())
            {
                return false;
            }
            while (currentToken.Type == TokenType.AO)
            {
                Consume();
                if (!ParseOperand())
                {
                    return false;
                }
            }
            return true;
        }
    }