using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.Layout;

namespace AfterburnerDataHandler.FlatControls
{
    public partial class PropertyContainer : ContentPanel
    {
        [Bindable(true)]
        [Browsable(true)]
        [EditorBrowsable(EditorBrowsableState.Always)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public override string Text { get { return base.Text; } set { base.Text = value; } }

        [Browsable(true), Category("Appearance")]
        public bool FitContent
        {
            get
            {
                if (LayoutEngine is PropertyContainerLayoutEngine)
                    return (LayoutEngine as PropertyContainerLayoutEngine).FitContent;
                else
                    return false;
            }
            set
            {
                if (LayoutEngine is PropertyContainerLayoutEngine)
                    (LayoutEngine as PropertyContainerLayoutEngine).FitContent = value;

                this.UpdateLayout();
                this.PerformLayout();
            }
        }

        [Browsable(true), Category("Appearance")]
        public bool AbsoluteLabelWidth
        {
            get
            {
                if (LayoutEngine is PropertyContainerLayoutEngine)
                    return (LayoutEngine as PropertyContainerLayoutEngine).AbsoluteLabelWidth;
                else
                    return false;
            }
            set
            {
                if (LayoutEngine is PropertyContainerLayoutEngine)
                    (LayoutEngine as PropertyContainerLayoutEngine).AbsoluteLabelWidth = value;

                this.UpdateLayout();
                this.PerformLayout();
            }
        }

        [Browsable(true), Category("Appearance")]
        public HorizontalAlignment ControlsAlignment
        {
            get
            {
                if (LayoutEngine is PropertyContainerLayoutEngine)
                    return (LayoutEngine as PropertyContainerLayoutEngine).ControlsAlignment;
                else
                    return HorizontalAlignment.Left;
            }
            set
            {
                if (LayoutEngine is PropertyContainerLayoutEngine)
                    (LayoutEngine as PropertyContainerLayoutEngine).ControlsAlignment = value;

                this.UpdateLayout();
                this.PerformLayout();
            }
        }

        [Browsable(true), Category("Appearance")]
        public ContentAlignment LabelAlignment
        {
            get { return labelAlignment; }
            set
            {
                labelAlignment = value;
                this.UpdateLayout();
            }
        }

        [Browsable(true), Category("Appearance")]
        public bool Multiline
        {
            get { return multiline; }
            set
            {
                multiline = value;
                this.UpdateLayout();
            }
        }

        [Browsable(true), Category("Appearance")]
        public int Indent
        {
            get { return indent; }
            set
            {
                indent = value;
                this.UpdateLayout();
            }
        }

        [Browsable(true), Category("Appearance")]
        public int IndentSize
        {
            get { return indentSize; }
            set
            {
                indentSize = value;
                this.UpdateLayout();
            }
        }

        [Browsable(true), Category("Appearance")]
        public virtual int LabelWidth
        {
            get { return labelWidth; }
            set
            {
                labelWidth = value > 0 ? value : 0;

                if (LayoutEngine is PropertyContainerLayoutEngine)
                    (LayoutEngine as PropertyContainerLayoutEngine).LabelWidth = labelWidth;

                this.UpdateLayout();
                this.PerformLayout();
            }
        }

        protected virtual int DefaultLabelWidth { get { return 110; } }
        protected override Size DefaultMinimumSize { get { return new Size(10, 10); } }
        protected override Size DefaultSize { get { return new Size(200, 32); } }

        public override LayoutEngine LayoutEngine
        {
            get
            {
                if (layoutEngine == null)
                    layoutEngine = new PropertyContainerLayoutEngine();

                return layoutEngine;
            }
        }

        protected virtual Rectangle LabelRect
        {
            get
            {
                int totalIndent = IndentSize * Indent;

                return new Rectangle(
                    this.Padding.Left + totalIndent,
                    0,
                    LabelWidth - totalIndent,
                    this.ClientRectangle.Height);
            }
        }

        private PropertyContainerLayoutEngine layoutEngine;
        private ContentAlignment labelAlignment = ContentAlignment.MiddleLeft;
        private bool multiline = true;
        private int indent = 0;
        private int indentSize = 16;
        private int labelWidth;
        private TextFormatFlags textFormat = TextFormatFlags.TextBoxControl;

        public PropertyContainer()
        {
            SetStyle(ControlStyles.ContainerControl, true);
            SetStyle(ControlStyles.Selectable, false);

            LabelWidth = DefaultLabelWidth;

            UpdateLayout();
        }

        public virtual void UpdateLayout()
        {
            textFormat = TextFormatFlags.TextBoxControl
                       | TextFormatFlags.EndEllipsis
                       | TextFormatFlags.WordBreak;

            textFormat |= DrawingUtils.ContentAlignmentToTextFormatFlags(labelAlignment);

            if (Multiline == true)
                textFormat &= ~TextFormatFlags.SingleLine;
            else
                textFormat |= TextFormatFlags.SingleLine;

            this.Invalidate();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            TextRenderer.DrawText(
                e.Graphics,
                this.Text,
                this.Font,
                this.LabelRect,
                this.TextColor,
                this.textFormat);
        }

        protected override void OnTextChanged(EventArgs e)
        {
            base.OnTextChanged(e);
            this.UpdateLayout();
        }
    }
}
