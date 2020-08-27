using System;
using System.IO;
using System.IO.MemoryMappedFiles;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using System.Globalization;

namespace AfterburnerDataHandler.SharedMemory.Afterburner
{
    public class MASM
    {
        public MAHM_SHARED_MEMORY_HEADER Header { get { return header; } }
        public List<MAHM_SHARED_MEMORY_ENTRY> Properties { get { return properties; } }
        public List<MAHM_SHARED_MEMORY_GPU_ENTRY> GPUData { get { return gpuData; } }
        public string MapName { get { return mapName; } set { mapName = value; } }


        private MAHM_SHARED_MEMORY_HEADER header;
        private List<MAHM_SHARED_MEMORY_ENTRY> properties = new List<MAHM_SHARED_MEMORY_ENTRY>();
        private List<MAHM_SHARED_MEMORY_GPU_ENTRY> gpuData = new List<MAHM_SHARED_MEMORY_GPU_ENTRY>();
        private string mapName = "MAHMSharedMemory";
        private MemoryMappedFile masmMappedFile;
        private MemoryMappedViewStream masmStream;
        bool serverState = false;

        public bool Start()
        {
            if (serverState == true) Stop();

            try
            {
                serverState = true;
                masmMappedFile = MemoryMappedFile.OpenExisting(mapName, MemoryMappedFileRights.ReadWrite);
                masmStream = masmMappedFile.CreateViewStream();
            }
            catch { serverState = false; }

            return serverState;
        }

