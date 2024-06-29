using System.Collections.Generic;

namespace WC3CheatDetector.Models
{
    /// <summary>
    /// Various extension methods
    /// </summary>
    public static class Extensions
    {
        /// <summary>
        /// Checks if a List is empty.
        /// </summary>
        /// <param name="list">The list to check.</param>
        /// <returns>True if empty. False if not.</returns>
        public static bool IsEmpty<T>(this List<T> list)
        {
            return list.Count == 0;
        }

        /// <summary>
        /// Returns the index of the nth occurence of a character.
        /// </summary>
        /// <param name="input">Input string.</param>
        /// <param name="character">Chracter to search for.</param>
        /// <param name="n">The nth occurence</param>
        /// <returns>The index of the nth occurrence of -1 if it does not exist</returns>
        public static int FindNthOccurence(this string input, char character, int n)
        {
            int occurences = 0;
            for (int i = 0; i < input.Length; i++)
            {
                char c = input[i];

                if (char.ToUpperInvariant(c) == char.ToUpperInvariant(character))
                {
                    occurences++;
                    if (occurences == n)
                    {
                        return i;
                    }
                }
            }

            return -1;
        }
    }
}
