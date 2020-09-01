using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using AfterburnerDataHandler.FlatControls;
using AfterburnerDataHandler.Projects;
using AfterburnerDataHandler.Servers;
using AfterburnerDataHandler.Servers.Serial;
using AfterburnerDataHandler.SharedMemory.Afterburner;

namespace AfterburnerDataHandler.Controls
{
    public class SerialTab : TabPanel
    {
        public VerticalListContainer MainContainer { get; protected set; }

        // Server Control Panel
        public PropertyContainer ControlPanel { get; protected set; }
        public FlatButton StartServerButton { get; protected set; }

        // Server Parameters Panel
        public VerticalListContainer ParametersPanel { get; protected set; }
        public PropertyContainer ParametersHeader { get; protected set; }
        public FlexColumnContainer ParametersContainer { get; protected set; }
        public FlatButton SaveServerSettingsButton { get; protected set; }
        public FlatButton LoadServerSettingsButton { get; protected set; }


        // Main Parameters Group
        public VerticalListContainer MainParametersGroup { get; protected set; }

        // Edit Data Property
        public FlatButton EditDataButton { get; protected set; }
        public PropertyContainer EditDataProperty { get; protected set; }

        // Port Speed Property
        public Dropdown PortSpeedField { get; protected set; }
        public PropertyContainer PortSpeedProperty { get; protected set; }


        // Connection Parameters Group
        public VerticalListContainer ConnectionParametersGroup { get; protected set; }

        // Auto Connect Property
        public Toggle AutoConnectToggle { get; protected set; }
        public PropertyContainer AutoConnectProperty { get; protected set; }

        // Auto Connect Request Property
        public InputField AutoConnectRequestField { get; protected set; }
        public PropertyContainer AutoConnectRequestProperty { get; protected set; }

        // Auto Connect Response Property
        public InputField AutoConnectResponseField { get; protected set; }
        public PropertyContainer AutoConnectResponseProperty { get; protected set; }

        // Auto Connect Response Timeout Property
        public NumericInputField AutoConnectResponseTimeoutField { get; protected set; }
        public PropertyContainer AutoConnectResponseTimeoutProperty { get; protected set; }

        // Connection Interval Property
        public NumericInputField ConnectionIntervalField { get; protected set; }
        public PropertyContainer ConnectionIntervalProperty { get; protected set; }


        // Send Mode Parameters Group
        public VerticalListContainer SendModeParametersGroup { get; protected set; }

        // Send Mode Property
        public Dropdown SendModeField { get; protected set; }
        public PropertyContainer SendModeProperty { get; protected set; }

        // Message Interval Property
        public NumericInputField MessageIntervalField { get; protected set; }
        public PropertyContainer MessageIntervalProperty { get; protected set; }

        // Connection Check Property
        public NumericInputField ConnectionCheckField { get; protected set; }
        public PropertyContainer ConnectionCheckProperty { get; protected set; }

        // Data Request Property
        public InputField DataRequestField { get; protected set; }
        public PropertyContainer DataRequestProperty { get; protected set; }

        public SerialPortServer Server
        {
            get
            {
                if (server == null)
                    server = new SerialPortServer();

                return server;
            }
        }

        private SerialPortServer server;

        public SerialTab()
        {
            InitializeGUI();
            InitializeServer();
            InitializeHandles();
            UpdateValues();
            Server.Settings.IsDirty = false;
        }
        protected virtual void InitializeServer()
        {
            Server.StateChanged += ServerStateChanged;

            EventHandler<EventArgs> updateValues = (object sender, EventArgs e) =>
            {
                UpdateValues();
            };

            Server.Settings.ParameterChanged += updateValues;
            Server.Settings.ProjectNameChanged += updateValues;

            Server.SettingsChanged += (object sender, EventArgs e) =>
            {
                Server.Settings.ParameterChanged -= updateValues;
                Server.Settings.ProjectNameChanged -= updateValues;
                Server.Settings.ParameterChanged += updateValues;
                Server.Settings.ProjectNameChanged += updateValues;

                UpdateValues();
            };
        }

