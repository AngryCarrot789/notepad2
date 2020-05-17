using Notepad2.Views;
using System;
using System.Collections.ObjectModel;
using System.Windows;

namespace Notepad2.InformationStuff
{
    public static class Information
    {
        public delegate void InformationEventArgs(InformationModel e);
        public static event InformationEventArgs InformationAdded;

        public static void Show(string text, string type)
        {
            InformationAdded?.Invoke(new InformationModel(type, DateTime.Now, text));
        }
        public static void Show(string text, InfoTypes type)
        {
            InformationAdded?.Invoke(new InformationModel(type, DateTime.Now, text));
        }
    }
}
