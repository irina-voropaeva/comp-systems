using System.Collections.Generic;
using System.Linq;
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

        private Iterator<Token> _iterator;
        private readonly List<SingleOperationCallDto> _groups;
        private SingleOperationCallDto _previous = null;
        private Iterator<Token> _previousCandidate = null;

        private readonly List<TokenType> _groupableTokens = new()
        {
            TokenType.LBrace,
            TokenType.Operand,
            TokenType.SingleOperationCall
        };

        public List<SingleOperationCallDto> BuildFromTokens()
        {
            while (!IsLast()) //[1] = [0]/d; add([1]-i)
            {
                var isCurrentOperationPrioritied = IsOperation(_iterator.CurrentElement) &&
                                                _previous?.Operation.Priority >= _iterator.CurrentElement.Priority;

                var nextNextTokenPos2 = _iterator.LookAt(2, Direction.Forward);

                var isNextOperationPrioritied = nextNextTokenPos2 != null && IsOperation(nextNextTokenPos2) && nextNextTokenPos2.Priority >= _iterator.CurrentElement.Priority;

                if (IsValidForProcess() || (isCurrentOperationPrioritied && !isNextOperationPrioritied))
                {
                    if ((IsOperation(_iterator.CurrentElement) && _previous?.Operation.Priority >= _iterator.CurrentElement.Priority))
                    {
                        var startIndex = _iterator.CurrentIndex;
                        var callLength = 2;

                        var singleCallList = new List<Token>
                        {
                           new Token(_previous.Name, TokenType.SingleOperationCall, 0)
                        };

                        for (var i = 0; i < callLength; i++)
                        {
                            singleCallList.Add(_iterator.CurrentElement);
                            _iterator.MoveForward();
                        }

                        var result = CreateSingleOperationCall(startIndex, singleCallList, callLength);


                        if (result)
                        {
                            _iterator.ChangeCollection(tokens => tokens.RemoveRange(_previous.BeginIndex - 1, 1));
                        }
                        else
                        {
                            _iterator.MoveForward();
                        }
                    }
                    else
                    {
                        var processed = ProcessSingleOperationCall();
                        if (!processed)
                        {
                            _iterator.MoveForward();
                        }
                    }
                    
                } 
                //else if ()
                //{

                //}
                else
                {
                    _iterator.MoveForward();
                }
            }
            
            var temp = _groups.Select(x => $"{x.FirstOperand.Value}, {x.Operation.Value}, {x.SecondOperand.Value}");

            return _groups;
        }

        public bool IsOperation(Token token)
        {
            return token.Type == TokenType.Division
                   || token.Type == TokenType.Minus
                   || token.Type == TokenType.Multiply
                   || token.Type == TokenType.Plus;
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

        private bool CreateSingleOperationCall(int startIndex, List<Token> groupTokens, int? count = null)
        {
            var layer = _iterator.Iteration;
            var group = new SingleOperationCallDto($"[{_groups.Count}]", startIndex, layer, groupTokens);

            if (!IsCorrectSingleOperationCall(group))
            {
                _iterator.MoveTo(startIndex);
                return false;
            }

            _previous = group;
            _groups.Add(group);
            ReplaceWithToken(group, count);

            return true;
        }

        private void ReplaceWithToken(SingleOperationCallDto singleOperationCallDto, int? count = null)
        {
            var countToDeelte = count ?? singleOperationCallDto.Size;

            var token = new Token(singleOperationCallDto.Name, TokenType.SingleOperationCall, 0);
            
            _iterator.ChangeCollection(tokens => tokens.RemoveRange(singleOperationCallDto.BeginIndex, countToDeelte));
            _iterator.ChangeCollection(tokens => tokens.Insert(singleOperationCallDto.BeginIndex, token));
        }

        private bool IsCorrectSingleOperationCall(SingleOperationCallDto group)
        {
            var higherPriority = group.Operation.Priority >= _iterator.CurrentElement.Priority ||
                                 group.Size is 5 || _iterator.CurrentElement.Type is TokenType.RBrace;

            var correctOperrads = group.FirstOperand.Type is not TokenType.RBrace &&
                                  group.SecondOperand.Type is not TokenType.LBrace;

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
