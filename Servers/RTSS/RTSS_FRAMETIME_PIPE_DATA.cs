using System;
using System.Runtime.InteropServices;

namespace AfterburnerDataHandler.Servers.RTSS
{
    [StructLayout(LayoutKind.Explicit)]
    unsafe struct RTSS_FRAMETIME_PIPE_DATA
    {
        [FieldOffset(0)] public UInt32 dwApp;
        [FieldOffset(4)] public UInt32 dwFrametime;
    }
}
