using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AfterburnerDataHandler.SharedMemory.Afterburner;
using AfterburnerDataHandler.Servers.RTSS;
using AfterburnerDataHandler.Projects;

namespace AfterburnerDataHandler.Servers.Logger
{
    public class LoggerServer : BaseServer
    {
        public LogWriter LogServer
        {
            get
            {
                if (logServer == null)
                    logServer = new LogWriter();

                return logServer;
            }
        }

        public MASM MASMData
        {
            get
            {
                if (masmData == null)
                    masmData = new MASM();

                return masmData;
            }
        }

        public MASM FrametimeData
        {
            get
            {
                if (frametimeData == null)
                    frametimeData = new MASM();

                return frametimeData;
            }
        }

        public RTSS_FrametimeServer FrametimeServer
        {
            get
            {
                if (frametimeServer == null)
                    frametimeServer = new RTSS_FrametimeServer();

                return frametimeServer;
            }
        }

        public string LogDirectory
        {
            get { return logDirectory; }
            set
            {
                logDirectory = value;
            }
        }

        public string LogName
        {
            get { return logName; }
            set
            {
                logName = value;
            }
        }

        public LoggerProject Settings
        {
            get
            {
                if (settings == null)
                {
                    settings = new LoggerProject();
                    OnSettingsChanged(EventArgs.Empty);
                }

                return settings;
            }
            set
            {
                this.Stop();
                settings = value;
                OnSettingsChanged(EventArgs.Empty);
            }
        }

        public event EventHandler<ServerStateEventArgs> LogStateChanged;
        public event EventHandler<EventArgs> SettingsChanged;

        public ServerState LogState
        {
            get { return logState; }
            protected set
            {
                logState = value;
                OnLogStateChanged(new ServerStateEventArgs(logState));
            }
        }


        private string logDirectory = string.Empty;
        private string logName = string.Empty;

        private Timer masmTimer;
        private MASM masmData;
        private MASM frametimeData;
        private RTSS_FrametimeServer frametimeServer;
        private LogWriter logServer;
        private LoggerProject settings;
        private ServerState logState = ServerState.Stop;

        private long currentFrame;
        private long currentFrametime;

        public LoggerServer()
        {
            FrametimeServer.StateChanged += FrametimeServerStateChanged;
            FrametimeServer.FrametimeDataReceived += FrametimeServerDataReceived;
        }

        public override bool Begin()
        {
            Stop();

            ServerState = ServerState.Begin;

            masmTimer = new Timer(
                MASMTimerTicked,
                true,
                0,
                Settings.DataUpdateInterval);

            if (Settings.UseFrametimeMode == true)
            {
                if (FrametimeServer.Begin() != true)
                {
                    this.Stop();
                    return false;
                }
            }

            return true;
        }

        public override void Stop()
        {
            if (ServerState != ServerState.Stop)
            {
                StopLog();

                if (FrametimeServer.ServerState != ServerState.Stop)
                    FrametimeServer?.Stop();

                masmTimer?.Dispose();
                masmTimer = null;

                base.Stop();
            }
        }

        public virtual bool BeginLog()
        {
            if (Settings.UseFrametimeMode == true
                && ServerState != ServerState.Connected
                || ServerState == ServerState.Stop)
            {
                return false;
            }

            if (LogState != ServerState.Stop)
                StopLog();

            string targetLogName = null;

            try
            {
                targetLogName = Settings.UseFrametimeMode == true
                    ? Path.GetFileNameWithoutExtension(LogServer.LogDirectoryPath)
                    : LogName;
            }
            catch { }

            if (LogServer.Open(LogDirectory, targetLogName, Settings.LogFileFormat))
            {
                LogServer.Append(Settings.StartText);
                currentFrame = 0;
                currentFrametime = 0;
                LogState = ServerState.Begin;
            }

            return LogState != ServerState.Stop;
        }

        public virtual void StopLog()
        {
            if (LogState != ServerState.Stop)
            {
                LogState = ServerState.Stop;
                LogServer.Append(Settings.FinalText);
                LogServer.Close();
            }
        }

        protected virtual void FrametimeServerStateChanged(object sender, ServerStateEventArgs e)
        {
            switch (e.state)
            {
                case ServerState.Begin:
                    break;
                case ServerState.Waiting:
                    ServerState = ServerState.Waiting;
                    break;
                case ServerState.Connected:
                    ServerState = ServerState.Connected;
                    break;
                case ServerState.Reconnect:
                    if (LogState != ServerState.Stop) this.StopLog();
                    ServerState = ServerState.Reconnect;
                    break;
                case ServerState.Stop:
                    break;
            }
        }

        protected virtual void MASMTimerTicked(object state)
        {
            MASMData.Update();

            if (ServerState != ServerState.Stop &&
                LogState != ServerState.Stop &&
                Settings.UseFrametimeMode == false)
            {
                LogServer.Append(Settings.DataFormatter.Format(MASMData));
            }
        }

        protected virtual void FrametimeServerDataReceived(object sender, FrametimeDataEventArgs e)
        {
            if (ServerState != ServerState.Stop &&
                LogState != ServerState.Stop &&
                Settings.UseFrametimeMode == true)
            {
                FrametimeData.Properties.Clear();
                FrametimeData.Properties.AddRange(MASMData.Properties);

                FrametimeData.Properties.Add(new MAHM_SHARED_MEMORY_ENTRY
                {
                    szSrcName = "Current frame",
                    data = currentFrame
                });

                FrametimeData.Properties.Add(new MAHM_SHARED_MEMORY_ENTRY
                {
                    szSrcName = "Frame time",
                    data = currentFrametime
                });

                FrametimeData.Properties.Add(new MAHM_SHARED_MEMORY_ENTRY
                {
                    szSrcName = "Frame duration",
                    data = e.frametime
                });

                currentFrame++;
                currentFrametime += e.frametime;

                LogServer.Append(Settings.DataFormatter.Format(FrametimeData));
            }
        }

        protected virtual void OnLogStateChanged(ServerStateEventArgs e)
        {
            LogStateChanged?.Invoke(this, e);
        }

        protected virtual void OnSettingsChanged(EventArgs e)
        {
            SettingsChanged?.Invoke(this, e);
        }
    }
}