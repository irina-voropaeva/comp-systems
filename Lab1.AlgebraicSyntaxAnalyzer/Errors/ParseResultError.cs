namespace Lab1.AlgebraicSyntaxAnalyzer.Errors
{
    public class ParseResultError : Symbol
    {
        public string ErrorMessage { get; set; }

        public string Expression { get; set; }

        public ParseResultError(string value, int position, string expression, string errorMessage)
        {
            Value = value;
            Position = position;
            Expression = expression;
            ErrorMessage = errorMessage;
        }
    }
}
