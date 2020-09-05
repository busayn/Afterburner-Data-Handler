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
                string targetDirectory = Path.GetFullPath(directory);
                string targetName = CreateLogFolderName(name);
                string targetExtension = string.IsNullOrWhiteSpace(extension) ? "txt" : extension;

                if (Path.IsPathRooted(targetDirectory) == false) return false;

                string newLogDirectoryPath = Path.Combine(targetDirectory, targetName);
                string newLogFileName = string.Format("{0} {1}.{2}", targetName, DateTime.Now.ToString("yyyy-MM-dd HH-mm-ss"), extension);
                string newLogFilePath = Path.Combine(newLogDirectoryPath, newLogFileName);

                if (File.Exists(newLogFilePath))
                {
                    newLogFileName = CreateUniqueFileName(newLogFilePath);
                    newLogFilePath = Path.Combine(newLogDirectoryPath, newLogFileName);
                }

                LogDirectoryPath = newLogDirectoryPath;
                LogName = newLogFileName;
                LogFilePath = newLogFilePath;
            }
            catch { return false; }

            return true;
        }

        public static string CreateLogFolderName(string name)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(name) != true)
                {
                    foreach (char c in Path.GetInvalidFileNameChars())
                    {
                        name = name.Replace(c.ToString(), "");
                    }

                    name = Path.GetFileName(name);

                    if (name.Length > 0) return name;
                }
            }
            catch { }

            return "NamelessLog";
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
