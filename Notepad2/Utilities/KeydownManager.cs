using System.Windows.Input;

namespace Notepad2.Utilities
{
    public delegate void KeydownEventHandler(Key key);
    public static class KeydownManager
    {
        public static event KeydownEventHandler KeyDown;
        public static event KeydownEventHandler KeyUp;
        public static bool[] KeysDown = new bool[256];
        public static bool CtrlPressed { get => Keydown(Key.LeftCtrl) || Keydown(Key.RightCtrl); }
        public static bool ShiftPressed { get => Keydown(Key.LeftShift) || Keydown(Key.RightShift); }

        public static void SetKeyDown(Key key)
        {
            KeysDown[KeyToInt(key)] = true;
            KeyDown?.Invoke(key);
        }

        public static void SetKeyUp(Key key)
        {
            KeysDown[KeyToInt(key)] = false;
            KeyUp?.Invoke(key);
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
