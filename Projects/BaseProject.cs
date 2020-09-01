using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace AfterburnerDataHandler.Projects
{
    public class BaseProject : IProjectElement
    {
        public event EventHandler<EventArgs> ParameterChanged;
        public event EventHandler<EventArgs> ProjectNameChanged;

        [XmlElement]
        public virtual string ProjectFormat { get { return string.Empty; } set { } }

        [XmlElement]
        public virtual int FormatVersion { get { return 1; } set { } }


        [XmlIgnore]
        public virtual string ProjectName
        {
            get { return projectName; }
            set
            {
                bool parameterChanged = projectName != value;
                projectName = value;

                if (parameterChanged == true)
                    OnProjectNameChanged(EventArgs.Empty);
            }
        }

        [XmlIgnore]
        public virtual bool IsDirty
        {
            get { return isDirty; }
            set
            {
                isDirty = value;
                SetDirty(value, value == true);
            }
        }

        private bool isDirty = false;
        private string projectName = "NewProject";

        public virtual void SetDirty(bool isDirty, bool invokeParameterChanged)
        {
            this.isDirty = isDirty;

            if (invokeParameterChanged == true)
                OnParameterChanged(EventArgs.Empty);
        }

        protected virtual void SetParameter<T>(ref T parameter, T value)
        {
            bool parameterChanged = !parameter?.Equals(value) ?? true;
            parameter = value;
            if (parameterChanged == true) IsDirty = true;

        }

        protected virtual void OnParameterChanged(EventArgs e)
        {
            ParameterChanged?.Invoke(this, e);
        }

        protected virtual void OnProjectNameChanged(EventArgs e)
        {
            ProjectNameChanged?.Invoke(this, e);
        }
    }
}
