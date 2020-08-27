using System;
using System.Runtime.InteropServices;

namespace AfterburnerDataHandler.SharedMemory.RivaTunerStatisticsServer
{
    [StructLayout(LayoutKind.Explicit)]
    public unsafe struct RTSS_SHARED_MEMORY_UNSAFE
    {
        [FieldOffset(0)] public UInt32 dwSignature;
        [FieldOffset(4)] public UInt32 dwVersion;
        [FieldOffset(8)] public UInt32 dwAppEntrySize;
        [FieldOffset(12)] public UInt32 dwAppArrOffset;
        [FieldOffset(16)] public UInt32 dwAppArrSize;
        [FieldOffset(20)] public UInt32 dwOSDEntrySize;
        [FieldOffset(24)] public UInt32 dwOSDArrOffset;
        [FieldOffset(28)] public UInt32 dwOSDArrSize;
        [FieldOffset(32)] public UInt32 dwOSDFrame;
    }

    [StructLayout(LayoutKind.Explicit)]
    public unsafe struct RTSS_SHARED_MEMORY_OSD_ENTRY_UNSAFE
    {
        [FieldOffset(0)] public fixed sbyte szOSD[256];
        [FieldOffset(256)] public fixed sbyte szOSDOwner[256];
    }

    [StructLayout(LayoutKind.Explicit)]
    public unsafe struct RTSS_SHARED_MEMORY_APP_ENTRY_UNSAFE
    {
        [FieldOffset(0)] public UInt32 dwProcessID;
        [FieldOffset(4)] public fixed sbyte szName[260];
        [FieldOffset(264)] public UInt32 dwFlags;
        [FieldOffset(268)] public UInt32 dwTime0;
        [FieldOffset(272)] public UInt32 dwTime1;
        [FieldOffset(276)] public UInt32 dwFrames;
        [FieldOffset(280)] public UInt32 dwFrameTime;
        [FieldOffset(284)] public UInt32 dwStatFlags;
        [FieldOffset(288)] public UInt32 dwStatTime0;
        [FieldOffset(292)] public UInt32 dwStatTime1;
        [FieldOffset(296)] public UInt32 dwStatFrames;
        [FieldOffset(300)] public UInt32 dwStatCount;
        [FieldOffset(304)] public UInt32 dwStatFramerateMin;
        [FieldOffset(308)] public UInt32 dwStatFramerateAvg;
        [FieldOffset(312)] public UInt32 dwStatFramerateMax;
        [FieldOffset(316)] public UInt32 dwOSDX;
        [FieldOffset(320)] public UInt32 dwOSDY;
        [FieldOffset(324)] public UInt32 dwOSDPixel;
        [FieldOffset(328)] public UInt32 dwOSDColor;
        [FieldOffset(332)] public UInt32 dwOSDFrame;
        [FieldOffset(336)] public UInt32 dwScreenCaptureFlags;
        [FieldOffset(340)] public fixed sbyte szScreenCapturePath[260];
    }
}
