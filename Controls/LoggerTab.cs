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
using AfterburnerDataHandler.HotkeysHandler;
using AfterburnerDataHandler.Projects;
using AfterburnerDataHandler.Servers.Logger;
using AfterburnerDataHandler.SharedMemory.Afterburner;
using AfterburnerDataHandler.Filesystem;

namespace AfterburnerDataHandler.Controls
{
    public class LoggerTab : TabPanel
    {
        public VerticalListContainer MainContainer { get; protected set; }

        // Server Control Panel
        public PropertyContainer ControlPanel { get; protected set; }
        public FlatButton StartServerButton { get; protected set; }
        public FlatButton StartLogButton { get; protected set; }

        // Server Parameters Panel
        public VerticalListContainer ParametersPanel { get; protected set; }
        public PropertyContainer ParametersHeader { get; protected set; }
        public FlexColumnContainer ParametersContainer { get; protected set; }
        public FlatButton SaveServerSettingsButton { get; protected set; }
        public FlatButton LoadServerSettingsButton { get; protected set; }

        // Log Name Property
        public InputField LogNameField { get; protected set; }
        public PropertyContainer LogNameProperty { get; protected set; }

        // Edit Data Property
        public FlatButton EditDataButton { get; protected set; }
        public PropertyContainer EditDataProperty { get; protected set; }

        // Log File Format
        public InputField LogFileFormatField { get; protected set; }
        public PropertyContainer LogFileFormatProperty { get; protected set; }

        // Update Interval Property
        public NumericInputField UpdateIntervalField { get; protected set; }
        public PropertyContainer UpdateIntervalProperty { get; protected set; }

        // Frametime Mode Property
        public Toggle FrametimeModeToggle { get; protected set; }
        public PropertyContainer FrametimeModeProperty { get; protected set; }

        public LoggerServer Server
        {
            get
            {
                if (server == null)
                    server = new LoggerServer();

                return server;
            }
        }

        public Hotkey StartLogHotkey
        {
            get
            {
                if (startLogHotkey == null)
                    startLogHotkey = new Hotkey(Properties.Settings.Default.Logger_LogHotkey);

                return startLogHotkey;
            }
        }

        private LoggerServer server;
        private Hotkey startLogHotkey;

        public LoggerTab()
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
            Server.LogStateChanged += LogStateChanged;

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
            string messageLabel = "Logger";
            string message = string.Empty;

            switch (e.state)
            {
                case Servers.ServerState.Begin:
                    message = "Server started";

                    MainForm.AppendMessage(message, messageLabel);

                    if (Properties.Settings.Default.Logger_ServerNotifications == true)
                        MainForm.ShowNotification(message, messageLabel);

                    ControlUtils.AsyncSafeInvoke(this, () =>
                    {
                        this.StartServerButton.Text = "Stop Server";
                        this.StartServerButton.Icon = Properties.Resources.stop;
                        this.ParametersPanel.Enabled = false;
                    });
                    break;

                case Servers.ServerState.Waiting:
                    MainForm.AppendMessage("Waiting app...", messageLabel);
                    break;

                case Servers.ServerState.Connected:
                    message = string.Empty;

                    if (Server.Settings.UseFrametimeMode == true)
                    {
                        message = "Connected to ";

                        try
                        {
                            message += Path.GetFileNameWithoutExtension(Server.FrametimeServer.ConnectedApp);
                        }
                        catch
                        {
                            message += Server.FrametimeServer.ConnectedApp ?? string.Empty;
                        }
                    }
                    else
                    {
                        message = "Connected";
                    }

                    MainForm.AppendMessage(message, messageLabel);

                    if (Properties.Settings.Default.Logger_ServerNotifications == true)
                        MainForm.ShowNotification(message, messageLabel);
                    break;

                case Servers.ServerState.Reconnect:
                    message = "Reconnecting";

                    MainForm.AppendMessage(message, messageLabel);

                    if (Properties.Settings.Default.Logger_ServerNotifications == true)
                        MainForm.ShowNotification(message, messageLabel);
                    break;

                case Servers.ServerState.Stop:
                    message = "Server stopped";

                    MainForm.AppendMessage(message, messageLabel);

                    if (Properties.Settings.Default.Logger_ServerNotifications == true)
                        MainForm.ShowNotification(message, messageLabel);

                    ControlUtils.AsyncSafeInvoke(this, () =>
                    {
                        this.StartServerButton.Text = "Start Server";
                        this.StartServerButton.Icon = Properties.Resources.play;
                        this.ParametersPanel.Enabled = true;
                    });
                    break;
            }

