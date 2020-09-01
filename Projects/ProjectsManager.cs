using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AfterburnerDataHandler.Servers.Serial;
using AfterburnerDataHandler.Servers.Logger;

namespace AfterburnerDataHandler.Projects
{
    class ProjectsManager
    {
        public static SerialPortServer SerialPortServer { get; set; }
        public static LoggerServer LoggerServer { get; set; }

        public static bool IsSerialPortServerSaved
        {
            get
            {
                return !SerialPortServer?.Settings?.IsDirty ?? true;
            }
        }

        public static bool IsLoggerServerSaved
        {
            get
            {
                return !LoggerServer?.Settings?.IsDirty ?? true;
            }
        }

        public static bool LoadLastSerialPortProject()
        {
            string projectPath = Properties.Settings.Default.SerialPort_LastProject;

            if (File.Exists(projectPath))
            {
                SerialPortProject newProject = null;

                ProjectsUtils.LoadProject(projectPath, ref newProject);

                if (SerialPortServer != null && ProjectsUtils.LoadProject(projectPath, ref newProject))
                {
                    SerialPortServer.Settings = newProject;
                    return true;
                }
            }

            return false;
        }

        public static bool LoadLastLoggerProject()
        {
            string projectPath = Properties.Settings.Default.Logger_LastProject;

            if (File.Exists(projectPath))
            {
                LoggerProject newProject = null;

                ProjectsUtils.LoadProject(projectPath, ref newProject);

                if (LoggerServer != null && ProjectsUtils.LoadProject(projectPath, ref newProject))
                {
                    LoggerServer.Settings = newProject;
                    return true;
                }
            }

            return false;
        }
    }
}
