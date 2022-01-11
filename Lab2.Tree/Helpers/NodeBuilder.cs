using System.Collections.Generic;
using System.Linq;
using Lab2.Tree.Tokens;
using Lab2.Tree.Tree;

namespace Lab2.Tree.Helpers
{
    public class NodeBuilder
    {
        private readonly List<SingleOperationCallDto> _groups;

        public NodeBuilder(List<SingleOperationCallDto> groups)
        {
            _groups = groups;
        } 

        public Node Build() => CreateNodeFromSingleOperationCallDto(_groups.Last());

        private Node CreateNode(Token token) =>
            token.Type is TokenType.SingleOperationCall ? CreateNodeFromSingleOperationCallDto(token) : new Node(token);

        private Node CreateNodeFromSingleOperationCallDto(Token token)
        {
            var tokenGroup = _groups.Find(group => group.Name == token.Value);

            return CreateNodeFromSingleOperationCallDto(tokenGroup);
        }

        private Node CreateNodeFromSingleOperationCallDto(SingleOperationCallDto singleOperationCallDto)
        {
            var node = new Node(singleOperationCallDto.Operation)
            {
                LeftChild = CreateNode(singleOperationCallDto.FirstOperand),
                RightChild = CreateNode(singleOperationCallDto.SecondOperand)
            };

            return node;
        }
    }
}
