using System.Collections.Generic;
using System.Linq;
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

        public ParallelOperation(
            ProcessorCore core, 
            SingleOperationCallDto operation, 
            int layer, 
            int operationNumber, 
            List<ProcessorCore> otherCoresStatus)
        {
            Core = new ProcessorCore(core.AvailableOperations, core.CoreId, core.IsBusy, operation);
            Operation = operation;
            Layer = layer;
            OperationNumber = operationNumber;
            OtherCoresStatus = otherCoresStatus.Select(ocs => new ProcessorCore(ocs.AvailableOperations, ocs.CoreId, ocs.IsBusy, ocs.CurrentWork)).ToList();
        }
    }
}
