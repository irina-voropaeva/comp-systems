using System.Collections.Generic;

namespace Core
{
    public class AvailableOperations
    {
        public List<string> AriphmeticOperations = new() 
            { "+", "-", "*", "/" };

        public Dictionary<string, List<string>> GetUnallowedSymbolsDict()
        {
            var unallowedSymbolsDict = new Dictionary<string, List<string>>()
            {
                { "+", new List<string>() { "-", "+", "*", "/" } },
                { "-", new List<string>() { "-", "+", "*", "/" } },
                { "*", new List<string>() { "-", "+", "*", "/" } },
                { "/", new List<string>() { "-", "+", "*", "/" } },
                { ")", new List<string>() { "(" } },
                { "(", new List<string>() { "+", "*", "/", ")" } },
                {".", new List<string>() { "-", "+", "*", "/", ")", "(" }}
            };

            var allowedLetters = GetAllowedLetters();
            foreach (var allowedLetter in allowedLetters)
            {
                unallowedSymbolsDict.Add(allowedLetter, allowedLetters);
            }

            unallowedSymbolsDict["."].AddRange(allowedLetters);

            return unallowedSymbolsDict;
        }
        public List<string> GetAllowedLetters() 
        {
            var list = new List<string>();
            for (var c = 'A'; c <= 'Z'; ++c) {
                list.Add(c.ToString());
            }

            for (var c = 'a'; c <= 'z'; ++c)
            {
                list.Add(c.ToString());
            }

            return list;
        }
    }
}
