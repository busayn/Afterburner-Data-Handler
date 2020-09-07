using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using AfterburnerDataHandler.FlatControls;

namespace AfterburnerDataHandler.Controls
{
    public partial class StatusBar : ThemedControl
    {
        public bool Expanded
        {
            get { return expanded; }
            set
            {
                expanded = value;
                MessageList.Visible = value;
                this.Height = PreferredHeight;
                UpdateLayout();
                ControlUtils.ScrollToBottom(MessageList);
                this.Select();
            }
        }

        [Browsable(true)]
        public int BarHeight
        {
            get { return barHeight; }
            set
            {
                barHeight = value > 0 ? value : 0;
                UpdateLayout();
            }
        }

        [Browsable(true)]
        public int MessageListHeight
        {
            get { return messageListHeight; }
            set
            {
                messageListHeight = value > 0 ? value : 0;
                UpdateLayout();
            }
        }

        [Browsable(true)]
        public int ExpandButtonSize
        {
            get { return expandButtonSize; }
            set
            {
                expandButtonSize = value > 0 ? value : 0;
                UpdateLayout();
            }
        }

        [Browsable(true)]
        public int ExpandButtonIconSize
        {
            get { return expandButtonIconSize; }
            set
            {
                expandButtonIconSize = value > 0 ? value : 0;
                UpdateLayout();
            }
        }

        [Browsable(true)]
        public float HighlightColorMultiplier
        {
            get { return highlightColorMultiplier; }
            set
            {
                highlightColorMultiplier = value > 0 ? value : 0;
                UpdateLayout();
            }
        }

        public override Color AccentColor
        {
            get { return base.AccentColor; }
            protected set
            {
                base.AccentColor = value;

                float multiplier = AccentColor.GetBrightness() > 0.5f
                    ? 1 - HighlightColorMultiplier
                    : 1 + HighlightColorMultiplier;

                int r = (int)(value.R * multiplier);
                int g = (int)(value.G * multiplier);
                int b = (int)(value.B * multiplier);

                highlightedAccentColor = Color.FromArgb(Math.Min(Math.Max(r, 0), 255),
                                                        Math.Min(Math.Max(g, 0), 255),
                                                        Math.Min(Math.Max(b, 0), 255));
            }
        }

        [Browsable(false)]
        protected virtual Color HighlightedAccentColor
        {
            get { return highlightedAccentColor; }
        }

        protected virtual Rectangle BarRect
        {
            get { return barRect; }
        }

        protected virtual Rectangle TextRect
        {
            get { return textRect; }
        }

        protected virtual Rectangle ExpandButtonRect
        {
            get { return expandButtonRect; }
        }

        public virtual RichTextBox MessageList
        {
            get
            {
                if (messageList == null)
                {
                    messageList = new RichTextBox
                    {
                        BorderStyle = BorderStyle.None,
                        Margin = new Padding(0),
                        ScrollBars = RichTextBoxScrollBars.Both,
                        ReadOnly = true,
                        WordWrap = true,
                        HideSelection = false,
                        Font = new Font("Consolas", 10),
                    };
                    this.Controls.Add(messageList);

                    messageList.LinkClicked += MessageListLinkClicked;
                }
                return messageList;
            }
        }

        protected int PreferredHeight
        {
            get { return BarHeight + (Expanded ? MessageListHeight : 0); }
        }

        protected override Padding DefaultMargin { get { return new Padding(0); } }
        protected override Size DefaultSize { get { return new Size(320, 24); } }

        private bool expanded = false;
        private Point[] expandArrow;
        private RichTextBox messageList;

        private Rectangle barRect = Rectangle.Empty;
        private Rectangle textRect = Rectangle.Empty;
        private Rectangle expandButtonRect = Rectangle.Empty;

        protected bool ExpandButtonHighlighted
        {
            get { return expandButtonHighlighted; }
            set
            {
                if (value != expandButtonHighlighted)
                    this.Invalidate();

                expandButtonHighlighted = value;
            }
        }

        private bool expandButtonHighlighted = false;
        private int barHeight = 24;
        private int messageListHeight = 200;
        private int expandButtonSize = 24;
        private int expandButtonIconSize = 10;
        private float highlightColorMultiplier = 0.15f;
        private Color highlightedAccentColor = Color.Empty;

        public StatusBar()
        {
            SetStyle(ControlStyles.ResizeRedraw, true);
            SetStyle(ControlStyles.Selectable | ControlStyles.StandardDoubleClick, false);
            Expanded = false;
        }

        public virtual void AppendMessage(string message) { AppendMessage(message, string.Empty, true); }

        public virtual void AppendMessage(string message, string label) { AppendMessage(message, label, true); }

