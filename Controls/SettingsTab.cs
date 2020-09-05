using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using AfterburnerDataHandler.FlatControls;
using AfterburnerDataHandler.Servers;

namespace AfterburnerDataHandler.Controls
{
    public class SettingsTab : TabPanel
    {
        public VerticalListContainer MainContainer { get; protected set; }
        public PropertyContainer Header { get; protected set; }
        public FlexColumnContainer SettingsContainer { get; protected set; }
        public LoggerSettingsPanel LoggerSettings { get; protected set; }
        public SerialSettingsPanel SerialSettings { get; protected set; }

        public SettingsTab()
        {
            InitializeGUI();
        }

        protected virtual void InitializeGUI()
        {
            this.MainContainer = new VerticalListContainer
            {
                Dock = DockStyle.Fill,
                AutoScroll = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                Padding = new Padding(0),
                AutoScrollOffset = new Point(0),
                BackgroundSource = Theme.BackgroundSource.Inherit
            };
            this.Controls.Add(MainContainer);

            this.Header = new PropertyContainer
            {
                Text = "Settings",
                Height = 38,
                AutoScroll = false,
                MinimumSize = new Size(380, 20),
                FitContent = false,
                ControlsAlignment = HorizontalAlignment.Right,
                Padding = new Padding(3),
                Margin = new Padding(12, 12, 12, 6),
                MaximumSize = new Size(1200, int.MaxValue),
                Font = MainForm.HeaderFont
            };
            MainContainer.Controls.Add(Header);

            this.SettingsContainer = new FlexColumnContainer
            {
                AutoScroll = false,
                Height = 600,
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                ColumnsLayout = FlexColumnsLayoutEngine.ColumnsLayout.HorizontalGrid,
                Padding = new Padding(6, 0, 6, 0),
                Margin = new Padding(0),
                MaximumSize = new Size(1232, int.MaxValue),
                BackgroundSource = Theme.BackgroundSource.Inherit,
                MinColumnCount = 1,
                MaxColumnCount = 4,
                MinColumnSize = 260,
                MaxColumnSize = 0,
            };
            MainContainer.Controls.Add(SettingsContainer);

            this.LoggerSettings = new LoggerSettingsPanel
            {
                Margin = new Padding(6)
            };
            SettingsContainer.Controls.Add(LoggerSettings);

            this.SerialSettings = new SerialSettingsPanel
            {
                Margin = new Padding(6)
            };
            SettingsContainer.Controls.Add(SerialSettings);
        }
    }
}
