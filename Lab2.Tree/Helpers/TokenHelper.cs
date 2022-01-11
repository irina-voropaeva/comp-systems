using System.Text.RegularExpressions;
using Core;
using Core.Iterating;

namespace Lab2.Tree.Helpers
{
    public static class TokenHelper
    {
        public static bool PreviousNullOrBracket(Iterator<char> iterator)
        {
            var previous = iterator.LookAt(1, Direction.Backward);

            return previous is default(char) || previous is '(';
        }

        public static bool IsNegativeOperand(Iterator<char> iterator) => PreviousNullOrBracket(iterator) 
            && (new AvailableOperations()).AriphmeticOperations.Contains(iterator.NextElement.ToString());

        public static bool IsBracket(Iterator<char> iterator) => iterator.CurrentElement is '(' || iterator.CurrentElement is ')';

        public static bool IsNegativeBracket(Iterator<char> iterator) => PreviousNullOrBracket(iterator) && iterator.NextElement is '(';

        public static bool EndsWithBrackets(Iterator<char> iterator) => iterator.CurrentElement is ')' &&
                                                                        iterator.NextElement is default(char);

        public static bool EndOfExpression(Iterator<char> iterator) => iterator.CurrentElement is default(char);
    }
}
