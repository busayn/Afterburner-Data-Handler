using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AfterburnerDataHandler.Filesystem
{
    public class FileUtils
    {
        public static string CreateCorrectFileName(string fileName)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(fileName) != true)
                {
                    foreach (char c in Path.GetInvalidFileNameChars())
                    {
                        fileName = fileName.Replace(c.ToString(), "");
                    }

                    fileName = Path.GetFileName(fileName);

                    if (fileName.Length > 0)
                        return fileName;
                }
            }
            catch { }

            return string.Empty;
        }

        public static bool ValidateFileName(string name)
        {
            if (string.IsNullOrWhiteSpace(name) != true)
            {
                foreach (char c in Path.GetInvalidFileNameChars())
                {
                    if (name.Contains(c.ToString()))
                        return false;
                }
            }

            return true;
        }

        public static bool ValidatePath(string path)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(path))
                    return false;

                Path.GetFullPath(path);
                Path.IsPathRooted(path);
            }
            catch { return false; }

            return true;
        }
    }
}
