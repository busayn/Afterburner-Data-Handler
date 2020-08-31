using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace AfterburnerDataHandler.Projects
{
    public interface IProjectElement
    {
        bool IsDirty { get; set; }
        event EventHandler<EventArgs> ParameterChanged;
    }
}
