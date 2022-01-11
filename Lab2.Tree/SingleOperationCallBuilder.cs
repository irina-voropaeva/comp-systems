using System.Collections.Generic;
using Core.Iterating;
using Lab2.Tree.Tokens;

namespace Lab2.Tree
{
    public class SingleOperationCallBuilder
    {
        public SingleOperationCallBuilder(List<Token> tokens)
        {
            _iterator = new Iterator<Token>(tokens, IterationOptions.KeepIterating);
            _groups = new List<SingleOperationCallDto>();
        }

        private readonly Iterator<Token> _iterator;
        private readonly List<SingleOperationCallDto> _groups;

        private readonly List<TokenType> _groupableTokens = new()
        {
            TokenType.LBrace,
            TokenType.Operand,
            TokenType.SingleOperationCall
        };

        public List<SingleOperationCallDto> BuildFromTokens()
        {
            while (!IsLast())
            {
                if (IsValidForProcess())
                {
                    var processed = ProcessSingleOperationCall();
                    if (!processed)
                    {
                        _iterator.MoveForward();
                    }
                }
                else
                {
                    _iterator.MoveForward();
                }
            }

            return _groups;
        }

        private bool IsValidForProcess()
        {
            return _groupableTokens.Contains(_iterator.CurrentElement.Type);
        }

        private bool ProcessSingleOperationCall()
        {
            if (NextTokenInvalid())
            {
                return false;
            }

            if (HasOpeningParenthesis() && !HasClosingParenthesis())
            {
                return false;
            }

            var singleCallList = new List<Token>();
            var startIndex = _iterator.CurrentIndex;
            var callLength = IsParenthesesSingleOperationCall() ? 5 : 3;

            if (HasOpeningParenthesis() && callLength == 3)
            {
                _iterator.MoveForward();
            }

            for (var i = 0; i < callLength; i++)
            {
                singleCallList.Add(_iterator.CurrentElement);
                _iterator.MoveForward();
            }

            return CreateSingleOperationCall(startIndex, singleCallList);
        }

        private bool CreateSingleOperationCall(int startIndex, List<Token> groupTokens)
        {
            var layer = _iterator.Iteration;
            var group = new SingleOperationCallDto($"[{_groups.Count}]", startIndex, layer, groupTokens);

            if (!IsCorrectSingleOperationCall(group))
            {
                _iterator.MoveTo(startIndex);
                return false;
            }

            _groups.Add(group);
            ReplaceWithToken(group);

            return true;
        }

        private void ReplaceWithToken(SingleOperationCallDto singleOperationCallDto)
        {
            var token = new Token(singleOperationCallDto.Name, TokenType.SingleOperationCall, 0);

            _iterator.ChangeCollection(tokens => tokens.RemoveRange(singleOperationCallDto.BeginIndex, singleOperationCallDto.Size));
            _iterator.ChangeCollection(tokens => tokens.Insert(singleOperationCallDto.BeginIndex, token));
        }

        private bool IsCorrectSingleOperationCall(SingleOperationCallDto singleOperationCallDto)
        {
            var higherPriority = singleOperationCallDto.Operation.Priority >= _iterator.CurrentElement.Priority ||
                                 singleOperationCallDto.Size is 5 || _iterator.CurrentElement.Type is TokenType.RBrace;

            var correctOperrads = singleOperationCallDto.FirstOperand.Type is not TokenType.RBrace &&
                                  singleOperationCallDto.SecondOperand.Type is not TokenType.LBrace;

            return higherPriority && correctOperrads;
        }

        private bool NextTokenInvalid() => _iterator.NextElement is default(Token) ||
                                           _iterator.NextElement.Type is TokenType.RBrace;

        private bool IsLast() => _iterator.CollectionLength is 0;

        private bool IsParenthesesSingleOperationCall() => HasOpeningParenthesis() && HasClosingParenthesis();

        private bool HasOpeningParenthesis() => _iterator.CurrentElement.Type is TokenType.LBrace;

        private bool HasClosingParenthesis()
        {
            var last = _iterator.LookAt(4, Direction.Forward);
            return last is not default(Token) && last.Type is TokenType.RBrace;
        }
    }
}
