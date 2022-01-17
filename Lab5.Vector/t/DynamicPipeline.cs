using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lab5.Vector
{
    public class DynamicPipeline
    {
        public DynamicPipeline(int coresCount, List<Command> commands)
        {
            this.commands = commands;
            cores = InitializeCores(coresCount);
            AppendAdditionalCommands(this.commands, coresCount);
            tacts = commands.First().Tacts;
            entry = cores.First;
            layer = 1;
        }

        private readonly List<Command> commands;
        private readonly LinkedList<ProcessorCore> cores;
        private readonly LinkedListNode<ProcessorCore>? entry;

        private int tacts;
        private int layer;

        public List<string> Execute()
        {
            var states = new List<string>();

            foreach (var command in commands)
            {
                var putState = PutCommand(command);
                states.Add(putState);

                var processState = RunTacts();
                states.AddRange(processState);
            }

            return states;
        }

        private List<string> RunTacts()
        {
            var runStates = new List<string>();

            for (var i = 0; i < tacts; i++)
            {
                layer++;
                foreach (var core in cores) core.Process();
                runStates.Add(GetCurrentState());
            }

            return runStates;
        }

        private bool HigherTactCommandExists(int tacts) =>
            cores.FirstOrDefault(core => core?.CurrentCommand?.Tacts > tacts) is not null;

        private string PutCommand(Command command)
        {
            var entryCore = entry?.Value;
            var nextCore = entry?.Next;
            entryCore?.InsertCommand(command, nextCore);

            if (tacts < command.Tacts || !HigherTactCommandExists(command.Tacts)) UpdateTacts(command.Tacts);

            return GetCurrentState();
        }

        private string GetCurrentState()
        {
            var state = new StringBuilder($"[ Tact: {layer}]\t |");

            foreach (var core in cores)
            {
                var operation = core?.CurrentCommand?.Operation ?? "~";
                state.Append($"| {operation} |");
            }
            return state.ToString();
        }

        private void UpdateTacts(int tacts)
        {
            this.tacts = tacts;

            foreach (var core in cores)
                core.UpdateTacts(tacts);
        }

        private static LinkedList<ProcessorCore> InitializeCores(int count = 1)
        {
            if (count < 1) throw new ArgumentException("Count must be more than 1");

            var cores = new List<ProcessorCore>(count);

            for (var i = 0; i < count; i++)
                cores.Add(new ProcessorCore());

            return new LinkedList<ProcessorCore>(cores);
        }

        private void AppendAdditionalCommands(List<Command> commands, int count)
        {
            var additional = new List<Command>();

            for (int i = 0; i < count; i++)
                additional.Add(new Command("~", 2));

            commands.AddRange(additional);
        }
    }

}