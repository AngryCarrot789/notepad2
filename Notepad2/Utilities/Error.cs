using Notepad2.Views;
using System.Windows;

namespace Notepad2.Utilities
{
    public static class Error
    {
        public static void Show(string textError, string titleError)
        {
            //ErrorReporterWindow erw = new ErrorReporterWindow(titleError, textError);
            //erw.Show();
            MessageBox.Show(textError, titleError);
        }

        public static void Show(string textError)
        {
            //ErrorReporterWindow erw = new ErrorReporterWindow("Message", textError);
            //erw.Show();
            MessageBox.Show(textError, "Error");
        }
    }
}
