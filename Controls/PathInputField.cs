using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using AfterburnerDataHandler.FlatControls;

namespace AfterburnerDataHandler.Controls
{
    public class PathInputField : InputField
    {
        public event EventHandler SelectedFromDialog; 

        public enum DialogType
        {
            OpenFile,
            OpenFolder,
            SaveFile
        }

        [Browsable(true), Category("Layout")]
        public virtual DialogType PathDialogType { get; set; } = DialogType.OpenFolder;

        [Browsable(true), Category("Layout")]
        public virtual int ButtonSize
        {
            get { return buttonSize; }
            set
            {
                buttonSize = value > 0 ? value : 0;
                UpdateLayout();
            }
        }

        [Browsable(true), Category("Layout")]
        public virtual int ButtonOffset
        {
            get { return buttonOffset; }
            set
            {
                buttonOffset = value > 0 ? value : 0;
                UpdateLayout();
            }
        }

        [Bindable(true)]
        [Browsable(true)]
        [EditorBrowsable(EditorBrowsableState.Always)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public override string Text
        {
            get { return this.TextBox.Text; }
            set
            {
                string newText = value.Trim(' ');

                foreach (char c in Path.GetInvalidPathChars())
                {
                    newText.Replace(c.ToString(), string.Empty);
                }

                TextBox.Text = newText;
            }
        }

        protected override Padding DefaultPadding { get { return new Padding(2, 0, 0, 0); } }

        protected override Rectangle TextBoxRect
        {
            get
            {
                Rectangle textBoxRect = base.TextBoxRect;
                textBoxRect.Width -= ButtonSize + ButtonOffset;
                return textBoxRect;
            }
        }

        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        protected virtual FlatButton PathButton
        {
            get
            {
                if (pathButton == null) pathButton = new FlatButton
                {
                    UseButtonBorder = false,
                    Padding = new Padding(4),
                    Icon = Properties.Resources.OpenFile,
                    Text = ""
                };

                return pathButton;
            }
        }

        protected FlatButton pathButton;

        private int buttonSize = 22;
        private int buttonOffset = 3;

        public PathInputField()
        {
            this.PathButton.Click += PathButtonClick;
            this.Controls.Add(PathButton);
        }

        protected virtual void OnSelectedFromDialog(EventArgs e)
        {
            SelectedFromDialog?.Invoke(this, e);
        }

        public override void UpdateTheme()
        {
            base.UpdateTheme();

            PathButton.UseGlobalTheme = false;
            PathButton.Theme = this.Theme;
            PathButton.UpdateTheme();
        }

        protected override void UpdateLayout()
        {
            base.UpdateLayout();

            Rectangle viewRect = this.ClientRectangle;
            Rectangle textBoxRect = TextBoxRect;
            Padding viewPadding = this.Padding;
            int lineSize = BorderSize;
            int buttonWidth = ButtonSize;

            if (PathButton != null)
            {
                PathButton.Size = new Size(
                    buttonWidth,
                    viewRect.Height - viewPadding.Vertical - lineSize);

                PathButton.Location = new Point(
                    textBoxRect.Right + ButtonOffset,
                    viewPadding.Top);

                pathButton.IconSize = buttonWidth;
            }
        }

        protected override void OnLeave(EventArgs e)
        {
            base.OnLeave(e);

            TextBox.SelectionStart = 0;
            TextBox.SelectionLength = 0;
        }

        private void PathButtonClick(object sender, EventArgs e)
        {
            switch (PathDialogType)
            {
                case DialogType.OpenFile:
                    using (OpenFileDialog dialog = new OpenFileDialog())
                    {
                        try
                        {
                            dialog.InitialDirectory = Path.GetDirectoryName(this.Text);
                            dialog.FileName = Path.GetFileName(this.Text);
                        }
                        catch { }

                        if (dialog.ShowDialog() == DialogResult.OK)
                        {
                            this.Text = dialog.FileName;
                            OnSelectedFromDialog(EventArgs.Empty);
                        }
                    }
                    break;

                case DialogType.OpenFolder:
                    using (FolderBrowserDialog dialog = new FolderBrowserDialog())
                    {
                        dialog.SelectedPath = this.Text;

                        if (dialog.ShowDialog() == DialogResult.OK)
                        {
                            this.Text = dialog.SelectedPath;
                            OnSelectedFromDialog(EventArgs.Empty);
                        }
                    }
                    break;

                case DialogType.SaveFile:
                    using (SaveFileDialog dialog = new SaveFileDialog())
                    {
                        try
                        {
                            dialog.InitialDirectory = Path.GetDirectoryName(this.Text);
                            dialog.FileName = Path.GetFileName(this.Text);
                        }
                        catch { }
                        
                        if (dialog.ShowDialog() == DialogResult.OK)
                        {
                            this.Text = dialog.FileName;
                            OnSelectedFromDialog(EventArgs.Empty);
                        }
                    }
                    break;
            }
        }
    }
}
