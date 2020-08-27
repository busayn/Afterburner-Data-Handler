using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Windows.Forms.Layout;

namespace AfterburnerDataHandler.FlatControls
{
    public class PropertyContainerLayoutEngine : LayoutEngine
    {
        public virtual int LabelWidth { get; set; } = 100;
        public virtual bool FitContent { get; set; } = true;
        public virtual HorizontalAlignment ControlsAlignment { get; set; } = HorizontalAlignment.Left;

        public override bool Layout(object container, LayoutEventArgs layoutEventArgs)
        {
            Control panel = container as Control;

            if (panel.Controls.Count < 1) return false;

            Size panelSize = panel.ClientRectangle.Size;
            Padding panelPadding = panel.Padding;
            Point nextControlLocation = panel.ClientRectangle.Location;

            int remainingWidth = panelSize.Width - panel.Padding.Horizontal - LabelWidth;
            int remainingItems = 0;
            int targetHeight = 0;

            nextControlLocation.X += LabelWidth + panel.Padding.Left;
            nextControlLocation.Y += panelPadding.Top;

            foreach (Control c in panel.Controls)
                if (c.Visible == true) remainingItems++;

            if (remainingItems < 1) return false;

            foreach (Control c in panel.Controls)
            {
                if (c.Visible == false) continue;
                
                if (FitContent)
                {
                    if (remainingItems < 1) break;

                    c.Width = remainingWidth / remainingItems - c.Margin.Horizontal;
                    remainingWidth -= c.Width + c.Margin.Horizontal;
                    remainingItems--;
                }

                c.Location = new Point(
                    nextControlLocation.X + c.Margin.Left,
                    nextControlLocation.Y + c.Margin.Top);

                nextControlLocation.X += c.Width + c.Margin.Horizontal;
                targetHeight = Math.Max(targetHeight, c.Height + c.Margin.Vertical);
            }

            remainingWidth = panel.ClientRectangle.Right - panel.Padding.Right - nextControlLocation.X;

            switch (ControlsAlignment)
            {
                case HorizontalAlignment.Right:
                    ShiftContent(panel.Controls, new Size(remainingWidth, 0));
                    break;
                case HorizontalAlignment.Center:
                    ShiftContent(panel.Controls, new Size(remainingWidth / 2, 0));
                    break;
            }

            panel.Height = targetHeight + panelPadding.Vertical;

            return false;
        }

        public virtual void ShiftContent(Control.ControlCollection controls, Size distance)
        {
            foreach (Control c in controls)
            {
                c.Location += distance;
            }
        }
    }
}