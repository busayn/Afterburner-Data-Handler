using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AfterburnerDataHandler.Servers.Serial
{
    public class SerialPortHandler
    {

        public virtual SerialPort BaseSerial
        {
            get
            {
                if (serial == null)
                    serial = new SerialPort()
                    {
                        ReadTimeout = SerialPort.InfiniteTimeout,
                        WriteTimeout = SerialPort.InfiniteTimeout,
                    };

                return serial;
            }
        }

        public virtual bool IsOpen { get { return BaseSerial.IsOpen; } }
        public Encoding Encoding
        {
            get { return BaseSerial.Encoding; }
            set { BaseSerial.Encoding = value; }
        }

        public string EndOfLineChar
        {
            get { return BaseSerial.NewLine; }
            set { BaseSerial.NewLine = value; }
        }

        public class TextReceivedEventArgs : EventArgs { public string text; }
        public event EventHandler<TextReceivedEventArgs> TextReceived;

        private SerialPort serial;
        private string receivedData = "";

        public SerialPortHandler()
        {
            BaseSerial.DataReceived += SerialDataReceived;
        }

        public virtual bool Open(string portName, int speed)
        {
            if (IsOpen == true) Close();

            if (string.IsNullOrEmpty(portName) == true)
                return false;

            try
            {
                BaseSerial.PortName = portName;
                BaseSerial.BaudRate = speed;
                BaseSerial.Open();
            }
            catch { }

            return IsOpen;
        }

        public virtual void Close()
        {
            BaseSerial?.Close();
        }

        public virtual bool SendText(string text)
        {
            if (IsOpen == false) return false;

            try
            {
                if (string.IsNullOrEmpty(text) == false)
                {
                    BaseSerial?.Write(text + (EndOfLineChar ?? "\n"));
                }
            }
            catch
            {
                return false;
            }

            return true;
        }

        public static List<string> GetAvailablePorts()
        {
            List<string> openPorts = new List<string>(SerialPort.GetPortNames());
            return openPorts;
        }

        protected virtual void SerialDataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            try
            {
                if (e.EventType == SerialData.Chars && sender is SerialPort)
                {
                    SerialPort serialPort = sender as SerialPort;
                    receivedData = (receivedData ?? String.Empty) + (serialPort.ReadExisting() ?? String.Empty);

                    if (receivedData.Contains(serialPort.NewLine))
                    {
                        string[] newLines = receivedData.Split(new string[] { serialPort.NewLine }, StringSplitOptions.None);

                        if (newLines.Length < 2)
                        {
                            receivedData = String.Empty;

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