            ControlUtils.AsyncSafeInvoke(this, () =>
            {
                if (Server.Settings.UseFrametimeMode == true
                    && Server.ServerState != Servers.ServerState.Connected
                    || Server.ServerState == Servers.ServerState.Stop)
                {
                    this.StartLogButton.Enabled = false;
                    StartLogHotkey.Disable();
                }
                else
                {
                    Server.LogDirectory = Properties.Settings.Default.Logger_LogPath;
                    Server.LogName = Properties.Settings.Default.Logger_LogName;
                    this.StartLogButton.Enabled = true;
                    StartLogHotkey.Enable();
                }
            });
        }

        private void LogStateChanged(object sender, Servers.ServerStateEventArgs e)
        {
            string messageLabel = "Logger";
            string message = string.Empty;

            switch (e.state)
            {
                case Servers.ServerState.Begin:
                    message = "Log Started";

                    MainForm.AppendMessage("Log Started", messageLabel);

                    if (Properties.Settings.Default.Logger_LoggingNotifications == true)
                        MainForm.ShowNotification(message, messageLabel);

                    ControlUtils.AsyncSafeInvoke(this, () =>
                    {
                        this.StartLogButton.Text = "Stop Log";
                        this.StartLogButton.Icon = Properties.Resources.stop;
                    });
                    break;

                case Servers.ServerState.Stop:
                    message = "Log Stopped";

                    if (string.IsNullOrWhiteSpace(Server.LogServer.LogFilePath) == false)
                        message += string.Format(". Saved to \"{0}\"", Server.LogServer.LogFilePath);

                    MainForm.AppendMessage(message, messageLabel);

                    if (Properties.Settings.Default.Logger_LoggingNotifications == true)
                        MainForm.ShowNotification(message, messageLabel);

                    ControlUtils.AsyncSafeInvoke(this, () =>
                    {
                        this.StartLogButton.Text = "Start Log";
                        this.StartLogButton.Icon = Properties.Resources.play;
                    });
                    break;
            }
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

            StartLogButton.Click += (object sender, EventArgs e) =>
            {
                if (Server.LogState == Servers.ServerState.Stop)
                    Server?.BeginLog();
                else
                    Server?.StopLog();
            };

            SaveServerSettingsButton.Click += (object sender, EventArgs e) =>
            {
                string projectPath = ProjectsUtils.ShowSaveProjectDialog(Server.Settings);

                if (projectPath != null)
                {
                    Properties.Settings.Default.Logger_LastProject = projectPath;
                    Properties.Settings.Default.Save();
                }
            };

            LoadServerSettingsButton.Click += (object sender, EventArgs e) =>
            {
                LoggerProject newProject = Server.Settings;
                string projectPath = ProjectsUtils.ShowOpenProjectDialog(ref newProject);

                if (projectPath != null)
                {
                    Properties.Settings.Default.Logger_LastProject = projectPath;
                    Properties.Settings.Default.Save();
                }

                if (Server.Settings != newProject)
                    Server.Settings = newProject;
            };


            LogNameField.Leave += (object sender, EventArgs e) =>
            {
                string newFileName = FileUtils.CreateCorrectFileName(LogNameField.Text);

                newFileName = string.IsNullOrWhiteSpace(newFileName) ? "NamelessLog" : newFileName;
                Properties.Settings.Default.Logger_LogName = newFileName;
                Properties.Settings.Default.Save();

                UpdateValues();
            };

            EditDataButton.Click += (object sender, EventArgs e) =>
            {
                MainForm.ShowControl(CreateLogDataEditor());
            };

            LogFileFormatField.Leave += (object sender, EventArgs e) =>
            {
                string newFileFormat = FileUtils.CreateCorrectFileName(LogFileFormatField.Text);

                newFileFormat = string.IsNullOrWhiteSpace(newFileFormat) ? "txt" : newFileFormat;
                Server.Settings.LogFileFormat = newFileFormat;

                UpdateValues();
            };

            FrametimeModeToggle.Click += (object sender, EventArgs e) =>
            {
                Server.Settings.UseFrametimeMode = FrametimeModeToggle.Checked;
                UpdateValues();
            };

            EventHandler updateIntervalFieldHandler = (object sender, EventArgs e) =>
            {
                Server.Settings.DataUpdateInterval = (int)UpdateIntervalField.Value;
                UpdateValues();
            };

            UpdateIntervalField.Leave += updateIntervalFieldHandler;
            UpdateIntervalField.AddButton.Click += updateIntervalFieldHandler;
            UpdateIntervalField.SubtractButton.Click += updateIntervalFieldHandler;

            Properties.Settings.Default.PropertyChanged += (object sender, PropertyChangedEventArgs e) =>
            {
                switch (e.PropertyName)
                {
                    case "Logger_LogHotkey":
                        StartLogHotkey.Key = Properties.Settings.Default.Logger_LogHotkey & ~Keys.Modifiers;
                        StartLogHotkey.Modifiers = Properties.Settings.Default.Logger_LogHotkey & Keys.Modifiers;
                        break;
                }
            };

            StartLogHotkey.HotkeyPressed += (object sender, EventArgs e) =>
            {
                if (Server.Settings.UseFrametimeMode == true && Server.ServerState == Servers.ServerState.Connected ||
                    Server.Settings.UseFrametimeMode == false && Server.ServerState == Servers.ServerState.Begin)
                {
                    if (Server.LogState == Servers.ServerState.Begin)
                    {
                        Server.StopLog();
                    }
                    else
                    {
                        Server.LogDirectory = Properties.Settings.Default.Logger_LogPath;
                        Server.LogName = Properties.Settings.Default.Logger_LogName;
                        Server.BeginLog();
                    }
                }
            };
        }

