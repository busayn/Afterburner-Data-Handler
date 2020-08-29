using AfterburnerDataHandler.SharedMemory.Afterburner;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace AfterburnerDataHandler.Servers.SerialPort
{
    public class SerialPortServer : BaseServer
    {
        public string TargetPort
        {
            get { return targetPort; }
            set
            {
                this.Stop();
                targetPort = value;
            }
        }

        public SerialPortSettings Settings
        {
            get
            {
                if (settings == null)
                    settings = new SerialPortSettings();

                return settings;
            }
            set
            {
                this.Stop();
                settings = value;
            }
        }

        public SerialPortHandler Serial
        {
            get
            {
                if (serial == null)
                    serial = new SerialPortHandler();

                return serial;
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

        private SerialPortSettings settings;
        private SerialPortHandler serial;
        private MASM masmData;
        private Timer serialPortTimer;
        private string targetPort = "";
        private string currentPort = "";

        public SerialPortServer()
        {
            Serial.TextReceived += TextReceived;
        }

        public override bool Begin()
        {
            Stop();

            ServerState = ServerState.Begin;
            MASMData.Start();
            Serial.EndOfLineChar = Settings.EndOfLineChar;
            Serial.Encoding = Settings.Encoding.ToTextEncoding();

            int connectionInterval = settings.AutoConnect == true
                ? settings.AutoConnectResponseTimeout
                : settings.ConnectionInterval;

            serialPortTimer = new Timer(
                SerialPortLoop,
                true,
                0,
                connectionInterval);

            return true;
        }

        public override void Stop()
        {
            if (ServerState != ServerState.Stop)
            {
                serialPortTimer?.Dispose();
                serialPortTimer = null;
                Serial.Close();
                MASMData.Stop();

                base.Stop();
            }
        }

        protected virtual void SerialPortLoop(object state)
        {
            if (this.ServerState == ServerState.Waiting)
            {
                if (Settings.AutoConnect == true)
                {
                    List<string> availablePorts = Serial.GetAvailablePorts();

                    if (availablePorts.Count > 0)
                    {
                        if (availablePorts.Contains(currentPort))
                        {
                            int nextPortIndex = (availablePorts.IndexOf(currentPort) + 1) % availablePorts.Count;
                            currentPort = availablePorts[nextPortIndex];
                        }
                        else currentPort = availablePorts[0];

                        if (Serial.Open(currentPort, (int)Settings.PortSpeed))
                        {
                            Serial.SendText(Regex.Unescape(Settings.AutoConnectRequest));
                        }
                    }
                }
                else
                {
                    if (Serial.Open(targetPort, (int)Settings.PortSpeed))
                    {
                        this.ServerState = ServerState.Connected;
                    }
                }
            }

            if (this.ServerState == ServerState.Connected && Settings.SendMode == SendMode.Stream)
            {
                SendData();
            }
        }

        private void TextReceived(object sender, SerialPortHandler.TextReceivedEventArgs e)
        {
            if (Settings.AutoConnect == true && this.ServerState == ServerState.Waiting)
            {
                string targetResponse = Regex.Unescape(Settings.AutoConnectResponse ?? "");

                if (string.IsNullOrEmpty(e.text) == false && e.text.CompareTo(targetResponse) == 0)
                {
                    this.ServerState = ServerState.Connected;

                    switch (Settings.SendMode)
                    {
                        case SendMode.Stream:
                            serialPortTimer?.Change(0, Settings.MessageInterval);
                            break;
                        case SendMode.Request:
                            serialPortTimer?.Change(0, Settings.ConnectionCheckInterval);
                            break;
                        default:
                            serialPortTimer?.Change(0, 1000);
                            break;
                    }
                }
            }

            if (this.ServerState == ServerState.Connected && Settings.SendMode == SendMode.Request)
            {
                string targetRequest = Regex.Unescape(Settings.DataRequest ?? "");

                if (string.IsNullOrEmpty(e.text) == false && e.text.CompareTo(targetRequest) == 0)
                {
                    SendData();
                }
            }
        }

        protected bool SendData()
        {
            bool sendingResult = Serial.SendText(
                Settings.DataFormatter.Format(MASMData.Update()));

            if (sendingResult == false)
            {
                this.ServerState = ServerState.Reconnect;
                this.ServerState = ServerState.Waiting;

                int connectionInterval = settings.AutoConnect == true
                    ? settings.AutoConnectResponseTimeout
                    : settings.ConnectionInterval;

                serialPortTimer?.Change(connectionInterval, connectionInterval);
            }

            return sendingResult;
        }
    }
}
