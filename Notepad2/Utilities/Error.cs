using Notepad2.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Notepad2.Utilities
{
    public static class Error
    {
        public static void Show(string textError, string titleError)
        {
            ErrorReporterWindow erw = new ErrorReporterWindow(titleError, textError);
            erw.Show();
        }

        public static void Show(string textError)
        {
            ErrorReporterWindow erw = new ErrorReporterWindow("Message", textError);
            erw.Show();
        }
    }
}
