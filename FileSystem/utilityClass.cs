using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileSystem
{

    public static class utilityClass
    {
        public static string[] mySplit(string str, char separator)
        {
            // Create a list to hold the substrings
            List<string> substrings = new List<string>();

            // Use a StringBuilder to build each substring
            StringBuilder currentSubstring = new StringBuilder();

            // Loop through each character in the string
            foreach (char c in str)
            {
                // If the current character is the separator,
                // add the current substring to the list and start
                // building a new substring
                if (c == separator)
                {
                    substrings.Add(currentSubstring.ToString());
                    currentSubstring.Clear();
                }
                else
                {
                    // If the current character is not the separator,
                    // append it to the current substring
                    currentSubstring.Append(c);
                }
            }

            // Add the final substring to the list
            substrings.Add(currentSubstring.ToString());

            // Return the list of substrings as an array
            return substrings.ToArray();
        }

        public static char[] ToCharArray(string content)
        {
            var arr = new char[content.Length];
            for (int i = 0; i < content.Length; i++)
            {
                arr[i] = content[i];
            }

            return arr;
        }


    }


}


