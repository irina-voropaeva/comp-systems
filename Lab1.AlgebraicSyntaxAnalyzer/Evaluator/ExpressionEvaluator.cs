using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using Core;
using Lab1.AlgebraicSyntaxAnalyzer.Errors;

namespace Lab1.AlgebraicSyntaxAnalyzer.Evaluator
{
    public class ExpressionEvaluator : DynamicObject
    {
        private readonly ParenthesesChecker _parenthesesChecker;

        public ExpressionEvaluator()
        {
            _parenthesesChecker = new ParenthesesChecker();
        }

        public List<ParseResultError> Parse(string expression)
        {
            var trimedExpression = expression.Trim();

            var errors = Validate(trimedExpression);

            if (errors.Any())
            {
                return errors;
            }

            var parsed = _parenthesesChecker.ParseParentheses(trimedExpression);

            errors.AddRange(parsed);

            return errors;
        }

        private List<ParseResultError> Validate(string expression)
        {
            var errors = new List<ParseResultError>();

            if (string.IsNullOrEmpty(expression))
            {
                var error = new ParseResultError("", 0, expression, ErrorMessages.EmptyExpression);

                errors.Add(error);

                return errors;
            }

            var startEndErrors = CheckStartEndExpression(expression);
            errors.AddRange(startEndErrors);

            var duplicatesErrors = CheckDuplicates(expression);
            errors.AddRange(duplicatesErrors);

            var unallowedSymbolsErrors = CheckSequenceOfSymbols(
                AvailableOperations.GetUnallowedSymbolsDict(),
                expression, 
                ErrorMessages.UnallowedSymbolsSequence);

            errors.AddRange(unallowedSymbolsErrors);

            return errors;
        }

        private List<ParseResultError> CheckDuplicates(string expression)
        {
            var errors = new List<ParseResultError>();

            var parenthesesDuplicates = AddErrorForSymbol(expression, "()", ErrorMessages.EmptyParenthless);
            errors.AddRange(parenthesesDuplicates);

            var minusDuplicates = AddErrorForSymbol(expression, "--", ErrorMessages.OperationDuplicate);
            errors.AddRange(minusDuplicates);

            var divideDuplicates = AddErrorForSymbol(expression, "//", ErrorMessages.OperationDuplicate);
            errors.AddRange(divideDuplicates);

            var multiplyDuplicates = AddErrorForSymbol(expression, "**", ErrorMessages.OperationDuplicate);
            errors.AddRange(multiplyDuplicates);

            var noOperationBetweenParenthese = AddErrorForSymbol(
                                                                    expression, 
                                                                    ")(", 
                                                                    ErrorMessages.MissingOperationBetweenParenthless);
            errors.AddRange(noOperationBetweenParenthese);

            return errors;
        }

        private List<ParseResultError> CheckStartEndExpression(string expression)
        {
            var errors = new List<ParseResultError>();
            if (expression.StartsWith(")")
                || expression.StartsWith("+")
                || expression.StartsWith("-")
                || expression.StartsWith("*")
                || expression.StartsWith("/"))
            {
                var error = new ParseResultError(expression[0].ToString(), 0, expression, ErrorMessages.StartExpressionSymbolInvalid);

                errors.Add(error);
            }

            if (expression.EndsWith("(")
                || expression.EndsWith("+")
                || expression.EndsWith("-")
                || expression.EndsWith("*")
                || expression.EndsWith("/"))
            {
                var error = new ParseResultError(expression[^1].ToString(), expression.Length - 1, expression, ErrorMessages.EndExpressionSymbolInvalid);

                errors.Add(error);
            }

            return errors;
        }

        private List<ParseResultError> AddErrorForSymbol(string expression, string symbolToFind, string errorMessage)
        {
            var errors = new List<ParseResultError>();

            var copiedExpression = string.Join("", expression.ToList());

            var isPresented = true;

            var oldIndex = 0;

            while (isPresented)
            {
                if (copiedExpression.Contains(symbolToFind))
                {
                    if (oldIndex > copiedExpression.Length)
                    {
                        break;
                    }

                    var index = expression.IndexOf(symbolToFind, oldIndex, StringComparison.Ordinal);

                    oldIndex = index + 1;

                    var clearer = copiedExpression.Remove(copiedExpression.IndexOf(symbolToFind, StringComparison.Ordinal), symbolToFind.Length);

                    copiedExpression = clearer;

                    var error = new ParseResultError(symbolToFind, index, expression, errorMessage);

                    errors.Add(error);
                }
                else
                {
                    isPresented = false;
                }
            }

            return errors;
        }

        private List<ParseResultError> CheckSequenceOfSymbols(Dictionary<string, List<string>> unallowedSymbols,
            string expression, string errorMessage)
        {
            var errors = new List<ParseResultError>();

            for (var i = 0; i <= expression.Length - 1; i++)
            {
                var valueForError = expression[i].ToString();

                var position = i;

                var previous = expression[i];

                for (var j = i + 1; j <= expression.Length - 1; j++)
                { 
                    if (j > i && j != i + 1)
                    {
                        previous = expression[j];
                    }

                    var isDotNext = expression[j] == '.';

                    var isNextDotAndAfterDotNonDidgit = !char.IsDigit(previous)
                                                        && expression[j] == '.'
                                                        && j + 1 <= expression.Length
                                                        && char.IsDigit(expression[j + 1]);

                    var isNextSymbolUnallowed = (char.IsLetter(previous) && char.IsDigit(expression[j]))
                                                || (char.IsDigit(previous) && char.IsLetter(expression[j]))
                                                || (isNextDotAndAfterDotNonDidgit
                                                    || (!char.IsDigit(previous) 
                                                        && unallowedSymbols[previous.ToString()].Contains(expression[j].ToString())));

                    if (isNextSymbolUnallowed)
                    {
                        valueForError = valueForError + expression[j];
                    }
                    else
                    {
                        i = j > 0 ? j - 1 : j;
                        break;
                    }

                    i = j;
                }

                if (valueForError != expression[position].ToString())
                {
                    var error = new ParseResultError(valueForError, position, expression, errorMessage);

                    errors.Add(error);
                }
                
            }

            return errors;
        }
    }
}