        public virtual void UpdateValues()
        {
            ParametersHeader.Text = Server.Settings.IsDirty
                ? "*" + Server.Settings.ProjectName
                : Server.Settings.ProjectName;

            LogNameField.Text = Properties.Settings.Default.Logger_LogName;
            LogFileFormatField.Text = Server.Settings.LogFileFormat;
            UpdateIntervalField.Value = Server.Settings.DataUpdateInterval;
            FrametimeModeToggle.Checked = Server.Settings.UseFrametimeMode;
        }

        protected virtual void InitializeGUI()
        {
            this.MainContainer = new VerticalListContainer
            {
                Dock = DockStyle.Fill,
                AutoScroll = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                Padding = new Padding(12),
                AutoScrollOffset = new Point(12, 12),
                BackgroundSource = Theme.BackgroundSource.Inherit
            };
            this.Controls.Add(MainContainer);

            this.ControlPanel = new PropertyContainer
            {
                Text = "Logger",
                Height = 40,
                AutoScroll = false,
                MinimumSize = new Size(380, 20),
                MaximumSize = new Size(1200, int.MaxValue),
                FitContent = false,
                ControlsAlignment = HorizontalAlignment.Right,
                Padding = new Padding(3),
                Margin = new Padding(0, 0, 0, 12),
                Font = MainForm.HeaderFont
            };
            MainContainer.Controls.Add(ControlPanel);

            this.LogNameField = new InputField
            {
                Font = MainForm.MainFont,
            };
            ControlPanel.Controls.Add(LogNameField);

            this.StartServerButton = new FlatButton
            {
                Text = "Start Server",
                Icon = Properties.Resources.play,
                IconOffset = 0,
                Padding = new Padding(6, 0, 6, 0),
                Font = MainForm.MainFont
            };
            ControlPanel.Controls.Add(StartServerButton);

            this.StartLogButton = new FlatButton
            {
                Text = "Start Log",
                Icon = Properties.Resources.play,
                IconOffset = 0,
                Padding = new Padding(6, 0, 6, 0),
                Font = MainForm.MainFont,
                Enabled = false
            };
            ControlPanel.Controls.Add(StartLogButton);

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
                Text = "NewProject",
                Height = 40,
                AutoScroll = false,
                MinimumSize = new Size(380, 20),
                MaximumSize = new Size(1200, int.MaxValue),
                FitContent = false,
                ControlsAlignment = HorizontalAlignment.Right,
                LabelAlignment = ContentAlignment.MiddleLeft,
                Padding = new Padding(3),
                Margin = new Padding(0),
                Font = MainForm.HeaderFont,
                LabelWidth = 75,
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
                Padding = new Padding(0, 6, 0, 12),
                Margin = new Padding(0),
                MinColumnCount = 1,
                MaxColumnCount = 4,
                MinColumnSize = 260,
                MaxColumnSize = 0,
            };
            ParametersPanel.Controls.Add(ParametersContainer);

            // Edit Data Property
            this.EditDataProperty = new PropertyContainer
            {
                Text = "Log Data",
                Margin = new Padding(8, 1, 8, 1)
            };
            ParametersContainer.Controls.Add(EditDataProperty);

