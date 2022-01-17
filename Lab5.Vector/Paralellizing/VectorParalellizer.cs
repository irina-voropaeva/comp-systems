using System.Collections.Generic;
using System.Linq;
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

            //first layer, all operands are known 
            var j = 0;
            var layer = 1;

            while(orderedGroups[0].Any())
            {
                var availableCore = _availableCores.FirstOrDefault(ac =>
                    !ac.IsBusy && ac.AvailableOperations.Contains(orderedGroups[0][0].Operation.Value));

                if (availableCore == null)
                {
                    layer++; // no available core for this operation, need to continue on layer 2

                    _availableCores.ForEach(ac => ac.IsBusy = false);

                    parallellOperationsList.Add(new List<ParallelOperation>());
                    continue;
                }

                var parallelOperation = new ParallelOperation(availableCore, orderedGroups[0][0], layer, j);

                orderedGroups[0].RemoveAt(0);

                parallellOperationsList[layer - 1].Add(parallelOperation);

                availableCore.IsBusy = true;

                j++;
            }


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
    }
}
