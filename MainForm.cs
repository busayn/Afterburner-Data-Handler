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
using AfterburnerDataHandler.Controls;
using AfterburnerDataHandler.Projects;

namespace AfterburnerDataHandler
{
    public partial class MainForm : FlatForm
    {
        public static MainForm Current { get; set; }

        public TabView MainMenu { get; protected set; }
        public MainTab MainTab { get; protected set; }
        public SerialTab SerialTab { get; protected set; }
        public LoggerTab LoggerTab { get; protected set; }
        public StatisticTab StatisticTab { get; protected set; }
        public RemoteTab RemoteTab { get; protected set; }
        public SettingsTab SettingsTab { get; protected set; }
        public Controls.StatusBar StatusBar { get; protected set; }
        public NotifyIcon TrayNotifyIcon { get; protected set; }

        public static Font MainFont = new Font("Segoe UI Semibold", 9, FontStyle.Regular, GraphicsUnit.Point);
        public static Font SidebarFont = new Font("Segoe UI Semibold", 11, FontStyle.Regular, GraphicsUnit.Point);
        public static Font HeaderFont = new Font("Segoe UI Semibold", 16, FontStyle.Regular, GraphicsUnit.Point);

        public MainForm()
        {
            MainForm.Current = this;

            InitializeComponent();
            InitializeGUI();
            InitializeServers();
        }

        protected virtual void InitializeGUI()
        {
            this.Font = MainFont;
            this.Text = "Afterburner Data Handler";

            this.TrayNotifyIcon = new NotifyIcon
            {
                Icon = Properties.Resources.MADSIcon,
                Visible = true,
                Text = this.Text,
            };
            TrayNotifyIcon.Click += TrayNotifyIconClick;

            this.StatusBar = new Controls.StatusBar
            {
                Dock = DockStyle.Bottom
            };
            this.Controls.Add(this.StatusBar);


            this.MainMenu = new TabView
            {
                Dock = DockStyle.Fill,
                TabsBarLayout = TabViewLayout.Vertical,
                TabsBarSeparatorSize = 0,
                TabsBarSize = 150,
                LabelIconSize = 22,
                LabelIconOffset = 6,
                UseSelectionMark = true,
                SelectionMarkSize = 4,
                LabelPadding = new Padding(8, 8, 4, 8),
                TabsBarPadding = new Padding(0, 8, 0, 0),
                TabsBarFont = SidebarFont,
            };

            this.MainTab = new MainTab
            {
                Text = "Main",
                Icon = Properties.Resources.MainScreenIcon,
            };

            this.SerialTab = new SerialTab
            {
                Text = "Serial Port",
                Icon = Properties.Resources.SerialIcon,
            };

            this.LoggerTab = new LoggerTab
            {
                Text = "Logger",
                Icon = Properties.Resources.LoggerIcon,
            };

            this.StatisticTab = new StatisticTab
            {
                Text = "Statistic",
                Icon = Properties.Resources.StatisticIcon,
            };

            this.RemoteTab = new RemoteTab
            {
                Text = "Remote",
                Icon = Properties.Resources.RemoteIcon,
            };

            this.SettingsTab = new SettingsTab
            {
                Text = "Settings",
                Icon = Properties.Resources.SettingsIcon,
            };

            this.Controls.Add(MainMenu);
            this.MainMenu.Controls.Add(LoggerTab);
            this.MainMenu.Controls.Add(SerialTab);
            this.MainMenu.Controls.Add(SettingsTab);
            //this.MainMenu.Controls.Add(MainTab);
            //this.MainMenu.Controls.Add(StatisticTab);
            //this.MainMenu.Controls.Add(RemoteTab);
        }

