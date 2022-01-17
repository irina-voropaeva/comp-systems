using System;
using Lab2.Tree;
using Lab2.Tree.Helpers;
using Lab5.Vector.Paralellizing;

namespace Lab5.Vector
{
    public class Program
    {
        static void Main(string[] args)
        {
            var showInput = true;

            while (showInput)
            {
                var expression = "a*c*d*g*y*u*i*o";
                    //"b/2*a+4*8-(3-4+x*y*3)";

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

                Console.WriteLine("");

                Console.WriteLine("Available: 2*; 2+; 1/");

                Console.WriteLine("");

                var parallelizer = new VectorParalellizer();

                var orderedGroups = parallelizer.GetOrderedSubGroups(singleOperationCallDtos);

                var result = parallelizer.ParallelizeBetweenCores(orderedGroups);

                var vectorPrinter = new VectorPrinter();

                vectorPrinter.Print(result);

                Console.ReadKey();
            }
        }
    }
}
