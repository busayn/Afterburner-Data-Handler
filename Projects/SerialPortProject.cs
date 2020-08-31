using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using AfterburnerDataHandler.Servers.Serial;
using AfterburnerDataHandler.SharedMemory.Afterburner;

namespace AfterburnerDataHandler.Projects
{
    public class SerialPortProject : BaseProject
    {
        [XmlElement]
        public override string ProjectFormat
        {
            get { return projectFormat; }
            set { SetParameter(ref projectFormat, value); }
        }

        [XmlElement]
        public override int FormatVersion
        {
            get { return formatVersion; }
            set { SetParameter(ref formatVersion, value); }
        }

        [XmlElement]
        public BaudRate PortSpeed
        {
            get { return portSpeed; }
            set { SetParameter(ref portSpeed, value); }
        }

        [XmlElement]
        public SerialPortEncoding Encoding
        {
            get { return encoding; }
            set { SetParameter(ref encoding, value); }
        }

        [XmlElement]
        public string EndOfLineChar
        {
            get { return endOfLineChar; }
            set { SetParameter(ref endOfLineChar, value); }
        }

        [XmlElement]
        public bool AutoConnect
        {
            get { return autoConnect; }
            set { SetParameter(ref autoConnect, value); }
        }

        [XmlElement]
        public string AutoConnectRequest
        {
            get { return autoConnectRequest; }
            set { SetParameter(ref autoConnectRequest, value); }
        }

        [XmlElement]
        public string AutoConnectResponse
        {
            get { return autoConnectResponse; }
            set { SetParameter(ref autoConnectResponse, value); }
        }

        [XmlElement]
        public int AutoConnectResponseTimeout
        {
            get { return autoConnectResponseTimeout; }
            set { SetParameter(ref autoConnectResponseTimeout, value); }
        }

        [XmlElement]
        public int ConnectionInterval
        {
            get { return connectionInterval; }
            set { SetParameter(ref connectionInterval, value); }
        }

        [XmlElement]
        public SendMode SendMode
        {
            get { return sendMode; }
            set { SetParameter(ref sendMode, value); }
        }

        [XmlElement]
        public int MessageInterval
        {
            get { return messageInterval; }
            set { SetParameter(ref messageInterval, value); }
        }

        [XmlElement]
        public int ConnectionCheckInterval
        {
            get { return connectionCheckInterval; }
            set { SetParameter(ref connectionCheckInterval, value); }
        }

        [XmlElement]
        public string DataRequest
        {
            get { return dataRequest; }
            set { SetParameter(ref dataRequest, value); }
        }

        [XmlElement]
        public MASM_Formatting DataFormatter
        {
            get { return dataFormatter; }
            set { SetParameter(ref dataFormatter, value); }
        }

        private string projectFormat = "adhts";
        private int formatVersion = 1;

        private BaudRate portSpeed = BaudRate.Baudrate9600;
        private SerialPortEncoding encoding = SerialPortEncoding.UTF8;
        private string endOfLineChar = "\\n";

        private bool autoConnect = true;
        private string autoConnectRequest = "ADH Connect";
        private string autoConnectResponse = "Hello ADH";
        private int autoConnectResponseTimeout = 500;
        private int connectionInterval = 1000;

        private SendMode sendMode = SendMode.Request;
        private int messageInterval = 1000;
        private int connectionCheckInterval = 5000;
        private string dataRequest = "Send Data";

        private MASM_Formatting dataFormatter = new MASM_Formatting();

        public SerialPortProject()
        {
            DataFormatter.ParameterChanged += DataFormatterChanged;
        }

        private void DataFormatterChanged(object sender, EventArgs e)
        {
            IsDirty = true;
        }
    }
}