            this.EditDataButton = new FlatButton
            {
                Text = "Edit"
            };
            EditDataProperty.Controls.Add(EditDataButton);

            // Log File Format Property
            this.LogFileFormatProperty = new PropertyContainer
            {
                Text = "Log File Format",
                Margin = new Padding(8, 1, 8, 1)
            };
            ParametersContainer.Controls.Add(LogFileFormatProperty);

            this.LogFileFormatField = new InputField
            {

            };
            LogFileFormatProperty.Controls.Add(LogFileFormatField);

            // Update Interval Property
            this.UpdateIntervalProperty = new PropertyContainer
            {
                Text = "Update Interval (ms)",
                Margin = new Padding(8, 1, 8, 1)
            };
            ParametersContainer.Controls.Add(UpdateIntervalProperty);

            this.UpdateIntervalField = new NumericInputField
            {
                Minimum = 100,
                Maximum = int.MaxValue,
                Increment = 50,
            };
            UpdateIntervalProperty.Controls.Add(UpdateIntervalField);

            // Frametime Mode Property
            this.FrametimeModeProperty = new PropertyContainer
            {
                Text = "Frametime Mode",
                Margin = new Padding(8, 1, 8, 1)
            };
            ParametersContainer.Controls.Add(FrametimeModeProperty);

            this.FrametimeModeToggle = new Toggle
            {
                Text = "",
                Style = Toggle.CheckmarkStyle.Toggle,
            };
            FrametimeModeProperty.Controls.Add(FrametimeModeToggle);
        }

        protected virtual MASMFormattingEditor CreateLogDataEditor()
        {
            MASMFormattingEditor editor = new MASMFormattingEditor
            {
                Text = "Log Data Editor",
            };

            editor.AvailableProperties = () =>
            {
                List<string> properties = new List<string>(new MASM().UpdateOnce().GetPropertiesList());

                if (Server.Settings.UseFrametimeMode == true)
                {
                    properties.Insert(0, "RTSS Frame duration");
                    properties.Insert(0, "RTSS Frame time");
                    properties.Insert(0, "RTSS Current frame");
                }

                return properties;
            };

            editor.Items.AddRange(this.Server.Settings.DataFormatter.FormattingItems);

            InputField startTextField = new InputField { Text = this.Server.Settings.StartText };
            InputField finalTextField = new InputField { Text = this.Server.Settings.FinalText };
            InputField globalPrefixField = new InputField { Text = this.Server.Settings.DataFormatter.GlobalPrefix};
            InputField globalPostfixField = new InputField { Text = this.Server.Settings.DataFormatter.GlobalPostfix};
            InputField decimalSeparatorField = new InputField { Text = this.Server.Settings.DataFormatter.DecimalSeparator };

            PropertyContainer startTextProperty = new PropertyContainer { Text = "Start Text" };
            PropertyContainer finalTextProperty = new PropertyContainer { Text = "Final Text" };
            PropertyContainer globalPrefixProperty = new PropertyContainer { Text = "Global Prefix" };
            PropertyContainer globalPostfixProperty = new PropertyContainer { Text = "Global Postfix" };
            PropertyContainer decimalSeparatorProperty = new PropertyContainer { Text = "Decimal Separator" };

            startTextProperty.Controls.Add(startTextField);
            finalTextProperty.Controls.Add(finalTextField);
            globalPrefixProperty.Controls.Add(globalPrefixField);
            globalPostfixProperty.Controls.Add(globalPostfixField);
            decimalSeparatorProperty.Controls.Add(decimalSeparatorField);

            editor.AdditionalProperties.Add(startTextProperty);
            editor.AdditionalProperties.Add(finalTextProperty);
            editor.AdditionalProperties.Add(globalPrefixProperty);
            editor.AdditionalProperties.Add(globalPostfixProperty);
            editor.AdditionalProperties.Add(decimalSeparatorProperty);

            editor.Apply += (object sender, EventArgs e) =>
            {
                if(editor.Items.Count != this.Server.Settings.DataFormatter.FormattingItems.Count)
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

                this.Server.Settings.StartText = startTextField.Text;
                this.Server.Settings.FinalText = finalTextField.Text;
                this.Server.Settings.DataFormatter.GlobalPrefix = globalPrefixField.Text;
                this.Server.Settings.DataFormatter.GlobalPostfix = globalPostfixField.Text;
                this.Server.Settings.DataFormatter.DecimalSeparator = decimalSeparatorField.Text;

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