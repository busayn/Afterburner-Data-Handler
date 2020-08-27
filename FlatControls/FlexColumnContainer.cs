using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.Layout;

namespace AfterburnerDataHandler.FlatControls
{
    public class FlexColumnContainer : ContentPanel
    {
        [Browsable(true), Category("Appearance")]
        public FlexColumnsLayoutEngine.ColumnsLayout ColumnsLayout
        {
            get { return (this.LayoutEngine as FlexColumnsLayoutEngine).ColumnsStyle; }
            set
            {
                (this.LayoutEngine as FlexColumnsLayoutEngine).ColumnsStyle = value;
                this.PerformLayout();
            }
        }

        [Browsable(true), Category("Appearance")]
        public int MinColumnSize
        {
            get { return (this.LayoutEngine as FlexColumnsLayoutEngine).MinMinColumnSizeSize; }
            set
            {
                (this.LayoutEngine as FlexColumnsLayoutEngine).MinMinColumnSizeSize = value > 0 ? value : 0;
                this.PerformLayout();
            }
        }

        [Browsable(true), Category("Appearance")]
        public int MaxColumnSize
        {
            get { return (this.LayoutEngine as FlexColumnsLayoutEngine).MaxColumnSize; }
            set
            {
                (this.LayoutEngine as FlexColumnsLayoutEngine).MaxColumnSize = value > 0 ? value : 0;
                this.PerformLayout();
            }
        }

        [Browsable(true), Category("Appearance")]
        public int MinColumnCount
        {
            get { return (this.LayoutEngine as FlexColumnsLayoutEngine).MinColumnCount; }
            set
            {
                (this.LayoutEngine as FlexColumnsLayoutEngine).MinColumnCount = value > 1 ? value : 1;
                this.PerformLayout();
            }
        }

        [Browsable(true), Category("Appearance")]
        public int MaxColumnCount
        {
            get { return (this.LayoutEngine as FlexColumnsLayoutEngine).MaxColumnCount; }
            set
            {
                (this.LayoutEngine as FlexColumnsLayoutEngine).MaxColumnCount = value > 0 ? value : 0;
                this.PerformLayout();
            }
        }

        public override LayoutEngine LayoutEngine
        {
            get
            {
                if (layoutEngine == null)
                    layoutEngine = new FlexColumnsLayoutEngine();

                return layoutEngine;
            }
        }

        private FlexColumnsLayoutEngine layoutEngine;

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
            Rectangle targetBounds = new Rectangle(
                Point.Empty,
                ClampSize(this.MinimumSize, this.MaximumSize, proposedSize - this.Padding.Size));

            Size preferredSize = new Size();
            Rectangle[] controlsBounds = layoutEngine.CalculateLayout(this, targetBounds);

            if (controlsBounds == null || controlsBounds.Length != this.Controls.Count)
                return preferredSize;

            for (int i = 0; i < controlsBounds.Length; i++)
            {
                Size newSize = new Size(
                    controlsBounds[i].Right + this.Controls[i].Margin.Right,
                    controlsBounds[i].Bottom + this.Controls[i].Margin.Bottom);

                preferredSize.Width = Math.Max(preferredSize.Width, newSize.Width);
                preferredSize.Height = Math.Max(preferredSize.Height, newSize.Height);
            }

            preferredSize.Width += this.Padding.Right;
            preferredSize.Height += this.Padding.Bottom;

            return preferredSize;
        }

        private Size ClampSize(Size min, Size max, Size value)
        {
            if (max.Width != 0)
                value.Width = Math.Min(max.Width, value.Width);

            if (max.Height != 0)
                value.Height = Math.Min(max.Height, value.Height);

            value.Width = Math.Max(min.Width, value.Width);
            value.Height = Math.Max(min.Height, value.Height);

            return value;
        }
    }
}