        private void ServerStateChanged(object sender, Servers.ServerStateEventArgs e)
        {
            switch (e.state)
            {
                case Servers.ServerState.Begin:
                    ControlUtils.AsyncSafeInvoke(this, () =>
                    {
                        this.StartServerButton.Text = "Stop Server";
                        this.StartServerButton.Icon = Properties.Resources.stop;
                        this.ParametersPanel.Enabled = false;
                    });
                    break;

                case Servers.ServerState.Waiting:
                    break;

                case Servers.ServerState.Connected:
                    ControlUtils.AsyncSafeInvoke(this, () =>
                    {

                    });
                    break;

                case Servers.ServerState.Reconnect:
                    break;

                case Servers.ServerState.Stop:
                    ControlUtils.AsyncSafeInvoke(this, () =>
                    {
                        this.StartServerButton.Text = "Start Server";
                        this.StartServerButton.Icon = Properties.Resources.play;
                        this.ParametersPanel.Enabled = true;
                    });
                    break;
            }

            Console.WriteLine("[Server State] " + e.state);
        }

        protected virtual void InitializeHandles()
        {
            StartServerButton.Click += (object sender, EventArgs e) =>
            {
                if (Server.ServerState == Servers.ServerState.Stop)
                    Server?.Begin();
                else
                    Server?.Stop();
            };

            SaveServerSettingsButton.Click += (object sender, EventArgs e) =>
            {
                ProjectsUtils.ShowSaveProjectDialog(Server.Settings);
            };

            LoadServerSettingsButton.Click += (object sender, EventArgs e) =>
            {
                SerialPortProject newProject = Server.Settings;

                ProjectsUtils.ShowOpenProjectDialog(ref newProject);

                if (Server.Settings != newProject)
                    Server.Settings = newProject;
            };

            EditDataButton.Click += (object sender, EventArgs e) =>
            {
                MainForm.ShowControl(CreateLogDataEditor());
            };

            PortSpeedField.ItemSelected += (object sender, ItemEventArgs e) =>
            {
                Server.Settings.PortSpeed = PortSpeedField.ToEnum(Server.Settings.PortSpeed);
                UpdateValues();
            };

            AutoConnectToggle.Click += (object sender, EventArgs e) =>
            {
                Server.Settings.AutoConnect = AutoConnectToggle.Checked;
                UpdateValues();
            };

            AutoConnectRequestField.Leave += (object sender, EventArgs e) =>
            {
                Server.Settings.AutoConnectRequest = AutoConnectRequestField.Text;
                UpdateValues();
            };

            AutoConnectResponseField.Leave += (object sender, EventArgs e) =>
            {
                Server.Settings.AutoConnectResponse = AutoConnectResponseField.Text;
                UpdateValues();
            };

            EventHandler autoConnectResponseTimeoutHandler = (object sender, EventArgs e) =>
            {
                Server.Settings.AutoConnectResponseTimeout = (int)AutoConnectResponseTimeoutField.Value;
                UpdateValues();
            };

            AutoConnectResponseTimeoutField.Leave += autoConnectResponseTimeoutHandler;
            AutoConnectResponseTimeoutField.AddButton.Click += autoConnectResponseTimeoutHandler;
            AutoConnectResponseTimeoutField.SubtractButton.Click += autoConnectResponseTimeoutHandler;

            EventHandler connectionIntervalHandler = (object sender, EventArgs e) =>
            {
                Server.Settings.ConnectionInterval = (int)ConnectionIntervalField.Value;
                UpdateValues();
            };

            ConnectionIntervalField.Leave += connectionIntervalHandler;
            ConnectionIntervalField.AddButton.Click += connectionIntervalHandler;
            ConnectionIntervalField.SubtractButton.Click += connectionIntervalHandler;

            SendModeField.ItemSelected += (object sender, ItemEventArgs e) =>
            {
                Server.Settings.SendMode = SendModeField.ToEnum(Server.Settings.SendMode);
                UpdateValues();
            };

            EventHandler connectionCheckHandler = (object sender, EventArgs e) =>
            {
                Server.Settings.ConnectionCheckInterval = (int)ConnectionCheckField.Value;
                UpdateValues();
            };

            ConnectionCheckField.Leave += connectionCheckHandler;
            ConnectionCheckField.AddButton.Click += connectionCheckHandler;
            ConnectionCheckField.SubtractButton.Click += connectionCheckHandler;

            EventHandler messageIntervalHandler = (object sender, EventArgs e) =>
            {
                Server.Settings.MessageInterval = (int)MessageIntervalField.Value;
                UpdateValues();
            };

            MessageIntervalField.Leave += messageIntervalHandler;
            MessageIntervalField.AddButton.Click += messageIntervalHandler;
            MessageIntervalField.SubtractButton.Click += messageIntervalHandler;

            DataRequestField.Leave += (object sender, EventArgs e) =>
            {
                Server.Settings.DataRequest = DataRequestField.Text;
                UpdateValues();
            };
        }

