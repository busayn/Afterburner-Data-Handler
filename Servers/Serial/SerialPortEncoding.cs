using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AfterburnerDataHandler.Servers.Serial
{
    public enum SerialPortEncoding
    {
        ASCII= 0,
        Unicode = 1,
        BigEndianUnicode = 2,
        UTF32 = 3,
        UTF8 = 4,
        UTF7 = 5
    }

    public static class SerialPortEncodingExtension
    {
        public static Encoding ToTextEncoding(this SerialPortEncoding spe)
        {
            switch (spe)
            {
                case SerialPortEncoding.ASCII: return Encoding.ASCII;
                case SerialPortEncoding.Unicode: return Encoding.Unicode;
                case SerialPortEncoding.BigEndianUnicode: return Encoding.BigEndianUnicode;
                case SerialPortEncoding.UTF32: return Encoding.UTF32;
                case SerialPortEncoding.UTF8: return Encoding.UTF8;
                case SerialPortEncoding.UTF7: return Encoding.UTF7;
                default: return Encoding.Default;
            }
        }
    }
}
