using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;
using System.Windows.Forms;
using AfterburnerDataHandler.SharedMemory.Afterburner;
using AfterburnerDataHandler.FlatControls;

namespace AfterburnerDataHandler.Controls
{
    public partial class MASMFormattingItemEditor : ThemedControl
    {
        public virtual int SidebarSize
        {
            get { return sidebarSize; }
            set
            {
                sidebarSize = value > 0 ? value : 0;
                UpdateLayout();
            }
        }

        public virtual int LabelSize
        {
            get { return labelSize; }
            set
            {
                labelSize = value > 0 ? value : 0;
                UpdateLayout();
            }
        }

        public virtual int ControlsSize
        {
            get { return controlsSize; }
            set
            {
                controlsSize = value > 0 ? value : 0;
                UpdateLayout();
            }
        }

        public virtual int ColumnsPadding
        {
            get { return columnsPadding; }
            set
            {
                columnsPadding = value > 0 ? value : 0;
                UpdateLayout();
            }
        }

        [Browsable(false)]
        public virtual Color ControlBackgroundColor
        {
            get { return panelBackgroundColor; }
            protected set
            {
                panelBackgroundColor = value;
                OnThemeDataChanged(EventArgs.Empty);
            }
        }

        public Dropdown ModeField { get; protected set; }
        public DropdownInputField PropertyField { get; protected set; }
        public InputField PrefixField { get; protected set; }
        public InputField PostfixField { get; protected set; }
        public Dropdown TargetField { get; protected set; }
        public Dropdown OperationField { get; protected set; }
        public InputField ValueField { get; protected set; }
        public Dropdown RoundField { get; protected set; }
        public Toggle ModuloToggle { get; protected set; }
        public InputField OutFormatField { get; protected set; }

        public FlatButton MoveUpButton { get; protected set; }
        public FlatButton MoveDownButton { get; protected set; }
        public FlatButton CopyButton { get; protected set; }
        public FlatButton RemoveButton { get; protected set; }
        public Toggle EnableToggle { get; protected set; }

        public MASMFormattingEditor.StringFormattingCollection TargetCollection
        {
            get { return targetCollection; }
            set
            {
                targetCollection = value;
                UpdateValues();
            }
        }

        public int Index
        {
            get { return index; }
            set
            {
                index = value;
                UpdateValues();
            }
        }

        public Func<string[]> AvailableProperties;

        protected virtual Rectangle ParametersRect
        {
            get
            {
                Rectangle view = this.ClientRectangle;
                return new Rectangle(
                    view.X + SidebarSize,
                    view.Y,
                    view.Width - SidebarSize * 2,
                    view.Height);
            }
        }

        protected virtual Rectangle LeftSidebarRect
        {
            get
            {
                Rectangle view = this.ClientRectangle;
                return new Rectangle(
                    view.X,
                    view.Y,
                    SidebarSize,
                    view.Height);
            }
        }

        protected virtual Rectangle RightSidebarRect
        {
            get
            {
                Rectangle view = this.ClientRectangle;
                return new Rectangle(
                    view.Right - SidebarSize,
                    view.Y,
                    SidebarSize,
                    view.Height);
            }
        }

        protected struct LabelData
        {
            public string text;
            public Rectangle bounds;

            public LabelData(string text, Rectangle bounds)
            {
                this.text = text ?? "";
                this.bounds = bounds;
            }
        }

        protected virtual List<LabelData> Labels
        {
            get
            {
                if (labels == null)
                    labels = new List<LabelData>();
                
                return labels;
            }
        }

        protected override Size DefaultSize { get { return new Size(300, 116); } }
        protected override Padding DefaultMargin { get { return new Padding(0, 6, 0, 6); } }
        protected override Padding DefaultPadding { get { return new Padding(6, 3, 6, 3); } }
        protected override Theme.BackgroundSource DefaultBackgroundSource { get { return Theme.BackgroundSource.Theme; } }

        private int index;
        private MASMFormattingEditor.StringFormattingCollection targetCollection;

