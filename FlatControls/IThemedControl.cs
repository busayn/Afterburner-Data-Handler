using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AfterburnerDataHandler.FlatControls
{
    public interface IThemedControl
    {
        bool UseGlobalTheme { get; set; }
        Theme Theme { get; set; }

        event EventHandler<EventArgs> ThemeDataChanged;

        void UpdateTheme();
    }
}
