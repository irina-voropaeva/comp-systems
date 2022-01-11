namespace Lab2.Tree
{
    public class Token
    {
        public char[] Source;
        public int? Start;
        public int End;
        public int Depth;
        public Token(char[] source, int? start, int end)
        {
            Source = source;
            Start = start;
            End = end;
            Depth = 0;
        }

        public override string ToString()
        {
            return $"Token({Source})";
        }
    }
}
