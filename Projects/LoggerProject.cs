using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using AfterburnerDataHandler.SharedMemory.Afterburner;

namespace AfterburnerDataHandler.Projects
{
    public class LoggerProject : BaseProject
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
        public string LogFileFormat
        {
            get { return logFileFormat; }
            set { SetParameter(ref logFileFormat, value); }
        }

        [XmlElement]
        public int DataUpdateInterval
        {
            get { return dataUpdateInterval; }
            set { SetParameter(ref dataUpdateInterval, value); }
        }

        [XmlElement]
        public bool UseFrametimeMode
        {
            get { return useFrametimeMode; }
            set { SetParameter(ref useFrametimeMode, value); }
        }

        [XmlElement]
        public string StartText
        {
            get { return startText; }
            set { SetParameter(ref startText, value); }
        }

        [XmlElement]
        public string FinalText
        {
            get { return finalText; }
            set { SetParameter(ref finalText, value); }
        }

        [XmlElement]
        public MASM_Formatting DataFormatter
        {
            get
            {
                if (dataFormatter == null)
                {
                    dataFormatter = new MASM_Formatting();
                    dataFormatter.ParameterChanged += DataFormatterChanged;
                }

                return dataFormatter;
            }
            set
            {
                if (dataFormatter != null)
                    dataFormatter.ParameterChanged -= DataFormatterChanged;

                SetParameter(ref dataFormatter, value);

                if (dataFormatter != null)
                    dataFormatter.ParameterChanged += DataFormatterChanged;
            }
        }

        private string projectFormat = "adhtl";
        private int formatVersion = 1;
        private string logFileFormat = "csv";
        private int dataUpdateInterval = 1000;
        private bool useFrametimeMode = false;
        private string startText = "";
        private string finalText = "";
        private MASM_Formatting dataFormatter;

        private void DataFormatterChanged(object sender, EventArgs e)
        {
            IsDirty = true;
        }
    }
}