using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using AfterburnerDataHandler.SharedMemory.Afterburner;

namespace AfterburnerDataHandler.Servers.Serial
{
    public class SerialPortSettings : BaseServerSettings
    {
        [XmlElement]
        public override string FileFormat
        {
            get { return fileFormat; }
            set
            {
                bool isValueChanged = fileFormat != value;
                fileFormat = value;
                if (isValueChanged) IsDirty = isValueChanged;
            }
        }

        [XmlElement]
        public override int FormatVersion
        {
            get { return formatVersion; }
            set
            {
                bool isValueChanged = formatVersion != value;
                formatVersion = value;
                if (isValueChanged) IsDirty = true;
            }
        }

        [XmlElement]
        public BaudRate PortSpeed
        {
            get { return portSpeed; }
            set
            {
                bool isValueChanged = portSpeed != value;
                portSpeed = value;
                if (isValueChanged) IsDirty = true;
            }
        }

        [XmlElement]
        public SerialPortEncoding Encoding
        {
            get { return encoding; }
            set
            {
                bool isValueChanged = encoding != value;
                encoding = value;
                if (isValueChanged) IsDirty = true;
            }
        }

        [XmlElement]
        public string EndOfLineChar
        {
            get { return endOfLineChar; }
            set
            {
                bool isValueChanged = endOfLineChar != value;
                endOfLineChar = value;
                if (isValueChanged) IsDirty = true;
            }
        }

        [XmlElement]
        public bool AutoConnect
        {
            get { return autoConnect; }
            set
            {
                bool isValueChanged = autoConnect != value;
                autoConnect = value;
                if (isValueChanged) IsDirty = true;
            }
        }

        [XmlElement]
        public string AutoConnectRequest
        {
            get { return autoConnectRequest; }
            set
            {
                bool isValueChanged = autoConnectRequest != value;
                autoConnectRequest = value;
                if (isValueChanged) IsDirty = true;
            }
        }

        [XmlElement]
        public string AutoConnectResponse
        {
            get { return autoConnectResponse; }
            set
            {
                bool isValueChanged = autoConnectResponse != value;
                autoConnectResponse = value;
                if (isValueChanged) IsDirty = true;
            }
        }

        [XmlElement]
        public int AutoConnectResponseTimeout
        {
            get { return autoConnectResponseTimeout; }
            set
            {
                bool isValueChanged = autoConnectResponseTimeout != value;
                autoConnectResponseTimeout = value;
                if (isValueChanged) IsDirty = true;
            }
        }

        [XmlElement]
        public int ConnectionInterval
        {
            get { return connectionInterval; }
            set
            {
                bool isValueChanged = connectionInterval != value;
                connectionInterval = value;
                if (isValueChanged) IsDirty = true;
            }
        }

        [XmlElement]
        public SendMode SendMode
        {
            get { return sendMode; }
            set
            {
                bool isValueChanged = sendMode != value;
                sendMode = value;
                if (isValueChanged) IsDirty = true;
            }
        }

        [XmlElement]
        public int MessageInterval
        {
            get { return messageInterval; }
            set
            {
                bool isValueChanged = messageInterval != value;
                messageInterval = value;
                if (isValueChanged) IsDirty = true;
            }
        }

        [XmlElement]
        public int ConnectionCheckInterval
        {
            get { return connectionCheckInterval; }
            set
            {
                bool isValueChanged = connectionCheckInterval != value;
                connectionCheckInterval = value;
                if (isValueChanged) IsDirty = true;
            }
        }

        [XmlElement]
        public string DataRequest
        {
            get { return dataRequest; }
            set
            {
                bool isValueChanged = dataRequest != value;
                dataRequest = value;
                if (isValueChanged) IsDirty = true;
            }
        }

        [XmlElement]
        public MASM_Formatting DataFormatter
        {
            get { return dataFormatter; }
            set
            {
                bool isValueChanged = dataFormatter != value;
                dataFormatter = value;
                if (isValueChanged) IsDirty = true;
            }
        }

        private string fileFormat = "adhts";
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

        public SerialPortSettings()
        {
            DataFormatter.ParameterChanged += DataFormatterChanged;
        }

        private void DataFormatterChanged(object sender, EventArgs e)
        {
            IsDirty = true;
        }
    }
}
