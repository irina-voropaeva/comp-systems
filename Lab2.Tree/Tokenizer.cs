using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Core;
using Core.Iterating;
using Lab2.Tree.Helpers;
using Lab2.Tree.Tokens;

namespace Lab2.Tree
{
    public class Tokenizer
    {
        private readonly AvailableOperations _availableOperations;

        public Tokenizer(string expression)
        {
            iterator = new Iterator<char>(expression.ToList(), IterationOptions.ToCollectionEnd);

            tokens = new List<Token>();
            _availableOperations = new AvailableOperations();
        }

        private Dictionary<TokenType, char> OperationTypes { get; } = new()
        {
            { TokenType.Minus, '-' },
            { TokenType.Plus, '+' },
            { TokenType.Division, '/' },
            { TokenType.Multiply, '*' }
        };

        private const string operandChars = "[.a-zA-Z\\d]";

        private readonly Iterator<char> iterator;
        private readonly List<Token> tokens;

        public List<Token> CreateTokensList()
        {
            while (!TokenHelper.EndOfExpression(iterator))
            {
                tokens.Add(ToToken(iterator.CurrentElement));
                iterator.MoveForward();
            }

            return tokens;
        }

        private Token ToToken(char currentChar)
        {
            return currentChar.ToString() switch
            {
                _ when TokenHelper.IsNegativeOperand(iterator) => GenerateOperandToken(negative: true),
                _ when TokenHelper.IsNegativeBracket(iterator) => GenerateBraceToken(negative: true),
                _ when TokenHelper.IsBracket(iterator) => GenerateBraceToken(),
                var operand when Regex.IsMatch(operand, operandChars) => GenerateOperandToken(),
                var operation when _availableOperations.AriphmeticOperations.Contains(operation) =>
                    GenerateOperationToken(),
                _ => throw new Exception("Unknown expression character")
            };
        }

        private Token GenerateOperationToken()
        {
            var current = iterator.CurrentElement;
            var type = OperationTypes.First(operationType => operationType.Value == current);
            var priority = (type.Value == '+') || (type.Value == '-') ? 1 : 2;

            return new Token(current.ToString(), type.Key, priority);
        }

        private Token GenerateOperandToken(bool negative = false)
        {
            var operand = new StringBuilder();

            if (negative)
            {
                operand.Append(iterator.CurrentElement);
                iterator.MoveForward();
            }

            while (Regex.IsMatch(iterator.CurrentElement.ToString(), operandChars))
            {
                operand.Append(iterator.CurrentElement);
                iterator.MoveForward();
            }

            if (iterator.NextElement is not default(char) || TokenHelper.EndsWithBrackets(iterator))
                iterator.MoveBackward();

            return new Token(operand.ToString(), TokenType.Operand, 0);
        }

        private Token GenerateBraceToken(bool negative = false)
        {
            if (negative) iterator.MoveForward();

            var type = iterator.CurrentElement is '(' ? TokenType.LBrace : TokenType.RBrace;
            var negativeSign = negative ? "-" : "";

            return new Token($"{negativeSign}{iterator.CurrentElement}", type, 3);
        }
    }
}
