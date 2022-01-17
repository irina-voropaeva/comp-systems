using System;
using System.Collections.Generic;

namespace Lab5.Vector
{
    internal class ProcessorCore
    {
        public Command? CurrentCommand { get; private set; }
        public int Tacts { get; private set; }
        public bool Complete => Tacts == 0;

        public void InsertCommand(Command command, LinkedListNode<ProcessorCore>? next)
        {
            if (!Complete && CurrentCommand is not null)
                throw new InvalidOperationException("Cannot insert while not complete");

            if (CurrentCommand is not null) next?.Value.InsertCommand(CurrentCommand, next?.Next);

            CurrentCommand = command;
            Tacts = command.Tacts;
        }

        public void UpdateTacts(int tacts)
        {
            if (tacts > Tacts) Tacts = tacts;
        }

        public void Process()
        {
            if (Tacts is not 0) Tacts--;
        }
    }

}