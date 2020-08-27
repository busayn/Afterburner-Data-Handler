using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using AfterburnerDataHandler.FlatControls;

namespace AfterburnerDataHandler.Controls
{
    public partial class StatusBar : ThemedControl
    {
        public int ExpandButtonSize { get; set; } = 24;
        public int ExpandButtonIconSize { get; set; } = 12;


        protected Rectangle TextRect
        {
            get
            {
                Rectangle viewRect = this.ClientRectangle;
                Padding viewPadding = this.Padding;

                return new Rectangle(
                    viewPadding.Left + ExpandButtonSize,
                    viewPadding.Top,
                    viewRect.Width - viewPadding.Horizontal - ExpandButtonSize,
                    viewRect.Height - viewPadding.Vertical);
            }
        }

        protected Rectangle ExpandButtonRect
        {
            get
            {
                Rectangle viewRect = this.ClientRectangle;
                Padding viewPadding = this.Padding;

                return new Rectangle(
                    viewPadding.Left,
                    viewPadding.Top,
                    ExpandButtonSize,
                    viewRect.Height - viewPadding.Vertical);
            }
        }

        protected override Padding DefaultMargin { get { return new Padding(0); } }
        protected override Size DefaultSize { get { return new Size(320, 24); } }

        public StatusBar()
        {

        }

        protected override void OnPaint(PaintEventArgs pe)
        {
            DrawingUtils.FillRectangle(pe.Graphics, this.AccentColor, this.ClientRectangle);

            TextRenderer.DrawText(
                pe.Graphics,
                this.Text,
                this.Font,
                this.TextRect,
                this.TextColor,
                TextFormatFlags.TextBoxControl
                | TextFormatFlags.VerticalCenter
                | TextFormatFlags.Left
                | TextFormatFlags.SingleLine
                | TextFormatFlags.EndEllipsis);

            using (SolidBrush brush = new SolidBrush(TextColor))
            {
                pe.Graphics.FillPolygon(
                    brush,
                    VectorIcons.UpArrow(ExpandButtonRect, ExpandButtonIconSize));
            }
        }
    }
}
