using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AfterburnerDataHandler.HotkeysHandler
{
    public static class HotkeyUtils
    {
        public static readonly Keys modifierKeysMask = Keys.Shift |
                                                       Keys.Control |
                                                       Keys.Alt;

        private static readonly List<Keys> invalidKeys = new List<Keys>
        {
            Keys.Shift,
            Keys.ShiftKey,
            Keys.LShiftKey,
            Keys.RShiftKey,
            Keys.Control,
            Keys.ControlKey,
            Keys.LControlKey,
            Keys.RControlKey,
            Keys.Alt,
            Keys.Menu,
            Keys.LMenu,
            Keys.RMenu,
            Keys.LWin,
            Keys.RWin,
            Keys.Escape,
            Keys.Enter,
            Keys.Return,
            Keys.Back,
        };

        public static bool IsHotkeyValid(Hotkey hotkey)
        {
            return IsModifiersValid(hotkey.Modifiers) && IsKeyValid(hotkey.Key);
        }

        public static bool IsModifiersValid(Keys modifiers)
        {
            return (modifiers & ~modifierKeysMask) == Keys.None;
        }

        public static bool IsKeyValid(Keys key)
        {
            foreach (Keys invalidKey in invalidKeys)
            {
                if (key == invalidKey) return false;
            }

            return true;
        }

        public static Keys GetModifierKeys(Keys keys)
        {
            Keys modifier = keys & modifierKeysMask;

            switch (keys)
            {
                case Keys.ShiftKey:
                case Keys.LShiftKey:
                case Keys.RShiftKey:
                    return modifier | Keys.Shift;

                case Keys.ControlKey:
                case Keys.LControlKey:
                case Keys.RControlKey:
                    return modifier | Keys.Control;

                case Keys.Menu:
                case Keys.LMenu:
                case Keys.RMenu:
                    return modifier | Keys.Alt;
            }

            return modifier;
        }

        public static string GetKeyName(Keys key)
        {

            switch (key)
            {
                case Keys.None: return "";
                case Keys.Back: return "Backspace";
                case Keys.Return: return "Enter";
                case Keys.Capital: return "CapsLock";
                case Keys.Apps: return "ContextMenu";
                case Keys.Prior: return "PageUp";
                case Keys.Next: return "PageDown";

                case Keys.Alt:
                case Keys.Menu:
                case Keys.LMenu:
                case Keys.RMenu:
                    return "Alt";

                case Keys.NumPad0:
                case Keys.NumPad1:
                case Keys.NumPad2:
                case Keys.NumPad3:
                case Keys.NumPad4:
                case Keys.NumPad5:
                case Keys.NumPad6:
                case Keys.NumPad7:
                case Keys.NumPad8:
                case Keys.NumPad9:
                case Keys.Add:
                case Keys.Subtract:
                case Keys.Divide:
                case Keys.Multiply:
                case Keys.Space:
                case Keys.Decimal:
                case Keys.Escape:
                    return Enum.GetName(typeof(Keys), key);
            }

            string keyChar = Convert.ToChar(MapVirtualKey((uint)key, 2)).ToString();

            if (keyChar.Length == 1 && keyChar.IndexOfAny(new char[] {
                ' ',
                '\x0',
                '\n',
                '\r',
                '\t',
                '\b',
                '\f',
                '\v'
            }) < 0) return keyChar;

            return key.ToString();
        }

        [DllImport("user32.dll", CharSet = CharSet.Ansi)]
        static extern uint MapVirtualKey(uint uCode, uint uMapType);
    }
}
