using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AfterburnerDataHandler.FlatControls
{
    public partial class ThemedControl : Control, IThemedControl
    {
        [Category("Appearance")]
        [Browsable(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public Theme.BackgroundSource BackgroundSource
        {
            get { return backgroundSource; }
            set
            {
                backgroundSource = value;
                this.OnBackColorChanged(EventArgs.Empty);
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
        public virtual Color DisabledAccentColor
        {
            get { return disabledAccentColor; }
            protected set
            {
                disabledAccentColor = value;
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

        [Browsable(false)]
        public virtual Color HighlightColor
        {
            get { return highlightColor; }
            protected set
            {
                highlightColor = value;
                OnThemeDataChanged(EventArgs.Empty);
            }
        }

        [Browsable(false)]
        public virtual Color PressColor
        {
            get { return pressColor; }
            protected set
            {
                pressColor = value;
                OnThemeDataChanged(EventArgs.Empty);
            }
        }

        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public override Color BackColor
        {
            get
            {
                switch (backgroundSource)
                {
                    case Theme.BackgroundSource.Inherit:
                        return base.BackColor;
                    case Theme.BackgroundSource.Theme:
                        return this.BackgroundColor;
                    default:
                        return base.BackColor;
                }
            }
            set
            {
                base.BackColor = value;
            }
        }

        protected Theme DefaultTheme { get { return UseGlobalTheme ? Theme.Current : new Theme(); } }
        protected virtual Theme.BackgroundSource DefaultBackgroundSource { get { return Theme.BackgroundSource.Inherit; } }

        private Theme theme = new Theme();
        private Color accentColor;
        private Color disabledAccentColor;
        private Color textColor;
        private Color disabledTextColor;
        private Color backgroundColor;
        private Color highlightColor;
        private Color pressColor;
        private Theme.BackgroundSource backgroundSource = Theme.BackgroundSource.Inherit;

        public ThemedControl()
        {
            SetStyle(ControlStyles.OptimizedDoubleBuffer
                | ControlStyles.DoubleBuffer
                | ControlStyles.ResizeRedraw, true);

            this.Theme = DefaultTheme;
            Theme.GlobalThemeChanged += GlobalThemeChanged;
            this.BackgroundSource = this.DefaultBackgroundSource;
        }

        public virtual void UpdateTheme()
        {
            this.AccentColor = this.Theme.AccentColor;
            this.DisabledAccentColor = this.Theme.DisabledAccentColor;
            this.TextColor = this.Theme.TextColor;
            this.DisabledTextColor = this.Theme.DisabledTextColor;
            this.BackgroundColor = this.Theme.ControlBackgroundColor;
            this.HighlightColor = this.Theme.ControlHighlightColor;
            this.PressColor = this.Theme.ControlPressColor;

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

        protected override void OnTextChanged(EventArgs e)
        {
            base.OnTextChanged(e);
            this.Invalidate();
        }
    }
}
