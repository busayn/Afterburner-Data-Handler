using System.ComponentModel;

namespace AfterburnerDataHandler.Servers.SerialPort
{
    public enum BaudRate : int
    {
        [Description("75")]
        Baudrate75 = 75,

        [Description("150")]
        Baudrate150 = 150,

        [Description("300")]
        Baudrate300 = 300,

        [Description("600")]
        Baudrate600 = 600,

        [Description("1200")]
        Baudrate1200 = 1200,

        [Description("2400")]
        Baudrate2400 = 2400,

        [Description("4800")]
        Baudrate4800 = 4800,

        [Description("9600")]
        Baudrate9600 = 9600,

        [Description("19200")]
        Baudrate19200 = 19200,

        [Description("38400")]
        Baudrate38400 = 38400,

        [Description("57600")]
        Baudrate57600 = 57600,

        [Description("115200")]
        Baudrate115200 = 115200,

        [Description("230400")]
        Baudrate230400 = 230400
    }
}
