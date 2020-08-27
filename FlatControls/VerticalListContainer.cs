using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Windows.Forms.Layout;

namespace AfterburnerDataHandler.FlatControls
{
    public class VerticalListContainer : ContentPanel
    {
        public override LayoutEngine LayoutEngine
        {
            get
            {
                if(layoutEngine == null)
                    layoutEngine = new VerticalListLayoutEngine();

                return layoutEngine;
            }
        }

        private VerticalListLayoutEngine layoutEngine;

        protected override void OnLayout(LayoutEventArgs levent)
        {
            if (levent.AffectedControl != null && AutoScroll)
            {
                base.OnLayout(levent);
            }

            AdjustFormScrollbars(AutoScroll);
            base.OnLayout(levent);
        }

        public override Size GetPreferredSize(Size proposedSize)
        {
            int totalWidth = this.Padding.Left;
            int totalHeight = this.Padding.Top;

            foreach (Control c in this.Controls)
            {
                Rectangle controlRect = c.Bounds;

                if (c.AutoSize == true)
                {
                    controlRect.Size = c.GetPreferredSize(this.ClientSize + this.Padding.Size);
                }

                controlRect.Size += new Size(c.Margin.Right, c.Margin.Bottom);

                if (controlRect.Right > totalWidth)
                    totalWidth = controlRect.Right;

                if (controlRect.Bottom > totalHeight)
                    totalHeight = controlRect.Bottom;
            }

            totalWidth += this.Padding.Right;
            totalHeight += this.Padding.Bottom;

            if (this.MaximumSize.Width > 0)
                totalWidth = Math.Min(this.MaximumSize.Width, totalWidth);

            if (this.MaximumSize.Height > 0)
                totalHeight = Math.Min(this.MaximumSize.Height, totalHeight);

            return new Size(
                Math.Max(this.MinimumSize.Width, totalWidth),
                Math.Max(this.MinimumSize.Height, totalHeight));
        }
    }
}
