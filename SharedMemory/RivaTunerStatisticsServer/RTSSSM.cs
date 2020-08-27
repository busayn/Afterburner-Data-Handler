using System;
using System.Collections.Generic;
using System.IO;
using System.IO.MemoryMappedFiles;
using System.Runtime.InteropServices;
using System.Text;

namespace AfterburnerDataHandler.SharedMemory.RivaTunerStatisticsServer
{
    class RTSSSM
    {
        public string mapName = "RTSSSharedMemoryV2";

        public RTSS_SHARED_MEMORY header;
        public List<RTSS_SHARED_MEMORY_OSD_ENTRY> OSDEntries = new List<RTSS_SHARED_MEMORY_OSD_ENTRY>();
        public List<RTSS_SHARED_MEMORY_APP_ENTRY> APPEntries = new List<RTSS_SHARED_MEMORY_APP_ENTRY>();

        private MemoryMappedFile rtssMappedFile;
        private MemoryMappedViewStream rtssStream;
        bool serverState = false;

        public bool Start()
        {
            if (serverState == true) Stop();

            try
            {
                serverState = true;
                rtssMappedFile = MemoryMappedFile.OpenExisting(mapName, MemoryMappedFileRights.ReadWrite);
                rtssStream = rtssMappedFile.CreateViewStream();
            }
            catch (Exception e) { Console.WriteLine(e); serverState = false; }

            return serverState;
        }

