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

        public Controls.StatusBar StatusBar { get; protected set; }
        public TabView MainMenu { get; protected set; }
        public MainTab MainTab { get; protected set; }
        public SerialTab SerialTab { get; protected set; }
        public LoggerTab LoggerTab { get; protected set; }
        public StatisticTab StatisticTab { get; protected set; }
        public RemoteTab RemoteTab { get; protected set; }
        public SettingsTab SettingsTab { get; protected set; }

        public static Font MainFont = new Font("Segoe UI Semibold", 9, FontStyle.Regular, GraphicsUnit.Point);
        public static Font SidebarFont = new Font("Segoe UI Semibold", 11, FontStyle.Regular, GraphicsUnit.Point);
        public static Font HeaderFont = new Font("Segoe UI Semibold", 16, FontStyle.Regular, GraphicsUnit.Point);

        public MainForm()
        {
            MainForm.Current = this;

            InitializeComponent();

            this.Font = MainFont;
            this.Text = "Afterburner Data Handler";

            this.StatusBar = new Controls.StatusBar
            {
                Dock = DockStyle.Bottom,
                Text = "[12:54:31][Logger] Connected to Starfall"
            };

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
            this.Controls.Add(this.StatusBar);
            this.MainMenu.Controls.Add(SerialTab);
            this.MainMenu.Controls.Add(MainTab);
            this.MainMenu.Controls.Add(StatisticTab);
            this.MainMenu.Controls.Add(RemoteTab);
            this.MainMenu.Controls.Add(LoggerTab);
            this.MainMenu.Controls.Add(SettingsTab);

            ProjectsManager.SerialPortServer = SerialTab.Server;
            ProjectsManager.LoggerServer = LoggerTab.Server;

            ProjectsManager.LoadLastSerialPortProject();
            ProjectsManager.LoadLastLoggerProject();
        }

        public static void ShowControl(Control control)
        {
            control.Dock = DockStyle.Fill;

            MainForm.Current?.Controls?.Add(control);
            control.BringToFront();
        }

        protected override void OnResizeBegin(EventArgs e)
        {
            base.OnResizeBegin(e);
            this.SuspendLayout();
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
            }

            base.OnClosing(e);
        }
    }
}
