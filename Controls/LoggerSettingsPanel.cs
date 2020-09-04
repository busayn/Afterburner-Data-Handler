using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using AfterburnerDataHandler.FlatControls;

namespace AfterburnerDataHandler.Controls
{
    public class LoggerSettingsPanel : VerticalListContainer
    {
        public PropertyContainer Header { get; protected set; }

        public Toggle ServerAutorunToggle { get; protected set; }
        public PropertyContainer ServerAutorunProperty { get; protected set; }

        public Toggle OpenLastProjectToggle { get; protected set; }
        public PropertyContainer OpenLastProjectProperty { get; protected set; }

        public Toggle ServerNotificationsToggle { get; protected set; }
        public PropertyContainer ServerNotificationsProperty { get; protected set; }

        public Toggle LoggingNotificationsToggle { get; protected set; }
        public PropertyContainer LoggingNotificationsProperty { get; protected set; }

        public InputField LogPathField { get; protected set; }
        public PropertyContainer LogPathProperty { get; protected set; }

        public HotkeysInputField LogHotkeyField { get; protected set; }
        public PropertyContainer LogHotkeyProperty { get; protected set; }

        private readonly List<string> targetProperties = new List<string>
        {
            "Logger_Autorun",
            "Logger_OpenLastProject",
            "Logger_ServerNotifications",
            "Logger_LoggingNotifications",
            "Logger_LogPath",
            "Logger_LogHotkey"
        };

        public LoggerSettingsPanel()
        {
            InitializeGUI();
            InitializeHandles();
            UpdateValues();
        }

        protected virtual void InitializeHandles()
        {
            Properties.Settings.Default.PropertyChanged += (object sender, PropertyChangedEventArgs e) =>
            {
                if (targetProperties.Contains(e.PropertyName))
                {
                    UpdateValues();
                }
            };

            ServerAutorunToggle.Click += (object sender, EventArgs e) =>
            {
                Properties.Settings.Default.Logger_Autorun = ServerAutorunToggle.Checked;
                Properties.Settings.Default.Save();
            };

            OpenLastProjectToggle.Click += (object sender, EventArgs e) =>
            {
                Properties.Settings.Default.Logger_OpenLastProject = OpenLastProjectToggle.Checked;
                Properties.Settings.Default.Save();
            };

            ServerNotificationsToggle.Click += (object sender, EventArgs e) =>
            {
                Properties.Settings.Default.Logger_ServerNotifications = ServerNotificationsToggle.Checked;
                Properties.Settings.Default.Save();
            };

            LoggingNotificationsToggle.Click += (object sender, EventArgs e) =>
            {
                Properties.Settings.Default.Logger_LoggingNotifications = LoggingNotificationsToggle.Checked;
                Properties.Settings.Default.Save();
            };

            LogPathField.Leave += (object sender, EventArgs e) =>
            {
                Properties.Settings.Default.Logger_LogPath = LogPathField.Text;
                Properties.Settings.Default.Save();
            };

            LogHotkeyField.Leave += (object sender, EventArgs e) =>
            {
                Properties.Settings.Default.Logger_LogHotkey = LogHotkeyField.Hotkeys;
                Properties.Settings.Default.Save();
                Console.WriteLine(LogHotkeyField.Hotkeys);
            };
        }

        protected virtual void UpdateValues()
        {
            ServerAutorunToggle.Checked = Properties.Settings.Default.Logger_Autorun;
            OpenLastProjectToggle.Checked = Properties.Settings.Default.Logger_OpenLastProject;
            ServerNotificationsToggle.Checked = Properties.Settings.Default.Logger_ServerNotifications;
            LoggingNotificationsToggle.Checked = Properties.Settings.Default.Logger_LoggingNotifications;
            LogPathField.Text = Properties.Settings.Default.Logger_LogPath;
            LogHotkeyField.Hotkeys = Properties.Settings.Default.Logger_LogHotkey;
        }

        protected virtual void InitializeGUI()
        {
            this.AutoSize = true;
            this.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            this.Padding = new Padding(3, 3, 3, 9);

            this.Header = new PropertyContainer
            {
                Text = "Logger",
                Height = 38,
                AutoScroll = false,
                MinimumSize = new Size(380, 20),
                MaximumSize = new Size(1200, int.MaxValue),
                FitContent = false,
                ControlsAlignment = HorizontalAlignment.Right,
                Padding = new Padding(0),
                Margin = new Padding(0),
                Font = MainForm.HeaderFont
            };
            this.Controls.Add(Header);

            this.ServerAutorunProperty = new PropertyContainer
            {
                Text = "Autorun",
                Margin = new Padding(6, 0, 6, 0)
            };
            this.Controls.Add(ServerAutorunProperty);

            this.ServerAutorunToggle = new Toggle
            {
                Text = ""
            };
            ServerAutorunProperty.Controls.Add(ServerAutorunToggle);

            this.OpenLastProjectProperty = new PropertyContainer
            {
                Text = "Open Last Project",
                Margin = new Padding(6, 0, 6, 0)
            };
            this.Controls.Add(OpenLastProjectProperty);

            this.OpenLastProjectToggle = new Toggle
            {
                Text = ""
            };
            OpenLastProjectProperty.Controls.Add(OpenLastProjectToggle);

            this.ServerNotificationsProperty = new PropertyContainer
            {
                Text = "Server Notifications",
                Margin = new Padding(6, 0, 6, 0)
            };
            this.Controls.Add(ServerNotificationsProperty);

            this.ServerNotificationsToggle = new Toggle
            {
                Text = ""
            };
            ServerNotificationsProperty.Controls.Add(ServerNotificationsToggle);

            this.LoggingNotificationsProperty = new PropertyContainer
            {
                Text = "Logging Notifications",
                Margin = new Padding(6, 0, 6, 0)
            };
            this.Controls.Add(LoggingNotificationsProperty);

            this.LoggingNotificationsToggle = new Toggle
            {
                Text = ""
            };
            LoggingNotificationsProperty.Controls.Add(LoggingNotificationsToggle);

            this.LogPathProperty = new PropertyContainer
            {
                Text = "Log Path",
                Margin = new Padding(6, 0, 6, 0)
            };
            this.Controls.Add(LogPathProperty);

            this.LogPathField = new InputField
            {

            };
            LogPathProperty.Controls.Add(LogPathField);

            this.LogHotkeyProperty = new PropertyContainer
            {
                Text = "Log Hotkey",
                Margin = new Padding(6, 0, 6, 0)
            };
            this.Controls.Add(LogHotkeyProperty);

            this.LogHotkeyField = new HotkeysInputField
            {

            };
            LogHotkeyProperty.Controls.Add(LogHotkeyField);
        }
    }
}
