using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.Layout;

namespace AfterburnerDataHandler.FlatControls
{
    public class FlatForm : Form
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
        public virtual Color TextColor
        {
            get { return textColor; }
            set
            {
                textColor = value;
            }
        }

        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public virtual Color DisabledTextColor
        {
            get { return disabledTextColor; }
            set
            {
                disabledTextColor = value;
            }
        }

        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
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

        private Theme theme = new Theme();
        private Color textColor = Color.FromArgb(255, 255, 255);
        private Color disabledTextColor = Color.FromArgb(255, 255, 255);
        private Color backgroundColor = Color.FromArgb(0, 0, 0);
        private Theme.BackgroundSource backgroundSource = Theme.BackgroundSource.Theme;

        public FlatForm()
        {
            this.Theme = DefaultTheme;
            Theme.GlobalThemeChanged += GlobalThemeChanged;
        }

        protected override void OnPaintBackground(PaintEventArgs pevent)
        {
            DrawingUtils.FillRectangle(pevent.Graphics, this.BackColor, pevent.ClipRectangle);
        }

        protected virtual void UpdateTheme()
        {
            this.BackgroundColor = this.Theme.WindowBackgroundColor;
            this.TextColor = this.Theme.TextColor;
            this.DisabledTextColor = this.Theme.DisabledTextColor;
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
    }
}
