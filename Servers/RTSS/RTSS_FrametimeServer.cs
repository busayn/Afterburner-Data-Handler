using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AfterburnerDataHandler.SharedMemory.RivaTunerStatisticsServer;

namespace AfterburnerDataHandler.Servers.RTSS
{
    public class RTSS_FrametimeServer : BaseServer
    {
        public string ConnectedApp { get; protected set; }
        public event EventHandler<FrametimeDataEventArgs> FrametimeDataReceived;

        protected Task serverTask = null;
        protected CancellationTokenSource cancellationToken = null;
        protected NamedPipeServerStream rtssServer;
        private readonly string frametimePipeName = "RTSS_Frametime";

        protected virtual async Task FrametimeServer()
        {
            if (cancellationToken == null)
            {
                ServerState = ServerState.Stop;
                return;
            }

            while (!cancellationToken.Token.IsCancellationRequested)
            {
                rtssServer = CreateNewServer();

                try
                {
                    ServerState = ServerState.Waiting;

                    await rtssServer.WaitForConnectionAsync(cancellationToken.Token);
                    cancellationToken.Token.ThrowIfCancellationRequested();

                    ConnectedApp = ReadAppName(rtssServer);
                    ServerState = ServerState.Connected;

                    unsafe
                    {
                        int frametimeBufferSize = sizeof(RTSS_FRAMETIME_PIPE_DATA);
                        byte[] frametimeBuffer = new byte[frametimeBufferSize];

                        fixed (void* bufferPointer = frametimeBuffer)
                        {
                            int bytesCount = 0;

                            while (rtssServer.IsConnected == true)
                            {
                                cancellationToken.Token.ThrowIfCancellationRequested();
                                bytesCount = rtssServer.Read(frametimeBuffer, 0, frametimeBufferSize);

                                if (bytesCount == frametimeBufferSize)
                                {
                                    var frametimePointer = (RTSS_FRAMETIME_PIPE_DATA*)bufferPointer;
                                    cancellationToken.Token.ThrowIfCancellationRequested();

                                    OnFrametimeDataReceived(new FrametimeDataEventArgs(
                                        frametimePointer->dwApp,
                                        frametimePointer->dwFrametime));
                                }
                                else
                                {
                                    break;
                                }
                            }
                        }
                    }
                }
                catch
                {

                }


                if (!cancellationToken.Token.IsCancellationRequested)
                {
                    ServerState = ServerState.Reconnect;
                }

                rtssServer?.Close();
                rtssServer?.Dispose();
            }

            ServerState = ServerState.Stop;
        }

        protected string ReadAppName(NamedPipeServerStream server)
        {
            unsafe
            {
                int frametimeBufferSize = sizeof(RTSS_FRAMETIME_PIPE_DATA);
                byte[] frametimeBuffer = new byte[frametimeBufferSize];

                fixed (void* bufferPointer = frametimeBuffer)
                {
                    if (server.Read(frametimeBuffer, 0, frametimeBufferSize) == frametimeBufferSize)
                    {
                        var frametimePointer = (RTSS_FRAMETIME_PIPE_DATA*)bufferPointer;
                        return RTSSSM.GetAppEntry(frametimePointer->dwApp).szName;
                    }
                }
            }

            return "";
        }

        protected NamedPipeServerStream CreateNewServer()
        {
            return new NamedPipeServerStream(
                frametimePipeName,
                PipeDirection.InOut,
                NamedPipeServerStream.MaxAllowedServerInstances,
                PipeTransmissionMode.Message,
                PipeOptions.Asynchronous,
                4096,
                4096);
        }

        public override bool Begin()
        {
            Stop();
            base.Begin();

            cancellationToken = new CancellationTokenSource();

            serverTask = Task.Run(async () =>
            {
                await FrametimeServer();
            }, cancellationToken.Token);

            return true;
        }

        public override void Stop()
        {
            if (ServerState != ServerState.Stop)
            {
                cancellationToken?.Cancel();
                rtssServer?.Close();

                if (!Task.WaitAll(new Task[] { serverTask }, 2000))
                {
                    Console.WriteLine("Cancellation Error");
                }
            }
        }

        protected void OnFrametimeDataReceived(FrametimeDataEventArgs e)
        {
            FrametimeDataReceived?.Invoke(this, e);
        }
    }
}