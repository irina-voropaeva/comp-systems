namespace Lab1.AlgebraicSyntaxAnalyzer.Errors
{
    public static class ErrorMessages
    {
        public static string StartExpressionSymbolInvalid = "Expression cannot starts from symbol. It should starts from letter or variable.";

        public static string EndExpressionSymbolInvalid = "Expression cannot ends with symbol. It should starts from letter or variable.";

        public static string OperationDuplicate = "Operation symbol duplicates. Please correct.";

        public static string MissingOperationBetweenVariables = "Operation symbol between two parameters is missed. Please correct";

        public static string MissingOperationBetweenParenthless = "Operation symbol between two parenthless is missed. Please correct";

        public static string MissingOpeningParenthless = "Opening parenthless is missing, please check";

        public static string MissingClosingParenthless = "Closing parenthless is missing, please check";

        public static string UnallowedSymbolsSequence = "Symbols sequence is unallowed, please check";


        public static string EmptyParenthless = "Empty parenthless, please check";

        public static string EmptyExpression = "Expression is empty. Please check";


    }
}
