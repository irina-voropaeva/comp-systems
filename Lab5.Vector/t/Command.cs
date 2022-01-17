using System.Collections.Generic;
using System.Linq;

namespace Lab5.Vector
{
    public class Command
    {
        public Command(string operation, int tacts)
        {
            Operation = operation;
            Tacts = tacts;
        }

        public Command(string operation)
        {
            Operation = operation;
            Tacts = CommandsTacts.FirstOrDefault(tacts => tacts.Key == operation).Value;
        }

        private Dictionary<string, int> CommandsTacts { get; init; } = new()
        {
            { "+", 1 },
            { "-", 2 },
            { "*", 3 },
            { "/", 4 },
        };

        public int Tacts { get; init; }
        public string Operation { get; init; }
    }

}