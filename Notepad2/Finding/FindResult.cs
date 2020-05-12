using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Notepad2.Finding
{
    public class FindResult
    {
        public int StartIndex { get; set; }
        public int EndIndex { get; set; }
        public int WordLength { get; set; }
        public string FoundText { get; set; }

        public FindResult(int start, int end, int length)
        {
            StartIndex = start;
            EndIndex = end;
            WordLength = length;
        }
        public FindResult(int start, int end)
        {
            StartIndex = start;
            EndIndex = end;
            WordLength = end - start;
        }
        public FindResult(int start, int end, string found)
        {
            StartIndex = start;
            EndIndex = end;
            WordLength = end - start;
            FoundText = found;
        }
    }
}
