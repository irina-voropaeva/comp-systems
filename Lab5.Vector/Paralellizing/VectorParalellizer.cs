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
            //calculate simple group, all operands are known 
            var layerPosition = new LayerOperationNumber()
            {
                OperationNumber = 0,
                Layer = 1
            };

            var parallelizePartialResult = new ParallelizePartialResult()
            {
                LayerWithPosition = new Dictionary<int, int>()
                {
                    { layerPosition.Layer, layerPosition.OperationNumber }
                }
            };

            ParallelizeCalls(orderedGroups, parallelizePartialResult, layerPosition);

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

        private  void ParallelizeCalls(
            List<List<SingleOperationCallDto>> orderedGroups, 
            ParallelizePartialResult parallelizePartialResult,
            LayerOperationNumber layerOperationNumber)
        {
            while (orderedGroups[0].Any() || orderedGroups[1].Any() || orderedGroups[2].Any())
            {
                var flatCalculatedCallsList = _parallelOperations.SelectMany(pol => pol.Select(po => po.Operation.Name))
                    .ToList();

                if (orderedGroups[0].Any())
                {
                    var availableCore = GetAvailableCore(orderedGroups[0][0].Operation.Value);

                    if (availableCore == null) // no available core for this operation, need to continue on layer 2
                    {
                        GoToNextLayer(layerOperationNumber, parallelizePartialResult, _parallelOperations);

                        continue;
                    }

                    var parallelOperation = new ParallelOperation(
                        availableCore,
                        orderedGroups[0][0],
                        layerOperationNumber.Layer,
                        layerOperationNumber.OperationNumber,
                        _availableCores);

                    _parallelOperations[layerOperationNumber.Layer - 1].Add(parallelOperation);

                    availableCore.IsBusy = true;

                    layerOperationNumber.OperationNumber++;

                    orderedGroups[0].RemoveAt(0);
                }

                if (orderedGroups[1].Any())
                {
                    // if possible, use available core for operation from 2 subgroup
                    var callFromSubGroup1 = orderedGroups[1].SingleOrDefault(og =>
                        flatCalculatedCallsList.Contains(og.FirstOperand.Value) && og.SecondOperand.Type == TokenType.Operand
                        || flatCalculatedCallsList.Contains(og.SecondOperand.Value) && og.FirstOperand.Type == TokenType.Operand);

                    if (callFromSubGroup1 != null)
                    {
                        var availableCoreForSubgroup1 = GetAvailableCore(callFromSubGroup1.Operation.Value);

                        if (availableCoreForSubgroup1 == null) // no available core for this operation, need to continue on layer 2
                        {
                            GoToNextLayer(layerOperationNumber, parallelizePartialResult, _parallelOperations);

                            continue;
                        }

                        var parallelOperationSubGroup1 = new ParallelOperation(
                            availableCoreForSubgroup1,
                            callFromSubGroup1,
                            layerOperationNumber.Layer,
                            layerOperationNumber.OperationNumber,
                            _availableCores);

                        _parallelOperations[layerOperationNumber.Layer - 1].Add(parallelOperationSubGroup1);


                        availableCoreForSubgroup1.IsBusy = true;

                        layerOperationNumber.OperationNumber++;

                        orderedGroups[1].RemoveAt(0);
                    }
                }

                if (orderedGroups[2].Any())
                {
                    // if possible, use available core for operation from subgroup 3

                    var callFromSubGroup2 = orderedGroups[2].SingleOrDefault(og => flatCalculatedCallsList.Contains(og.FirstOperand.Value)
                        && flatCalculatedCallsList.Contains(og.SecondOperand.Value));

                    if (callFromSubGroup2 != null)
                    {
                        var availableCoreForSubgroup2 = GetAvailableCore(callFromSubGroup2.Operation.Value);

                        if (availableCoreForSubgroup2 == null) // no available core for this operation, need to continue on layer 2
                        {
                            GoToNextLayer(layerOperationNumber, parallelizePartialResult, _parallelOperations);

                            continue;
                        }

                        
                        var parallelOperationSubGroup2 = new ParallelOperation(
                            availableCoreForSubgroup2,
                            callFromSubGroup2,
                            layerOperationNumber.Layer,
                            layerOperationNumber.OperationNumber,
                            _availableCores);

                        _parallelOperations[layerOperationNumber.Layer - 1].Add(parallelOperationSubGroup2);

                        availableCoreForSubgroup2.IsBusy = true;

                        layerOperationNumber.OperationNumber++;

                        orderedGroups[2].RemoveAt(0);
                    }
                }
            }
        }

        private void GoToNextLayer(
            LayerOperationNumber layerOperationNumber, 
            ParallelizePartialResult parallelizePartialResult, 
            List<List<ParallelOperation>> parallellOperationsList)
        {
            layerOperationNumber.Layer++;

            parallelizePartialResult.LayerWithPosition.Add(layerOperationNumber.Layer, layerOperationNumber.OperationNumber);

            _availableCores.ForEach(ac => ac.IsBusy = false);

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
