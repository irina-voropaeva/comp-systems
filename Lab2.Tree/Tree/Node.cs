using Lab2.Tree.Tokens;

namespace Lab2.Tree.Tree
{
    public class Node
    {
        public Node(Token token) => Token = token;

        public readonly Token Token;
        public Node LeftChild { get; set; }
        public Node RightChild { get; set; }
    }
}
