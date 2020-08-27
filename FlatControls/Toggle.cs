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
    public class Toggle : ThemedControl
    {
        [Browsable(true), Category("Appearance")]
        public bool Checked
        {
            get { return isChecked; }
            set
            {
                isChecked = value;
                UpdateLayout();
                OnCheckedChanged(EventArgs.Empty);
            }
        }

        [Browsable(false)]
        public event EventHandler<EventArgs> CheckedChanged;

        [Browsable(true), Category("Appearance")]
        public virtual int CheckmarkWidth
        { 
            get { return checkmarkWidth; }
            set
            {
                checkmarkWidth = value > 0 ? value : 0; ;
                UpdateLayout();
            }
        }

        [Browsable(true), Category("Appearance")]
        public virtual int CheckmarkHeight
        {
            get { return checkmarkHeight; }
            set
            {
                checkmarkHeight = value > 0 ? value : 0; ;
                UpdateLayout();
            }
        }

        public enum VerticalAlignment
        {
            Top = 1,
            Middle = 2,
            Bottom = 4
        }

        [Browsable(true), Category("Appearance")]
        public virtual VerticalAlignment CheckmarkAlignment
        {
            get { return checkmarkAlignment; }
            set
            {
                checkmarkAlignment = value;
                UpdateLayout();
            }
        }

        [Browsable(true), Category("Appearance")]
        public virtual int TextOffset
        {
            get { return textOffset; }
            set
            {
                textOffset = value > 0 ? value : 0; ;
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

        public enum CheckmarkStyle
        {
            Checkmark = 0,
            Square = 1,
            Toggle = 2
        }

        [Browsable(true), Category("Appearance")]
        public virtual CheckmarkStyle Style
        {
            get { return style; }
            set
            {
                style = value;
                UpdateLayout();
            }
        }

        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public override Color BackColor { get { return base.BackColor; } set { base.BackColor = value; } }

        protected virtual Rectangle TextRect { get { return textRect; } set { textRect = value; } }
        protected virtual Rectangle CheckmarkRect { get { return checkmarkRect; } set { checkmarkRect = value; } }
        protected virtual Rectangle CheckmarkBackRect { get { return checkmarkBackRect; } set { checkmarkBackRect = value; } }

        protected bool IsHover { get { return isHover; } set { isHover = value; } }
        protected bool IsFocused { get { return isFocused; } set { isFocused = value; } }
        protected bool IsPressed { get { return isPressed; } set { isPressed = value; } }

        protected Point[] CheckmarkIcon { get { return checkmarkIcon; } set { checkmarkIcon = value; } }
        protected override Size DefaultSize { get { return new Size(120, 26); }}

        private bool isChecked = false;
        private int checkmarkWidth = 36;
        private int checkmarkHeight = 18;

        private Rectangle textRect;
        private Rectangle checkmarkRect;
        private Rectangle checkmarkBackRect;

        private bool isHover = false;
        private bool isFocused = false;
        private bool isPressed = false;
        private bool isKeyPressed = false;
        private Point[] checkmarkIcon = new Point[0];
        
        private VerticalAlignment checkmarkAlignment = VerticalAlignment.Middle;
        private CheckmarkStyle style = CheckmarkStyle.Checkmark;

        private int textOffset = 3;
        private bool multiline = false;
        private ContentAlignment textAlignment = ContentAlignment.MiddleLeft;
        private TextFormatFlags textFormat = TextFormatFlags.TextBoxControl;

        public Toggle()
        {
            SetStyle(
                ControlStyles.Selectable |
                ControlStyles.OptimizedDoubleBuffer |
                ControlStyles.StandardClick,
                true);

            SetStyle(
                ControlStyles.ContainerControl | 
                ControlStyles.StandardDoubleClick,
                false);
        }

        public virtual void SetValueSilently(bool value)
        {
            this.isChecked = value;
            UpdateLayout();
        }

        protected virtual void UpdateLayout()
        {
            Rectangle viewRect = this.ClientRectangle;
            Padding viewPadding = this.Padding;

            Rectangle labelRect;
            Rectangle checkBackRect;
            Rectangle checkRect;

            int checkWidth = this.CheckmarkWidth;
            int checkHeight = this.CheckmarkHeight;
            int checkOffset = 5;

            checkBackRect = new Rectangle();

            switch (CheckmarkAlignment)
            {
                case VerticalAlignment.Top:
                    checkBackRect.X = viewRect.Left + viewPadding.Left;
                    checkBackRect.Y = viewRect.Top + viewPadding.Top;
                    break;
                case VerticalAlignment.Middle:
                    checkBackRect.X = viewRect.Left + viewPadding.Left;
                    checkBackRect.Y = viewPadding.Top + (viewRect.Height - viewPadding.Vertical - checkHeight) / 2;
                    break;
                case VerticalAlignment.Bottom:
                    checkBackRect.X = viewRect.Left + viewPadding.Left;
                    checkBackRect.Y = viewRect.Bottom - viewPadding.Bottom - checkHeight;
                    break;
            }

            switch (Style)
            {
                case CheckmarkStyle.Checkmark:
                    checkBackRect.Width = checkBackRect.Height = checkHeight;
                    checkRect = checkBackRect;
                    checkRect.Width = checkRect.Height -= checkOffset * 2;
                    checkRect.X += checkOffset;
                    checkRect.Y += checkOffset;
                    this.CheckmarkIcon = VectorIcons.Checkmark(checkRect);
                    break;

                case CheckmarkStyle.Square:
                    checkBackRect.Width = checkBackRect.Height = checkHeight;
                    checkRect = checkBackRect;
                    checkRect.Width = checkRect.Height -= checkOffset * 2;
                    checkRect.X += checkOffset;
                    checkRect.Y += checkOffset;
                    break;

                case CheckmarkStyle.Toggle:
                    checkBackRect.Width = checkWidth;
                    checkBackRect.Height = checkHeight;
                    checkRect = checkBackRect;
                    checkRect.Width -= checkOffset * 2;
                    checkRect.Width /= 2;
                    checkRect.Height -= checkOffset * 2;
                    checkRect.X += checkOffset;
                    checkRect.Y += checkOffset;

                    if (Checked == true) 
                        checkRect.X += checkRect.Width;
                    break;

                default:
                    checkBackRect.Width = checkWidth;
                    checkBackRect.Height = checkHeight;
                    checkRect = checkBackRect;
                    break;
                    
            }

            labelRect = new Rectangle(
                viewRect.Left + viewPadding.Left + checkBackRect.Width + TextOffset,
                viewRect.Top + viewPadding.Top,
                viewRect.Width - viewPadding.Horizontal - checkBackRect.Width - TextOffset,
                viewRect.Height - viewPadding.Vertical);

            TextRect = labelRect;
            CheckmarkBackRect = checkBackRect;
            CheckmarkRect = checkRect;

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

        protected virtual void OnCheckedChanged(EventArgs e)
        {
            CheckedChanged?.Invoke(this, e);
        }

        protected override void OnPaintBackground(PaintEventArgs pevent)
        {
            using (SolidBrush brush = new SolidBrush(this.BackColor))
            {
                pevent.Graphics.FillRectangle(brush, pevent.ClipRectangle);
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            bool highlighted = this.IsHover;
            bool focused = this.IsFocused;
            bool pressed = this.IsPressed;
            bool hasChecked = this.Checked;

            Color backgroundColor = highlighted ? this.HighlightColor : this.BackgroundColor;
            Color borderColor = focused == true ? this.AccentColor : this.TextColor;
            Color textColor = this.TextColor;
            Color accentColor = this.AccentColor;

            Rectangle checkBackRect = CheckmarkBackRect;

            if (pressed == true)
            {
                backgroundColor = this.PressColor;
            }

            if (this.Enabled == false)
            {
                backgroundColor = this.BackgroundColor;
                borderColor = textColor = this.DisabledTextColor;
                accentColor = this.DisabledAccentColor;
            }

            TextRenderer.DrawText(
                e.Graphics,
                this.Text,
                this.Font,
                this.TextRect,
                textColor,
                this.textFormat);

            using (SolidBrush brush = new SolidBrush(backgroundColor))
            {
                e.Graphics.FillRectangle(brush, checkBackRect);
            }

            using (Pen pen = new Pen(borderColor))
            {
                pen.Width = 2;
                pen.Alignment = PenAlignment.Inset;

                e.Graphics.DrawRectangle(pen, checkBackRect);
            }

            switch (Style)
            {
                case CheckmarkStyle.Checkmark:
                    if (hasChecked)
                    {
                        SmoothingMode lastSmoothingMode = e.Graphics.SmoothingMode;
                        e.Graphics.SmoothingMode = SmoothingMode.HighQuality;

                        using (Pen pen = new Pen(accentColor))
                        {
                            pen.Width = 2.5f;
                            pen.Alignment = PenAlignment.Center;
                            pen.SetLineCap(LineCap.Round, LineCap.Round, DashCap.Round);

                            e.Graphics.DrawLines(pen, this.CheckmarkIcon);
                        }

                        e.Graphics.SmoothingMode = lastSmoothingMode;
                    }
                    break;
                case CheckmarkStyle.Square:
                    if (hasChecked)
                    {
                        using (SolidBrush brush = new SolidBrush(accentColor))
                        {
                            e.Graphics.FillRectangle(brush, checkmarkRect);
                        }
                    }
                    break;
                case CheckmarkStyle.Toggle:
                    using (SolidBrush brush = new SolidBrush(hasChecked == true ? accentColor : textColor))
                    {
                        e.Graphics.FillRectangle(brush, checkmarkRect);
                    }
                    break;
            }
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
            Checked = !Checked;
            base.OnClick(e);
            this.Invalidate();
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
    }
}