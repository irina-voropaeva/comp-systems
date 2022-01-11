namespace Lab2.Tree.Tree
{
    public class NodeInfo
    {
        public Node Node;
        public string Text;
        public int StartPos;

        public int Size
        {
            get { return Text.Length; }
        }

        public int EndPos
        {
            get { return StartPos + Size; }
            set { StartPos = value - Size; }
        }

        public NodeInfo Parent, Left, Right;
    }
}
