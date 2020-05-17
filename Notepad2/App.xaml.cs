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
            MainWindow mWnd = e.Args.Length == 1 ? new MainWindow(e.Args[0]) : new MainWindow();
            MainWindow = mWnd;
            mWnd.Show();
            mWnd.LoadSettings();
        }
    }
}
