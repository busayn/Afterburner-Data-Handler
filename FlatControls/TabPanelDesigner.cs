using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.Design;

namespace AfterburnerDataHandler.FlatControls
{
    [System.Security.Permissions.PermissionSet(System.Security.Permissions.SecurityAction.Demand, Name = "FullTrust")]
    class TabPanelDesigner : ParentControlDesigner
    {
        private static readonly string[] hiddenProperties =
        {
            "AutoSize",
            "AutoSizeMode",
            "Margin",
            "MinimumSize",
            "MaximumSize",
            "Dock",
            "Location",
            "Size",
            "Anchor",
            "Visible",
        };

        protected override void PreFilterProperties(System.Collections.IDictionary properties)
        {
            base.PreFilterAttributes(properties);

            foreach (string property in hiddenProperties)
            {
                properties.Remove(property);
            }
        }

        protected override void OnPaintAdornments(PaintEventArgs pe)
        {
            base.OnPaintAdornments(pe);

            using (Pen outlinePen = new Pen(Color.FromArgb(84, 91, 101), 1))
            {
                outlinePen.DashStyle = System.Drawing.Drawing2D.DashStyle.Dash;
                pe.Graphics.DrawRectangle(outlinePen, new Rectangle(Point.Empty, this.Control.Size - new Size(1, 1)));
            }
        }
    }
}
