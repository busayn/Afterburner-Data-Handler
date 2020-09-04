using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AfterburnerDataHandler.Servers.Logger
{
    public class LogWriter
    {
        public virtual string LogFilePath { get; protected set; }
        public virtual string LogName { get; protected set; }
        public virtual string LogDirectoryPath { get; protected set; }
        public virtual bool IsLogOpen { get; protected set; }
        protected FileStream stream;

        public virtual bool Open(string directory, string name, string extension)
        {
            if (IsLogOpen == true) Close();

            if (CreateLogPaths(name, directory, extension))
            {
                IsLogOpen = true;

                try
                {
                    if (Directory.Exists(LogDirectoryPath) == false)
                        Directory.CreateDirectory(LogDirectoryPath);

                    stream = new FileStream(LogFilePath, FileMode.Append, FileAccess.Write);
                }
                catch (Exception)
                {
                    IsLogOpen = false;
                }
            }

            return IsLogOpen;
        }

        public bool Append(string text)
        {
            if (IsLogOpen == false) return false;

            try
            {
                byte[] data = Encoding.UTF8.GetBytes(text);
                stream.Write(data, 0, data.Length);
            }
            catch
            {
                return false;
            }

            return true;
        }

        public virtual void Close()
        {
            try
            {
                stream?.Dispose();
                stream = null;
                IsLogOpen = false;
            }
            catch { }
        }

        public bool CreateLogPaths(string name, string directory, string extension)
        {
            try
            {
                LogName = CreateLogFolderName(name);
                LogDirectoryPath = Path.Combine(directory, LogName);
                string fileName = string.Format("{0} {1}.{2}", LogName, DateTime.Now.ToString("yyyy-MM-dd HH-mm-ss"), extension);
                LogFilePath = Path.Combine(LogDirectoryPath, fileName);

                if (File.Exists(LogFilePath))
                {
                    fileName = CreateUniqueFileName(LogFilePath);
                    LogFilePath = Path.Combine(LogDirectoryPath, fileName);
                }
            }
            catch { return false; }

            return true;
        }

        public static string CreateLogFolderName(string name)
        {
            if (string.IsNullOrEmpty(name) != true)
            {
                foreach (char c in Path.GetInvalidFileNameChars())
                {
                    name = name.Replace(c.ToString(), "");
                }

                name = Path.GetFileName(name);

                if (name.Length > 0) return name;
            }

            return "Nameless_Log";
        }

        public static bool IsPathValid(string path)
        {
            if (path == null) return false;

            try
            {
                Path.GetFullPath(path);
                string directory = Path.GetPathRoot(path);
                string file = Path.GetFileName(path);

                if (file.Trim() == string.Empty)
                    return false;
            }
            catch { return false; }

            return true;
        }

        public static string CreateUniqueFileName(string path)
        {
            string newPath = "";
            string fileName = Path.GetFileNameWithoutExtension(path);
            string extension = Path.GetExtension(path);
            string directory = Path.GetDirectoryName(path);
            int fileCount = Directory.GetFiles(directory).Length;

            for (int i = 0; i < fileCount; i++)
            {
                newPath = Path.Combine(directory, String.Format("{0} ({1}){2}", fileName, i + 1, extension));
                if (!File.Exists(newPath)) break;
            }

            return Path.GetFileName(newPath);
        }
    }
}
