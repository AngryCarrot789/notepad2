using Notepad2.ViewModels;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace Notepad2
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            MainWindow mWnd;
            if (e.Args.Length == 1)
            {
                mWnd = new MainWindow(e.Args[0]);
            }
            else
            {
                mWnd = new MainWindow();
            }

            mWnd.Show();
        }
    }
}
