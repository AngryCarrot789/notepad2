using Notepad2.Views;
using System;
using System.Collections.ObjectModel;
using System.Windows;

namespace Notepad2.InformationStuff
{
    public static class Information
    {
        public static Action<InformationModel> AddErrorCallback { get; set; }
        public static void Show(string text, string type)
        {
            AddErrorCallback?.Invoke(new InformationModel(type, DateTime.Now, text));
        }
        public static void Show(string text, InfoTypes type)
        {
            AddErrorCallback?.Invoke(new InformationModel(type, DateTime.Now, text));
        }
    }
}
