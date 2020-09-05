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
    public class SerialSettingsPanel : VerticalListContainer
    {
        public PropertyContainer Header { get; protected set; }

        public Toggle ServerAutorunToggle { get; protected set; }
        public PropertyContainer ServerAutorunProperty { get; protected set; }

        public Toggle OpenLastProjectToggle { get; protected set; }
        public PropertyContainer OpenLastProjectProperty { get; protected set; }

        public Toggle ServerNotificationsToggle { get; protected set; }
        public PropertyContainer ServerNotificationsProperty { get; protected set; }

        public InputField PortWhitelistField { get; protected set; }
        public PropertyContainer PortWhitelistProperty { get; protected set; }

        public InputField PortBlacklistField { get; protected set; }
        public PropertyContainer PortBlacklistProperty { get; protected set; }

        private readonly List<string> targetProperties = new List<string>
        {
            "SerialPort_Autorun",
            "SerialPort_OpenLastProject",
            "SerialPort_ServerNotifications",
            "SerialPort_PortsWhitelist",
            "SerialPort_PortsBlacklist"
        };

        public SerialSettingsPanel()
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
                Properties.Settings.Default.SerialPort_Autorun = ServerAutorunToggle.Checked;
                Properties.Settings.Default.Save();
            };

            OpenLastProjectToggle.Click += (object sender, EventArgs e) =>
            {
                Properties.Settings.Default.SerialPort_OpenLastProject = OpenLastProjectToggle.Checked;
                Properties.Settings.Default.Save();
            };

            ServerNotificationsToggle.Click += (object sender, EventArgs e) =>
            {
                Properties.Settings.Default.SerialPort_ServerNotifications = ServerNotificationsToggle.Checked;
                Properties.Settings.Default.Save();
            };

            PortWhitelistField.Leave += (object sender, EventArgs e) =>
            {
                Properties.Settings.Default.SerialPort_PortWhitelist = PortWhitelistField.Text;
                Properties.Settings.Default.Save();
            };

            PortBlacklistField.Leave += (object sender, EventArgs e) =>
            {
                Properties.Settings.Default.SerialPort_PortBlacklist = PortBlacklistField.Text;
                Properties.Settings.Default.Save();
            };
        }

        protected virtual void UpdateValues()
        {
            ServerAutorunToggle.Checked = Properties.Settings.Default.SerialPort_Autorun;
            OpenLastProjectToggle.Checked = Properties.Settings.Default.SerialPort_OpenLastProject;
            ServerNotificationsToggle.Checked = Properties.Settings.Default.SerialPort_ServerNotifications;
            PortWhitelistField.Text = Properties.Settings.Default.SerialPort_PortWhitelist;
            PortBlacklistField.Text = Properties.Settings.Default.SerialPort_PortBlacklist;
        }

        protected virtual void InitializeGUI()
        {
            this.AutoSize = true;
            this.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            this.Padding = new Padding(3, 3, 3, 9);

            this.Header = new PropertyContainer
            {
                Text = "Serial Port",
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

            this.PortWhitelistProperty = new PropertyContainer
            {
                Text = "Port Whitelist",
                Margin = new Padding(6, 0, 6, 0)
            };
            this.Controls.Add(PortWhitelistProperty);

            this.PortWhitelistField = new InputField
            {

            };
            PortWhitelistProperty.Controls.Add(PortWhitelistField);

            this.PortBlacklistProperty = new PropertyContainer
            {
                Text = "Port Blacklist",
                Margin = new Padding(6, 0, 6, 0)
            };
            this.Controls.Add(PortBlacklistProperty);

            this.PortBlacklistField = new InputField
            {

            };
            PortBlacklistProperty.Controls.Add(PortBlacklistField);
        }
    }
}
