using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace AfterburnerDataHandler.FlatControls
{
    public class Theme
    {
        public static event EventHandler<EventArgs> GlobalThemeChanged;

        public static Theme Current
        {
            get { return current; }
            set
            {
                current = value ?? new Theme();
                Update();
            }
        }

        private static Theme current = new Theme();

        public enum BackgroundSource
        {
            Theme = 0,
            Inherit = 1
        }


        public Theme()
        {
        }

        public static void Update()
        {
            GlobalThemeChanged?.Invoke(current, EventArgs.Empty);
        }


        public Color AccentColor { get; set; } = Color.FromArgb(90, 160, 210);
        public Color DisabledAccentColor { get; set; } = Color.FromArgb(70, 120, 180);
        public Color BorderColor { get; set; } = Color.FromArgb(90, 160, 230);
        public Color DisabledBorderColor { get; set; } = Color.FromArgb(70, 120, 180);
        public Color TextColor { get; set; } = Color.FromArgb(245, 245, 255);
        public Color DisabledTextColor { get; set; } = Color.FromArgb(150, 150, 160);

        public Color WindowBackgroundColor { get; set; } = Color.FromArgb(35,38,42);
        public Color PanelBackgroundColor { get; set; } = Color.FromArgb(50, 54, 60);

        public Color ControlBackgroundColor { get; set; } = Color.FromArgb(64, 70, 80);
        public Color ControlHighlightColor { get; set; } = Color.FromArgb(85, 95, 110);
        public Color ControlPressColor { get; set; } = Color.FromArgb(40, 50, 58);

        public Color TabViewAccentColor { get; set; } = Color.FromArgb(90, 160, 210);
        public Color TabViewBackgroundColor { get; set; } = Color.FromArgb(50, 54, 60);
        public Color TabViewTextColor { get; set; } = Color.FromArgb(245, 245, 255);
        public Color TabViewSelectedTextColor { get; set; } = Color.FromArgb(245, 245, 255);
        public Color TabViewHighlightColor { get; set; } = Color.FromArgb(60, 64, 70);
        public Color TabViewSelectColor { get; set; } = Color.FromArgb(70, 76, 84);
    }
}
