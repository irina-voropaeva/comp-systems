using System.Collections.Generic;
using System.Linq;
using Lab2.Tree;
using Lab2.Tree.Tokens;

namespace Lab5.Vector.Paralellizing
{
    public class VectorParalellizer
    {
        private readonly List<ProcessorCore> _availableCores = new()
        {
            new ProcessorCore(new List<string>() { "+", "-" }, 1),
            new ProcessorCore(new List<string>() { "+", "-" }, 2),
            new ProcessorCore(new List<string>() { "*" }, 3),
            new ProcessorCore(new List<string>() { "*" }, 4),
            new ProcessorCore(new List<string>() { "/" }, 5),
        };

        private readonly List<List<ParallelOperation>> _parallelOperations;

        public VectorParalellizer()
        {
            _parallelOperations = new List<List<ParallelOperation>>()
            {
                new()
            };
        }

        public List<List<ParallelOperation>> ParallelizeBetweenCores(List<List<SingleOperationCallDto>> orderedGroups)
        {
            ParallelizeCalls(orderedGroups);

            return _parallelOperations;
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

        private void ParallelizeCalls(
            List<List<SingleOperationCallDto>> orderedGroups)
        {
            var layerOperationNumber = new LayerOperationNumber()
            {
                OperationNumber = 0,
                Layer = 1
            };

            var parallelizePartialResult = new ParallelizePartialResult()
            {
                LayerWithPosition = new Dictionary<int, int>()
                {
                    { layerOperationNumber.Layer, layerOperationNumber.OperationNumber }
                }
            };

            while (orderedGroups[0].Any())
            {
                var availableCore = GetAvailableCore(orderedGroups[0][0].Operation.Value);

                if (availableCore == null) // no available core for this operation, need to continue on layer next
                {
                    MoveToNextLayer(layerOperationNumber, parallelizePartialResult, _parallelOperations);

                    continue;
                }

                availableCore.IsBusy = true;
                availableCore.CurrentWork = orderedGroups[0][0];

                var parallelOperation = new ParallelOperation(
                    availableCore,
                    orderedGroups[0][0],
                    layerOperationNumber.Layer,
                    layerOperationNumber.OperationNumber,
                    _availableCores);

                _parallelOperations[layerOperationNumber.Layer - 1].Add(parallelOperation);

                layerOperationNumber.OperationNumber++;

                orderedGroups[0].RemoveAt(0);
            }

            if (orderedGroups[1].Any())
            {
                MoveToNextLayer(layerOperationNumber, parallelizePartialResult, _parallelOperations);
            }

            while (orderedGroups[1].Any())
            {
                var flatCalculatedCallsList = _parallelOperations.SelectMany(pol => pol
                        .Where(po => po.Layer < layerOperationNumber.Layer)
                        .Select(po => po.Operation.Name))
                    .ToList();

                // if possible, use available core for operation from 2 subgroup
                var callFromSubGroup1 = orderedGroups[1][0];
                var availableCoreForSubgroup1 = GetAvailableCore(callFromSubGroup1.Operation.Value);

                var isCurrentCallCalculatable = IsCurrentCallCalculatable(callFromSubGroup1, flatCalculatedCallsList);

                if (availableCoreForSubgroup1 == null || !isCurrentCallCalculatable) // no available core for this operation, need to continue on layer next
                {
                    MoveToNextLayer(layerOperationNumber, parallelizePartialResult, _parallelOperations);

                    continue;
                }

                availableCoreForSubgroup1.IsBusy = true;
                availableCoreForSubgroup1.CurrentWork = callFromSubGroup1;

                var parallelOperationSubGroup1 = new ParallelOperation(
                    availableCoreForSubgroup1,
                    callFromSubGroup1,
                    layerOperationNumber.Layer,
                    layerOperationNumber.OperationNumber,
                    _availableCores);

                _parallelOperations[layerOperationNumber.Layer - 1].Add(parallelOperationSubGroup1);

                layerOperationNumber.OperationNumber++;

                orderedGroups[1].RemoveAt(0);
            }

            if (orderedGroups[2].Any())
            {
                MoveToNextLayer(layerOperationNumber, parallelizePartialResult, _parallelOperations);
            }

            while (orderedGroups[2].Any())
            {
                var flatCalculatedCallsList = _parallelOperations.SelectMany(pol => pol
                        .Where(po => po.Layer < layerOperationNumber.Layer)
                        .Select(po => po.Operation.Name))
                    .ToList();

                // if possible, use available core for operation from subgroup 2

                var callFromSubGroup2 = orderedGroups[2][0];
                var availableCoreForSubgroup2 = GetAvailableCore(callFromSubGroup2.Operation.Value);

                var isCurrentCallCalculatable = IsCurrentCallCalculatable(callFromSubGroup2, flatCalculatedCallsList);

                if (availableCoreForSubgroup2 == null || !isCurrentCallCalculatable) // no available core for this operation, need to continue on layer next
                {
                    MoveToNextLayer(layerOperationNumber, parallelizePartialResult, _parallelOperations);

                    continue;
                }


                availableCoreForSubgroup2.IsBusy = true;
                availableCoreForSubgroup2.CurrentWork = callFromSubGroup2;

                var parallelOperationSubGroup2 = new ParallelOperation(
                    availableCoreForSubgroup2,
                    callFromSubGroup2,
                    layerOperationNumber.Layer,
                    layerOperationNumber.OperationNumber,
                    _availableCores);

                _parallelOperations[layerOperationNumber.Layer - 1].Add(parallelOperationSubGroup2);

                layerOperationNumber.OperationNumber++;

                orderedGroups[2].RemoveAt(0);

            }
        }

        private bool IsCurrentCallCalculatable(SingleOperationCallDto call, List<string> flatCalculatedCallsList)
        {
            var isCurrentCallCalculatable = (call.FirstOperand.Type == TokenType.Operand && flatCalculatedCallsList.Contains(call.SecondOperand.Value))
                                            || (call.SecondOperand.Type == TokenType.Operand && flatCalculatedCallsList.Contains(call.FirstOperand.Value))
                                            || (flatCalculatedCallsList.Contains(call.FirstOperand.Value) && flatCalculatedCallsList.Contains(call.SecondOperand.Value));

            return isCurrentCallCalculatable;
        }

        private void MoveToNextLayer(
            LayerOperationNumber layerOperationNumber,
            ParallelizePartialResult parallelizePartialResult,
            List<List<ParallelOperation>> parallellOperationsList)
        {
            layerOperationNumber.Layer++;

            parallelizePartialResult.LayerWithPosition.Add(layerOperationNumber.Layer, layerOperationNumber.OperationNumber);

            _availableCores.ForEach(ac => ac.IsBusy = false);
            _availableCores.ForEach(ac => ac.CurrentWork = null);

            parallellOperationsList.Add(new List<ParallelOperation>());
        }

        private ProcessorCore GetAvailableCore(string value)
        {
            return _availableCores.FirstOrDefault(ac =>
                !ac.IsBusy && ac.AvailableOperations.Contains(value));
        }
    }

    public class ParallelizePartialResult
    {
        public Dictionary<int, int> LayerWithPosition { get; set; }
    }

    public class LayerOperationNumber
    {
        public int Layer { get; set; }
        public int OperationNumber { get; set; }
    }
}
