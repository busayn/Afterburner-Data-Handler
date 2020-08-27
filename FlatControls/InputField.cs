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
    public class InputField : UserControl, IThemedControl
    {
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

        public event EventHandler<EventArgs> ThemeDataChanged;

        [Browsable(false)]
        public virtual Color AccentColor
        {
            get { return accentColor; }
            protected set
            {
                accentColor = value;
                OnThemeDataChanged(EventArgs.Empty);
            }
        }

        [Browsable(false)]
        public virtual Color TextColor
        {
            get { return textColor; }
            protected set
            {
                textColor = value;
                OnThemeDataChanged(EventArgs.Empty);
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

        [Browsable(false)]
        public virtual Color BackgroundColor
        {
            get { return backgroundColor; }
            protected set
            {
                backgroundColor = value;
                OnThemeDataChanged(EventArgs.Empty);
            }
        }

        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public virtual FlatTextBox TextBox { get { return textBox; } protected set { textBox = value; } }

        [Browsable(true), Category("Appearance")]
        public virtual int BorderSize
        {
            get { return borderSize; }
            set
            {
                borderSize = value > 0 ? value : 0;
                UpdateLayout();
            }
        }

        [Bindable(true)]
        [Browsable(true)]
        [EditorBrowsable(EditorBrowsableState.Always)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public override string Text
        {
            get { return this.TextBox?.Text; }
            set
            {
                if (TextBox != null) TextBox.Text = value;
            }
        }

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

        protected override Size DefaultSize { get { return new Size(120, 26); } }
        protected override Padding DefaultPadding { get { return new Padding(3, 0, 3, 0); } }

        protected virtual Rectangle TextBoxRect
        {
            get
            {
                Rectangle viewRect = this.ClientRectangle;
                Padding viewPadding = this.Padding;

                return new Rectangle(
                    viewPadding.Left,
                    viewPadding.Top,
                    viewRect.Width - viewPadding.Horizontal,
                    viewRect.Height - viewPadding.Vertical);
            }
        }

        protected bool FieldFocused { get { return fieldFocused; } set { fieldFocused = value; } }
        protected Theme DefaultTheme { get { return UseGlobalTheme ? Theme.Current : new Theme(); } }

        private Theme theme = new Theme();
        private Color accentColor = Color.FromArgb(140, 190, 240);
        private Color textColor = Color.FromArgb(255, 255, 255);
        private Color disabledTextColor = Color.FromArgb(180, 180, 180);
        private Color backgroundColor = Color.FromArgb(140, 190, 240);

        private FlatTextBox textBox = null;
        private int borderSize = 2;
        private bool fieldFocused = false;
        private bool isSilent = false;


        public InputField()
        {
            SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.DoubleBuffer, true);
            this.ForeColor = Color.FromArgb(255, 255, 255);

            this.TextBox = new FlatTextBox
            {
                Text = this.Text,
                UseGlobalTheme = false,
                Theme = this.Theme
            };

            this.TextBox.TextChanged += TextBoxTextChanged;
            this.Controls.Add(this.TextBox);

            this.Theme = DefaultTheme;
            Theme.GlobalThemeChanged += GlobalThemeChanged;
        }

        public virtual void UpdateTheme()
        {
            this.AccentColor = theme.AccentColor;
            this.TextColor = theme.TextColor;
            this.DisabledTextColor = theme.DisabledTextColor;
            this.BackgroundColor = theme.ControlBackgroundColor;

            TextBox.UseGlobalTheme = false;
            TextBox.Theme = this.Theme;
            TextBox.UpdateTheme();

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

        public virtual void SetValueSilently(object value)
        { 
            SetValueSilently(value.ToString());
        }

        public virtual void SetValueSilently(string value)
        {
            lock (this)
            {
                isSilent = true;
                this.Text = value;
                isSilent = false;
            }

            UpdateLayout();
        }

        protected virtual void UpdateLayout()
        {
            Rectangle viewRect = TextBoxRect;
            Padding viewPadding = this.Padding;
            int lineSize = BorderSize;

            if (this.TextBox != null)
            {
                this.TextBox.Width = viewRect.Width - viewPadding.Horizontal;

                this.TextBox.Location = new Point(
                    viewRect.Left + viewPadding.Left,
                    viewRect.Top + (viewRect.Height - TextBox.Height) / 2
                        + viewPadding.Top - viewPadding.Bottom - lineSize / 2);
            }

            this.Invalidate();
        }

        private void TextBoxTextChanged(object sender, EventArgs e)
        {
            if (isSilent == false)
            {
                this.OnTextChanged(e);
                UpdateLayout();
            }
        }

        protected override void OnLayout(LayoutEventArgs e)
        {
            base.OnLayout(e);
            UpdateLayout();
        }

        protected override void OnBackColorChanged(EventArgs e)
        {
            if (this.TextBox != null) this.TextBox.BackColor = this.BackColor;
            base.OnBackColorChanged(e);
        }

        protected override void OnForeColorChanged(EventArgs e)
        {
            if (this.TextBox != null) this.TextBox.ForeColor = this.ForeColor;
            base.OnForeColorChanged(e);
        }

        protected override void OnPaintBackground(PaintEventArgs pevent)
        {
            Rectangle backgroundRect = this.ClientRectangle;
            Color backgroundColor = this.BackgroundColor;
            Color lineColor = FieldFocused == true ? this.AccentColor : this.TextColor;
            int lineSize = borderSize;
            int lineOffset = lineSize / 2;

            if (this.Enabled == false) lineColor = disabledTextColor;

            SmoothingMode lastSmoothingMode = pevent.Graphics.SmoothingMode;
            pevent.Graphics.SmoothingMode = SmoothingMode.None;

            using (SolidBrush brush = new SolidBrush(backgroundColor))
            {
                pevent.Graphics.FillRectangle(brush, pevent.ClipRectangle);
            }

            using (Pen pen = new Pen(lineColor, lineSize))
            {
                pen.Alignment = PenAlignment.Left;

                pevent.Graphics.DrawLine(
                    pen,
                    backgroundRect.Left,
                    backgroundRect.Bottom - lineOffset,
                    backgroundRect.Right,
                    backgroundRect.Bottom - lineOffset);
            }

            pevent.Graphics.SmoothingMode = lastSmoothingMode;
        }

        protected override void OnEnter(EventArgs e)
        {
            FieldFocused = true;
            base.OnEnter(e);
            UpdateLayout();
        }

        protected override void OnLeave(EventArgs e)
        {
            FieldFocused = false;
            base.OnLeave(e);
            UpdateLayout();
        }

        protected override void OnEnabledChanged(EventArgs e)
        {
            base.OnEnabledChanged(e);

            if (this.TextBox != null)
            {
                if (this.Enabled == true)
                {
                    this.TextBox.ForeColor = TextColor;
                }
                else
                {
                    this.TextBox.ForeColor = DisabledTextColor;
                }
            }
        }
    }
}