        public virtual void UpdateValues()
        {
            ParametersHeader.Text = Server.Settings.IsDirty
                ? "*" + Server.Settings.ProjectName
                : Server.Settings.ProjectName;

            PortSpeedField.SelectedItem = Server.Settings.PortSpeed;
            AutoConnectToggle.Checked = Server.Settings.AutoConnect;
            AutoConnectRequestField.Text = Server.Settings.AutoConnectRequest;
            AutoConnectResponseField.Text = Server.Settings.AutoConnectResponse;
            AutoConnectResponseTimeoutField.Value = Server.Settings.AutoConnectResponseTimeout;
            ConnectionIntervalField.Value = Server.Settings.ConnectionInterval;
            SendModeField.SelectedItem = Server.Settings.SendMode;
            MessageIntervalField.Value = Server.Settings.MessageInterval;
            ConnectionCheckField.Value = Server.Settings.ConnectionCheckInterval;
            DataRequestField.Text = Server.Settings.DataRequest;

            AutoConnectRequestProperty.Enabled = Server.Settings.AutoConnect;
            AutoConnectResponseProperty.Enabled = Server.Settings.AutoConnect;
            AutoConnectResponseTimeoutProperty.Enabled = Server.Settings.AutoConnect;
            ConnectionIntervalProperty.Enabled = !Server.Settings.AutoConnect;

            MessageIntervalProperty.Enabled = Server.Settings.SendMode == SendMode.Stream;
            ConnectionCheckProperty.Enabled = Server.Settings.SendMode == SendMode.Request;
            DataRequestProperty.Enabled = Server.Settings.SendMode == SendMode.Request;
        }

