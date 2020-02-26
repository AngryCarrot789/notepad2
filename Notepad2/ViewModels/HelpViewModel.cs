using NamespaceHere;
using Notepad2.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Notepad2.ViewModels
{
    public class HelpViewModel : BaseViewModel
    {
        public ICommand HelpCommand { get; set; }

        private HelpDialog HelpWindow = new HelpDialog();

        public HelpViewModel()
        {
            HelpCommand = new Command(ShowHelpWindow);
        }

        public void ShowHelpWindow()
        {
            HelpWindow.Show();
        }
    }
}
