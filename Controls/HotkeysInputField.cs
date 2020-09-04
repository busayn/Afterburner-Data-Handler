using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using AfterburnerDataHandler.FlatControls;
using AfterburnerDataHandler.HotkeysHandler;

namespace AfterburnerDataHandler.Controls
{
    public class HotkeysInputField : InputField
    {
        public class HotkeysEventArgs : EventArgs
        {
            public Keys Hotkeys;

            public HotkeysEventArgs(Keys hotkeys)
            {
                Hotkeys = hotkeys;
            }
        }

        public event EventHandler<HotkeysEventArgs> HotkeysChanged;

        public Keys Hotkeys
        {
            get { return (key & ~Keys.Modifiers) | (modifiers & Keys.Modifiers); }
            set
            {
                newKey = value & ~Keys.Modifiers;
                newModifiers = value & Keys.Modifiers;
                RecordHotkeys();
            }
        }

        private Keys key = Keys.None;
        private Keys modifiers = Keys.None;

        private Keys newKey = Keys.None;
        private Keys newModifiers = Keys.None;
        private bool hotkeyRecorded = true;
        private bool hotkeysHandlerSuspended = false;

        public HotkeysInputField()
        {
            TextBox.ReadOnly = false;
            TextBox.KeyDown += TextBoxKeyDown;
            TextBox.KeyUp += TextBoxKeyUp;
            RecordHotkeys();
        }

        protected virtual void UpdateHotkeyName()
        {
            TextBox.Text = GetHotkeyName(newKey, newModifiers);
            TextBox.SelectionStart = Text.Length > 0 ? Text.Length : 0;
            TextBox.SelectionLength = 0;
        }

        protected virtual string GetHotkeyName(Keys key, Keys modifiers)
        {
            return Hotkey.GetHotkeyName(key, modifiers);
        }

        protected virtual void RecordHotkeys()
        {
            hotkeyRecorded = true;

            if (newKey == Keys.None || HotkeyUtils.IsKeyValid(newKey) == false)
            {
                newKey = Keys.None;
                newModifiers = Keys.None;
            }

            key = newKey;
            modifiers = newModifiers;

            OnHotkeysChanged(new HotkeysEventArgs(Hotkeys));
            UpdateHotkeyName();
        }

        protected virtual void OnHotkeysChanged(HotkeysEventArgs e)
        {
            HotkeysChanged?.Invoke(this, e);
        }

        private void TextBoxKeyDown(object sender, KeyEventArgs e)
        {
            e.Handled = true;
            e.SuppressKeyPress = true;

            if (hotkeyRecorded == true)
            {
                hotkeyRecorded = false;
                newKey = Keys.None;
                newModifiers = Keys.None;
            }

            if (HotkeyUtils.GetModifierKeys(e.KeyCode) == Keys.None)
            {
                newKey = e.KeyCode;
            }

            newModifiers = e.Modifiers;

            UpdateHotkeyName();
        }

        private void TextBoxKeyUp(object sender, KeyEventArgs e)
        {
            e.Handled = true;
            e.SuppressKeyPress = true;

            if (HotkeyUtils.GetModifierKeys(e.KeyCode) == Keys.None)
            {
                RecordHotkeys();
                return;
            }

            if (hotkeyRecorded == false)
            {
                newModifiers = e.Modifiers;
            }

            UpdateHotkeyName();
        }

        protected override void OnEnter(EventArgs e)
        {
            base.OnEnter(e);
            Hotkey.SuspendAllHotkeys();
            hotkeysHandlerSuspended = true;
            RecordHotkeys();
        }

        protected override void OnLeave(EventArgs e)
        {
            Hotkey.ResumeAllHotkeys();
            hotkeysHandlerSuspended = false;
            RecordHotkeys();
            base.OnLeave(e);
        }

        protected override void OnEnabledChanged(EventArgs e)
        {
            RecordHotkeys();
            base.OnEnabledChanged(e);

            if (this.Enabled == false && hotkeysHandlerSuspended == true)
            {
                Hotkey.ResumeAllHotkeys();
                hotkeysHandlerSuspended = false;
            }
        }

        protected override void OnVisibleChanged(EventArgs e)
        {
            RecordHotkeys();
            base.OnVisibleChanged(e);

            if (this.Enabled == false && hotkeysHandlerSuspended == true)
            {
                Hotkey.ResumeAllHotkeys();
                hotkeysHandlerSuspended = false;
            }
        }
    }
}
