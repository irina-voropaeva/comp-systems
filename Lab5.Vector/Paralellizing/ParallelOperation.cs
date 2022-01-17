using System.Collections.Generic;
using Lab2.Tree;

namespace Lab5.Vector.Paralellizing
{
    public class ParallelOperation
    {
        public ProcessorCore Core { get; set; }

        public SingleOperationCallDto Operation { get; set; }

        public int Layer { get; set; }

        public int OperationNumber { get; set; }

        public List<ProcessorCore> OtherCoresStatus { get; set; }

        public ParallelOperation(ProcessorCore core, SingleOperationCallDto operation, int layer, int operationNumber, List<ProcessorCore> otherCoresStatus)
        {
            Core = core;
            Operation = operation;
            Layer = layer;
            OperationNumber = operationNumber;
            OtherCoresStatus = otherCoresStatus;
        }
    }
}
