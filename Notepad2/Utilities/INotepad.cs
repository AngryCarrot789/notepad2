using Notepad2.Notepad;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Notepad2.Utilities
{
    public interface INotepad
    {
        FormatModel DocumentFormat { get; set; }
        DocumentModel Document { get; set; }
    }
}