        public RTSSSM Update()
        {
            if (serverState == false) Start();

            header = new RTSS_SHARED_MEMORY();

            if (OSDEntries == null) OSDEntries = new List<RTSS_SHARED_MEMORY_OSD_ENTRY>();
            else OSDEntries.Clear();

            if (APPEntries == null) APPEntries = new List<RTSS_SHARED_MEMORY_APP_ENTRY>();
            else APPEntries.Clear();

            try
            {
                int headerBufferSize;
                int osdEntryBufferSize;
                int appEntryBufferSize;

                unsafe
                {
                    headerBufferSize = sizeof(RTSS_SHARED_MEMORY_UNSAFE);
                    osdEntryBufferSize = sizeof(RTSS_SHARED_MEMORY_OSD_ENTRY_UNSAFE);
                    appEntryBufferSize = sizeof(RTSS_SHARED_MEMORY_APP_ENTRY_UNSAFE);
                }

                var headerBuffer = new byte[headerBufferSize];
                var osdEntryBuffer = new byte[osdEntryBufferSize];
                var appEntryBuffer = new byte[appEntryBufferSize];

                if (rtssStream.CanRead && rtssStream.Capacity >= headerBufferSize)
                {
                    unsafe
                    {
                        fixed (void* bufferPointer = headerBuffer)
                        {
                            rtssStream.Position = 0;
                            rtssStream.Read(headerBuffer, 0, headerBufferSize);
                            var headerPointer = (RTSS_SHARED_MEMORY_UNSAFE*)bufferPointer;

                            // READ RTSS_SHARED_MEMORY
                            if (headerPointer->dwSignature == 0x52545353)
                            {
                                header = new RTSS_SHARED_MEMORY
                                {
                                    dwSignature = headerPointer->dwSignature,
                                    dwVersion = headerPointer->dwVersion,
                                    dwAppEntrySize = headerPointer->dwAppEntrySize,
                                    dwAppArrOffset = headerPointer->dwAppArrOffset,
                                    dwAppArrSize = headerPointer->dwAppArrSize,
                                    dwOSDEntrySize = headerPointer->dwOSDEntrySize,
                                    dwOSDArrOffset = headerPointer->dwOSDArrOffset,
                                    dwOSDArrSize = headerPointer->dwOSDArrSize,
                                    dwOSDFrame = headerPointer->dwOSDFrame
                                };
                            }
                        }

                        // READ RTSS_SHARED_MEMORY_OSD_ENTRY
                        if(rtssStream.Capacity > header.dwOSDArrOffset + osdEntryBufferSize * header.dwOSDArrSize)
                        {
                            fixed (void* bufferPointer = osdEntryBuffer)
                            {
                                for (int i = 0; i < header.dwOSDArrSize; i++)
                                {
                                    rtssStream.Position = header.dwOSDArrOffset + header.dwOSDEntrySize * i;
                                    rtssStream.Read(osdEntryBuffer, 0, osdEntryBufferSize);

                                    var osdPointer = (RTSS_SHARED_MEMORY_OSD_ENTRY_UNSAFE*)bufferPointer;

                                    OSDEntries.Add(new RTSS_SHARED_MEMORY_OSD_ENTRY
                                    {
                                        szOSD = new string(osdPointer->szOSD, 0, 256, Encoding.Default).Trim('\x0'),
                                        szOSDOwner = new string(osdPointer->szOSDOwner, 0, 256, Encoding.Default).Trim('\x0'),
                                    });
                                }
                            }
                        }

                        // READ RTSS_SHARED_MEMORY_APP_ENTRY
                        if (rtssStream.Capacity > header.dwAppArrOffset + appEntryBufferSize * header.dwOSDArrSize)
                        {
                            fixed (void* bufferPointer = appEntryBuffer)
                            {
                                for (int i = 0; i < header.dwAppArrSize; i++)
                                {
                                    rtssStream.Position = header.dwAppArrOffset + header.dwAppEntrySize * i;
                                    rtssStream.Read(appEntryBuffer, 0, appEntryBufferSize);

                                    var appPointer = (RTSS_SHARED_MEMORY_APP_ENTRY_UNSAFE*)bufferPointer;

                                    APPEntries.Add(new RTSS_SHARED_MEMORY_APP_ENTRY
                                    {
                                        dwProcessID = appPointer->dwProcessID,
                                        szName = new string(appPointer->szName, 0, 260, Encoding.Default).Trim('\x0'),
                                        dwFlags = appPointer->dwFlags,
                                        dwTime0 = appPointer->dwTime0,
                                        dwTime1 = appPointer->dwTime1,
                                        dwFrames = appPointer->dwFrames,
                                        dwFrameTime = appPointer->dwFrameTime,
                                        dwStatFlags = appPointer->dwStatFlags,
                                        dwStatTime0 = appPointer->dwStatTime0,
                                        dwStatTime1 = appPointer->dwStatTime1,
                                        dwStatFrames = appPointer->dwStatFrames,
                                        dwStatCount = appPointer->dwStatCount,
                                        dwStatFramerateMin = appPointer->dwStatFramerateMin,
                                        dwStatFramerateAvg = appPointer->dwStatFramerateAvg,
                                        dwStatFramerateMax = appPointer->dwStatFramerateMax,
                                        dwOSDX = appPointer->dwOSDX,
                                        dwOSDY = appPointer->dwOSDY,
                                        dwOSDPixel = appPointer->dwOSDPixel,
                                        dwOSDColor = appPointer->dwOSDColor,
                                        dwOSDFrame = appPointer->dwOSDFrame,
                                        dwScreenCaptureFlags = appPointer->dwScreenCaptureFlags,
                                        szScreenCapturePath = new string(appPointer->szScreenCapturePath, 0, 260, Encoding.Default).Trim('\x0')
                                    });
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception e) { Console.WriteLine(e); }

            return this;
        }

        public RTSSSM UpdateOnce()
        {
            Update();
            Stop();

            return this;
        }

        public void Stop()
        {
            try
            {
                serverState = false;
                rtssStream?.Dispose();
                rtssMappedFile?.Dispose();
            }
            catch (Exception e) { Console.WriteLine(e); }
        }

        public static RTSS_SHARED_MEMORY_OSD_ENTRY GetOSDEntry(uint osdID, string mapName = "RTSSSharedMemoryV2")
        {
            RTSS_SHARED_MEMORY header = new RTSS_SHARED_MEMORY();
            RTSS_SHARED_MEMORY_OSD_ENTRY osd = new RTSS_SHARED_MEMORY_OSD_ENTRY();

            if (osdID < 0) return osd;

            try
            {
                using (MemoryMappedFile mmf = MemoryMappedFile.OpenExisting(mapName, MemoryMappedFileRights.ReadWrite))
                {
                    using (MemoryMappedViewStream stream = mmf.CreateViewStream())
                    {
                        int headerBufferSize;
                        int osdEntryBufferSize;

                        unsafe
                        {
                            headerBufferSize = sizeof(RTSS_SHARED_MEMORY_UNSAFE);
                            osdEntryBufferSize = sizeof(RTSS_SHARED_MEMORY_OSD_ENTRY_UNSAFE);
                        }

                        var headerBuffer = new byte[headerBufferSize];
                        var osdEntryBuffer = new byte[osdEntryBufferSize];

                        unsafe
                        {
                            fixed (void* bufferPointer = headerBuffer)
                            {
                                stream.Position = 0;
                                stream.Read(headerBuffer, 0, headerBufferSize);

                                var headerPointer = (RTSS_SHARED_MEMORY_UNSAFE*)bufferPointer;

                                if (headerPointer->dwSignature == 0x52545353)
                                {
                                    header = new RTSS_SHARED_MEMORY
                                    {
                                        dwSignature = headerPointer->dwSignature,
                                        dwVersion = headerPointer->dwVersion,
                                        dwAppEntrySize = headerPointer->dwAppEntrySize,
                                        dwAppArrOffset = headerPointer->dwAppArrOffset,
                                        dwAppArrSize = headerPointer->dwAppArrSize,
                                        dwOSDEntrySize = headerPointer->dwOSDEntrySize,
                                        dwOSDArrOffset = headerPointer->dwOSDArrOffset,
                                        dwOSDArrSize = headerPointer->dwOSDArrSize,
                                        dwOSDFrame = headerPointer->dwOSDFrame
                                    };
                                }
                            }

                            int entrySize = (int)header.dwAppEntrySize;
                            int osdOffset = (int)(header.dwOSDArrOffset + entrySize * osdID);

                            if (stream.Capacity > osdOffset + entrySize)
                            {
                                fixed (void* bufferPointer = osdEntryBuffer)
                                {
                                    stream.Position = osdOffset;
                                    stream.Read(osdEntryBuffer, 0, osdEntryBufferSize);

                                    var osdPointer = (RTSS_SHARED_MEMORY_OSD_ENTRY_UNSAFE*)bufferPointer;

                                    osd = new RTSS_SHARED_MEMORY_OSD_ENTRY
                                    {
                                        szOSD = new string(osdPointer->szOSD, 0, 256, Encoding.Default).Trim('\x0'),
                                        szOSDOwner = new string(osdPointer->szOSDOwner, 0, 256, Encoding.Default).Trim('\x0'),
                                    };
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception e) { Console.WriteLine(e.Message); }

            return osd;
        }

        public static RTSS_SHARED_MEMORY_APP_ENTRY GetAppEntry(uint appID, string mapName = "RTSSSharedMemoryV2")
        {
            RTSS_SHARED_MEMORY header = new RTSS_SHARED_MEMORY();
            RTSS_SHARED_MEMORY_APP_ENTRY app = new RTSS_SHARED_MEMORY_APP_ENTRY();

            if (appID < 0) return app;

            try
            {
                using (MemoryMappedFile mmf = MemoryMappedFile.OpenExisting(mapName, MemoryMappedFileRights.ReadWrite))
                {
                    using (MemoryMappedViewStream stream = mmf.CreateViewStream())
                    {
                        int headerBufferSize;
                        int appEntryBufferSize;

                        unsafe
                        {
                            headerBufferSize = sizeof(RTSS_SHARED_MEMORY_UNSAFE);
                            appEntryBufferSize = sizeof(RTSS_SHARED_MEMORY_APP_ENTRY_UNSAFE);
                        }

                        var headerBuffer = new byte[headerBufferSize];
                        var appEntryBuffer = new byte[appEntryBufferSize];

                        unsafe
                        {
                            fixed (void* bufferPointer = headerBuffer)
                            {
                                stream.Position = 0;
                                stream.Read(headerBuffer, 0, headerBufferSize);

                                var headerPointer = (RTSS_SHARED_MEMORY_UNSAFE*)bufferPointer;

                                if (headerPointer->dwSignature == 0x52545353)
                                {
                                    header = new RTSS_SHARED_MEMORY
                                    {
                                        dwSignature = headerPointer->dwSignature,
                                        dwVersion = headerPointer->dwVersion,
                                        dwAppEntrySize = headerPointer->dwAppEntrySize,
                                        dwAppArrOffset = headerPointer->dwAppArrOffset,
                                        dwAppArrSize = headerPointer->dwAppArrSize,
                                        dwOSDEntrySize = headerPointer->dwOSDEntrySize,
                                        dwOSDArrOffset = headerPointer->dwOSDArrOffset,
                                        dwOSDArrSize = headerPointer->dwOSDArrSize,
                                        dwOSDFrame = headerPointer->dwOSDFrame
                                    };
                                }
                            }

                            int entrySize = (int)header.dwAppEntrySize;
                            int appOffset = (int)(header.dwAppArrOffset + entrySize * appID);

                            if (stream.Capacity > appOffset + entrySize)
                            {
                                fixed (void* bufferPointer = appEntryBuffer)
                                {
                                    stream.Position = appOffset;
                                    stream.Read(appEntryBuffer, 0, appEntryBufferSize);

                                    var appPointer = (RTSS_SHARED_MEMORY_APP_ENTRY_UNSAFE*)bufferPointer;

                                    app = new RTSS_SHARED_MEMORY_APP_ENTRY
                                    {
                                        dwProcessID = appPointer->dwProcessID,
                                        szName = new string(appPointer->szName, 0, 260, Encoding.Default).Trim('\x0'),
                                        dwFlags = appPointer->dwFlags,
                                        dwTime0 = appPointer->dwTime0,
                                        dwTime1 = appPointer->dwTime1,
                                        dwFrames = appPointer->dwFrames,
                                        dwFrameTime = appPointer->dwFrameTime,
                                        dwStatFlags = appPointer->dwStatFlags,
                                        dwStatTime0 = appPointer->dwStatTime0,
                                        dwStatTime1 = appPointer->dwStatTime1,
                                        dwStatFrames = appPointer->dwStatFrames,
                                        dwStatCount = appPointer->dwStatCount,
                                        dwStatFramerateMin = appPointer->dwStatFramerateMin,
                                        dwStatFramerateAvg = appPointer->dwStatFramerateAvg,
                                        dwStatFramerateMax = appPointer->dwStatFramerateMax,
                                        dwOSDX = appPointer->dwOSDX,
                                        dwOSDY = appPointer->dwOSDY,
                                        dwOSDPixel = appPointer->dwOSDPixel,
                                        dwOSDColor = appPointer->dwOSDColor,
                                        dwOSDFrame = appPointer->dwOSDFrame,
                                        dwScreenCaptureFlags = appPointer->dwScreenCaptureFlags,
                                        szScreenCapturePath = new string(appPointer->szScreenCapturePath, 0, 260, Encoding.Default).Trim('\x0')
                                    };
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception e) { Console.WriteLine(e.Message); }

            return app;
        }
    }
}