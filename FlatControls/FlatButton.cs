using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AfterburnerDataHandler.FlatControls
{
    public class FlatButton : ThemedControl
    {
        [Browsable(true), Category("Appearance")]
        public virtual Image Icon
        {
            get { return icon; }
            set
            {
                icon = value;
                UpdateLayout();
            }
        }

        [Browsable(true), Category("Appearance")]
        public virtual Color IconMultiplyColor
        {
            get { return iconMultiplyColor; }
            set
            {
                iconMultiplyColor = value;
                this.Invalidate();
            }
        }

        [Browsable(true), Category("Appearance")]
        public virtual int IconSize
        {
            get { return iconSize; }
            set
            {
                iconSize = value > 0 ? value : 0;
                UpdateLayout();
            }
        }

        [Browsable(true), Category("Appearance")]
        public virtual int IconOffset
        {
            get { return iconOffset; }
            set
            {
                iconOffset = value > 0 ? value : 0;
                UpdateLayout();
            }
        }

        [Browsable(true), Category("Appearance")]
        public virtual bool UseButtonBorder
        {
            get { return useButtonBorder; }
            set
            {
                useButtonBorder = value;
                UpdateLayout();
            }
        }

        [Browsable(true), Category("Appearance")]
        public virtual bool Multiline
        {
            get { return multiline; }
            set
            {
                multiline = value;
                UpdateLayout();
            }
        }

        [Browsable(true), Category("Appearance")]
        public virtual ContentAlignment TextAlignment
        {
            get { return textAlignment; }
            set
            {
                textAlignment = value;
                UpdateLayout();
            }
        }

        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public override Color BackColor { get { return base.BackColor; } set { base.BackColor = value; } }
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public override Color ForeColor { get { return base.ForeColor; } set { base.ForeColor = value; } }

        public bool IsHover { get { return isHover; } protected set { isHover = value; } }
        public bool IsFocused { get { return isFocused; } protected set { isFocused = value; } }
        public bool IsPressed { get { return isPressed; } protected set { isPressed = value; } }

        protected virtual ContentAlignment DefaulTextAlignment { get { return ContentAlignment.MiddleCenter; } }

        protected virtual Rectangle TextRect
        {
            get
            {
                int iconArea = Icon == null ? 0 : IconSize + IconOffset;
                Rectangle viewRect = this.ClientRectangle;
                Padding viewPadding = this.Padding;

                return new Rectangle(
                    viewPadding.Left + iconArea,
                    viewPadding.Top,
                    viewRect.Width - viewPadding.Horizontal - iconArea,
                    viewRect.Height - viewPadding.Vertical);
            }
        }

        protected virtual Rectangle IconRect
        {
            get
            {
                Rectangle viewRect = this.ClientRectangle;
                Padding viewPadding = this.Padding;

                return new Rectangle(
                    viewPadding.Left,
                    viewPadding.Top,
                    Math.Min(IconSize, viewRect.Width - viewPadding.Horizontal),
                    viewRect.Height - viewPadding.Vertical);
            }
        }


        protected override Size DefaultSize { get { return new Size(120, 26); } }
        protected override Padding DefaultPadding { get { return new Padding(2, 0, 2, 0); } }

        protected override Theme.BackgroundSource DefaultBackgroundSource
        {
            get { return Theme.BackgroundSource.Theme; }
        }

        private bool isHover = false;
        private bool isFocused = false;
        private bool isPressed = false;
        private bool isKeyPressed = false;
        private bool useButtonBorder = true;
        private bool multiline = false;
        private ContentAlignment textAlignment;
        private TextFormatFlags textFormat = TextFormatFlags.TextBoxControl;
        private Image icon;
        private Color iconMultiplyColor = Color.White;
        private int iconSize = 16;
        private int iconOffset = 0;
        private Rectangle iconRect = new Rectangle();
        private Rectangle textRect = new Rectangle();

        public FlatButton()
        {
            SetStyle(ControlStyles.StandardDoubleClick, false);
            this.TextAlignment = DefaulTextAlignment;
        }

        protected virtual void UpdateLayout()
        {
            this.iconRect = this.IconRect;
            this.textRect = this.TextRect;

            textFormat = DrawingUtils.ContentAlignmentToTextFormatFlags(TextAlignment);

            textFormat |= TextFormatFlags.TextBoxControl
                        | TextFormatFlags.WordBreak
                        | TextFormatFlags.EndEllipsis;

            if (Multiline)
            {
                textFormat |= TextFormatFlags.WordBreak;
                textFormat &= ~TextFormatFlags.SingleLine;
            }
            else
            {
                textFormat |= TextFormatFlags.SingleLine;
                textFormat &= ~TextFormatFlags.WordBreak;
            }

            this.Invalidate();
        }

        protected override void OnPaint(PaintEventArgs pevent)
        {
            bool pressed = this.IsPressed;
            bool highlighted = this.IsHover;

            highlighted |= (this.UseButtonBorder == false && this.IsFocused == true);

            Rectangle backgroundRect = this.ClientRectangle;
            Color backgroundColor = highlighted ? this.HighlightColor : this.BackColor;
            Color borderColor = this.IsFocused == true ? this.AccentColor : this.TextColor;
            Color labelColor = this.TextColor;

            if (pressed == true) backgroundColor = this.PressColor;

            if (this.Enabled == false)
            {
                borderColor = labelColor = DisabledTextColor;
            }

            using (SolidBrush brush = new SolidBrush(backgroundColor))
            {
                pevent.Graphics.FillRectangle(brush, backgroundRect);
            }

            SmoothingMode lastSmoothingMode = pevent.Graphics.SmoothingMode;
            pevent.Graphics.SmoothingMode = SmoothingMode.None;

            if (this.UseButtonBorder == true)
            {
                using (Pen pen = new Pen(borderColor, 2))
                {
                    pen.Alignment = PenAlignment.Inset;
                    pevent.Graphics.DrawRectangle(pen, backgroundRect);
                }
            }

            pevent.Graphics.SmoothingMode = lastSmoothingMode;

            TextRenderer.DrawText(
                pevent.Graphics,
                this.Text,
                this.Font,
                this.textRect,
                labelColor,
                this.textFormat);

            DrawingUtils.DrawIcon(
                pevent.Graphics,
                this.iconRect,
                this.Icon,
                this.Enabled == true ? IconMultiplyColor : DisabledTextColor);
        }

        protected override void OnHandleCreated(EventArgs e)
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint, true);
            base.OnHandleCreated(e);
        }

        protected override void OnLayout(LayoutEventArgs levent)
        {
            base.OnLayout(levent);
            UpdateLayout();
        }

        protected override void OnMouseDown(MouseEventArgs mevent)
        {
            IsPressed = true;
            base.OnMouseDown(mevent);
            this.Select();
            this.Invalidate();
        }

        protected override void OnMouseUp(MouseEventArgs mevent)
        {
            IsPressed = false;
            base.OnMouseUp(mevent);
            this.Invalidate();
        }

        protected override void OnClick(EventArgs e)
        {
            if (this.CanSelect == true)
            {
                base.OnClick(e);
                this.Invalidate();
            }
        }

        protected override void OnKeyDown(KeyEventArgs kevent)
        {
            if (kevent.KeyCode == Keys.Enter || kevent.KeyCode == Keys.Space)
            {
                IsPressed = true;
                isKeyPressed = true;
                kevent.Handled = true;
                this.Select();
                this.Invalidate();
            }
            base.OnKeyDown(kevent);
        }

        protected override void OnKeyUp(KeyEventArgs kevent)
        {
            if (isKeyPressed == true && kevent.KeyCode == Keys.Enter || kevent.KeyCode == Keys.Space)
            {
                this.InvokeOnClick(this, new EventArgs());
                IsPressed = false;
                isKeyPressed = false;
                kevent.Handled = true;
                this.Invalidate();
            }

            base.OnKeyUp(kevent);
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

        protected override void OnMouseEnter(EventArgs e)
        {
            IsHover = true;
            base.OnMouseEnter(e);
            this.Invalidate();
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            IsHover = false;
            base.OnMouseLeave(e);
            this.Invalidate();
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
        }
    }
}