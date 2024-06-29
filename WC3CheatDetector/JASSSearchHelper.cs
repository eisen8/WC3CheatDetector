using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using WC3CheatDetector.Models;

namespace WC3CheatDetector
{
    /// <summary>
    /// Helper class for searching text from JASS files
    /// </summary>
    public class JASSSearchHelper
    {
        /**
         * We are solving two problems here. One is the we want to ignore whitespace in the JASS however we want the output match to include any 
         * whitespace originally present. The other is that the JASS file might use "\r", \"\n", "\n\r", "\r\n" or other combinations of
         * the two which will mess with our searching/regex. I am solving them both in a single solution here. We create a searchable version of the
         * string without whitespace and normalizing the newlines to just a single "\n". Then we use an index map to map the searchable string
         * to the original input so that we can return the original string when a match is found.
        **/

        // The original input string
        public string Input { get; private set; }

        // A more searchable version of the input string. Ignoring whitespace and normalizing newlines
        private string _searchableString;

        // An index 'map' between the searchable input and the original input string. The index corresponds to the index in the
        // _searchableString and the corresponding value is the corresponding index in the Input.
        private List<int> _indexMap;

        // A reverse index map from the original input string to the searchable string. The index corresponds to the index in the
        // Input and the corresponding value is the corresponding index in the _searchableString.
        private List<int> _revIndexMap;

        // The searchable string above broken up by lines. 
        private List<string> _searchableStringLines;

        // The input string above broken up into its lines.
        private List<string> _inputLines;


        private static readonly Regex WS_REGEX = new Regex(@"\s");

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="input">The input JASS string to help search.</param>
        public JASSSearchHelper(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
            {
                throw new ArgumentNullException("input", "Input cannot be null or empty");
            }
            init(input);
        }

        /// <summary>
        /// Check if the JASS string contains a substring.
        /// </summary>
        /// <param name="substring">The substring to search for.</param>
        /// <param name="ignoreWhitespace">Whether to ignore whitespace or not when checking the match.</param>
        /// <returns>All lines that contain the substring.</returns>
        public List<string> GetContains(string substring, bool ignoreWhitespace = true)
        {
            List<string> toSearch = ignoreWhitespace ? _inputLines : _searchableStringLines;
            if (ignoreWhitespace)
            {
                substring = substring.Replace(" ", "");
            }

            return toSearch.Where(s => s.Contains(substring)).ToList();
        }

        /// <summary>
        /// Check if the JASS string contains any of a list of substring.
        /// </summary>
        /// <param name="substrings">The substrings to search for.</param>
        /// <param name="ignoreWhitespace">Whether to ignore whitespace or not when checking the match.</param>
        /// <returns>All lines that contain any of the substrings.</returns>
        public List<string> GetContainsAny(List<string> substrings, bool ignoreWhitespace = true)
        {
            List<string> linesContaining = new List<string>();
            List<string> toSearch = ignoreWhitespace ? _inputLines : _searchableStringLines;
            if (ignoreWhitespace)
            {
                toSearch.ForEach(s => s.Replace(" ", ""));
            }

            foreach (string line in toSearch)
            {
                foreach (string substring in substrings)
                {
                    if (line.Contains(substring))
                    {
                        linesContaining.Add(line);
                    }
                }
            }

            return linesContaining;
        }


        /// <summary>
        /// Attmpts to find Matches while ignoring whitespace and normalizing newlines.
        /// </summary>
        /// <param name="pattern">The regex. Do not include whitespace in the regex. Match on "\n" for newlines.</param>
        /// <param name="options">RegexOptions</param>
        /// <returns>The matches if any or an empty list if none</returns>
        public List<JRMatch> FindMatches(string pattern, RegexOptions options = RegexOptions.IgnoreCase, bool ignoreWhitespace = true)
        {
            List<JRMatch> rMatches = new List<JRMatch>();

            if (string.IsNullOrWhiteSpace(pattern))
            {
                throw new ArgumentNullException("regex", "Input regex cannot be null");
            }

            if (!ignoreWhitespace)
            {

                MatchCollection matches = Regex.Matches(Input, pattern, options);
                foreach (Match m in matches)
                {
                    string value = Input.Substring(m.Index, m.Length);
                    rMatches.Add(new JRMatch(value, m.Index));
                }
            }
            else
            {
                pattern = pattern.Replace(" ", ""); // Remove any ws from regex since we are ignoring ws.
                MatchCollection matches = Regex.Matches(_searchableString, pattern, options);

                foreach (Match m in matches)
                {
                    // Get back the match from the original input using our position 'map'.
                    int startInd = _indexMap[m.Index];
                    int endInd = _indexMap[m.Index + (m.Value.Length - 1)];
                    string value = Input.Substring(startInd, (endInd - startInd) + 1);
                    rMatches.Add(new JRMatch(value, startInd));
                }
            }

            return rMatches;
        }