        public MASM Update()
        {
            if (serverState == false) Start();

            header = new MAHM_SHARED_MEMORY_HEADER();

            if (properties == null) properties = new List<MAHM_SHARED_MEMORY_ENTRY>();
            else properties.Clear();

            if (gpuData == null) gpuData = new List<MAHM_SHARED_MEMORY_GPU_ENTRY>();
            else gpuData.Clear();

            try
            {
                int headerBufferSize;
                int propertyBufferSize;
                int gpuDataBufferSize;

                unsafe
                {
                    headerBufferSize = sizeof(MAHM_SHARED_MEMORY_HEADER_UNSAFE);
                    propertyBufferSize = sizeof(MAHM_SHARED_MEMORY_ENTRY_UNSAFE);
                    gpuDataBufferSize = sizeof(MAHM_SHARED_MEMORY_GPU_ENTRY_UNSAFE);
                }

                var headerBuffer = new byte[headerBufferSize];
                var propertyBuffer = new byte[propertyBufferSize];
                var gpuDataBuffer = new byte[gpuDataBufferSize];

                if (masmStream.CanRead && masmStream.Capacity >= headerBufferSize)
                {
                    unsafe
                    {
                        fixed (void* bufferPointer = headerBuffer)
                        {
                            masmStream.Position = 0;
                            masmStream.Read(headerBuffer, 0, headerBufferSize);
                            var headerPointer = (MAHM_SHARED_MEMORY_HEADER_UNSAFE*)bufferPointer;

                            // READ MAHM_SHARED_MEMORY_HEADER
                            if (headerPointer->dwSignature == 0x4D41484D)
                            {
                                header = new MAHM_SHARED_MEMORY_HEADER
                                {
                                    dwSignature = "MAHM",
                                    dwVersion = headerPointer->dwVersion,
                                    dwHeaderSize = headerPointer->dwHeaderSize,
                                    dwNumEntries = headerPointer->dwNumEntries,
                                    dwEntrySize = headerPointer->dwEntrySize,
                                    time = headerPointer->time,
                                    dwNumGpuEntries = headerPointer->dwNumGpuEntries,
                                    dwGpuEntrySize = headerPointer->dwGpuEntrySize,
                                };
                            }
                        }

                        // READ MAHM_SHARED_MEMORY_ENTRY
                        if (header.SharedMemoryFound() && masmStream.Capacity >=
                            propertyBufferSize * header.dwNumEntries +
                            gpuDataBufferSize * header.dwNumGpuEntries +
                            headerBufferSize)
                        {
                            fixed (void* bufferPointer = propertyBuffer)
                            {
                                masmStream.Position = header.dwHeaderSize;

                                for (int i = 0; i < header.dwNumEntries; i++)
                                {
                                    masmStream.Read(propertyBuffer, 0, propertyBufferSize);
                                    var gpuDataPointer = (MAHM_SHARED_MEMORY_ENTRY_UNSAFE*)bufferPointer;

                                    properties.Add(new MAHM_SHARED_MEMORY_ENTRY
                                    {
                                        szSrcName = new string(gpuDataPointer->szSrcName, 0, 260, Encoding.Default).Trim('\x0'),
                                        szSrcUnits = new string(gpuDataPointer->szSrcUnits, 0, 260, Encoding.Default).Trim('\x0'),
                                        szLocalizedSrcName = new string(gpuDataPointer->szLocalizedSrcName, 0, 260, Encoding.Default).Trim('\x0'),
                                        szLocalizedSrcUnits = new string(gpuDataPointer->szLocalizedSrcUnits, 0, 260, Encoding.Default).Trim('\x0'),
                                        szRecommendedFormat = new string(gpuDataPointer->szRecommendedFormat, 0, 260, Encoding.Default).Trim('\x0'),
                                        data = gpuDataPointer->data,
                                        minLimit = gpuDataPointer->minLimit,
                                        maxLimit = gpuDataPointer->maxLimit,
                                        dwFlags = gpuDataPointer->dwFlags,
                                        dwGpu = gpuDataPointer->dwGpu,
                                        dwSrcId = gpuDataPointer->dwSrcId
                                    });
                                }
                            }

                            // READ MAHM_SHARED_MEMORY_GPU_ENTRY
                            fixed (void* bufferPointer = gpuDataBuffer)
                            {
                                masmStream.Position = header.dwHeaderSize + header.dwEntrySize * header.dwNumEntries;

                                for (int i = 0; i < header.dwNumGpuEntries; i++)
                                {
                                    masmStream.Read(gpuDataBuffer, 0, gpuDataBufferSize);
                                    var gpuDataPointer = (MAHM_SHARED_MEMORY_GPU_ENTRY_UNSAFE*)bufferPointer;

                                    gpuData.Add(new MAHM_SHARED_MEMORY_GPU_ENTRY
                                    {
                                        szGpuId = new string(gpuDataPointer->szGpuId, 0, 260, Encoding.Default).Trim('\x0'),
                                        szFamily = new string(gpuDataPointer->szFamily, 0, 260, Encoding.Default).Trim('\x0'),
                                        szDevice = new string(gpuDataPointer->szDevice, 0, 260, Encoding.Default).Trim('\x0'),
                                        szDriver = new string(gpuDataPointer->szDriver, 0, 260, Encoding.Default).Trim('\x0'),
                                        szBIOS = new string(gpuDataPointer->szBIOS, 0, 260, Encoding.Default).Trim('\x0'),
                                        dwMemAmount = gpuDataPointer->dwMemAmount
                                    });
                                }
                            }
                        }
                    }
                }
            }
            catch { }

            return this;
        }

        public MASM UpdateOnce()
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
                masmStream?.Dispose();
                masmMappedFile?.Dispose();
            }
            catch { }
        }

        public string[] GetPropertiesList()
        {
            string[] propertyNames = new string[properties.Count];

            for (int i = 0; i < propertyNames.Length ; i++)
            {
                propertyNames[i] = properties[i].szSrcName;
            }

            return propertyNames;
        }

        public double GetPropertyValue(string propertyName)
        {
            return FindProperty(propertyName).data;
        }

        public MAHM_SHARED_MEMORY_ENTRY FindProperty(string propertyName)
        {
            foreach (MAHM_SHARED_MEMORY_ENTRY entry in properties)
            {
                if (entry.szSrcName == propertyName) return entry;
            }

            return new MAHM_SHARED_MEMORY_ENTRY();
        }

        public override string ToString()
        {
            string str = "Header:\r\n";
            string nl = Environment.NewLine;

            str += (header.ToString() ?? "") + nl;
            str += "GPUs:\r\n";

            foreach (var item in gpuData)
                str += (item.ToString() ?? "") + nl;

            str += "Properties:\r\n";

            foreach (var item in properties)
                str += (item.ToString() ?? "") + nl;

            return str;
        }
    }
}