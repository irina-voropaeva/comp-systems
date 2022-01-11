using System.Collections.Generic;
using Core;

namespace Lab2.Tree.Tree
{

    public class Group
    {
        public Group(string name, int beginIndex, int layer, List<Token> tokens)
        {
            Name = name;
            BeginIndex = beginIndex;
            Layer = layer;
            Size = tokens.Count;
            Initialize(tokens);
        }

        public string Name { get; init; }
        public int BeginIndex { get; init; }
        public int Size { get; init; }
        public int Layer { get; init; }
        public Token FirstOperand { get; private set; }
        public Token Operation { get; private set; }
        public Token SecondOperand { get; private set; }

        private void Initialize(List<Token> tokens)
        {
            var shift = tokens[0].type is TokenType.LBrace ? 1 : 0;

            FirstOperand = tokens[0 + shift];
            Operation = tokens[1 + shift];
            SecondOperand = tokens[2 + shift];
        }
    }

    public class GroupsConstructor
    {
        public GroupsConstructor(List<Token> tokens)
        {
            iterator = new Iterator<Token>(tokens, IterationOptions.KeepIterating);
            groups = new List<Group>();
        }

        private readonly Iterator<Token> iterator;
        private readonly List<Group> groups;

        private readonly List<TokenType> groupableTokens = new()
        {
            TokenType.LBrace,
            TokenType.Operand,
            TokenType.Group
        };

        public List<Group> ConstructFromTokens()
        {
            while (!IsLastGroup())
            {
                if (CanStartCapturing())
                {
                    var captured = CaptureGroup();
                    if (!captured) iterator.MoveForward();
                }
                else iterator.MoveForward();
            }

            return groups;
        }

        private bool CanStartCapturing() => groupableTokens.Contains(iterator.CurrentElement.type);

        private bool CaptureGroup()
        {
            if (NextTokenInvalid()) return false;

            if (HasOpeningBracket() && !HasClosingBracket()) return false;

            var groupTokens = new List<Token>();
            var startIndex = iterator.CurrentIndex;
            var groupLength = IsBracketGroup() ? 5 : 3;

            if (HasOpeningBracket() && groupLength == 3) iterator.MoveForward();

            for (int i = 0; i < groupLength; i++)
            {
                groupTokens.Add(iterator.CurrentElement);
                iterator.MoveForward();
            }

            return CreateGroup(startIndex, groupTokens);
        }

        private bool CreateGroup(int startIndex, List<Token> groupTokens)
        {
            var layer = iterator.Iteration;
            var group = new Group($"[{groups.Count}]", startIndex, layer, groupTokens);

            if (!IsCorrectGroup(group))
            {
                iterator.MoveTo(startIndex);
                return false;
            }

            groups.Add(group);
            ReplaceWithToken(group);

            return true;
        }

        private void ReplaceWithToken(Group group)
        {
            var token = new Token(group.Name, TokenType.Group, 0);

            iterator.ChangeCollection(tokens => tokens.RemoveRange(group.BeginIndex, group.Size));
            iterator.ChangeCollection(tokens => tokens.Insert(group.BeginIndex, token));
        }

        private bool IsCorrectGroup(Group group)
        {
            var higherPriority = group.Operation.priority >= iterator.CurrentElement.priority ||
                                 group.Size is 5 || iterator.CurrentElement.type is TokenType.RBrace;

            var correctOperrads = group.FirstOperand.type is not TokenType.RBrace &&
                                  group.SecondOperand.type is not TokenType.LBrace;

            return higherPriority && correctOperrads;
        }

        private bool NextTokenInvalid() => iterator.NextElement is default(Token) ||
                                           iterator.NextElement.type is TokenType.RBrace;

        private bool IsLastGroup() => iterator.CollectionLength is 0;

        private bool IsBracketGroup() => HasOpeningBracket() && HasClosingBracket();

        private bool HasOpeningBracket() => iterator.CurrentElement.type is TokenType.LBrace;

        private bool HasClosingBracket()
        {
            var last = iterator.LookAt(4, Direction.Forward);
            return last is not default(Token) && last.type is TokenType.RBrace;
        }
    }
}