        public virtual void AppendMessage(string message, string label, bool showTime)
        {
            ControlUtils.AsyncSafeInvoke(this, () =>
            {
                string text = (MessageList.Lines?.Length ?? 0) > 0 ? "\r\n" : string.Empty;

                if (showTime)
                    text += string.Format("[{0:T}]", DateTime.Now);

                if (string.IsNullOrWhiteSpace(label) == false)
                    text += string.Format("[{0}]", label ?? string.Empty);

                if (string.IsNullOrWhiteSpace(text) == false)
                    text += " " + message ?? string.Empty;
                else
                    text = message ?? string.Empty;

                MessageList.AppendText(text);
                ControlUtils.ScrollToBottom(MessageList);
                this.Invalidate();
            });
        }

        public virtual void UpdateLayout()
        {
            Rectangle viewRect = this.ClientRectangle;
            Padding viewPadding = this.Padding;

            MessageList.Bounds = new Rectangle(
                0,
                2,
                viewRect.Width,
                MessageListHeight - 2);

            barRect = new Rectangle(
                    0,
                    viewRect.Bottom - BarHeight,
                    viewRect.Width,
                    BarHeight);

            barRect = BarRect;

            expandButtonRect = new Rectangle(
                    viewPadding.Left,
                    barRect.Bottom - viewPadding.Bottom - BarHeight,
                    ExpandButtonSize,
                    BarHeight - viewPadding.Vertical);

            textRect = new Rectangle(
                    viewPadding.Left + ExpandButtonSize,
                    barRect.Bottom - viewPadding.Bottom - BarHeight,
                    barRect.Width - viewPadding.Horizontal - ExpandButtonSize,
                    barRect.Height - viewPadding.Vertical);

            expandArrow = Expanded == true
                ? VectorIcons.DownArrow(ExpandButtonRect, ExpandButtonIconSize)
                : VectorIcons.UpArrow(ExpandButtonRect, ExpandButtonIconSize);
        }

        public override void UpdateTheme()
        {
            base.UpdateTheme();

            BackgroundColor = this.Theme.PanelBackgroundColor;
            MessageList.BackColor = this.BackgroundColor;
            MessageList.ForeColor = this.TextColor;
        }

        protected override void OnPaint(PaintEventArgs pe)
        {
            DrawingUtils.FillRectangle(pe.Graphics, this.AccentColor, BarRect);

            if (ExpandButtonHighlighted == true)
                DrawingUtils.FillRectangle(pe.Graphics, this.HighlightedAccentColor, ExpandButtonRect);

            if (Expanded == false && MessageList.Lines != null && MessageList.Lines.Length > 0)
            {
                TextRenderer.DrawText(
                    pe.Graphics,
                    MessageList.Lines[MessageList.Lines.Length - 1] ?? string.Empty,
                    this.Font,
                    this.TextRect,
                    this.TextColor,
                    TextFormatFlags.TextBoxControl
                    | TextFormatFlags.VerticalCenter
                    | TextFormatFlags.Left
                    | TextFormatFlags.SingleLine
                    | TextFormatFlags.EndEllipsis);
            }

            using (SolidBrush brush = new SolidBrush(TextColor))
            {
                System.Drawing.Drawing2D.SmoothingMode lastSmoothingMode = pe.Graphics.SmoothingMode;
                pe.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

                pe.Graphics.FillPolygon(
                    brush,
                    expandArrow);

                pe.Graphics.SmoothingMode = lastSmoothingMode;
            }
        }

        protected override void OnLayout(LayoutEventArgs levent)
        {
            base.OnLayout(levent);
            UpdateLayout();
        }

        protected override void SetBoundsCore(int x, int y, int width, int height, BoundsSpecified specified)
        {
            height = PreferredHeight;
            base.SetBoundsCore(x, y, width, height, specified | BoundsSpecified.Height);
        }

        public override Size GetPreferredSize(Size proposedSize)
        {
            proposedSize.Height = PreferredHeight;
            return base.GetPreferredSize(proposedSize);
        }

        protected override void SetClientSizeCore(int x, int y)
        {
            base.SetClientSizeCore(x, PreferredHeight);
        }

        protected override void OnPaintBackground(PaintEventArgs pevent)
        {
            DrawingUtils.FillRectangle(pevent.Graphics, this.AccentColor, pevent.ClipRectangle);
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            ExpandButtonHighlighted = ExpandButtonRect.Contains(e.Location);
        }

        protected override void OnClick(EventArgs e)
        {
            base.OnClick(e);

            if (expandButtonHighlighted == true)
            {
                Expanded = !Expanded;
            }
        }

        protected override void OnLeave(EventArgs e)
        {
            base.OnLeave(e);
            ExpandButtonHighlighted = false;

            this.Invalidate();
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);
            ExpandButtonHighlighted = false;

            this.Invalidate();
        }

        private void MessageListLinkClicked(object sender, LinkClickedEventArgs e)
        {
            try
            {
                System.Diagnostics.Process.Start(e.LinkText);
            }
            catch { }
        }
    }
}
