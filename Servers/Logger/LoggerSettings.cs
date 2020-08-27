using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using AfterburnerDataHandler.SharedMemory.Afterburner;

namespace AfterburnerDataHandler.Servers.Logger
{
    public class LoggerSettings
    {
        [XmlElement]
        public string FileFormat
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
        public int FormatVersion
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
        public string LogFileFormat
        {
            get { return logFileFormat; }
            set
            {
                bool isValueChanged = logFileFormat != value;
                logFileFormat = value;
                if (isValueChanged) IsDirty = true;
            }
        }

        [XmlElement]
        public int DataUpdateInterval
        {
            get { return dataUpdateInterval; }
            set
            {
                bool isValueChanged = dataUpdateInterval != value;
                dataUpdateInterval = value;
                if (isValueChanged) IsDirty = true;
            }
        }

        [XmlElement]
        public bool UseFrametimeMode
        {
            get { return useFrametimeMode; }
            set
            {
                bool isValueChanged = useFrametimeMode != value;
                useFrametimeMode = value;
                if (isValueChanged) IsDirty = true;
            }
        }

        [XmlElement]
        public string StartText
        {
            get { return startText; }
            set
            {
                bool isValueChanged = startText != value;
                startText = value;
                if (isValueChanged) IsDirty = true;
            }
        }

        [XmlElement]
        public string FinalText
        {
            get { return finalText; }
            set
            {
                bool isValueChanged = finalText != value;
                finalText = value;
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

        [XmlIgnore]
        public bool IsDirty
        {
            get { return isDirty; }
            set { isDirty = value; if (value == true) OnParameterChanged(EventArgs.Empty); }
        }

        public event EventHandler<EventArgs> ParameterChanged;

        private string fileFormat = "adhtl";
        private int formatVersion = 1;
        private string logFileFormat = "csv";
        private int dataUpdateInterval = 1000;
        private bool useFrametimeMode = false;
        private string startText = "";
        private string finalText = "";
        private MASM_Formatting dataFormatter = new MASM_Formatting();

        private bool isDirty = false;

        public LoggerSettings()
        {
            DataFormatter.ParameterChanged += DataFormatterChanged;
        }

        protected virtual void OnParameterChanged(EventArgs e)
        {
            ParameterChanged?.Invoke(this, e);
        }

        private void DataFormatterChanged(object sender, EventArgs e)
        {
            IsDirty = true;
        }
    }
}