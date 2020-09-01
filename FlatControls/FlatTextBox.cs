using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AfterburnerDataHandler.FlatControls
{
    public class FlatTextBox : TextBox
    {
        [Browsable(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public string LabelText
        {
            get { return labelText; }
            set
            {
                labelText = value;
                this.Invalidate();
            }
        }

        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public virtual bool UseGlobalTheme { get; set; } = true;

        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Theme Theme
        {
            get
            {
                if (theme == null) theme = this.DefaultTheme;
                return theme;
            }
            set
            {
                theme = value;
                this.UpdateTheme();
            }
        }

        [Browsable(false)]
        public virtual Color DisabledTextColor
        {
            get { return disabledTextColor; }
            protected set
            {
                disabledTextColor = value;
                OnThemeDataChanged(EventArgs.Empty);
            }
        }

        public event EventHandler<EventArgs> ThemeDataChanged;

        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public override Color BackColor
        {
            get { return Theme.ControlBackgroundColor; }
            set { base.BackColor = Theme.ControlBackgroundColor; }
        }

        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public override Color ForeColor
        {
            get { return Theme.TextColor; }
            set { base.ForeColor = Theme.TextColor; }
        }

        protected Theme DefaultTheme { get { return UseGlobalTheme ? Theme.Current : new Theme(); } }

        private string labelText = string.Empty;
        private Theme theme = new Theme();
        private Color disabledTextColor;
        private TextFormatFlags textFormat = TextFormatFlags.TextBoxControl
                                           | TextFormatFlags.NoClipping
                                           | TextFormatFlags.NoPadding;
        public FlatTextBox()
        {
            this.ResizeRedraw = true;
            this.BorderStyle = BorderStyle.None;
            this.Theme = DefaultTheme;
            Theme.GlobalThemeChanged += GlobalThemeChanged;
        }

        public virtual void UpdateTheme()
        {
            this.BackColor = theme.ControlBackgroundColor;
            this.ForeColor = theme.TextColor;
            this.DisabledTextColor = theme.DisabledTextColor;
            this.Invalidate();

            this.Invalidate();
        }

        public virtual void OnThemeDataChanged(EventArgs e)
        {
            this.Invalidate();
            ThemeDataChanged?.Invoke(this, e);
        }

        protected void GlobalThemeChanged(object sender, EventArgs e)
        {
            this.Theme = Theme.Current;
        }

        protected override void WndProc(ref Message m)
        {
            base.WndProc(ref m);

            if (m.Msg == NativeMethods.WM_PAINT && this.IsHandleCreated == true)
            {
                if (this.Enabled == false ||
                    string.IsNullOrEmpty(this.Text) && !string.IsNullOrEmpty(this.LabelText) && this.Focused == false)
                {
                    Graphics g = m.HWnd != IntPtr.Zero ? Graphics.FromHwnd(m.HWnd) : null;

                    if (g != null)
                    {
                        DrawingUtils.FillRectangle(g, this.BackColor, this.ClientRectangle);

                        TextRenderer.DrawText(
                            g,
                            this.Enabled == true ? this.LabelText : this.Text,
                            this.Font,
                            this.ClientRectangle,
                            this.DisabledTextColor,
                            this.textFormat);
                    }
                }
            }
        }

        protected override void OnTextAlignChanged(EventArgs e)
        {
            switch (this.TextAlign)
            {
                case HorizontalAlignment.Left:
                    this.textFormat |= TextFormatFlags.Left;
                    this.textFormat &= ~(TextFormatFlags.HorizontalCenter | TextFormatFlags.Right);
                    break;
                case HorizontalAlignment.Right:
                    this.textFormat |= TextFormatFlags.Right;
                    this.textFormat &= ~(TextFormatFlags.Right | TextFormatFlags.HorizontalCenter);
                    break;
                case HorizontalAlignment.Center:
                    this.textFormat |= TextFormatFlags.HorizontalCenter;
                    this.textFormat &= ~(TextFormatFlags.Right | TextFormatFlags.Left);
                    break;
            }

            base.OnTextAlignChanged(e);
        }

        protected override void OnRightToLeftChanged(EventArgs e)
        {
            switch (this.RightToLeft)
            {
                case RightToLeft.No:
                    this.textFormat &= ~TextFormatFlags.RightToLeft;
                    break;
                case RightToLeft.Yes:
                    this.textFormat |= TextFormatFlags.RightToLeft;
                    break;
                case RightToLeft.Inherit:
                    this.textFormat &= ~TextFormatFlags.RightToLeft;
                    break;
            }

            base.OnRightToLeftChanged(e);
        }

        protected override void OnMultilineChanged(EventArgs e)
        {
            base.OnMultilineChanged(e);

            if (this.Multiline == true)
            {
                this.textFormat &= ~TextFormatFlags.SingleLine;
                this.textFormat |= TextFormatFlags.WordBreak;
                this.textFormat &= ~TextFormatFlags.NoPadding;
            }
            else
            {
                this.textFormat |= TextFormatFlags.SingleLine;
                this.textFormat &= ~TextFormatFlags.WordBreak;
                this.textFormat |= TextFormatFlags.NoPadding;
            }
        }

        protected override void OnTextChanged(EventArgs e)
        {
            base.OnTextChanged(e);

            if (string.IsNullOrEmpty(this.Text))
                this.Invalidate();
        }
    }
}
