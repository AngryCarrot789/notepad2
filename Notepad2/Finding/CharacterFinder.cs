using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Notepad2.Finding
{
    public static class CharacterFinder
    {
        /// <summary>
        /// Finds every occourance of a word and returns a list of FindResults
        /// </summary>
        /// <param name="toFind">The string you want to find.</param>
        /// <param name="heapOfText">The text which will be searched</param>
        /// <returns></returns>
        public static List<FindResult> FindTextOccurrences(this string heapOfText, string toFind)
        {
            if (string.IsNullOrEmpty(heapOfText))
                return null;

            List<FindResult> indexes = new List<FindResult>();
            for (int index = 0; ; index += toFind.Length)
            {
                index = heapOfText.IndexOf(toFind, index);
                if (index == -1)
                    return indexes;
                FindResult fr = new FindResult(index, index + toFind.Length, toFind);
                indexes.Add(fr);
            }
        }
    }
}
