using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Windows.Forms;
using System.Globalization;
using System.IO;

namespace AfterburnerDataHandler.Controls
{
    public static class ControlUtils
    {
        private const int WM_VSCROLL = 0x115;
        private const int SB_TOP = 6;
        private const int SB_BOTTOM = 7;

        public static void DisableComboBoxScroll(ComboBox comboBox)
        {
            if (comboBox != null)
                comboBox.MouseWheel += new MouseEventHandler(IgnoreControlScroll);
        }

        public static void IgnoreControlScroll(object sender, MouseEventArgs e)
        {
            ((HandledMouseEventArgs)e).Handled = true;
        }

        public static bool AsyncSafeInvoke(Control targetControl, Action action)
        {
            bool invokeRequired = targetControl.InvokeRequired;

            if (invokeRequired) targetControl.BeginInvoke(action);
            else action.Invoke();

            return invokeRequired;
        }

        public static void ScrollToTop(Control control)
        {
            if (control != null && control.IsHandleCreated == true)
            {
                SendMessage(control.Handle, WM_VSCROLL, SB_TOP, 0);
            }
        }

        public static void ScrollToBottom(Control control)
        {
            if (control != null && control.IsHandleCreated == true)
            {
                SendMessage(control.Handle, WM_VSCROLL, SB_BOTTOM, 0);
            }
        }

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern IntPtr SendMessage(IntPtr hWnd, int Msg, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern IntPtr SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
    }
}
