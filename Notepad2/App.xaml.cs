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
        private ResourceDictionary ThemeDictionary
        {
            // You could probably get it via its name with some query logic as well.
            get { return Resources.MergedDictionaries[0]; }
            set { Resources.MergedDictionaries[0] = value; }
        }

        private void ChangeTheme(Uri uri)
        {
            //ThemeDictionary.MergedDictionaries.Clear();
            ThemeDictionary = new ResourceDictionary() { Source = uri };
            //ThemeDictionary.MergedDictionaries.Insert(0, new ResourceDictionary() { Source = uri });
        }

        public enum Theme
        {
            Dark, Light
        }

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

            mWnd.CurrentApp = this;
            mWnd.LoadSettings();
            mWnd.Show();
        }

        public void SetTheme(Theme theme)
        {
            string themeName = null;
            switch (theme)
            {
                case Theme.Dark: themeName = "DarkTheme"; break;
                case Theme.Light: themeName = "LightTheme"; break;
            }

            try
            {
                if (!string.IsNullOrEmpty(themeName))
                    ChangeTheme(new Uri($"Themes/{themeName}.xaml", UriKind.Relative));
            }
            catch { }
        }
    }
}
