using System;
using System.Collections.Generic;

namespace Lab5.Vector.Paralellizing
{
    public class VectorPrinter
    {
        public void Print(List<List<ParallelOperation>> parallelOperations)
        {
            foreach (var parallelLevel in parallelOperations)
            {
                foreach (var parallelOperation in parallelLevel)
                {
                    Console.WriteLine($"{parallelOperation.Operation.Name} = {parallelOperation.Operation.FirstOperand.Value}, " +
                                      $"{parallelOperation.Operation.Operation.Value}, " +
                                      $"{parallelOperation.Operation.SecondOperand.Value}; " +
                                      $"Core: {parallelOperation.Core.CoreName}; Layer: {parallelOperation.Layer}; Number: {parallelOperation.OperationNumber}");
                }

                Console.WriteLine();
            }
        }

    }
}
