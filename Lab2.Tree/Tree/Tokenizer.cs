using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Core;

namespace Lab2.Tree.Tree
{

    public enum TokenType
    {
        Plus,
        Minus,
        Multiply,
        Division,
        Operand,
        RBrace,
        LBrace,
        Group,
        EOF
    }

    public class Token
    {
        public Token(string value, TokenType type, int priority)
        {
            this.value = value;
            this.type = type;
            this.priority = priority;
        }

        public readonly string value;
        public readonly TokenType type;
        public readonly int priority;
    }

    public class Tokenizer
    {
        public Tokenizer(string expression)
        {
            var formated = RemoveSpaces(expression);
            iterator = new Iterator<char>(formated.ToList(), IterationOptions.ToCollectionEnd);
            tokens = new List<Token>();
        }

        public static string RemoveSpaces(string expression) =>
            Regex.Replace(expression, " ", "");


        private Dictionary<TokenType, char> OperationTypes { get; } = new()
        {
            { TokenType.Minus, '-' },
            { TokenType.Plus, '+' },
            { TokenType.Division, '/' },
            { TokenType.Multiply, '*' }
        };

        private const string operationChars = "[-+/*]";
        private const string operandChars = "[.a-zA-Z\\d]";

        private readonly Iterator<char> iterator;
        private readonly List<Token> tokens;

        public List<Token> TokenizeExpression()
        {
            while (!EndOfExpression())
            {
                tokens.Add(ToToken(iterator.CurrentElement));
                iterator.MoveForward();
            }

            return tokens;
        }

        private Token ToToken(char currentChar) => currentChar.ToString() switch
        {
            _ when IsNegativeOperand() => GenerateOperandToken(negative: true),
            _ when IsNegativeBracket() => GenerateBraceToken(negative: true),
            _ when IsBracket() => GenerateBraceToken(),
            var operand when Regex.IsMatch(operand, operandChars) => GenerateOperandToken(),
            var operation when Regex.IsMatch(operation, operationChars) => GenerateOperationToken(),
            _ => throw new Exception("Unknown expression character")
        };

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

            if (iterator.NextElement is not default(char) || EndsWithBrackets())
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

        private bool PreviousNullOrBracket()
        {
            var previous = iterator.LookAt(1, Direction.Backward);

            return previous is default(char) || previous is '(';
        }

        private bool IsNegativeOperand() => PreviousNullOrBracket()
                                            && Regex.IsMatch(iterator.NextElement.ToString(), operandChars);

        private bool IsBracket() => iterator.CurrentElement is '(' || iterator.CurrentElement is ')';

        private bool IsNegativeBracket() => PreviousNullOrBracket() && iterator.NextElement is '(';

        private bool EndsWithBrackets() => iterator.CurrentElement is ')' &&
                                           iterator.NextElement is default(char);

        private bool EndOfExpression() => iterator.CurrentElement is default(char);
    }
}
