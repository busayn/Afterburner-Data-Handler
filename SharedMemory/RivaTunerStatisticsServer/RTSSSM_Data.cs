using System;
using System.Text;

namespace AfterburnerDataHandler.SharedMemory.RivaTunerStatisticsServer
{
    public struct RTSS_SHARED_MEMORY
    {
        public UInt32 dwSignature;
        public UInt32 dwVersion;
        public UInt32 dwAppEntrySize;
        public UInt32 dwAppArrOffset;
        public UInt32 dwAppArrSize;
        public UInt32 dwOSDEntrySize;
        public UInt32 dwOSDArrOffset;
        public UInt32 dwOSDArrSize;
        public UInt32 dwOSDFrame;

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder("RTSS Shared Memory V2" + Environment.NewLine, 400);

            sb.AppendLine(String.Format("{0,-30}{1}", "Signature", dwSignature));
            sb.AppendLine(String.Format("{0,-30}{1}.{2}", "Version", dwVersion >> 16, dwVersion & 0xffff));
            sb.AppendLine(String.Format("{0,-30}{1}", "App Entry Size", dwAppEntrySize));
            sb.AppendLine(String.Format("{0,-30}{1}", "App Array Offset", dwAppArrOffset));
            sb.AppendLine(String.Format("{0,-30}{1}", "App Array Size", dwAppArrSize));
            sb.AppendLine(String.Format("{0,-30}{1}", "OSD Entry Size", dwOSDEntrySize));
            sb.AppendLine(String.Format("{0,-30}{1}", "OSD Array Offset", dwOSDArrOffset));
            sb.AppendLine(String.Format("{0,-30}{1}", "OSD Array Offset", dwOSDArrSize));
            sb.AppendLine(String.Format("{0,-30}{1}", "OSD Frame", dwOSDFrame));

            return sb.ToString();
        }
    }

    public struct RTSS_SHARED_MEMORY_OSD_ENTRY
    {
        public string szOSD;
        public string szOSDOwner;

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder("RTSS Shared Memory V2 OSD Entry" + Environment.NewLine, 600);
            
            sb.AppendLine(String.Format("{0,-30}{1}", "OSD", szOSD));
            sb.AppendLine(String.Format("{0,-30}{1}", "OSD Owner", szOSDOwner));

            return sb.ToString();
        }
    }

    public struct RTSS_SHARED_MEMORY_APP_ENTRY
    {
        public UInt32 dwProcessID;
        public string szName;
        public UInt32 dwFlags;
        public UInt32 dwTime0;
        public UInt32 dwTime1;
        public UInt32 dwFrames;
        public UInt32 dwFrameTime;
        public UInt32 dwStatFlags;
        public UInt32 dwStatTime0;
        public UInt32 dwStatTime1;
        public UInt32 dwStatFrames;
        public UInt32 dwStatCount;
        public UInt32 dwStatFramerateMin;
        public UInt32 dwStatFramerateAvg;
        public UInt32 dwStatFramerateMax;
        public UInt32 dwOSDX;
        public UInt32 dwOSDY;
        public UInt32 dwOSDPixel;
        public UInt32 dwOSDColor;
        public UInt32 dwOSDFrame;
        public UInt32 dwScreenCaptureFlags;
        public string szScreenCapturePath;

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder("RTSS Shared Memory V2 APP Entry" + Environment.NewLine, 1400);

            sb.AppendLine(String.Format("{0,-30}{1}", "Process ID", dwProcessID));
            sb.AppendLine(String.Format("{0,-30}{1}", "Name", szName));
            sb.AppendLine(String.Format("{0,-30}{1}", "Flags", dwFlags));
            sb.AppendLine(String.Format("{0,-30}{1}", "Time 0", dwTime0));
            sb.AppendLine(String.Format("{0,-30}{1}", "Time 1", dwTime1));
            sb.AppendLine(String.Format("{0,-30}{1}", "Frames", dwFrames));
            sb.AppendLine(String.Format("{0,-30}{1}", "Frame Time", dwFrameTime));
            sb.AppendLine(String.Format("{0,-30}{1}", "Stat Flags", dwStatFlags));
            sb.AppendLine(String.Format("{0,-30}{1}", "Stat Time 0", dwStatTime0));
            sb.AppendLine(String.Format("{0,-30}{1}", "Stat Time 1", dwStatTime1));
            sb.AppendLine(String.Format("{0,-30}{1}", "Stat Frames", dwStatFrames));
            sb.AppendLine(String.Format("{0,-30}{1}", "Stat Count", dwStatCount));
            sb.AppendLine(String.Format("{0,-30}{1}", "Stat Framerate Min", dwStatFramerateMin));
            sb.AppendLine(String.Format("{0,-30}{1}", "Stat Framerate Avg", dwStatFramerateAvg));
            sb.AppendLine(String.Format("{0,-30}{1}", "Stat Framerate Max", dwStatFramerateMax));
            sb.AppendLine(String.Format("{0,-30}{1}", "OSD X", dwOSDX));
            sb.AppendLine(String.Format("{0,-30}{1}", "OSD Y", dwOSDY));
            sb.AppendLine(String.Format("{0,-30}{1}", "OSD Pixel", dwOSDPixel));
            sb.AppendLine(String.Format("{0,-30}{1}", "OSD Color", dwOSDColor));
            sb.AppendLine(String.Format("{0,-30}{1}", "OSD Frame", dwOSDFrame));
            sb.AppendLine(String.Format("{0,-30}{1}", "Screen Capture Flags", dwScreenCaptureFlags));
            sb.AppendLine(String.Format("{0,-30}{1}", "Screen Capture Path", szScreenCapturePath));

            return sb.ToString();
        }
    }
}
