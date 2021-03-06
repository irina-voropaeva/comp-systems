using System;
using Lab2.Tree.Helpers;

namespace Lab2.Tree
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var showInput = true;

            while (showInput)
            {
                var expression = "b/2*a+4*8-(3-4+x*y*3)";

                var tokenizer = new Tokenizer(expression);

                var tokenized = tokenizer.CreateTokensList();

                Console.WriteLine(expression);
                Console.WriteLine("Operations priority");

                foreach (var token in tokenized)
                {
                    Console.WriteLine($"Value: {token.Value} is {token.Type}; priority: {token.Priority}");
                }

                var singleOperationCallBuilder = new SingleOperationCallBuilder(tokenized);
                var singleOperationCallDtos = singleOperationCallBuilder.BuildFromTokens();

                var root = new NodeBuilder(singleOperationCallDtos).Build();

                TreePrinter.Print(root);

                Console.WriteLine(" ");

                Console.ReadKey();
            }
        }
    }
}

