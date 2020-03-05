using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Notepad2.Utilities
{
    public static class KeydownManager
    {
        public static bool[] KeysDown = new bool[256];

        public static void SetKeyDown(Key key)
        {
            KeysDown[KeyToInt(key)] = true;
        }

        public static void SetKeyUp(Key key)
        {
            KeysDown[KeyToInt(key)] = false;
        }

        public static int KeyToInt(Key key)
        {
            return (int)key;
        }

        public static bool Keydown(Key key)
        {
            return KeysDown[KeyToInt(key)];
        }
    }
}
