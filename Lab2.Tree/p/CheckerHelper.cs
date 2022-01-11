using Core;

namespace Lab2.Tree
{
    public class CheckerHelper
    {
        public AvailableOperations AvailableOperations;
        public CheckerHelper()
        {
            AvailableOperations = new AvailableOperations();
        }

        public bool IsValue(string expression)
        {
            var allowedLetters = AvailableOperations.GetAllowedLetters();

            foreach (var value in expression)
            {
                if (!char.IsDigit(value) || !allowedLetters.Contains(value.ToString()))
                {
                    return false;
                }
            }

            return true;
        }

        public bool IsOperation(string source)
        {
            return AvailableOperations.AriphmeticOperations.Contains(source);
        }

        public bool IsOpenParentheses(string source)
        {
            return source == "(";
        }

        public bool IsCloseParentheses(string source)
        {
            return source == ")";
        }

        public bool IsParentheses(string source)
        {
            return IsOpenParentheses(source) || IsCloseParentheses(source);
        }

        public bool IsValidToken(string source)
        {
            return IsValue(source) || IsOperation(source) || IsParentheses(source);
        }
    }
}
