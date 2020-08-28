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
            if (IsOpen) return false;

            try
            {
                if (string.IsNullOrEmpty(text) == false)
                {
                    serial.WriteLine(text);
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
                    string line = serialPort.ReadLine();

                    if (string.IsNullOrEmpty(line) == false && line.IndexOf(serialPort.NewLine) != 0)
                    {
                        OnTextReceived(new TextReceivedEventArgs { text = line.Trim(serialPort.NewLine.ToArray()) });
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
