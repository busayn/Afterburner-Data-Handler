using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AfterburnerDataHandler.Servers.SerialPort
{
    public class SerialPortHandler
    {

        public virtual System.IO.Ports.SerialPort Serial
        {
            get
            {
                if (serial == null)
                    serial = new System.IO.Ports.SerialPort()
                    {
                        ReadTimeout = System.IO.Ports.SerialPort.InfiniteTimeout,
                        WriteTimeout = System.IO.Ports.SerialPort.InfiniteTimeout,
                    };

                return serial;
            }
        }

        public virtual bool IsOpen { get { return Serial.IsOpen; } }
        public Encoding Encoding
        {
            get { return Serial.Encoding; }
            set { Serial.Encoding = value; }
        }

        public string EndOfLineChar
        {
            get { return Serial.NewLine; }
            set { Serial.NewLine = value; }
        }

        public class TextReceivedEventArgs : EventArgs { public string text; }
        public event EventHandler<TextReceivedEventArgs> TextReceived;

        private System.IO.Ports.SerialPort serial;
        private string receivedData = "";

        public SerialPortHandler()
        {
            Serial.DataReceived += SerialDataReceived;
        }

        public virtual bool Open(string portName, int speed)
        {
            if (IsOpen == true) Close();

            if (string.IsNullOrEmpty(portName) == true)
                return false;

            try
            {
                Serial.PortName = portName;
                Serial.BaudRate = speed;
                Serial.Open();
            }
            catch { }

            return IsOpen;
        }

        public virtual void Close()
        {
            Serial?.Close();
        }

        public virtual bool SendText(string text)
        {
            if (IsOpen == false) return false;

            try
            {
                if (string.IsNullOrEmpty(text) == false)
                {
                    Serial?.Write(text + (EndOfLineChar ?? "\n"));
                }
            }
            catch
            {
                return false;
            }

            return true;
        }

        public virtual List<string> GetAvailablePorts()
        {
            List<string> openPorts = new List<string>(System.IO.Ports.SerialPort.GetPortNames());

            return openPorts;
        }

        protected virtual void SerialDataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            try
            {
                if (e.EventType == SerialData.Chars && sender is System.IO.Ports.SerialPort)
                {
                    System.IO.Ports.SerialPort serialPort = sender as System.IO.Ports.SerialPort;
                    receivedData = (receivedData ?? "") + (serialPort.ReadExisting() ?? "");

                    if (receivedData.Contains(serialPort.NewLine))
                    {
                        string[] newLines = receivedData.Split(new string[] { serialPort.NewLine }, StringSplitOptions.None);

                        if (newLines.Length < 2)
                        {
                            receivedData = "";

                            if (newLines.Length > 0 && string.IsNullOrEmpty(newLines[0]) == false)
                                OnTextReceived(new TextReceivedEventArgs { text = newLines[0] });
                        }
                        else
                        {
                            receivedData = newLines[newLines.Length - 1];

                            for (int i = 0; i < newLines.Length - 1; i++)
                            {
                                if (string.IsNullOrEmpty(newLines[i]) == false)
                                {
                                    OnTextReceived(new TextReceivedEventArgs { text = newLines[i] });
                                }
                            }
                        }
                    }
                }
            }
            catch { }
        }

        protected virtual void OnTextReceived(TextReceivedEventArgs e)
        {
            TextReceived.Invoke(this, e);
        }
    }
}
