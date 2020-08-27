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
    public class DropdownMenuItem : Control
    {
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public virtual Color AccentColor
        {
            get { return accentColor; }
            set
            {
                accentColor = value;
                this.Invalidate();
            }
        }

        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public virtual Color TextColor
        {
            get { return textColor; }
            set
            {
                textColor = value;
                this.Invalidate();
            }
        }

        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public virtual Color BackgroundColor
        {
            get { return backgroundColor; }
            set
            {
                backgroundColor = value;
                this.Invalidate();
            }
        }

        protected bool IsFocused { get { return isFocused; } set { isFocused = value; } }
        protected override Size DefaultSize { get { return new Size(100, 26); } }
        protected override Padding DefaultMargin { get { return new Padding(0, 0, 0, 0); } }
        protected override Padding DefaultPadding { get { return new Padding(3, 0, 0, 0); } }

        private bool isFocused = false;
        private Color accentColor = Color.FromArgb(140, 190, 240);
        private Color textColor = Color.FromArgb(255, 255, 255);
        private Color backgroundColor = Color.FromArgb(70, 80, 90);

        public DropdownMenuItem()
        {
            SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw, true);
            SetStyle(ControlStyles.Selectable, true);
            SetStyle(ControlStyles.ContainerControl, false);

            Theme.GlobalThemeChanged += GlobalThemeChanged;
            UpdateTheme();
        }

        protected virtual void UpdateTheme()
        {
            Theme theme = Theme.Current;

            this.AccentColor = theme.AccentColor;
            this.TextColor = theme.TextColor;
            this.BackgroundColor = theme.ControlBackgroundColor;

            this.Invalidate();
        }

        private void GlobalThemeChanged(object sender, EventArgs e)
        {
            UpdateTheme();
        }

        protected override void OnPaint(PaintEventArgs pevent)
        {
            Color backgroundColor = this.IsFocused ? this.AccentColor : this.BackgroundColor;

            using (SolidBrush brush = new SolidBrush(backgroundColor))
            {
                pevent.Graphics.FillRectangle(brush, pevent.ClipRectangle);
            }

            Padding itemPadding = this.Padding;
            Rectangle itemRect = this.ClientRectangle;

            TextRenderer.DrawText(
                pevent.Graphics,
                this.Text,
                this.Font,
                new Rectangle(
                    itemRect.X + itemPadding.Left,
                    itemRect.Y + itemPadding.Top,
                    itemRect.Width - Padding.Horizontal,
                    itemRect.Height - Padding.Vertical),
                this.TextColor,
                TextFormatFlags.TextBoxControl |
                TextFormatFlags.VerticalCenter |
                TextFormatFlags.EndEllipsis);
        }

        protected override void OnEnter(EventArgs e)
        {
            IsFocused = true;
            base.OnEnter(e);
            this.Invalidate();
        }

        protected override void OnLeave(EventArgs e)
        {
            IsFocused = false;
            base.OnLeave(e);
            this.Invalidate();
        }
    }
}
