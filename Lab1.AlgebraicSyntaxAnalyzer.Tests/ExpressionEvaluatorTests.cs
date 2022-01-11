using System;
using System.Threading;
using FluentAssertions;
using Lab1.AlgebraicSyntaxAnalyzer.Evaluator;
using NUnit.Framework;

namespace Lab1.AlgebraicSyntaxAnalyzer.Tests
{
    public class ExpressionEvaluatorTests
    {
        private ExpressionEvaluator _expressionEvaluator;

        [OneTimeSetUp]
        public void SetUp()
        {
            _expressionEvaluator = new ExpressionEvaluator();
        }

        [Test]
        public void Test()
        {
            var expression = "*ab+/1.2)(-4(/";

            var result = _expressionEvaluator.Parse(expression);

            result.Should()
                  .BeEmpty();
        }
        
    }
}