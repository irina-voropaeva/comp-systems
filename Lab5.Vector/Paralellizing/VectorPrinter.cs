using System;
using System.Collections.Generic;
using System.Linq;
using Lab2.Tree;

namespace Lab5.Vector.Paralellizing
{
    public class VectorPrinter
    {
        public void PrintParallelOperations(List<List<ParallelOperation>> parallelOperations)
        {
            foreach (var parallelLevel in parallelOperations)
            {
                foreach (var parallelOperation in parallelLevel)
                {
                    Console.WriteLine(
                        $"{parallelOperation.Operation.Name} = {parallelOperation.Operation.FirstOperand.Value} " +
                        $"{parallelOperation.Operation.Operation.Value} " +
                        $"{parallelOperation.Operation.SecondOperand.Value}; " +
                        $"Core: {parallelOperation.Core.CoreName}; " +
                        $"Layer: {parallelOperation.Layer}; " +
                        $"Operation total number: {parallelOperation.OperationNumber}");
                }

                if (parallelLevel.Any())
                {
                    Console.WriteLine();
                }
            }
        }

        public void PrintOrderedSubGroups(List<List<SingleOperationCallDto>> calls)
        {
            foreach (var parallelCall in calls)
            {
                foreach (var parallelOperation in parallelCall)
                {
                    Console.WriteLine($"{parallelOperation.Name} = {parallelOperation.FirstOperand.Value} " +
                                      $"{parallelOperation.Operation.Value} " +
                                      $"{parallelOperation.SecondOperand.Value} ");
                }

                if (parallelCall.Any())
                {
                    Console.WriteLine();
                }
            }
        }
    }
}
