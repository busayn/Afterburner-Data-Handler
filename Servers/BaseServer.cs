using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AfterburnerDataHandler.Servers
{
    public class BaseServer
    {
        public event EventHandler<ServerStateEventArgs> StateChanged;

        public ServerState ServerState
        {
            get { return serverState; }
            protected set
            {
                serverState = value;
                OnStateChanged(new ServerStateEventArgs(serverState));
            }
        }

        private ServerState serverState = ServerState.Stop;

        public virtual bool Begin()
        {
            this.ServerState = ServerState.Begin;
            return true;
        }

        public virtual void Stop()
        {
            this.ServerState = ServerState.Stop;
        }

        protected virtual void OnStateChanged(ServerStateEventArgs e)
        {
            StateChanged?.Invoke(this, e);
        }
    }
}
