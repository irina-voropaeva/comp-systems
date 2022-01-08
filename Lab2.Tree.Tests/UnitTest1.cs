using System;
using NUnit.Framework;

namespace Lab2.Tree.Tests
{
    public class Tests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void Test1()
        {
            var builder = new ArithmeticTreeBuilder();

            var result = builder.Build("(a+b)*(c+d)/((a+b)-c)");

        }
    }
}