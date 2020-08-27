using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AfterburnerDataHandler.FlatControls
{
    public class ArrowButton : FlatButton
    {
        [Browsable(true), Category("Appearance")]
        public virtual ArrowDirection ArrowDirection
        {
            get { return arrowDirection; }
            set
            {
                arrowDirection = value;
                UpdateLayout();
            }
        }

        [Browsable(true), Category("Appearance")]
        public virtual int ArrowSize
        {
            get { return arrowSize; }
            set
            {
                arrowSize = value;
                UpdateLayout();
            }
        }

        public override string Text { get { return string.Empty; } set { base.Text = string.Empty; } }

        protected override Size DefaultSize { get { return new Size(26, 26); } }
        protected override Padding DefaultPadding { get { return new Padding(3, 3, 3, 3); } }
        protected Point[] Arrow { get { return arrow; } set { arrow = value; } }

        private Point[] arrow = new Point[0];
        private ArrowDirection arrowDirection = ArrowDirection.Up;
        private int arrowSize = 10;

        public ArrowButton()
        {
            this.UseButtonBorder = false;
        }

        protected override void UpdateLayout()
        {
            base.UpdateLayout();

            int iconSize = ArrowSize;
            Rectangle baseRect = ClientRectangle;
            baseRect.Size -= this.Padding.Size;
            baseRect.X += this.Padding.Left;
            baseRect.Y += this.Padding.Top;

            Rectangle arrowRect = ClientRectangle;
            arrowRect.Width = iconSize < baseRect.Width ? iconSize : baseRect.Width;
            arrowRect.Height = iconSize < baseRect.Height ? iconSize : baseRect.Height;
            arrowRect.X = baseRect.X + (baseRect.Width - arrowRect.Width) / 2;
            arrowRect.Y = baseRect.Y + (baseRect.Height - arrowRect.Height) / 2;

            Arrow = VectorIcons.Arrow(arrowRect, ArrowDirection);

            this.Invalidate();
        }

        protected override void OnPaintBackground(PaintEventArgs pevent)
        {
            base.OnPaintBackground(pevent);

            using (SolidBrush brush = new SolidBrush(Color.Red))
            {
                pevent.Graphics.FillRectangle(brush, pevent.ClipRectangle);
            }
        }

        protected override void OnPaint(PaintEventArgs pevent)
        {
            base.OnPaint(pevent);


            SmoothingMode lastSmoothingMode = pevent.Graphics.SmoothingMode;
            pevent.Graphics.SmoothingMode = SmoothingMode.AntiAlias;

            using (SolidBrush brush = new SolidBrush(this.Enabled == true ? this.TextColor : this.DisabledTextColor))
            {
                pevent.Graphics.FillPolygon(brush, Arrow);
            }

            pevent.Graphics.SmoothingMode = lastSmoothingMode;
        }

        protected override void OnLayout(LayoutEventArgs levent)
        {
            base.OnLayout(levent);
            UpdateLayout();
        }
    }
}
