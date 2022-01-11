using System.Collections.Generic;

namespace Lab2.Tree
{
    public class Node
    {
        public int Id;
        public Token Token;
        public List<Node> Childs;
        public int Depth;
        public int? Phase;

        public Node (Token token)
        {
            Token = token;
            Childs = new List<Node>();
            Depth = 0;
            Phase = null;
        }

        public void AddChild(Node node)
        {
            Childs.Add(node);
        }

        public void Plot()
        {

        }

        public override string ToString()
        {
            return $"Node({Token.Source})";
        }
    }
}
