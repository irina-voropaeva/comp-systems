using System.Collections.Generic;

namespace Core
{
    public class AvailableOperations
    {

        public static readonly List<string> AlgebraicOperations = new()
        {
            "+", "-", "*", "/"
        };

        public static List<string> GetAllowedLetters() 
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
