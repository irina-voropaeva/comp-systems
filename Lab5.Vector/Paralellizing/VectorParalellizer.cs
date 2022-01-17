using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using Lab2.Tree;
using Lab2.Tree.Tokens;

namespace Lab5.Vector.Paralellizing
{
    public class VectorParalellizer
    {
        private List<ProcessorCore> _availableCores = new List<ProcessorCore>()
        {
            new ProcessorCore(new List<string>() { "+", "-" }, 1),
            new ProcessorCore(new List<string>() { "+", "-" }, 2),
            new ProcessorCore(new List<string>() { "*" }, 3),
            new ProcessorCore(new List<string>() { "*" }, 4),
            new ProcessorCore(new List<string>() { "/" }, 5),
        };

        private List<ParallelOperation> _parallelOperations;

        public VectorParalellizer()
        {
            _parallelOperations = new List<ParallelOperation>();
        }

        public List<List<ParallelOperation>> ParallelizeBetweenCores(List<List<SingleOperationCallDto>> orderedGroups)
        {
            var parallellOperationsList = new List<List<ParallelOperation>>()
            {
                new List<ParallelOperation>()
            };

            //calculate simple group, all operands are known 
            var parallelizePartialResult = new ParallelizePartialResult()
            {
                J = 0,
                Layer = 1
            };

            ParallelizeSimpleCalls(orderedGroups, parallellOperationsList, parallelizePartialResult);

            return parallellOperationsList;
        }

        public List<List<SingleOperationCallDto>> GetOrderedSubGroups(List<SingleOperationCallDto> groups)
        {
            var orderedgroup1 = new List<SingleOperationCallDto>();
            var orderedgroup2 = new List<SingleOperationCallDto>();
            var orderedgroup3 = new List<SingleOperationCallDto>();


            orderedgroup1.AddRange(groups.Where(g => g.FirstOperand.Type == TokenType.Operand
                                                     && g.SecondOperand.Type == TokenType.Operand));

            orderedgroup2.AddRange(groups.Where(g => (g.FirstOperand.Type == TokenType.SingleOperationCall
                                                      && g.SecondOperand.Type == TokenType.Operand)
                                                     || (g.FirstOperand.Type == TokenType.Operand
                                                         && g.SecondOperand.Type == TokenType.SingleOperationCall)));

            orderedgroup3.AddRange(groups.Where(g => (g.FirstOperand.Type == TokenType.SingleOperationCall
                                                      && g.SecondOperand.Type == TokenType.SingleOperationCall)));

            var result = new List<List<SingleOperationCallDto>>();

            result.Add(orderedgroup1);
            result.Add(orderedgroup2);
            result.Add(orderedgroup3);

            return result;
        }

        private  void ParallelizeSimpleCalls(List<List<SingleOperationCallDto>> orderedGroups, List<List<ParallelOperation>> parallellOperationsList, ParallelizePartialResult parallelizePartialResult)
        {
            while (orderedGroups[0].Any())
            {
                var availableCore = _availableCores.FirstOrDefault(ac =>
                    !ac.IsBusy && ac.AvailableOperations.Contains(orderedGroups[0][0].Operation.Value));

                if (availableCore == null)
                {
                    parallelizePartialResult.Layer++; // no available core for this operation, need to continue on layer 2

                    _availableCores.ForEach(ac => ac.IsBusy = false);

                    parallellOperationsList.Add(new List<ParallelOperation>());

                    continue;
                }

                var parallelOperation = new ParallelOperation(
                                                    availableCore, 
                                                    orderedGroups[0][0], 
                                                    parallelizePartialResult.Layer, 
                                                    parallelizePartialResult.J, 
                                                    _availableCores);

                orderedGroups[0].RemoveAt(0);

                parallellOperationsList[parallelizePartialResult.Layer - 1].Add(parallelOperation);

                availableCore.IsBusy = true;

                parallelizePartialResult.J++;
            }
        }
    }

    public class ParallelizePartialResult 
    {
        public int Layer { get; set; }
        public int J { get; set; }
    }
}
