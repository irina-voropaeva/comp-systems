using Lab2.Tree;

namespace Lab5.Vector.Paralellizing
{
    public class ParallelOperation
    {
        public ProcessorCore Core { get; set; }

        public SingleOperationCallDto Operation { get; set; }

        public int Layer { get; set; }

        public int OperationNumber { get; set; }

        public ParallelOperation(ProcessorCore core, SingleOperationCallDto operation, int layer, int operationNumber)
        {
            Core = core;
            Operation = operation;
            Layer = layer;
            OperationNumber = operationNumber;
        }
    }
}
