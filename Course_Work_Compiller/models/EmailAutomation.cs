using System.Collections.Generic;
using System.Text;

public class EmailAutomation
{
    private enum State
    {
        Start,
        LocalPart,
        AfterAt,
        DomainPart,
        AfterDot,
        TldPart,
        ValidEmail
    }

    public List<MatchResult> FindEmails(string text)
    {
        var matches = new List<MatchResult>();
        var currentState = State.Start;
        int startIndex = -1;
        var currentMatch = new StringBuilder();

        for (int i = 0; i < text.Length; i++)
        {
            char c = text[i];

            switch (currentState)
            {
                case State.Start:
                    if (IsValidLocalPartChar(c))
                    {
                        startIndex = i;
                        currentMatch.Append(c);
                        currentState = State.LocalPart;
                    }
                    break;

                case State.LocalPart:
                    if (IsValidLocalPartChar(c))
                    {
                        currentMatch.Append(c);
                    }
                    else if (c == '@')
                    {
                        currentMatch.Append(c);
                        currentState = State.AfterAt;
                    }
                    else
                    {
                        ResetState(ref currentState, ref startIndex, currentMatch);
                    }
                    break;

                case State.AfterAt:
                    if (IsValidDomainChar(c) && c != '.') // Запрещаем точку сразу после @
                    {
                        currentMatch.Append(c);
                        currentState = State.DomainPart;
                    }
                    else
                    {
                        ResetState(ref currentState, ref startIndex, currentMatch);
                    }
                    break;

                case State.DomainPart:
                    if (IsValidDomainChar(c))
                    {
                        currentMatch.Append(c);
                    }
                    else if (c == '.')
                    {
                        currentMatch.Append(c);
                        currentState = State.AfterDot;
                    }
                    else if (IsWordBoundary(c))
                    {
                        // Проверяем, что домен содержит точку после @
                        string current = currentMatch.ToString();
                        if (current.IndexOf('.', current.IndexOf('@')) > 0)
                        {
                            currentState = State.ValidEmail;
                            i--; // Возвращаемся на символ назад, чтобы обработать границу
                        }
                        else
                        {
                            ResetState(ref currentState, ref startIndex, currentMatch);
                        }
                    }
                    else
                    {
                        ResetState(ref currentState, ref startIndex, currentMatch);
                    }
                    break;


                case State.AfterDot:
                    if (char.IsLetter(c))
                    {
                        currentMatch.Append(c);
                        currentState = State.TldPart;
                    }
                    else
                    {
                        ResetState(ref currentState, ref startIndex, currentMatch);
                    }
                    break;

                case State.TldPart:
                    if (char.IsLetter(c))
                    {
                        currentMatch.Append(c);
                    }
                    else if (IsWordBoundary(c))
                    {
                        currentState = State.ValidEmail;
                        i--; // Возвращаемся на символ назад
                    }
                    else
                    {
                        ResetState(ref currentState, ref startIndex, currentMatch);
                    }
                    break;

                case State.ValidEmail:
                    AddValidEmail(matches, startIndex, currentMatch);
                    ResetState(ref currentState, ref startIndex, currentMatch);
                    break;
            }
        }

        // Обработка email в конце строки
        if (currentState == State.TldPart || currentState == State.DomainPart)
        {
            // Проверяем, что домен содержит точку
            string email = currentMatch.ToString();
            if (email.Contains('@') && email.Substring(email.IndexOf('@')).Contains('.'))
            {
                AddValidEmail(matches, startIndex, currentMatch);
            }
        }

        return matches;
    }

    private void ResetState(ref State state, ref int startIndex, StringBuilder currentMatch)
    {
        state = State.Start;
        startIndex = -1;
        currentMatch.Clear();
    }

    private void AddValidEmail(List<MatchResult> matches, int startIndex, StringBuilder currentMatch)
    {
        string email = currentMatch.ToString();
        // Дополнительная проверка на минимальную длину TLD (2 символа)
        if (email.Contains('.') && 
            email.Length - email.LastIndexOf('.') - 1 >= 2 && 
            !email.EndsWith(".") && 
            !email.Contains("@."))
        {
            matches.Add(new MatchResult
            {
                Index = startIndex,
                Value = email
            });
        }
    }

    private bool IsValidLocalPartChar(char c)
    {
        return char.IsLetterOrDigit(c) || c == '.' || c == '_' || c == '%' || c == '+' || c == '-';
    }

    private bool IsValidDomainChar(char c)
    {
        return char.IsLetterOrDigit(c) || c == '-' || c == '.';
    }

    private bool IsWordBoundary(char c)
    {
        return char.IsWhiteSpace(c) || c == '\t' || c == '\n' || c == '\r' || 
               c == ',' || c == ';' || c == ':' || c == '"' || c == '\'' ||
               c == '(' || c == ')' || c == '[' || c == ']' || c == '{' || c == '}' ||
               c == '<' || c == '>' || c == '!' || c == '?';
    }
}

public class MatchResult
{
    public int Index { get; set; }
    public string Value { get; set; }
}
