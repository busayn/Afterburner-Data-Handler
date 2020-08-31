using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AfterburnerDataHandler.Serialisation;

namespace AfterburnerDataHandler.Projects
{
    public class ProjectsUtils
    {
        public static bool LoadProject<T>(string path, ref T target) where T : BaseProject, new()
        {
            try
            {
                if (File.Exists(path) == false)
                    return false;

                string data = File.ReadAllText(path, Encoding.UTF8);
                T newProject;

                if (TryParceProject(data, out newProject))
                {
                    target = newProject;
                    target.ProjectName = Path.GetFileNameWithoutExtension(path);
                    target.IsDirty = false;
                }
                else { return false; }
            }
            catch { return false; }

            return true;
        }

        public static bool SaveProject<T>(string path, T project) where T : BaseProject
        {
            try
            {
                string data = SerializeProject(project);

                File.WriteAllText(path, data, Encoding.UTF8);

                project.ProjectName = Path.GetFileNameWithoutExtension(path);
                project.IsDirty = false;
            }
            catch { return false; }

            return true;
        }

        public static bool TryParceProject<T>(string data, out T project) where T : BaseProject, new()
        {
            T sampleProject = new T();

            try
            {
                T parsedProject = XmlSerialization.FromXMLString<T>(data);

                if (parsedProject.ProjectFormat == sampleProject.ProjectFormat)
                {
                    project = parsedProject;
                }
                else
                {
                    project = sampleProject;
                    return false;
                }
            }
            catch
            {
                project = sampleProject;
                return false;
            }

            return true;
        }

        public static string SerializeProject<T>(T project) where T : BaseProject
        {
            return XmlSerialization.ToXMLString(project);
        }
    }
}
