using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AfterburnerDataHandler.FlatControls
{
    [Designer(typeof(TabViewDesigner))]
    public partial class TabView : Control, IThemedControl
    {
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public virtual bool UseGlobalTheme { get; set; } = true;

        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Theme Theme
        {
            get
            {
                if (theme == null) theme = this.DefaultTheme;
                return theme;
            }
            set
            {
                theme = value;
                this.UpdateTheme();
            }
        }

        public event EventHandler<EventArgs> ThemeDataChanged;

        [Browsable(true), Category("\tTabs Bar Appearance")]
        public virtual Font TabsBarFont
        {
            get { return tabsBarFont; }
            set
            {
                tabsBarFont = value;
                UpdateLayout();
            }
        }

        [Browsable(true), Category("\tTabs Bar Layout")]
        public virtual TabViewLayout TabsBarLayout
        {
            get { return tabsBarLayout; }
            set
            {
                tabsBarLayout = value;

                if (value == TabViewLayout.Vertical)
                {
                    textFormat &= ~TextFormatFlags.SingleLine;
                }
                else
                {
                    textFormat |= TextFormatFlags.SingleLine;
                }

                UpdateLayout();
            }
        }

        [Browsable(true), Category("\tTabs Bar Layout")]
        public virtual int TabsBarSize
        {
            get { return tabsBarSize; }
            set
            {
                tabsBarSize = value > 0 ? value : 0;
                UpdateLayout();
            }
        }

        [Browsable(true), Category("\tTabs Bar Layout")]
        public virtual Padding TabsBarPadding
        {
            get { return tabsBarPadding; }
            set
            {
                tabsBarPadding = value;
                UpdateLayout();
            }
        }

        [Browsable(true), Category("\tTabs Bar Layout")]
        public virtual int TabsBarSeparatorSize
        {
            get { return tabsBarSeparatorSize; }
            set
            {
                tabsBarSeparatorSize = value > 0 ? value : 0;
                UpdateLayout();
            }
        }

        public enum SelectionRectStyle
        {
            Fill = 0,
            Mark = 1
        }

        [Browsable(true), Category("\tTabs Label Layout")]
        public virtual bool UseSelectionMark
        {
            get { return useSelectionMark; }
            set
            {
                useSelectionMark = value;
                UpdateLayout();
            }
        }

        [Browsable(true), Category("\tTabs Label Layout")]
        public virtual int SelectionMarkSize
        {
            get { return selectionMarkSize; }
            set
            {
                selectionMarkSize = value > 0 ? value : 0;
                UpdateLayout();
            }
        }

        [Browsable(true), Category("\tTabs Label Layout")]
        public virtual Padding LabelMargin
        {
            get { return labelMargin; }
            set
            {
                labelMargin = value;
                UpdateLayout();
            }
        }

        [Browsable(true), Category("\tTabs Label Layout")]
        public virtual Padding LabelPadding
        {
            get { return labelPadding; }
            set
            {
                labelPadding = value;
                UpdateLayout();
            }
        }

        [Browsable(true), Category("\tTabs Label Layout")]
        public virtual int MaximumLabelSize
        {
            get { return maximumLabelSize; }
            set
            {
                maximumLabelSize = value > 0 ? value : 0;
                UpdateLayout();
            }
        }

        [Browsable(true), Category("\tTabs Label Layout")]
        public virtual bool UseLabelIcons
        {
            get { return useLabelIcons; }
            set
            {
                useLabelIcons = value;
                UpdateLayout();
            }
        }

        [Browsable(true), Category("\tTabs Label Layout")]
        public virtual int LabelIconSize
        {
            get { return labelIconSize; }
            set
            {
                labelIconSize = value > 0 ? value : 0;
                UpdateLayout();
            }
        }

        [Browsable(true), Category("\tTabs Label Layout")]
        public virtual int LabelIconOffset
        {
            get { return labelIconOffset; }
            set
            {
                labelIconOffset = value > 0 ? value : 0;
                UpdateLayout();
            }
        }

        [Browsable(true), Category("\tAll Tabs Button")]
        public virtual int AllTabsButtonSize
        {
            get { return allTabsButtonSize; }
            set
            {
                allTabsButtonSize = value > 0 ? value : 0;
                UpdateLayout();
            }
        }

        [Browsable(true), Category("\tAll Tabs Button")]
        public virtual int AllTabsIconSize
        {
            get { return allTabsIconSize; }
            set
            {
                allTabsIconSize = value > 0 ? value : 0;
                UpdateLayout();
            }
        }

        [Browsable(true), Category("\tAll Tabs Button")]
        public virtual bool DisplayOnlyHiddenLabels
        {
            get { return displayOnlyHiddenLabels; }
            set
            {
                displayOnlyHiddenLabels = value;
                UpdateLayout();
            }
        }

        [Browsable(true), Category("\t\tTab Panel Settings")]
        public virtual TabPanel SelectedTab { get; private set; } = null;

        [Browsable(false)]
        public virtual TabPanel HighlightedTab { get; private set; } = null;

        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public override Color BackColor { get { return base.BackColor; } set { base.BackColor = value; } }

        public virtual Color TabViewSelectColor
        {
            get { return tabViewSelectColor; }
            set
            {
                tabViewSelectColor = value;
                OnThemeDataChanged(EventArgs.Empty);
            }
        }

        public virtual Color TabsBarTextColor
        {
            get { return tabsBarTextColor; }
            set
            {
                tabsBarTextColor = value;
                OnThemeDataChanged(EventArgs.Empty);
            }
        }

        public virtual Color TabsBarBackgroundColor
        {
            get { return tabsBarBackgroundColor; }
            set
            {
                tabsBarBackgroundColor = value;
                OnThemeDataChanged(EventArgs.Empty);
            }
        }

        public virtual Color AccentColor
        {
            get { return accentColor; }
            set
            {
                accentColor = value;
                OnThemeDataChanged(EventArgs.Empty);
            }
        }

        public virtual Color TabsBarHighlightColor
        {
            get { return highlightColor; }
            set
            {
                highlightColor = value;
                OnThemeDataChanged(EventArgs.Empty);
            }
        }

        protected struct TabData
        {
            public TabPanel panel;
            public Rectangle tabRect;
            public Rectangle textRect;
            public Rectangle iconRect;
        }

        protected List<TabData> tabsData = new List<TabData>();

        protected virtual int DisplayedTabs
        {
            get
            {
                return displayedTabs < tabsData.Count ? displayedTabs : tabsData.Count;
            }
            set
            {
                displayedTabs = value < tabsData.Count ? value : tabsData.Count;
            }
        }

        protected override Size DefaultSize { get { return new Size(200, 150); } }
        protected Theme DefaultTheme { get { return UseGlobalTheme ? Theme.Current : new Theme(); } }

        protected Rectangle tabsBarRect = new Rectangle();
        protected Rectangle tabsBarSeparatorRect = new Rectangle();
        protected Rectangle allTabsButtonRect = new Rectangle();
        protected Point[] allTabsButtonIcon = new Point[0];
        protected int displayedTabs = -1;        protected bool allTabsButtonHighlighted = false;
        protected TabViewContextMenu allTabsContextMenu = null;

        private TabViewLayout tabsBarLayout = TabViewLayout.Vertical;
        private TextFormatFlags textFormat = (TextFormatFlags.TextBoxControl
                                           | TextFormatFlags.WordEllipsis
                                           | TextFormatFlags.EndEllipsis
                                           | TextFormatFlags.WordBreak
                                           | TextFormatFlags.VerticalCenter)
                                           & ~TextFormatFlags.SingleLine;

        private int tabsBarSize = 100;
        private int tabsBarSeparatorSize = 4;
        private bool useLabelIcons = true;
        private bool useSelectionMark = false;
        private int selectionMarkSize = 4;
        private int labelIconSize = 20;
        private int labelIconOffset = 3;
        private int maximumLabelSize = 200;
        private Padding tabsBarPadding = new Padding(0);
        private Padding labelMargin = new Padding(0);
        private Padding labelPadding = new Padding(6);

        private int allTabsButtonSize = 20;
        private int allTabsIconSize = 8;
        private bool displayOnlyHiddenLabels = true;

        private Font tabsBarFont = SystemFonts.DefaultFont;
        private Theme theme = new Theme();

        private Color tabViewSelectColor;

        private Color tabsBarTextColor = Color.FromArgb(70, 70, 80);
        private Color tabsBarBackgroundColor = Color.FromArgb(190, 210, 230);
        private Color accentColor = Color.FromArgb(245, 250, 255);
        private Color highlightColor = Color.FromArgb(220, 230, 240);

        public TabView()
        {
            SetStyle(ControlStyles.DoubleBuffer | ControlStyles.OptimizedDoubleBuffer, true);
            SetStyle(ControlStyles.Selectable, false);
            SetStyle(ControlStyles.ContainerControl, true);

            Theme.GlobalThemeChanged += GlobalThemeChanged;
            UpdateTheme();
            
            ShowTab(SelectedTab);
        }

        public virtual void OnThemeDataChanged(EventArgs e)
        {
            this.Invalidate();
            ThemeDataChanged?.Invoke(this, e);
        }

        protected void GlobalThemeChanged(object sender, EventArgs e)
        {
            this.Theme = Theme.Current;
        }

        /// <summary>
        /// Updates the layout and causes rendering.
        /// </summary>
        public virtual void UpdateLayout()
        {
            TabViewLayout layout = TabsBarLayout;

            tabsBarRect = CreateTabsBarRect(layout, tabsBarSize);
            tabsBarSeparatorRect = CreateTabsBarSeparatorRect(layout, tabsBarRect, tabsBarSeparatorSize);
            allTabsButtonRect = CreateAllTabsButtonRect(layout, tabsBarRect, allTabsButtonSize);
            allTabsButtonIcon = CreateAllTabsButtonIcon(layout, allTabsButtonRect, allTabsIconSize);

            UpdateTabsBarLayout();
            UpdatePanelsLayout();

            this.Invalidate();
        }

        /// <summary>
        /// Applies the current theme.
        /// </summary>
        public virtual void UpdateTheme()
        {
            this.TabViewSelectColor = this.Theme.TabViewSelectColor;

            this.BackColor = theme.WindowBackgroundColor;
            this.AccentColor = theme.TabViewAccentColor;
            this.TabsBarBackgroundColor = theme.TabViewBackgroundColor;
            this.TabsBarTextColor = theme.TabViewTextColor;
            this.TabsBarHighlightColor = theme.TabViewHighlightColor;

            UpdateLayout();
        }

        /// <summary>
        /// Updates the layout of the child controls.
        /// </summary>
        protected virtual void UpdatePanelsLayout()
        {
            Point panelsLocation = new Point(this.Padding.Left, this.Padding.Top);
            Size panelsSize = this.Size - this.Padding.Size;
            int panelsOffset = TabsBarSize + tabsBarSeparatorSize;

            if (tabsBarLayout == TabViewLayout.Vertical)
            {
                panelsLocation.X += panelsOffset;
                panelsSize.Width -= panelsOffset;
            }
            else
            {
                panelsLocation.Y += panelsOffset;
                panelsSize.Height -= panelsOffset;
            }

            foreach (Control control in this.Controls)
            {
                if (control is TabPanel)
                {
                    control.Location = panelsLocation;
                    control.Size = panelsSize;
                }
            }
        }

        /// <summary>
        /// Updates the layout of the tab bar
        /// </summary>
        protected virtual void UpdateTabsBarLayout()
        {
            if (tabsBarLayout == TabViewLayout.Vertical)
            {
                CalculateVerticalBarRects();
            }
            else
            {
                CalculateHorizontalBarRects();
            }

            DisplayedTabs = GetDisplayedTabs(tabsData, tabsBarRect, tabsBarLayout);
        }

        /// <summary>
        /// Creates rectangles of tab labels for a vertical layout.
        /// </summary>
        protected virtual void CalculateVerticalBarRects()
        {
            Font textFont = TabsBarFont;
            int contentOffset = UseSelectionMark == true ? SelectionMarkSize : 0;
            int iconHeight = 0;
            int iconPadding = 0;


            if (useLabelIcons)
            {
                int maxIconHeight = TabsBarSize - LabelMargin.Horizontal
                    - tabsBarPadding.Horizontal - LabelPadding.Horizontal - contentOffset;

                iconHeight = LabelIconSize < maxIconHeight ? LabelIconSize : maxIconHeight;
                iconPadding = LabelIconOffset;
            }

            if (iconHeight > MaximumLabelSize) iconHeight = maximumLabelSize;

            Size measuredTextSize = Size.Empty;
            Point tabPosition = new Point(LabelMargin.Left + tabsBarPadding.Left, LabelMargin.Top + tabsBarPadding.Top);
            Size targetTabSize = new Size(TabsBarSize - LabelMargin.Horizontal - tabsBarPadding.Horizontal, 0);

            Size targetTextSize = new Size(
                targetTabSize.Width - LabelPadding.Horizontal - iconHeight - iconPadding - contentOffset,
                MaximumLabelSize);


            tabsData.Clear();

            foreach (Control c in this.Controls)
            {
                if (c is TabPanel)
                {
                    TabData tab = new TabData { panel = c as TabPanel };

                    tab.tabRect.Location = tabPosition;
                    measuredTextSize = TextRenderer.MeasureText(tab.panel.Text, textFont, targetTextSize, textFormat);

                    tab.iconRect = new Rectangle(
                        tabPosition.X + LabelPadding.Left + contentOffset,
                        tabPosition.Y + LabelPadding.Top,
                        iconHeight,
                        iconHeight);

                    if (measuredTextSize.Height > MaximumLabelSize)
                        measuredTextSize.Height = MaximumLabelSize;

                    tab.textRect = new Rectangle(
                        tab.iconRect.Right + iconPadding,
                        tabPosition.Y + LabelPadding.Top,
                        targetTextSize.Width,
                        measuredTextSize.Height > iconHeight
                                                ? measuredTextSize.Height
                                                : iconHeight);

                    if (iconHeight < tab.textRect.Height)
                    {
                        tab.iconRect.Y += (tab.textRect.Height - iconHeight) / 2;
                    }

                    tab.tabRect.Size = new Size(
                        targetTabSize.Width,
                        LabelPadding.Vertical + tab.textRect.Height);

                    tabPosition.Y += tab.tabRect.Height + LabelMargin.Vertical;

                    tabsData.Add(tab);
                }
            }
        }

        /// <summary>
        /// Creates rectangles of tab labels for a horizontal layout.
        /// </summary>
        protected virtual private void CalculateHorizontalBarRects()
        {
            Font textFont = TabsBarFont;
            int contentOffset = UseSelectionMark == true ? SelectionMarkSize : 0;
            int iconWidth = 0;
            int iconPadding = 0;

            if (useLabelIcons)
            {
                int maxIconWidth = TabsBarSize - LabelMargin.Vertical - tabsBarPadding.Vertical - LabelPadding.Vertical - contentOffset;

                iconWidth = LabelIconSize < maxIconWidth ? LabelIconSize : maxIconWidth;
                iconPadding = LabelIconOffset;
            }

            Point tabPosition = new Point(LabelMargin.Left + tabsBarPadding.Left, LabelMargin.Top + tabsBarPadding.Top);
            Size measuredTextSize = Size.Empty;

            Size targetTabSize = new Size(
                MaximumLabelSize + iconWidth + iconPadding,
                TabsBarSize - LabelMargin.Vertical - tabsBarPadding.Vertical);

            Size targetTextSize = new Size(
                targetTabSize.Width - LabelPadding.Horizontal,
                targetTabSize.Height - LabelPadding.Vertical - contentOffset);


            tabsData.Clear();

            foreach (Control c in this.Controls)
            {
                if (c is TabPanel)
                {
                    TabData tab = new TabData { panel = c as TabPanel };

                    measuredTextSize = TextRenderer.MeasureText(tab.panel.Text, textFont, targetTextSize, textFormat);
                    tab.tabRect.Location = tabPosition;

                    if (measuredTextSize.Width > MaximumLabelSize)
                        measuredTextSize.Width = MaximumLabelSize;

                    tab.iconRect = new Rectangle(
                        tabPosition.X + LabelPadding.Left,
                        tabPosition.Y + LabelPadding.Top,
                        iconWidth,
                        iconWidth);

                    tab.textRect = new Rectangle(
                        tab.iconRect.Right + iconPadding,
                        tabPosition.Y + LabelPadding.Top,
                        measuredTextSize.Width,
                        targetTextSize.Height);

                    if (iconWidth < tab.textRect.Height)
                    {
                        tab.iconRect.Y += (tab.textRect.Height - iconWidth) / 2;
                    }

                    tab.tabRect.Size = new Size(
                        measuredTextSize.Width + iconWidth + iconPadding + LabelPadding.Horizontal,
                        targetTabSize.Height);

                    tabPosition.X += tab.tabRect.Width + LabelMargin.Horizontal;

                    tabsData.Add(tab);
                }
            }
        }

        /// <summary>
        /// Creates a rectangle for the tab bar.
        /// </summary>
        public Rectangle CreateTabsBarRect(TabViewLayout layout, int size)
        {
            if (layout == TabViewLayout.Vertical)
                return new Rectangle(0, 0, size, this.Height);
            else
                return new Rectangle(0, 0, this.Width, size);
        }

        /// <summary>
        /// Creates a rectangle for the line that separates the tab bar and the tab view.
        /// </summary>
        public virtual Rectangle CreateTabsBarSeparatorRect(TabViewLayout layout, Rectangle tabsBar, int size)
        {
            if (layout == TabViewLayout.Vertical)
            {
                return new Rectangle(
                    tabsBar.Right, tabsBar.Top,
                    size, tabsBar.Height);
            }
            else
            {
                return new Rectangle(
                    tabsBar.Left, tabsBar.Bottom,
                    tabsBar.Width, size);
            }
        }

        /// <summary>
        /// Creates a rectangle for a button that displays a list of all tabs.
        /// </summary>
        public Rectangle CreateAllTabsButtonRect(TabViewLayout layout, Rectangle tabsBar, int size)
        {
            if (layout == TabViewLayout.Vertical)
            {
                return new Rectangle(
                    tabsBar.Left, tabsBar.Bottom - size,
                    tabsBar.Width, size);
            }
            else
            {
                return new Rectangle(
                    tabsBar.Right - size, tabsBar.Top,
                    size, tabsBar.Height);
            }
        }

        /// <summary>
        /// Creates an icon for a button that displays a list of all tabs.
        /// </summary>
        protected Point[] CreateAllTabsButtonIcon(TabViewLayout layout, Rectangle rect, int size)
        {
            if (layout == TabViewLayout.Vertical)
                return VectorIcons.DownArrow(rect, size);
            else
                return VectorIcons.RightArrow(rect, size);
        }

        /// <summary>
        /// Returns the number of tabs that fit on the tab bar.
        /// </summary>
        protected virtual private int GetDisplayedTabs(List<TabData> tabs, Rectangle tabsBar, TabViewLayout layout)
        {
            int displayedTabs = -1;

            if (layout == TabViewLayout.Vertical)
            {
                for (int i = 0; i < tabs.Count; i++)
                {
                    if (tabs[i].tabRect.Bottom > tabsBar.Bottom)
                    {
                        displayedTabs = i;
                        break;
                    }
                }
            }
            else
            {
                for (int i = 0; i < tabs.Count; i++)
                {
                    if (tabs[i].tabRect.Right > tabsBar.Right)
                    {
                        displayedTabs = i;
                        break;
                    }
                }
            }

            if (displayedTabs > 0)
            {
                Rectangle previewTabRect = tabs[displayedTabs - 1].tabRect;

                if (layout == TabViewLayout.Vertical)
                {
                    if (previewTabRect.Bottom > allTabsButtonRect.Top) displayedTabs--;
                }
                else
                {
                    if (previewTabRect.Right > allTabsButtonRect.Left) displayedTabs--;
                }
            }

            return displayedTabs;
        }

        /// <summary>
        /// Opens a tab.
        /// </summary>
        public virtual bool ShowTab(TabPanel tab)
        {
            if (ShowTabPanel(tab))
            {
                SelectedTab = tab;
                return true;
            }
            else return false;
        }

        /// <summary>
        /// Displays the panel.
        /// </summary>
        protected bool ShowTabPanel(TabPanel tab)
        {
            if (tab == null || !this.Controls.Contains(tab)) return false;

            this.SuspendLayout();
            foreach (Control c in this.Controls)
            {
                if (c == tab) c.Show();
                else c.Hide();
            }
            this.Select();
            this.ResumeLayout();
            return true;
        }

        /// <summary>
        /// The context menu used for the list of tabs.
        /// </summary>
        protected class TabViewContextMenu : ContextMenuStrip
        {
            protected override void OnPaintBackground(PaintEventArgs e)
            {
                using (SolidBrush brush = new SolidBrush(this.BackColor))
                {
                    e.Graphics.FillRectangle(brush, e.ClipRectangle);
                }
            }
        }

        /// <summary>
        /// Opens a menu that lists all tabs.
        /// </summary>
        protected void OpenAllTabsMenu(Point location)
        {
            if (allTabsContextMenu != null) allTabsContextMenu.Dispose();

            Font textFont = TabsBarFont;
            int firstTab = displayOnlyHiddenLabels == true ? DisplayedTabs : 0;

            allTabsContextMenu = new TabViewContextMenu
            {
                BackColor = TabsBarBackgroundColor,
                ForeColor = TabsBarTextColor,
            };

            allTabsContextMenu.Items.Clear();

            for (int i = firstTab; i < tabsData.Count; i++)
            {
                ToolStripMenuItem menuItem = new ToolStripMenuItem();

                menuItem.Font = textFont;
                menuItem.Tag = tabsData[i];
                menuItem.Text = tabsData[i].panel.Text;
                menuItem.Enabled = tabsData[i].panel != SelectedTab;
                menuItem.Click += AllTabsMenuAction;

                if (useLabelIcons)
                {
                    menuItem.DisplayStyle = ToolStripItemDisplayStyle.ImageAndText;
                    menuItem.Image = tabsData[i].panel.Icon;
                }
                else
                {
                    menuItem.DisplayStyle = ToolStripItemDisplayStyle.Text;
                }

                allTabsContextMenu.Items.Add(menuItem);
            }

            allTabsContextMenu.Show(this, location);
        }

        /// <summary>
        /// The method is called when an item is clicked from the menu of all tabs.
        /// </summary>
        protected void AllTabsMenuAction(object sender, EventArgs e)
        {
            if (sender is ToolStripMenuItem)
            {
                ToolStripMenuItem item = sender as ToolStripMenuItem;

                if (item.Tag is TabData)
                {
                    ShowTab(((TabData)item.Tag).panel);
                }
            }
        }

        protected override void OnPaintBackground(PaintEventArgs pevent)
        {
            base.OnPaintBackground(pevent);

            using (SolidBrush brush = new SolidBrush(TabsBarBackgroundColor))
            {
                pevent.Graphics.FillRectangle(brush, tabsBarRect);
            }

            if (TabsBarSeparatorSize > 0)
            {
                using (SolidBrush brush = new SolidBrush(AccentColor))
                {
                    pevent.Graphics.FillRectangle(brush, tabsBarSeparatorRect);
                }
            }
        }

        protected override void OnPaint(PaintEventArgs pe)
        {
            base.OnPaint(pe);

            if (pe.Graphics == null) return;

            TabData tab;
            int displayedTabsCount = DisplayedTabs;
            bool useIcon = UseLabelIcons;
            Font textFont = TabsBarFont;

            Pen outlinePen = new Pen(Color.Black, 2);
            Color textColor = TabsBarTextColor;
            SolidBrush textBrush = new SolidBrush(TabsBarTextColor);
            SolidBrush selecedtBrush = new SolidBrush(TabViewSelectColor);
            SolidBrush accentBrush = new SolidBrush(AccentColor);
            SolidBrush highlightedBrush = new SolidBrush(TabsBarHighlightColor);

            for (int i = 0; i < tabsData.Count; i++)
            {
                if (displayedTabsCount > -1 && i >= displayedTabsCount) break;

                tab = tabsData[i];

                if (tab.panel != null)
                {
                    if (tab.panel == SelectedTab)
                    {
                        pe.Graphics.FillRectangle(selecedtBrush, tab.tabRect);

                        if (UseSelectionMark == true)
                        {
                            switch (TabsBarLayout)
                            {
                                case TabViewLayout.Vertical:
                                    pe.Graphics.FillRectangle(accentBrush, new Rectangle(
                                        tab.tabRect.X,
                                        tab.tabRect.Y,
                                        SelectionMarkSize,
                                        tab.tabRect.Height
                                        ));
                                    break;
                                case TabViewLayout.Horizontal:
                                    pe.Graphics.FillRectangle(accentBrush, new Rectangle(
                                        tab.tabRect.X,
                                        tab.tabRect.Y + tab.tabRect.Height - SelectionMarkSize,
                                        tab.tabRect.Width,
                                        SelectionMarkSize
                                        ));
                                    break;
                            }
                        }
                    }
                    else if (tab.panel == HighlightedTab)
                    {
                        pe.Graphics.FillRectangle(highlightedBrush, tab.tabRect);
                    }

                    if (useIcon == true && tab.panel.Icon != null)
                    {
                        InterpolationMode lastInterpolationMode = pe.Graphics.InterpolationMode;
                        pe.Graphics.InterpolationMode = InterpolationMode.HighQualityBilinear;

                        pe.Graphics.DrawImage(
                            tab.panel.Icon,
                            DrawingUtils.CreateRectFromAspect(tab.iconRect, tab.panel.Icon.Size));

                        pe.Graphics.InterpolationMode = lastInterpolationMode;
                    }
                    
                    TextRenderer.DrawText(pe.Graphics, tab.panel.Text, textFont, tab.textRect, textColor, textFormat);
                }
            }

            if (DisplayedTabs > -1)
            {
                if (allTabsButtonHighlighted == true)
                    pe.Graphics.FillRectangle(highlightedBrush, allTabsButtonRect);

                PixelOffsetMode oldPixelOffsetMode = pe.Graphics.PixelOffsetMode;

                pe.Graphics.PixelOffsetMode = PixelOffsetMode.Half;
                pe.Graphics.FillPolygon(textBrush, allTabsButtonIcon);
                pe.Graphics.PixelOffsetMode = oldPixelOffsetMode;
            }

            outlinePen.Dispose();
            selecedtBrush.Dispose();
            accentBrush.Dispose();
            highlightedBrush.Dispose();
            textBrush.Dispose();
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            TabPanel lastHighlightedTab = HighlightedTab;
            bool lastAllTabsButtonState = allTabsButtonHighlighted;
            HighlightedTab = null;

            TabData tab;
            int displayedTabsCount = DisplayedTabs;
            int tabsCount = displayedTabsCount > -1 ? displayedTabsCount : tabsData.Count;

            if (tabsBarRect.Contains(e.Location))
            {
                for (int i = 0; i < tabsCount; i++)
                {
                    tab = tabsData[i];

                    if (tab.panel != null)
                    {
                        if (tab.tabRect.Contains(e.Location))
                        {
                            HighlightedTab = tab.panel;
                            break;
                        }
                    }
                }
            }
            else
            {
                HighlightedTab = null;
            }

            if (DisplayedTabs > -1 && allTabsButtonRect.Contains(e.Location))
            {
                allTabsButtonHighlighted = true;
                HighlightedTab = null;
            }
            else
            {
                allTabsButtonHighlighted = false;
            }

            if (lastHighlightedTab != HighlightedTab ||
                lastAllTabsButtonState != allTabsButtonHighlighted)
            {
                this.Invalidate();
            }
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);

            HighlightedTab = null;
            allTabsButtonHighlighted = false;

            this.Invalidate();
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);

            if (e.Button != MouseButtons.Left) return;

            TabData tab;
            int displayedTabsCount = DisplayedTabs;
            int tabsCount = displayedTabsCount > -1 ? displayedTabsCount : tabsData.Count;

            if (displayedTabs > -1 && allTabsButtonRect.Contains(e.Location))
            {
                this.Invalidate();
                OpenAllTabsMenu(e.Location);
                return;
            }

            for (int i = 0; i < tabsCount; i++)
            {
                tab = tabsData[i];

                if (tab.panel != null)
                {
                    if (tab.tabRect.Contains(e.Location) && tab.panel != SelectedTab)
                    {
                        ShowTab(tab.panel as TabPanel);
                        this.Invalidate();

                        break;
                    }
                }
            }
        }

        protected override void OnControlAdded(ControlEventArgs e)
        {
            base.OnControlAdded(e);

            if (SelectedTab != null)
            {
                ShowTab(SelectedTab);
            }
            else if (e.Control is TabPanel)
            {
                ShowTab(e.Control as TabPanel);
            }

            if (this.IsHandleCreated == true)
            {
                var h = e.Control.Handle;
                e.Control.CreateControl();
            }
        }

        protected override void OnControlRemoved(ControlEventArgs e)
        {
            base.OnControlRemoved(e);

            foreach (Control c in this.Controls)
            {
                if (c is TabPanel)
                {
                    ShowTab(c as TabPanel);
                    return;
                }
            }

            SelectedTab = null;
        }

        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);

            foreach (Control c in this.Controls)
            {
                if (c.IsHandleCreated == false)
                {
                    var h = c.Handle;
                    c.CreateControl();
                }
            }
        }

        protected override void OnLayout(LayoutEventArgs levent)
        {
            base.OnLayout(levent);
            UpdateLayout();
        }

        protected override void InitLayout()
        {
            base.InitLayout();
            UpdateLayout();
        }

        protected override void OnFontChanged(EventArgs e)
        {
            base.OnFontChanged(e);
            UpdateLayout();
        }
    }
}