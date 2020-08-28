using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AfterburnerDataHandler.Servers.SerialPort
{
    public class SerialPortServer : BaseServer
    {
        public SerialPortSettings Settings
        {
            get
            {
                if (settings == null)
                    settings = new SerialPortSettings();

                return settings;
            }
            set
            {
                this.Stop();
                settings = value;
            }
        }

        private SerialPortSettings settings;
        private Timer serialPortTimer;

        public override bool Begin()
        {
            if (ServerState != ServerState.Stop) Stop();

            ServerState = ServerState.Begin;

            serialPortTimer = new Timer(
                SerialPortLoop,
                true,
                0,
                1000);

            return true;
        }

        public override void Stop()
        {
            serialPortTimer?.Dispose();
            serialPortTimer = null;

            base.Stop();
        }

        protected virtual void SerialPortLoop(object state)
        {

        }
    }
}