        protected virtual void InitializeGUI()
        {
            this.MainContainer = new VerticalListContainer
            {
                Dock = DockStyle.Fill,
                AutoScroll = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                Padding = new Padding(16),
                AutoScrollOffset = new Point(16, 16),
                BackgroundSource = Theme.BackgroundSource.Inherit
            };
            this.Controls.Add(MainContainer);

            this.ControlPanel = new PropertyContainer
            {
                Text = "Serial Port",
                Height = 40,
                AutoScroll = false,
                MinimumSize = new Size(380, 20),
                MaximumSize = new Size(1200, int.MaxValue),
                FitContent = false,
                ControlsAlignment = HorizontalAlignment.Right,
                Padding = new Padding(3),
                Margin = new Padding(0, 0, 0, 6),
                Font = MainForm.HeaderFont
            };
            MainContainer.Controls.Add(ControlPanel);

            this.StartServerButton = new FlatButton
            {
                Text = "Start Server",
                Icon = Properties.Resources.play,
                IconOffset = 0,
                Padding = new Padding(6, 0, 6, 0),
                Font = MainForm.MainFont
            };
            ControlPanel.Controls.Add(StartServerButton);

            this.ParametersPanel = new VerticalListContainer
            {
                AutoScroll = false,
                MinimumSize = new Size(380, 20),
                MaximumSize = new Size(1200, int.MaxValue),
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                Padding = new Padding(0),
                Margin = new Padding(0),
            };
            MainContainer.Controls.Add(ParametersPanel);

            this.ParametersHeader = new PropertyContainer
            {
                Text = "",
                Height = 40,
                AutoScroll = false,
                MinimumSize = new Size(380, 20),
                MaximumSize = new Size(1200, int.MaxValue),
                FitContent = false,
                ControlsAlignment = HorizontalAlignment.Right,
                LabelAlignment = ContentAlignment.MiddleLeft,
                Padding = new Padding(3),
                Margin = new Padding(0),
                Font = MainForm.HeaderFont
            };
            ParametersPanel.Controls.Add(ParametersHeader);

            this.LoadServerSettingsButton = new FlatButton
            {
                Text = "",
                Size = new Size(32, 32),
                Icon = Properties.Resources.OpenFile,
                IconSize = 32,
                IconOffset = 0,
                Padding = new Padding(4),
                IconMultiplyColor = Color.FromArgb(215, 172, 106),
                Font = MainForm.MainFont,
                UseButtonBorder = false,
                BackgroundSource = Theme.BackgroundSource.Inherit
            };
            ParametersHeader.Controls.Add(LoadServerSettingsButton);

            this.SaveServerSettingsButton = new FlatButton
            {
                Text = "",
                Size = new Size(32, 32),
                Icon = Properties.Resources.SaveFile,
                IconSize = 32,
                IconOffset = 0,
                Padding = new Padding(4),
                IconMultiplyColor = Color.FromArgb(114, 186, 236),
                Font = MainForm.MainFont,
                UseButtonBorder = false,
                BackgroundSource = Theme.BackgroundSource.Inherit
            };
            ParametersHeader.Controls.Add(SaveServerSettingsButton);

            this.ParametersContainer = new FlexColumnContainer
            {
                AutoScroll = false,
                Height = 600,
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                ColumnsLayout = FlexColumnsLayoutEngine.ColumnsLayout.HorizontalStaggeredGrid,
                Padding = new Padding(0, 6, 0, 12),
                Margin = new Padding(0),
                MinColumnCount = 1,
                MaxColumnCount = 4,
                MinColumnSize = 260,
                MaxColumnSize = 0,
            };
            ParametersPanel.Controls.Add(ParametersContainer);

            // Main Parameters Group
            this.MainParametersGroup = new VerticalListContainer
            {
                AutoScroll = false,
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                Padding = new Padding(0),
                Margin = new Padding(0),
            };
            ParametersContainer.Controls.Add(MainParametersGroup);

            // Edit Data Property
            this.EditDataProperty = new PropertyContainer
            {
                Text = "Serial Port Data",
                Margin = new Padding(8, 1, 8, 1)
            };
            MainParametersGroup.Controls.Add(EditDataProperty);

            this.EditDataButton = new FlatButton
            {
                Text = "Edit"
            };
            EditDataProperty.Controls.Add(EditDataButton);

            // Port Speed Property
            this.PortSpeedProperty = new PropertyContainer
            {
                Text = "Port Speed",
                Margin = new Padding(8, 1, 8, 1)
            };
            MainParametersGroup.Controls.Add(PortSpeedProperty);

            this.PortSpeedField = new Dropdown
            {

            };
            PortSpeedField.FromEnum(Server.Settings.PortSpeed);
            PortSpeedProperty.Controls.Add(PortSpeedField);


            // Connection Parameters Group
            this.ConnectionParametersGroup = new VerticalListContainer
            {
                AutoScroll = false,
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                Padding = new Padding(0),
                Margin = new Padding(0),
            };
            ParametersContainer.Controls.Add(ConnectionParametersGroup);

            // Auto Connect Property
            this.AutoConnectProperty = new PropertyContainer
            {
                Text = "Auto Connect",
                Margin = new Padding(8, 1, 8, 1)
            };
            ConnectionParametersGroup.Controls.Add(AutoConnectProperty);

            this.AutoConnectToggle = new Toggle
            {
                Text = "",
                Style = Toggle.CheckmarkStyle.Toggle,
            };
            AutoConnectProperty.Controls.Add(AutoConnectToggle);

            // Auto Connect Request Property
            this.AutoConnectRequestProperty = new PropertyContainer
            {
                Text = "Connection Request",
                Margin = new Padding(8, 1, 8, 1),
                Indent = 1
            };
            ConnectionParametersGroup.Controls.Add(AutoConnectRequestProperty);

            this.AutoConnectRequestField = new InputField
            {

            };
            AutoConnectRequestProperty.Controls.Add(AutoConnectRequestField);

            // Auto Connect Response Property
            this.AutoConnectResponseProperty = new PropertyContainer
            {
                Text = "Connection Response",
                Margin = new Padding(8, 1, 8, 1),
                Indent = 1
            };
            ConnectionParametersGroup.Controls.Add(AutoConnectResponseProperty);

            this.AutoConnectResponseField = new InputField
            {

            };
            AutoConnectResponseProperty.Controls.Add(AutoConnectResponseField);

            // Response Timeout Property
            this.AutoConnectResponseTimeoutProperty = new PropertyContainer
            {
                Text = "Response Timeout (ms)",
                Margin = new Padding(8, 1, 8, 1),
                Indent = 1
            };
            ConnectionParametersGroup.Controls.Add(AutoConnectResponseTimeoutProperty);

            this.AutoConnectResponseTimeoutField = new NumericInputField
            {
                Minimum = 100,
                Maximum = int.MaxValue,
                Increment = 50,
            };
            AutoConnectResponseTimeoutProperty.Controls.Add(AutoConnectResponseTimeoutField);

            // Connection Interval Property
            this.ConnectionIntervalProperty = new PropertyContainer
            {
                Text = "Connection Interval (ms)",
                Margin = new Padding(8, 1, 8, 1),
            };
            ConnectionParametersGroup.Controls.Add(ConnectionIntervalProperty);

            this.ConnectionIntervalField = new NumericInputField
            {
                Minimum = 100,
                Maximum = int.MaxValue,
                Increment = 50,
            };
            ConnectionIntervalProperty.Controls.Add(ConnectionIntervalField);

            // Send Mode Parameters Group
            this.SendModeParametersGroup = new VerticalListContainer
            {
                AutoScroll = false,
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                Padding = new Padding(0),
                Margin = new Padding(0),
            };
            ParametersContainer.Controls.Add(SendModeParametersGroup);

            // Send Mode Property
            this.SendModeProperty = new PropertyContainer
            {
                Text = "Send Mode",
                Margin = new Padding(8, 1, 8, 1)
            };
            SendModeParametersGroup.Controls.Add(SendModeProperty);

            this.SendModeField = new Dropdown
            {

            };
            SendModeField.FromEnum(Server.Settings.SendMode);
            SendModeProperty.Controls.Add(SendModeField);

            // Message Interval Property
            this.MessageIntervalProperty = new PropertyContainer
            {
                Text = "Message Interval (ms)",
                Margin = new Padding(8, 1, 8, 1),
                Indent = 1
            };
            SendModeParametersGroup.Controls.Add(MessageIntervalProperty);

            this.MessageIntervalField = new NumericInputField
            {
                Minimum = 100,
                Maximum = int.MaxValue,
                Increment = 50,
            };
            MessageIntervalProperty.Controls.Add(MessageIntervalField);

            // Message Interval Property
            this.ConnectionCheckProperty = new PropertyContainer
            {
                Text = "Connection Check (ms)",
                Margin = new Padding(8, 1, 8, 1),
                Indent = 1
            };
            SendModeParametersGroup.Controls.Add(ConnectionCheckProperty);

            this.ConnectionCheckField = new NumericInputField
            {
                Minimum = 100,
                Maximum = int.MaxValue,
                Increment = 50,
            };
            ConnectionCheckProperty.Controls.Add(ConnectionCheckField);

            // Data Request Property
            this.DataRequestProperty = new PropertyContainer
            {
                Text = "Data Request",
                Margin = new Padding(8, 1, 8, 1),
                Indent = 1
            };
            SendModeParametersGroup.Controls.Add(DataRequestProperty);

            this.DataRequestField = new InputField
            {

            };
            DataRequestProperty.Controls.Add(DataRequestField);
        }