        private int sidebarSize = 40;
        private int controlsSize = 32;
        private int labelSize = 80;
        private int columnsPadding = 4;
        private Rectangle parametersRect;
        private Rectangle leftSidebarRect;
        private Rectangle rightSidebarRect;
        private Color panelBackgroundColor;
        private List<LabelData> labels = new List<LabelData>();

        private TextFormatFlags indexLabelTextFormat = TextFormatFlags.TextBoxControl
                                                     | TextFormatFlags.VerticalCenter
                                                     | TextFormatFlags.HorizontalCenter
                                                     | TextFormatFlags.SingleLine;

        private TextFormatFlags labelsTextFormat = TextFormatFlags.TextBoxControl
                                                 | TextFormatFlags.VerticalCenter
                                                 | TextFormatFlags.Left
                                                 | TextFormatFlags.WordBreak
                                                 | TextFormatFlags.EndEllipsis;

        public MASMFormattingItemEditor()
        {
            SetStyle(ControlStyles.Selectable, false);
            SetStyle(ControlStyles.ContainerControl, true);
            InitializeControls();
            InitializeHandles();
        }

        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);
            SetStyle(ControlStyles.Selectable, false);
            SetStyle(ControlStyles.ContainerControl, true);
        }

        protected virtual void InitializeControls()
        {
            this.SuspendLayout();

            this.MoveUpButton = new FlatButton
            {
                UseButtonBorder = false,
                Icon = Properties.Resources.ArrowUp,
                IconMultiplyColor = this.Theme.TextColor

            };
            this.Controls.Add(MoveUpButton);

            this.MoveDownButton = new FlatButton
            {
                UseButtonBorder = false,
                Icon = Properties.Resources.ArrowDown,
                IconMultiplyColor = this.Theme.TextColor
            };
            this.Controls.Add(MoveDownButton);

            this.ModeField = new Dropdown
            {

            };
            this.ModeField.FromEnum(MASM_FormattingItem.ItemMode.Property);
            this.Controls.Add(ModeField);

            this.PropertyField = new DropdownInputField
            {

            };
            this.Controls.Add(PropertyField);

            this.PrefixField = new InputField
            {

            };
            this.Controls.Add(PrefixField);

            this.PostfixField = new InputField
            {

            };
            this.Controls.Add(PostfixField);


            this.TargetField = new Dropdown
            {

            };
            this.TargetField.FromEnum(MASM_FormattingItem.TargetData.PropertyValue);
            this.Controls.Add(TargetField);

            this.OperationField = new Dropdown
            {

            };
            this.OperationField.FromEnum(MASM_FormattingItem.OperationType.None);
            this.Controls.Add(OperationField);

            this.ValueField = new InputField
            {

            };
            this.Controls.Add(ValueField);

            this.RoundField = new Dropdown
            {

            };
            this.RoundField.FromEnum(MASM_FormattingItem.RoundingType.None);
            this.Controls.Add(RoundField);

            this.ModuloToggle = new Toggle
            {

            };
            this.Controls.Add(ModuloToggle);

            this.OutFormatField = new InputField
            {

            };
            this.Controls.Add(OutFormatField);

            this.EnableToggle = new Toggle
            {
                BackgroundSource = Theme.BackgroundSource.Theme
            };
            this.Controls.Add(EnableToggle);

            this.CopyButton = new FlatButton
            {
                UseButtonBorder = false,
                Icon = Properties.Resources.Copy,
                IconMultiplyColor = this.Theme.TextColor
            };
            this.Controls.Add(CopyButton);

            this.RemoveButton = new FlatButton
            {
                UseButtonBorder = false,
                Icon = Properties.Resources.Remove,
                IconMultiplyColor = Color.FromArgb(240, 60, 40)
            };
            this.Controls.Add(RemoveButton);

            this.ResumeLayout();
            this.UpdateLayout();
        }

        public virtual void InitializeHandles()
        {
            MoveUpButton.Click += new EventHandler(ItemParameterChanged);
            MoveDownButton.Click += new EventHandler(ItemParameterChanged);
            EnableToggle.Click += new EventHandler(ItemParameterChanged);
            CopyButton.Click += new EventHandler(ItemParameterChanged);
            RemoveButton.Click += new EventHandler(ItemParameterChanged);
            PrefixField.Leave += new EventHandler(ItemParameterChanged);
            PostfixField.Leave += new EventHandler(ItemParameterChanged);
            PropertyField.Leave += new EventHandler(ItemParameterChanged);
            PropertyField.ItemSelected += new EventHandler<ItemEventArgs>(ItemParameterChanged);
            PropertyField.DropDown += PropertyFieldOpen;
            ModeField.ItemSelected += new EventHandler<ItemEventArgs>(ItemParameterChanged);
            OperationField.ItemSelected += new EventHandler<ItemEventArgs>(ItemParameterChanged);
            ValueField.Leave += new EventHandler(ItemParameterChanged);
            TargetField.ItemSelected += new EventHandler<ItemEventArgs>(ItemParameterChanged);
            ModuloToggle.Click += new EventHandler(ItemParameterChanged);
            RoundField.ItemSelected += new EventHandler<ItemEventArgs>(ItemParameterChanged);
            OutFormatField.Leave += new EventHandler(ItemParameterChanged);
        }

        public virtual void UpdateValues()
        {
            if (TargetCollection == null || targetCollection.Count < 1
                || Index < 0 || Index >= targetCollection.Count) return;

            UpdateFieldsAvailability();

            EnableToggle.Checked = TargetCollection[Index].enable;
            ModeField.SelectedItem = TargetCollection[Index].mode;
            PropertyField.Text = TargetCollection[Index].property;
            PrefixField.Text = TargetCollection[Index].prefix;
            PostfixField.Text = TargetCollection[Index].postfix;
            TargetField.SelectedItem = TargetCollection[Index].targetData;
            OperationField.SelectedItem = TargetCollection[Index].operationType;
            ValueField.Text = float.IsNaN(TargetCollection[Index].operationValue)
                ? "0" : TargetCollection[Index].operationValue.ToString("0.#######", CultureInfo.InvariantCulture);
            RoundField.SelectedItem = TargetCollection[Index].roundMode;
            ModuloToggle.Checked = TargetCollection[Index].modulo;
            OutFormatField.Text = TargetCollection[Index].outFormat;
        }

        public virtual void UpdateFieldsAvailability()
        {
            if (TargetCollection == null || targetCollection.Count < 1
                || Index < 0 || Index >= targetCollection.Count) return;

            ModeField.Enabled = TargetCollection[Index].enable;

            PropertyField.Enabled = TargetCollection[Index].enable
                && TargetCollection[Index].mode != MASM_FormattingItem.ItemMode.Time;

            PrefixField.Enabled = TargetCollection[Index].enable;

            PostfixField.Enabled = TargetCollection[Index].enable;

            TargetField.Enabled = TargetCollection[Index].enable
                && TargetCollection[Index].mode == MASM_FormattingItem.ItemMode.Property;

            OperationField.Enabled = TargetCollection[Index].enable
                && TargetCollection[Index].mode == MASM_FormattingItem.ItemMode.Property
                && (int)TargetCollection[Index].targetData < 3;

            ValueField.Enabled = TargetCollection[Index].enable
                && TargetCollection[Index].mode == MASM_FormattingItem.ItemMode.Property
                && (int)TargetCollection[Index].targetData < 3;

            RoundField.Enabled = TargetCollection[Index].enable
                && TargetCollection[Index].mode == MASM_FormattingItem.ItemMode.Property
                && (int)TargetCollection[Index].targetData < 3;

            ModuloToggle.Enabled = TargetCollection[Index].enable
                && TargetCollection[Index].mode == MASM_FormattingItem.ItemMode.Property
                && (int)TargetCollection[Index].targetData < 3;

            OutFormatField.Enabled = TargetCollection[Index].enable
                && TargetCollection[Index].mode == MASM_FormattingItem.ItemMode.Property
                || TargetCollection[Index].mode == MASM_FormattingItem.ItemMode.Time;
        }

        private void ItemParameterChanged(object sender, EventArgs e)
        {
            if (TargetCollection == null || targetCollection.Count < 1
                || Index < 0 || Index >= targetCollection.Count) return;

            MASM_FormattingItem updatedValues = TargetCollection[Index];

            updatedValues.enable = EnableToggle.Checked;
            updatedValues.prefix = PrefixField.Text;
            updatedValues.postfix = PostfixField.Text;
            updatedValues.property = PropertyField.Text;
            updatedValues.mode = ModeField.ToEnum(MASM_FormattingItem.ItemMode.Property);
            updatedValues.operationValue = ParseFloat(ValueField.Text);
            updatedValues.operationType = OperationField.ToEnum(MASM_FormattingItem.OperationType.None);
            updatedValues.targetData = TargetField.ToEnum(MASM_FormattingItem.TargetData.PropertyValue);
            updatedValues.roundMode = RoundField.ToEnum(MASM_FormattingItem.RoundingType.None);
            updatedValues.modulo = ModuloToggle.Checked;
            updatedValues.outFormat = OutFormatField.Text;

            if (TargetCollection[Index] != updatedValues)
            {
                TargetCollection[Index] = updatedValues;
            }

            if (sender == CopyButton)
            {
                TargetCollection.Insert(index + 1, TargetCollection[Index]);
            }

            if (sender == MoveUpButton && index - 1 > -1)
            {
                TargetCollection.Move(index, index - 1);
            }

            if (sender == MoveDownButton && index + 1 < TargetCollection.Count)
            {
                TargetCollection.Move(index, index + 1);
            }

            if (sender == RemoveButton
                && MessageBox.Show("Are you sure to delete item " + index + "?",
                    "Confirm Delete!", MessageBoxButtons.OKCancel) == DialogResult.OK)
            {
                TargetCollection.RemoveAt(index);
                return;
            }
        }

        public virtual void UpdateLayout()
        {
            parametersRect = this.ParametersRect;
            leftSidebarRect = this.LeftSidebarRect;
            rightSidebarRect = this.RightSidebarRect;

            Padding parametersPadding = this.Padding;
            Rectangle contentRect = new Rectangle(
                parametersRect.X + parametersPadding.Left,
                parametersRect.Y + parametersPadding.Top,
                parametersRect.Width - parametersPadding.Horizontal,
                parametersRect.Height - parametersPadding.Vertical);

            int labelWidth = this.LabelSize;
            int columnsPadding = this.ColumnsPadding;

            float lineHeight = contentRect.Height / 3f;

            int firstLineLocation = contentRect.Top;
            int secondLineLocation = contentRect.Top + (int)lineHeight;
            int thirdLinenLocation = secondLineLocation + (int)lineHeight;

            int totalWidth = contentRect.Width - columnsPadding * 2;
            float firstColumnWidth = totalWidth * 0.4f;
            float secondColumnsWidth = totalWidth * 0.3f;

            int firstColumnLocation = contentRect.Left;
            int secondColumnLocation = contentRect.Left + (int)firstColumnWidth + columnsPadding;
            int thirdColumnLocation = secondColumnLocation + (int)secondColumnsWidth + columnsPadding;

            this.SuspendLayout();
            Labels.Clear();

            Labels.Add(new LabelData(
                "Mode",
                new Rectangle(
                    firstColumnLocation,
                    firstLineLocation,
                    labelWidth,
                    (int)lineHeight)));

            FitControlToBounds(this.ModeField, new Rectangle(
                firstColumnLocation + labelWidth,
                firstLineLocation,
                (int)firstColumnWidth - labelWidth,
                (int)lineHeight));

            Labels.Add(new LabelData(
                "Property",
                new Rectangle(
                    firstColumnLocation,
                    secondLineLocation,
                    labelWidth,
                    (int)lineHeight)));

            FitControlToBounds(this.PropertyField, new Rectangle(
                firstColumnLocation + labelWidth,
                secondLineLocation,
                (int)firstColumnWidth - labelWidth,
                (int)lineHeight));

            int subcolumnWidth = (int)((firstColumnWidth - columnsPadding) / 2);

            Labels.Add(new LabelData(
                "Prefix",
                new Rectangle(
                    firstColumnLocation,
                    thirdLinenLocation,
                    labelWidth,
                    (int)lineHeight)));

            FitControlToBounds(this.PrefixField, new Rectangle(
                firstColumnLocation + labelWidth,
                thirdLinenLocation,
                subcolumnWidth - labelWidth,
                (int)lineHeight));

            Labels.Add(new LabelData(
                "Postfix",
                new Rectangle(
                    firstColumnLocation + subcolumnWidth + columnsPadding,
                    thirdLinenLocation,
                    labelWidth,
                    (int)lineHeight)));

            FitControlToBounds(this.PostfixField, new Rectangle(
                (int)(firstColumnLocation + firstColumnWidth + labelWidth - subcolumnWidth),
                thirdLinenLocation,
                (int)(subcolumnWidth - labelWidth),
                (int)lineHeight));

            Labels.Add(new LabelData(
                "Target Data",
                new Rectangle(
                    secondColumnLocation,
                    firstLineLocation,
                    labelWidth,
                    (int)lineHeight)));

            FitControlToBounds(this.TargetField, new Rectangle(
                secondColumnLocation + labelWidth,
                firstLineLocation,
                (int)secondColumnsWidth - labelWidth,
                (int)lineHeight));

            Labels.Add(new LabelData(
                "Operation",
                new Rectangle(
                    secondColumnLocation,
                    secondLineLocation,
                    labelWidth,
                    (int)lineHeight)));

            FitControlToBounds(this.OperationField, new Rectangle(
                secondColumnLocation + labelWidth,
                secondLineLocation,
                (int)secondColumnsWidth - labelWidth,
                (int)lineHeight));

            Labels.Add(new LabelData(
                "Operation Value",
                new Rectangle(
                    secondColumnLocation,
                    thirdLinenLocation,
                    labelWidth,
                    (int)lineHeight)));

            FitControlToBounds(this.ValueField, new Rectangle(
                secondColumnLocation + labelWidth,
                thirdLinenLocation,
                (int)secondColumnsWidth - labelWidth,
                (int)lineHeight));

            Labels.Add(new LabelData(
                "Round",
                new Rectangle(
                    thirdColumnLocation,
                    firstLineLocation,
                    labelWidth,
                    (int)lineHeight)));

            FitControlToBounds(this.RoundField, new Rectangle(
                thirdColumnLocation + labelWidth,
                firstLineLocation,
                (int)secondColumnsWidth - labelWidth,
                (int)lineHeight));

            Labels.Add(new LabelData(
                "Modulo",
                new Rectangle(
                    thirdColumnLocation,
                    secondLineLocation,
                    labelWidth,
                    (int)lineHeight)));

            FitControlToBounds(this.ModuloToggle, new Rectangle(
                thirdColumnLocation + labelWidth,
                secondLineLocation,
                (int)secondColumnsWidth - labelWidth,
                (int)lineHeight));

            Labels.Add(new LabelData(
                "Out Format",
                new Rectangle(
                    thirdColumnLocation,
                    thirdLinenLocation,
                    labelWidth,
                    (int)lineHeight)));

            FitControlToBounds(this.OutFormatField, new Rectangle(
                thirdColumnLocation + labelWidth,
                thirdLinenLocation,
                (int)secondColumnsWidth - labelWidth,
                (int)lineHeight));

            int controlsSize = this.ControlsSize;
            Padding arrowPadding = new Padding(6);
            Padding iconPadding = new Padding(6);

            this.MoveUpButton.Padding = arrowPadding;
            this.MoveUpButton.IconSize = sidebarSize;
            this.MoveUpButton.Bounds = new Rectangle(
                leftSidebarRect.X,
                leftSidebarRect.Y,
                leftSidebarRect.Width,
                controlsSize);

            this.MoveDownButton.Padding = arrowPadding;
            this.MoveDownButton.IconSize = sidebarSize;
            this.MoveDownButton.Bounds = new Rectangle(
                leftSidebarRect.X,
                leftSidebarRect.Y + leftSidebarRect.Height - controlsSize,
                leftSidebarRect.Width,
                controlsSize);

            this.CopyButton.Padding = iconPadding;
            this.CopyButton.IconSize = controlsSize;
            this.CopyButton.Bounds = new Rectangle(
                rightSidebarRect.X,
                rightSidebarRect.Y + rightSidebarRect.Height - controlsSize * 2,
                rightSidebarRect.Width,
                controlsSize);

            this.RemoveButton.Padding = iconPadding;
            this.RemoveButton.IconSize = controlsSize;
            this.RemoveButton.Bounds = new Rectangle(
                rightSidebarRect.X,
                rightSidebarRect.Y + rightSidebarRect.Height - controlsSize,
                rightSidebarRect.Width,
                controlsSize);

            this.EnableToggle.Bounds = new Rectangle(
                rightSidebarRect.X + (sidebarSize - this.EnableToggle.CheckmarkHeight) / 2,
                rightSidebarRect.Y + (sidebarSize - this.EnableToggle.CheckmarkHeight) / 2,
                this.EnableToggle.CheckmarkHeight,
                this.EnableToggle.CheckmarkHeight);

            this.ResumeLayout();
        }

        protected virtual void PropertyFieldOpen(object sender, EventArgs e)
        {
            if (sender is DropdownInputField && AvailableProperties != null)
            {
                DropdownInputField dropdown = sender as DropdownInputField;
                string[] properties = AvailableProperties?.Invoke();

                if (properties == null) return;

                //dropdown.Focus();
                dropdown.Items.Clear();
                dropdown.Items.AddRange(properties);
            }
        }

        public override void UpdateTheme()
        {
            base.UpdateTheme();
            this.ControlBackgroundColor = this.Theme.ControlBackgroundColor;
            this.BackgroundColor = this.Theme.PanelBackgroundColor;
        }

        protected override void OnPaintBackground(PaintEventArgs e)
        {
            DrawingUtils.FillRectangle(e.Graphics, this.BackgroundColor, e.ClipRectangle);
            DrawingUtils.FillRectangle(e.Graphics, this.ControlBackgroundColor, this.leftSidebarRect);
            DrawingUtils.FillRectangle(e.Graphics, this.ControlBackgroundColor, this.rightSidebarRect);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            TextRenderer.DrawText(
                    e.Graphics,
                    Index.ToString(),
                    this.Font,
                    this.LeftSidebarRect,
                    this.TextColor,
                    this.indexLabelTextFormat);

            foreach (LabelData label in this.Labels)
            {
                if (string.IsNullOrEmpty(label.text) == true) continue;

                TextRenderer.DrawText(
                    e.Graphics,
                    label.text,
                    this.Font,
                    label.bounds,
                    this.TextColor,
                    this.labelsTextFormat);
            }
        }

        protected override void OnLayout(LayoutEventArgs e)
        {
            base.OnLayout(e);
            UpdateLayout();
        }

        private void FitControlToBounds(Control c, Rectangle bounds)
        {
            c.Size = new Size(bounds.Width, c.Height);
            c.Location = new Point(bounds.X, bounds.Y + (bounds.Height - c.Bounds.Height) / 2);
        }

        private float ParseFloat(string text)
        {
            float result = float.NaN;
            if (string.IsNullOrEmpty(text)) return result;

            text = text.Replace(',', '.');

            try
            {
                result = Convert.ToSingle(text, CultureInfo.InvariantCulture.NumberFormat);
            }
            catch { }

            return result;
        }
    }
}