using System.Collections.Generic;
using System.Linq;

namespace Lab2.Tree.Tree
{

    public class Node
    {
        public Node(Token token) => Token = token;

        public readonly Token Token;
        public Node LeftChild { get; set; }
        public Node RightChild { get; set; }
    }

    public class TreeBuilder
    {
        public TreeBuilder(List<Group> groups) => this.groups = groups;

        private readonly List<Group> groups;

        public Node Build() => CreateNodeFromGroup(groups.Last());

        private Node CreateNode(Token token) =>
            token.type is TokenType.Group ? CreateNodeFromGroup(token) : new Node(token);

        private Node CreateNodeFromGroup(Token token)
        {
            var tokenGroup = groups.Find(group => group.Name == token.value);

            return CreateNodeFromGroup(tokenGroup);
        }

        private Node CreateNodeFromGroup(Group group)
        {
            var node = new Node(group.Operation)
            {
                LeftChild = CreateNode(group.FirstOperand),
                RightChild = CreateNode(group.SecondOperand)
            };

            return node;
        }
    }
}