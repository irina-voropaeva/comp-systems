using System;

namespace Lab2.Tree.Tree
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var showInput = true;

            while (showInput)
            {
                var expression = "a+b*c/d-i+5*g*(o+p)";

                var tokenized = new Tokenizer(expression).TokenizeExpression();

                    tokenized.ForEach(token => Console.WriteLine(
                        $"Token: [{token.value}] Type: [{token.type}] Priority: [{token.priority}]"));

                var groups = new GroupsConstructor(tokenized).ConstructFromTokens();
                var root = new Tree.TreeBuilder(groups).Build();

                PrintMessage("Visualizing tree...", ConsoleColor.Yellow);
                TreePrinter.Print(root);

                Console.WriteLine(" ");
                //PrintMessage("Building layers...", ConsoleColor.Yellow);
                //Console.WriteLine(" ");

                //var orderedGroups = groups.ToLayeredGroupsDictionary();

                //foreach (var group in orderedGroups)
                //{
                //    var operations = group.Value.ConvertAll(expression =>
                //    $"( {expression.FirstOperand.value} {expression.Operation.value} {expression.SecondOperand.value} )");

                //    Console.WriteLine($"[ Layer: {group.Key} ] | {string.Join(" : ", operations)}");
                //}

                //Console.WriteLine(" ");

                //var orderedCommands = groups.ToLayeredCommandsDictionary();

                //foreach (var group in orderedCommands)
                //{
                //    var operations = group.Value.ConvertAll(command => $"( {command.Operation} )");

                //    Console.WriteLine($"[ Layer: {group.Key} ] | {string.Join(" : ", operations)}");
                //}

                //Console.WriteLine(" ");

                //PrintMessage("Optimize expression? Press Enter to show or other key to skip...", ConsoleColor.Yellow);
                //var optimizeInput = Console.ReadKey(true).Key;
                //var optimize = optimizeInput is ConsoleKey.Enter;

                //var commands = optimize ? groups.ToOptimalCommandsList() : groups.ToCommandsList();

                //Console.WriteLine(" ");
                //PrintMessage("Running pipeline...", ConsoleColor.Yellow);
                //Console.WriteLine(" ");

                //var pipeline = new DynamicPipeline(4, commands);
                //var executed = pipeline.Execute();

                //executed.ForEach(state => Console.WriteLine(state));

                Console.ReadKey();
            }
        }

        static void PrintMessage(string message, ConsoleColor color)
        {
            Console.ForegroundColor = color;
            Console.WriteLine(message);
            Console.ForegroundColor = ConsoleColor.White;
        }
    }
}

