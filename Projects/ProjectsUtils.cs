using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using AfterburnerDataHandler.Serialisation;

namespace AfterburnerDataHandler.Projects
{
    public class ProjectsUtils
    {
        public static string ShowOpenProjectDialog<T>(ref T project) where T : BaseProject, new()
        {
            T loadedProject = new T();
            string projectTypeName = GetTypeName<T>();
            OpenFileDialog openFileDialog = new OpenFileDialog();

            openFileDialog.Filter = string.Format(
                "{0} (*.{1})|*.{2}|All files (*.*)|*.*",
                projectTypeName,
                loadedProject.ProjectFormat?.ToUpper() ?? string.Empty,
                loadedProject.ProjectFormat ?? string.Empty);

            openFileDialog.Title = "Open " + projectTypeName;

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                string targetPath = openFileDialog.FileName;

                if (LoadProject(targetPath, ref loadedProject))
                {
                    project = loadedProject;
                    openFileDialog?.Dispose();
                    return targetPath;
                }
                else
                {
                    MessageBox.Show("Invalid project file.", "Error", MessageBoxButtons.OK);
                }
            }

            if (loadedProject is IDisposable)
                (loadedProject as IDisposable).Dispose();

            openFileDialog?.Dispose();

            return null;
        }

        public static string ShowSaveProjectDialog<T>(T project) where T : BaseProject, new()
        {
            if (project == null) return null;

            string projectTypeName = GetTypeName<T>();
            SaveFileDialog saveFileDialog = new SaveFileDialog();

            saveFileDialog.Filter = string.Format(
                "{0} (*.{1})|*.{2}|All files (*.*)|*.*",
                projectTypeName,
                project.ProjectFormat?.ToUpper() ?? string.Empty,
                project.ProjectFormat ?? string.Empty);

            saveFileDialog.Title = "Save " + projectTypeName;
            saveFileDialog.FileName = project.ProjectName ?? string.Empty;

            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                string targetPath = saveFileDialog.FileName;

                if (SaveProject(saveFileDialog.FileName, project))
                {
                    saveFileDialog?.Dispose();
                    return targetPath;
                }
                else
                {
                    MessageBox.Show("Invalid project file.", "Error", MessageBoxButtons.OK);
                }
            }

            saveFileDialog?.Dispose();

            return null;
        }

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
                    target.SetDirty(false, true);
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
                project.SetDirty(false, true);
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

        private static string GetTypeName<T>()
        {
            string typeName = typeof(T).Name;
            StringBuilder nameBuilder = new StringBuilder(typeName.Length);

            for (int i = 0; i < typeName.Length; i++)
            {
                if (i > 0 && char.IsUpper(typeName[i]) && typeName[i - 1] != ' ')
                {
                    nameBuilder.Append(' ');
                }
                
                nameBuilder.Append(typeName[i]);
            }

            return nameBuilder.ToString();
        }
    }
}
