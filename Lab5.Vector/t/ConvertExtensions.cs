using System.Collections.Generic;
using System.Linq;
using Lab2.Tree;

namespace Lab5.Vector
{
    public static class ConvertExtensions
    {
        public static Dictionary<int, List<Command>> ToLayeredCommandsDictionary(this List<SingleOperationCallDto> tokenGroups)
        {
            return tokenGroups.GroupBy(tokenGroup => tokenGroup.Layer).ToDictionary(
                grouped => grouped.Key,
                grouped =>
                {
                    var commandsByLayer = new List<Command>();

                    foreach (var group in grouped)
                    {
                        var command = new Command(group.Operation.Value);
                        commandsByLayer.Add(command);
                    }

                    return commandsByLayer;
                });
        }

        public static Dictionary<int, List<SingleOperationCallDto>> ToLayeredGroupsDictionary(this List<SingleOperationCallDto> tokenGroups)
        {
            return tokenGroups.GroupBy(tokenGroup => tokenGroup.Layer).ToDictionary(
                grouped => grouped.Key,
                grouped =>
                {
                    var groupsByLayer = new List<SingleOperationCallDto>();

                    foreach (var group in grouped) groupsByLayer.Add(group);

                    return groupsByLayer;
                });
        }

        public static List<Command> ToCommandsList(this List<SingleOperationCallDto> tokenGroups) =>
            tokenGroups.ConvertAll(group => new Command(group.Operation.Value));

        public static List<Command> ToOptimalCommandsList(this List<SingleOperationCallDto> tokenGroups) =>
            tokenGroups.OrderBy(group => group.Layer).ThenByDescending(group => group.Operation.Priority)
                .ToList().ToCommandsList();
    }

}