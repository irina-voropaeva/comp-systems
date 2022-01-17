using System;
using System.Collections.Generic;
using Lab2.Tree;

namespace Lab5.Vector.Paralellizing
{
    public class ProcessorCore
    {
        public List<string> AvailableOperations { get; set; }

        public bool IsBusy { get; set; }

        public SingleOperationCallDto CurrentWork { get; set; }

        public int CoreId { get; set; }

        public string CoreName => $"{CoreId} : {string.Join(',', AvailableOperations.ToArray())}";

        public ProcessorCore(List<string> availableOperations, int coreId, bool isBusy = false)
        {
            AvailableOperations = availableOperations;
            IsBusy = isBusy;
            CoreId = coreId;
        }
    }
}
