using System;
using System.Collections.Generic;
using System.Linq;
using Lab1.AlgebraicSyntaxAnalyzer.Errors;

namespace Lab1.AlgebraicSyntaxAnalyzer.Evaluator
{
    public class ParenthesesChecker
    {
        public List<ParseResultError> ParseParentheses(string expression)
        {
            var errors = new List<ParseResultError>();

            var parenthesesErrors = ValidateParentheses(expression);

            errors.AddRange(parenthesesErrors);

            return errors;
        }

        private List<ParseResultError> ValidateParentheses(string expression)
        {
            var errors = new List<ParseResultError>();

            if (expression.Contains(")") || expression.Contains("("))
            {
                var elementsList = expression.ToCharArray().Select(e => e.ToString()).ToList();

                var parentheses = new List<Parentheses>();

                for (var position = 0; position <= elementsList.Count - 1; position++)
                {
                    var parError = CheckParenthesesErrors(elementsList, parentheses, position, expression);

                    if (parError != null)
                    {
                        errors.Add(parError);
                    }
                }

                var parErrors = GetParenthesesErrors(parentheses, expression);

                errors.AddRange(parErrors);

            }

            return errors;
        }

        private ParseResultError CheckParenthesesErrors(List<string> charArray, List<Parentheses> parentheses,
            int position, string expression)
        {
            if (charArray[position] == "(")
            {
                var addedParentheses = parentheses.FirstOrDefault(p => p.Left == null);

                if (addedParentheses == null)
                {
                    parentheses.Add(new Parentheses()
                    {
                        Left = new Parentheses() { Position = position }
                    });
                }
                else
                {
                    addedParentheses.Left = new Parentheses()
                    {
                        Position = position
                    };
                }
            }

            if (charArray[position] == ")")
            {
                var addedParentheses = parentheses.FirstOrDefault(p => p.Right == null);

                if (addedParentheses == null)
                {
                    return new ParseResultError(")", position, expression, ErrorMessages.MissingOpeningParenthless);
                }
                else
                {
                    addedParentheses.Right = new Parentheses()
                    {
                        Position = position
                    };
                }
            }

            return null;
        }

        private List<ParseResultError> GetParenthesesErrors(List<Parentheses> parentheses, string expression)
        {
            var errors = new List<ParseResultError>();

            foreach (var element in parentheses)
            {
                if (element.Left == null)
                {
                    var error = new ParseResultError(")", element.Right.Position,
                        expression,
                        ErrorMessages.MissingOpeningParenthless);

                    errors.Add(error);
                }
                else if (element.Right == null)
                {
                    var error = new ParseResultError("(", element.Left.Position,
                        expression,
                        ErrorMessages.MissingClosingParenthless);

                    errors.Add(error);
                }
            }

            return errors;
        }
    }
}
