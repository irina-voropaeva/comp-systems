using System.Collections.Generic;
using Lab2.Tree.Tokens;

namespace Lab2.Tree
{
    public class SingleOperationCallDto
    {
        public string Name { get; init; }
        public int BeginIndex { get; init; }
        public int Size { get; init; }
        public int Layer { get; init; }
        public Token FirstOperand { get; }
        public Token Operation { get; }
        public Token SecondOperand { get; }

        public SingleOperationCallDto(string name, int beginIndex, int layer, List<Token> tokens)
        {
            Name = name;
            BeginIndex = beginIndex;
            Layer = layer;
            Size = tokens.Count;

            //initialize
            var shift = tokens[0].Type is TokenType.LBrace ? 1 : 0;

            FirstOperand = tokens[0 + shift];
            Operation = tokens[1 + shift];
            SecondOperand = tokens[2 + shift];
        }
    }
}
