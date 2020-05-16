using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Notepad2.Utilities
{
    public static class GlobalPreferences
    {
        public const double WARN_FILE_SIZE_KB = 100.0d;
        public const double ALERT_FILE_SIZE_KB = 250.0d;
        public static Color WARN_FILE_TOO_BIG_COLOUR = Colors.Orange;
        public static Color ALERT_FILE_TOO_BIG_COLOUR = Colors.Red;

        public static string[] PRESET_EXTENSIONS = new string[13]
{
            ".txt",
            ".text",
            ".cs",
            ".c",
            ".cpp",
            ".h",
            ".xaml",
            ".xml",
            ".htm",
            ".html",
            ".css",
            ".js",
            ".exe",
};
    }
}