        protected virtual MASMFormattingEditor CreateLogDataEditor()
        {
            MASMFormattingEditor editor = new MASMFormattingEditor
            {
                Text = "Serial Port Data Editor",
            };

            editor.Items.AddRange(this.Server.Settings.DataFormatter.FormattingItems);

            InputField globalPrefixField = new InputField { Text = this.Server.Settings.DataFormatter.GlobalPrefix };
            InputField globalPostfixField = new InputField { Text = this.Server.Settings.DataFormatter.GlobalPostfix };
            InputField decimalSeparatorField = new InputField { Text = this.Server.Settings.DataFormatter.DecimalSeparator };
            Dropdown encodingField = new Dropdown { };
            encodingField.FromEnum(this.Server.Settings.Encoding);
            InputField endOfLineCharField = new InputField { Text = this.Server.Settings.EndOfLineChar };


            PropertyContainer globalPrefixProperty = new PropertyContainer { Text = "Global Prefix" };
            PropertyContainer globalPostfixProperty = new PropertyContainer { Text = "Global Postfix" };
            PropertyContainer decimalSeparatorProperty = new PropertyContainer { Text = "Decimal Separator" };
            PropertyContainer encodingProperty = new PropertyContainer { Text = "Encoding" };
            PropertyContainer endOfLineCharProperty = new PropertyContainer { Text = "End Of Line Character" };


            globalPrefixProperty.Controls.Add(globalPrefixField);
            globalPostfixProperty.Controls.Add(globalPostfixField);
            decimalSeparatorProperty.Controls.Add(decimalSeparatorField);
            encodingProperty.Controls.Add(encodingField);
            endOfLineCharProperty.Controls.Add(endOfLineCharField);

            editor.AdditionalProperties.Add(globalPrefixProperty);
            editor.AdditionalProperties.Add(globalPostfixProperty);
            editor.AdditionalProperties.Add(decimalSeparatorProperty);
            editor.AdditionalProperties.Add(encodingProperty);
            editor.AdditionalProperties.Add(endOfLineCharProperty);

            editor.AvailableProperties = () =>
            {
                return new List<string>(new MASM().UpdateOnce().GetPropertiesList());
            };

            editor.Apply += (object sender, EventArgs e) =>
            {
                if (editor.Items.Count != this.Server.Settings.DataFormatter.FormattingItems.Count)
                {
                    this.Server.Settings.DataFormatter.FormattingItems.Clear();
                    this.Server.Settings.DataFormatter.FormattingItems.AddRange(editor.Items);
                }
                else
                {
                    for (int i = 0; i < editor.Items.Count; i++)
                    {
                        this.Server.Settings.DataFormatter.FormattingItems[i] = editor.Items[i];
                    }
                }

                this.Server.Settings.DataFormatter.GlobalPrefix = globalPrefixField.Text;
                this.Server.Settings.DataFormatter.GlobalPostfix = globalPostfixField.Text;
                this.Server.Settings.DataFormatter.DecimalSeparator = decimalSeparatorField.Text;
                this.Server.Settings.Encoding = encodingField.ToEnum(SerialPortEncoding.UTF8);
                this.Server.Settings.EndOfLineChar = endOfLineCharField.Text;

                editor.Dispose();
            };

            editor.Cancel += (object sender, EventArgs e) =>
            {
                editor.Dispose();
            };

            return editor;
        }
    }
}
