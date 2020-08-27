using System;
using System.Runtime.InteropServices;

namespace AfterburnerDataHandler.SharedMemory.Afterburner
{
    [StructLayout(LayoutKind.Explicit)]
    public unsafe struct MAHM_SHARED_MEMORY_HEADER_UNSAFE
    {
        [FieldOffset(0)] public UInt32 dwSignature;
        [FieldOffset(4)] public UInt32 dwVersion;
        [FieldOffset(8)] public UInt32 dwHeaderSize;
        [FieldOffset(12)] public UInt32 dwNumEntries;
        [FieldOffset(16)] public UInt32 dwEntrySize;
        [FieldOffset(20)] public UInt32 time;
        [FieldOffset(24)] public UInt32 dwNumGpuEntries;
        [FieldOffset(28)] public UInt32 dwGpuEntrySize;
    }

    [StructLayout(LayoutKind.Explicit)]
    public unsafe struct MAHM_SHARED_MEMORY_GPU_ENTRY_UNSAFE
    {
        [FieldOffset(0)] public fixed sbyte szGpuId[260];
        [FieldOffset(260)] public fixed sbyte szFamily[260];
        [FieldOffset(520)] public fixed sbyte szDevice[260];
        [FieldOffset(780)] public fixed sbyte szDriver[260];
        [FieldOffset(1040)] public fixed sbyte szBIOS[260];
        [FieldOffset(1300)] public UInt32 dwMemAmount;
    }

    [StructLayout(LayoutKind.Explicit)]
    public unsafe struct MAHM_SHARED_MEMORY_ENTRY_UNSAFE
    {
        [FieldOffset(0)] public fixed sbyte szSrcName[260];
        [FieldOffset(260)] public fixed sbyte szSrcUnits[260];
        [FieldOffset(520)] public fixed sbyte szLocalizedSrcName[260];
        [FieldOffset(780)] public fixed sbyte szLocalizedSrcUnits[260];
        [FieldOffset(1040)] public fixed sbyte szRecommendedFormat[260];
        [FieldOffset(1300)] public Single data;
        [FieldOffset(1304)] public Single minLimit;
        [FieldOffset(1308)] public Single maxLimit;
        [FieldOffset(1312)] public UInt32 dwFlags;
        [FieldOffset(1316)] public UInt32 dwGpu;
        [FieldOffset(1320)] public UInt32 dwSrcId;
    }
}
