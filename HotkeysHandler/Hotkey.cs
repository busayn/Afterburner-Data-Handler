using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AfterburnerDataHandler.HotkeysHandler
{
    public class Hotkey
    {
        #region Hotkey Object

        public Keys Key { get { return key; } set { key = value; } }
        public Keys Modifiers { get { return modifiers; } set { modifiers = value; } }
        public event EventHandler<EventArgs> HotkeyPressed;

        private Keys key;
        private Keys modifiers;
        private int hotkeySuspendCount = 0;

        public Hotkey(Keys key, Keys modifiers)
        {
            this.Key = key;
            this.Modifiers = modifiers;
        }

        public void Enable()
        {
            if (!activeHotkeys.Contains(this))
            {
                activeHotkeys.Add(this);
            }
        }

        public void Disable()
        {
            activeHotkeys.Remove(this);
        }

        public void Suspend()
        {
            hotkeySuspendCount++;
        }

        public void Resume() { Resume(false); }

        public void Resume(bool ignoreSuspendLevel)
        {
            if (hotkeySuspendCount > 0)
            {
                hotkeySuspendCount = ignoreSuspendLevel == true ? 0 : hotkeySuspendCount - 1;
            }
        }

        public static string GetHotkeyName(Keys key, Keys modifiers)
        {
            string hotkeyName = string.Empty;
            Keys keyModifiers = HotkeyUtils.GetModifierKeys(modifiers);

            if ((keyModifiers & Keys.Control) == Keys.Control) hotkeyName += "Ctrl + ";
            if ((keyModifiers & Keys.Alt) == Keys.Alt) hotkeyName += "Alt + ";
            if ((keyModifiers & Keys.Shift) == Keys.Shift) hotkeyName += "Shift + ";

            hotkeyName += HotkeyUtils.GetKeyName(key & ~HotkeyUtils.modifierKeysMask);

            return hotkeyName;
        }

        public override string ToString()
        {
            return GetHotkeyName(Key, Modifiers);
        }

        protected virtual void OnHotkeyPressed(EventArgs e)
        {
            if (hotkeySuspendCount < 1)
            {
                HotkeyPressed?.Invoke(this, e);
            }
        }

        #endregion

        #region Hotkeys Handler

        private struct KeyboardHookData
        {
            public KeyboardMessageType keyState;
            public KBDLLHOOKSTRUCT keyData;

            public KeyboardHookData(KeyboardMessageType keyState, KBDLLHOOKSTRUCT keyData)
            {
                this.keyState = keyState;
                this.keyData = keyData;
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct KBDLLHOOKSTRUCT
        {
            public uint vkCode;
            public uint scanCode;
            public KBDLLHOOKSTRUCTFlags flags;
            public uint time;
            public UIntPtr dwExtraInfo;
        }

        [Flags]
        public enum KBDLLHOOKSTRUCTFlags : uint
        {
            LLKHF_EXTENDED = 0x01,
            LLKHF_INJECTED = 0x10,
            LLKHF_ALTDOWN = 0x20,
            LLKHF_UP = 0x80,
        }

        private enum HookType : int
        {
            WH_KEYBOARD = 2,
            WH_MOUSE = 7,
            WH_KEYBOARD_LL = 13,
            WH_MOUSE_LL = 14
        }

        private enum KeyboardMessageType : int
        {
            WM_ACTIVATE = 0x0006,
            WM_APPCOMMAND = 0x0319,
            WM_CHAR = 0x0102,
            WM_DEADCHAR = 0x0103,
            WM_HOTKEY = 0x0312,
            WM_KEYDOWN = 0x0100,
            WM_KEYUP = 0x0101,
            WM_KILLFOCUS = 0x0008,
            WM_SETFOCUS = 0x0007,
            WM_SYSDEADCHAR = 0x0107,
            WM_SYSKEYDOWN = 0x0104,
            WM_SYSKEYUP = 0x0105,
            WM_UNICHAR = 0x0109
        }

        private delegate IntPtr HookProc(int code, IntPtr wParam, IntPtr lParam);

        private static HookProc keyboardHookCallback;
        private static IntPtr keyboardHook = IntPtr.Zero;
        private static Keys pressedKey = Keys.None;
        private static Keys pressedModifierKeys = Keys.None;
        private static readonly object keysLocker = new object();
        private static readonly List<Hotkey> activeHotkeys = new List<Hotkey>();
        private static int allHotkeysSuspendCount = 0;

        static Hotkey()
        {
            AddHook();
        }

        public static void SuspendAllHotkeys()
        {
            allHotkeysSuspendCount++;

            RemoveHook();

            pressedKey = Keys.None;
            pressedModifierKeys = Keys.None;
        }

        public static void ResumeAllHotkeys() { ResumeAllHotkeys(false); }

        public static void ResumeAllHotkeys(bool ignoreSuspendLevel)
        {
            if (allHotkeysSuspendCount > 0)
            {
                allHotkeysSuspendCount = ignoreSuspendLevel == true ? 0 : allHotkeysSuspendCount - 1;

                if (allHotkeysSuspendCount < 1)
                {
                    pressedKey = Keys.None;
                    pressedModifierKeys = Keys.None;

                    AddHook();
                }
            }
        }

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr SetWindowsHookEx(HookType hookType, HookProc lpfn, IntPtr hMod, uint dwThreadId);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern bool UnhookWindowsHookEx(IntPtr hhk);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr CallNextHookEx(IntPtr hhk, int code, IntPtr wParam, IntPtr lParam);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern IntPtr GetModuleHandle(string lpModuleName);

        private static void AddHook()
        {
            keyboardHookCallback -= KeyboardHook;
            keyboardHookCallback += KeyboardHook;

            try
            {
                keyboardHook = SetWindowsHookEx(
                    HookType.WH_KEYBOARD_LL,
                    keyboardHookCallback,
                    GetModuleHandle("user32"),
                    0);
            }
            catch { }
        }

        private static void RemoveHook()
        {
            if (keyboardHook != IntPtr.Zero)
            {
                UnhookWindowsHookEx(keyboardHook);
                keyboardHook = IntPtr.Zero;
            }
        }

        private static IntPtr KeyboardHook(int code, IntPtr wParam, IntPtr lParam)
        {
            KeyboardMessageType keyState = (KeyboardMessageType)wParam;

            if (code >= 0 &&
                (keyState == KeyboardMessageType.WM_KEYDOWN ||
                keyState == KeyboardMessageType.WM_KEYUP ||
                keyState == KeyboardMessageType.WM_SYSKEYDOWN ||
                keyState == KeyboardMessageType.WM_SYSKEYUP))
            {
                ThreadPool.QueueUserWorkItem(
                    KeyboardHookHandler,
                    new KeyboardHookData(keyState, Marshal.PtrToStructure<KBDLLHOOKSTRUCT>(lParam)));
            }

            return CallNextHookEx(keyboardHook, code, wParam, lParam);
        }

        private static void KeyboardHookHandler(object data)
        {
            if (allHotkeysSuspendCount > 0) return;

            lock (keysLocker)
            {
                KeyboardHookData hookData = (KeyboardHookData)data;
                KeyboardMessageType keyState = hookData.keyState;
                KBDLLHOOKSTRUCT keyboardData = hookData.keyData;

                Keys newKeys = (Keys)keyboardData.vkCode;
                Keys newModifierKeys = HotkeyUtils.GetModifierKeys(newKeys);

                if (keyState == KeyboardMessageType.WM_KEYDOWN || keyState == KeyboardMessageType.WM_SYSKEYDOWN)
                {
                    pressedModifierKeys |= newModifierKeys;

                    if (newKeys != pressedKey)
                    {
                        pressedKey = newKeys;
                        HandleHotkeys(newModifierKeys == Keys.None ? pressedKey : Keys.None);
                    }
                }
                else if (keyState == KeyboardMessageType.WM_KEYUP || keyState == KeyboardMessageType.WM_SYSKEYUP)
                {
                    pressedKey = Keys.None;
                    pressedModifierKeys &= ~newModifierKeys;
                }
            }
        }

        private static void HandleHotkeys(Keys key)
        {
            foreach (Hotkey hotkey in activeHotkeys)
            {
                if (hotkey.Key == key && hotkey.Modifiers == pressedModifierKeys)
                {
                    hotkey.OnHotkeyPressed(EventArgs.Empty);
                }
            }
        }

        #endregion

    }
}
