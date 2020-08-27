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
    class ControlUtils
    {
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
    }
}