        /// <summary>
        ///Attmpts to find a match while ignoring whitespace and normalizing newlines.
        /// </summary>
        /// <param name="pattern">The regex pattern to search for.</param>
        /// <param name="options">RegexOptions.</param>
        /// <returns>The first match or an emty match if none.</returns>
        public JRMatch FindMatch(string pattern, RegexOptions options = RegexOptions.None)
        {
            if (string.IsNullOrWhiteSpace(pattern))
            {
                throw new ArgumentNullException("regex", "Input regex cannot be null");
            }

            pattern = pattern.Replace(" ", ""); // Remove any ws from regex since we are ignoring ws.
            Match m = Regex.Match(_searchableString, pattern, options);
            if (m.Success)
            {
                int startInd = _indexMap[m.Index];
                int endInd = _indexMap[m.Index + (m.Value.Length - 1)];
                string value = Input.Substring(startInd, (endInd - startInd) + 1);
                return new JRMatch(value, startInd);
            }
            else
            {
                return new JRMatch();
            }    
        }

        /// <summary>
        ///Attmpts to find a match (starting at a specific index) while ignoring whitespace and normalizing newlines.
        /// </summary>
        /// <param name="pattern">The regex pattern to search for.</param>
        /// <param name="startIndex">The index to start searching at.</param>
        /// <param name="options">RegexOptions.</param>
        /// <returns>The first match or an emty match if none.</returns>
        public JRMatch FindMatch(string pattern, int startIndex, RegexOptions options = RegexOptions.None)
        {
            if (string.IsNullOrWhiteSpace(pattern))
            {
                throw new ArgumentNullException("regex", "Input regex cannot be null");
            }

            pattern = pattern.Replace(" ", ""); // Remove any ws from regex since we are ignoring ws.
            int searchStartIndex = _revIndexMap[startIndex];

            Match m = Regex.Match(_searchableString[searchStartIndex..], pattern, options);
            if (m.Success)
            {
                int startInd = _indexMap[m.Index + searchStartIndex];
                int endInd = _indexMap[m.Index + searchStartIndex + (m.Value.Length - 1)];
                string value = Input.Substring(startInd, (endInd - startInd) + 1);
                return new JRMatch(value, startInd);
            }
            else
            {
                return new JRMatch();
            }
        }

        /// <summary>
        /// Inserts a substring into the JASS input string.
        /// </summary>
        /// <param name="index">Starting index to insert at.</param>
        /// <param name="subString">Substring to insert.</param>
        public void Insert(int index, string subString)
        {
            init(Input.Insert(index, subString));
        }

        /// <summary>
        /// Creates the necessary indexs and strings necessary to do the searching.
        /// </summary>
        /// <param name="input">The input JASS string to Init (or reinit)</param>
        private void init(string input)
        {
            Input = input;
            _indexMap = new List<int>(input.Length);
            _revIndexMap = new List<int>(input.Length);
            StringBuilder sb = new StringBuilder();
            int j = 0;
            for (int i = 0; i < input.Length; i++)
            {
                char c = input[i];
                if (c == '\n' || c == '\r') // newline case
                {
                    bool end = false;
                    while (!end) // Continue until no more newline characters
                    {
                        i++;
                        if (i < input.Length)
                        {
                            c = input[i];
                            if (!(c == '\n' || c == '\r'))
                            {
                                end = true;
                                i--;
                            }
                        }
                        else // End of string
                        {
                            end = true;
                            i--;
                        }
                        _revIndexMap.Add(j);
                    }
                    sb.Append("\n");
                    _indexMap.Add(i);
                    j++;
                }
                else if (WS_REGEX.IsMatch(c.ToString())) //whitespace
                {
                    _revIndexMap.Add(j);
                }
                else // All other characters
                {
                    sb.Append(c);
                    _indexMap.Add(i);
                    _revIndexMap.Add(j);
                    j++;
                }
            }

            _searchableString = sb.ToString();
            _searchableStringLines = _searchableString.Split('\n').ToList();
            _inputLines = Input.Split('\n').ToList();
        }
    }
}
