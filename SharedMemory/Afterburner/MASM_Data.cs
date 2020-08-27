using System;
using System.Globalization;

namespace AfterburnerDataHandler.SharedMemory.Afterburner
{

    public unsafe struct MAHM_SHARED_MEMORY_HEADER
    {
        public string dwSignature;
        public UInt32 dwVersion;
        public UInt32 dwHeaderSize;
        public UInt32 dwNumEntries;
        public UInt32 dwEntrySize;
        public UInt32 time;
        public UInt32 dwNumGpuEntries;
        public UInt32 dwGpuEntrySize;

        public bool SharedMemoryFound()
        {
            return (dwSignature ?? "") == "MAHM" && dwVersion >= 0x00020000;
        }

        public static DateTime CTimeToSharpTime(UInt32 time)
        {
            TimeSpan timeSpan = TimeSpan.FromTicks(time * TimeSpan.TicksPerSecond);
            DateTime dateTime = new DateTime(1970, 1, 1).Add(timeSpan);

            return TimeZone.CurrentTimeZone.ToLocalTime(dateTime);
        }

        public override string ToString()
        {
            string str = "";
            string nl = Environment.NewLine;

            str += String.Format("{0,-30}{1}{2}", "Signature", dwSignature, nl);
            str += String.Format("{0,-30}{1}.{2}{3}", "Version", dwVersion >> 16, dwVersion & 0xffff, nl);
            str += String.Format("{0,-30}{1}{2}", "Header Size", dwHeaderSize, nl);
            str += String.Format("{0,-30}{1}{2}", "Entries Count", dwNumEntries, nl);
            str += String.Format("{0,-30}{1}{2}", "Entry Size", dwEntrySize, nl);
            str += String.Format("{0,-30}{1}{2}", "Time", CTimeToSharpTime(time).ToString(), nl);
            str += String.Format("{0,-30}{1}{2}", "Gpu Entries Count", dwNumGpuEntries, nl);
            str += String.Format("{0,-30}{1}{2}", "Gpu Entry Size", dwGpuEntrySize, nl);

            return str;
        }
    }

    public struct MAHM_SHARED_MEMORY_ENTRY
    {
        public string szSrcName;
        public string szSrcUnits;
        public string szLocalizedSrcName;
        public string szLocalizedSrcUnits;
        public string szRecommendedFormat;
        public Double data;
        public Double minLimit;
        public Double maxLimit;
        public UInt32 dwFlags;
        public UInt32 dwGpu;
        public UInt32 dwSrcId;

        public override string ToString()
        {
            return String.Format("{0,-30}{1,-30}{2:0.###} {3}", szLocalizedSrcName, szSrcName, data, szSrcUnits);
        }
    }

    public struct MAHM_SHARED_MEMORY_GPU_ENTRY
    {
        public string szGpuId;
        public string szFamily;
        public string szDevice;
        public string szDriver;
        public string szBIOS;
        public UInt32 dwMemAmount;

        public override string ToString()
        {
            string str = "";
            string nl = Environment.NewLine;

            str += String.Format("{0,-30}{1}{2}", "GPU ID", szGpuId, nl);
            str += String.Format("{0,-30}{1}{2}", "Family", szFamily, nl);
            str += String.Format("{0,-30}{1}{2}", "Device", szDevice, nl);
            str += String.Format("{0,-30}{1}{2}", "Driver", szDriver, nl);
            str += String.Format("{0,-30}{1}{2}", "BIOS", szBIOS, nl);
            str += String.Format("{0,-30}{1}{2:0.###}", "Memory Amount", dwMemAmount, nl);

            return str;
        }
    }
}
