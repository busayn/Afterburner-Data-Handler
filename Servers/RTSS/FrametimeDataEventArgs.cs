using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AfterburnerDataHandler.Servers.RTSS
{
    public class FrametimeDataEventArgs : EventArgs
    {
        public uint appID;
        public uint frametime;

        public FrametimeDataEventArgs(uint appID, uint frametime)
        {
            this.appID = appID;
            this.frametime = frametime;
        }
    }
}
