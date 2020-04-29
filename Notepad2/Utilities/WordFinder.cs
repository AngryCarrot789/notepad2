using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Notepad2.Utilities
{
    public static class WordFinder
    {
        /// <summary>
        /// Finds every occourance of a word and returns a list of the start indexes of every word.
        /// </summary>
        /// <param name="toFind">The string you want to find.</param>
        /// <param name="heapOfText">The text which will be searched</param>
        /// <returns></returns>
        public static List<int> AllIndexesOf(this string heapOfText, string toFind)
        {
            if (string.IsNullOrEmpty(heapOfText))
                return null;

            List<int> indexes = new List<int>();
            for (int index = 0; ; index += toFind.Length)
            {
                index = heapOfText.IndexOf(toFind, index);
                if (index == -1)
                    return indexes;
                indexes.Add(index);
            }
        }
    }
}
