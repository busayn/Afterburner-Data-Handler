using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace AfterburnerDataHandler.Servers
{
    public class BaseServerSettings
    {
        public event EventHandler<EventArgs> ParameterChanged;

        [XmlElement]
        public virtual string FileFormat { get { return ""; } set { } }

        [XmlElement]
        public virtual int FormatVersion { get { return 1; } set { } }

        [XmlIgnore]
        public virtual bool IsDirty
        {
            get { return isDirty; }
            set { isDirty = value; if (value == true) OnParameterChanged(EventArgs.Empty); }
        }

        private bool isDirty = false;

        protected virtual void OnParameterChanged(EventArgs e)
        {
            ParameterChanged?.Invoke(this, e);
        }
    }
}
