using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AfterburnerDataHandler.Servers
{
    public class ServerStateEventArgs : EventArgs
    {
        public readonly ServerState state;

        public ServerStateEventArgs (ServerState state)
        {
            this.state = state;
        }
    }
}
