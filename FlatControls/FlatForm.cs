using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.Layout;

namespace AfterburnerDataHandler.FlatControls
{
    public class FlatForm : Form
    {
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public override Color BackColor
        {
            get { return base.BackColor; }
            set { base.BackColor = Theme.Current.WindowBackgroundColor; }
        }

        public FlatForm()
        {
            Theme.GlobalThemeChanged += GlobalThemeChanged;
            UpdateTheme();
        }

        protected override void OnPaintBackground(PaintEventArgs pevent)
        {
            using (SolidBrush brush = new SolidBrush(this.BackColor))
            {
                pevent.Graphics.FillRectangle(brush, pevent.ClipRectangle);
            }
        }

        protected virtual void UpdateTheme()
        {
            Theme theme = Theme.Current;
            this.BackColor = theme.WindowBackgroundColor;
            this.Invalidate();
        }

        private void GlobalThemeChanged(object sender, EventArgs e)
        {
            UpdateTheme();
        }
    }
}