        protected virtual void InitializeServers()
        {
            ProjectsManager.LoggerServer = LoggerTab.Server;
            ProjectsManager.SerialPortServer = SerialTab.Server;

            if (Properties.Settings.Default.Logger_OpenLastProject)
            {
                if (ProjectsManager.LoadLastLoggerProject())
                {
                    AppendMessage(
                        string.Format("Loaded from \"{0}\"", Properties.Settings.Default.Logger_LastProject),
                        "Logger");
                }
                else
                {
                    Properties.Settings.Default.Logger_LastProject = string.Empty;
                    Properties.Settings.Default.Save();
                }
            }

            if (Properties.Settings.Default.SerialPort_OpenLastProject)
            {
                if (ProjectsManager.LoadLastSerialPortProject())
                {
                    AppendMessage(
                    string.Format("Loaded from \"{0}\"", Properties.Settings.Default.SerialPort_LastProject),
                    "SerialPort");
                }
                else
                {
                    Properties.Settings.Default.SerialPort_LastProject = string.Empty;
                    Properties.Settings.Default.Save();
                }
            }

            if (string.IsNullOrWhiteSpace(Properties.Settings.Default.Logger_LogPath) == true)
            {
                string documents = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                string loggerOutput = Path.Combine(Path.Combine(documents, "Afterburner Data Handler"), "Logger Output");

                Properties.Settings.Default.Logger_LogPath = loggerOutput;
                Properties.Settings.Default.Save();
            }

            if (Properties.Settings.Default.Logger_Autorun)
                ProjectsManager.LoggerServer.Begin();

            if (Properties.Settings.Default.SerialPort_Autorun)
                ProjectsManager.SerialPortServer.Begin();
        }

        public static void ShowControl(Control control)
        {
            control.Dock = DockStyle.Fill;

            MainForm.Current?.Controls?.Add(control);
            control.BringToFront();
        }

        public static bool AppendMessage(string message) { return AppendMessage(message, string.Empty, true); }

        public static bool AppendMessage(string message, string label) { return AppendMessage(message, label, true); }

        public static bool AppendMessage(string message, string label, bool showTime)
        {
            if (Current == null)
                return false;

            Current.StatusBar.AppendMessage(message, label, showTime);

            return true;
        }

        public static bool ShowNotification(string message)
        {
            return ShowNotification(message, System.Reflection.Assembly.GetExecutingAssembly().GetName().Name);
        }

        public static bool ShowNotification(string message, string title)
        {
            if (Current == null || Current.TrayNotifyIcon == null)
                return false;

            Current.TrayNotifyIcon.Visible = false;
            Current.TrayNotifyIcon.Visible = true;

            Current.TrayNotifyIcon.ShowBalloonTip(
                3,
                title,
                message,
                ToolTipIcon.None);

            return true;
        }

        protected override void OnResizeBegin(EventArgs e)
        {
            base.OnResizeBegin(e);
            this.SuspendLayout();
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);

            if (this.WindowState == FormWindowState.Minimized)
            {
                ShowNotification("Window minimized to tray.");
                this.ResumeLayout();
                this.Hide();
            }
        }

        protected override void OnResizeEnd(EventArgs e)
        {
            base.OnResizeEnd(e);
            this.ResumeLayout();
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            if (ProjectsManager.IsLoggerServerSaved == false ||
                ProjectsManager.IsSerialPortServerSaved == false)
            {
                if (MessageBox.Show(
                    "If you close the application, all unsaved data will be lost. Are you sure you want to leave the application?",
                    "Unsaved data found.",
                    MessageBoxButtons.OKCancel) == DialogResult.Cancel)
                {
                    e.Cancel = true;
                }
                else
                {
                    ProjectsManager.LoggerServer.Stop();
                    ProjectsManager.SerialPortServer.Stop();
                }
            }

            base.OnClosing(e);
        }

        private void TrayNotifyIconClick(object sender, EventArgs e)
        {
            if (this.IsHandleCreated == true)
            {
                this.Show();
                this.WindowState = FormWindowState.Normal;
                this.Activate();
            }
        }
    }
}
