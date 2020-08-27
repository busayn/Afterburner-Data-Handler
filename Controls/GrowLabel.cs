using System;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using System.ComponentModel;

public class GrowLabel : Label
{
    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public override bool AutoSize { get { return false; } set { base.AutoSize = false; } }

    public GrowLabel()
    {
        this.AutoSize = false;
    }

    private static readonly TextFormatFlags textFormat = TextFormatFlags.WordBreak |
                                                         TextFormatFlags.TextBoxControl;

    private void UpdateLabelHeight()
    {
        int newHeight = TextRenderer.MeasureText(
            this.Text,
            this.Font,
            new Size(this.ClientSize.Width - this.Padding.Horizontal, Int32.MaxValue),
            textFormat).Height;

        this.ClientSize = new Size(this.ClientSize.Width, newHeight + this.Padding.Vertical);
    }

    protected override void OnSizeChanged(EventArgs e)
    {
        base.OnSizeChanged(e);
        UpdateLabelHeight();
    }

    protected override void OnPaddingChanged(EventArgs e)
    {
        base.OnPaddingChanged(e);
        UpdateLabelHeight();
    }

    protected override void OnFontChanged(EventArgs e)
    {
        base.OnFontChanged(e);
        UpdateLabelHeight();
    }

    protected override void OnTextChanged(EventArgs e)
    {
        base.OnTextChanged(e);
        UpdateLabelHeight();
    }
}