namespace Lab2.Tree.Tokens
{
    public class Token
    {
        public Token(string value, TokenType type, int priority)
        {
            Value = value;
            Type = type;
            Priority = priority;
        }

        public readonly string Value;
        public readonly TokenType Type;
        public readonly int Priority;
    }
}
