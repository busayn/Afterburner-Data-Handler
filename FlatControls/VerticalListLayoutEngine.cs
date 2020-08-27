using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Windows.Forms.Layout;

namespace AfterburnerDataHandler.FlatControls
{
    public class VerticalListLayoutEngine : LayoutEngine
    {
        public override bool Layout(object container, LayoutEventArgs layoutEventArgs)
        {
            Control panel = container as Control;
            Size panelSize = panel.DisplayRectangle.Size;
            Point nextControlLocation = panel.DisplayRectangle.Location;

            panelSize.Width = panel.ClientRectangle.Width - panel.Padding.Horizontal;

            foreach (Control c in panel.Controls)
            {
                if (!c.Visible) continue;

                c.Width = panelSize.Width - c.Margin.Horizontal;

                if (c.AutoSize)
                {
                    c.Height = c.GetPreferredSize(new Size(panelSize.Width - c.Margin.Horizontal - panel.Padding.Horizontal, 0)).Height;
                }

                c.Location = new Point(
                    nextControlLocation.X + c.Margin.Left,
                    nextControlLocation.Y + c.Margin.Top);

                nextControlLocation.Y += c.Height + c.Margin.Vertical;

            }
            return true;
        }
    }
}
