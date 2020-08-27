using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.Layout;

namespace AfterburnerDataHandler.FlatControls
{
    public class FlexColumnsLayoutEngine : LayoutEngine
    {
        public virtual int MinMinColumnSizeSize { get; set; } = 100;
        public virtual int MaxColumnSize { get; set; } = 200;
        public virtual int MinColumnCount { get; set; } = 1;
        public virtual int MaxColumnCount { get; set; } = 3;


        public enum ColumnsLayout
        {
            HorizontalGrid = 0,
            VerticalGrid = 1,
            HorizontalStaggeredGrid = 2,
            /// <summary>
            /// In developing. It works incorrectly!
            /// </summary>
            VerticalStaggeredGrid = 3
        }

        public virtual ColumnsLayout ColumnsStyle { get; set; } = ColumnsLayout.HorizontalGrid;

        public override bool Layout(object container, LayoutEventArgs layoutEventArgs)
        {
            if ((container is Control) == false) return true;

            Control panel = container as Control;
            Rectangle bounds = panel.DisplayRectangle;
            bounds.Width = panel.ClientRectangle.Width - panel.Padding.Horizontal;
            Rectangle[] controlsBounds = CalculateLayout(panel, bounds);

            if (controlsBounds == null || controlsBounds.Length != panel.Controls.Count)
                return true;

            for (int i = 0; i < controlsBounds.Length; i++)
            {
                panel.Controls[i].Bounds = controlsBounds[i];
            }

            return true;
        }

        public virtual Rectangle[] CalculateLayout(Control container, Rectangle targetBounds)
        {
            if ((container is Control) == false)
                return new Rectangle[0];
            
            Control panel = container as Control;

            if (panel.Controls == null || panel.Controls.Count < 1)
                return new Rectangle[0];

            int columnCount = 1;
            if (MinMinColumnSizeSize > 0) columnCount = targetBounds.Width / MinMinColumnSizeSize;
            if (columnCount < MinColumnCount) columnCount = MinColumnCount;
            if (columnCount > MaxColumnCount && MaxColumnCount > 0) columnCount = MaxColumnCount;
            if (columnCount > panel.Controls.Count) columnCount = panel.Controls.Count;

            int columnSize = targetBounds.Width;

            if (columnCount > 1)
                columnSize = (int)Math.Floor(targetBounds.Width / (float)columnCount);

            if (columnSize < MinMinColumnSizeSize)
                columnSize = MinMinColumnSizeSize;

            if (columnSize > MaxColumnSize && MaxColumnSize > 0)
                columnSize = MaxColumnSize;

            switch (ColumnsStyle)
            {
                case ColumnsLayout.HorizontalGrid:
                    return CalculateHorizontalGrid(panel.Controls, targetBounds, columnSize, columnCount);
                case ColumnsLayout.VerticalGrid:
                    return CalculateVerticalGrid(panel.Controls, targetBounds, columnSize, columnCount);
                case ColumnsLayout.HorizontalStaggeredGrid:
                    return CalculateHorizontalStaggeredGrid(panel.Controls, targetBounds, columnSize, columnCount);
                case ColumnsLayout.VerticalStaggeredGrid:
                    return CalculateVerticalStaggeredGrid(panel.Controls, targetBounds, columnSize, columnCount);
            }

            return new Rectangle[0];
        }

        protected virtual Rectangle[] CalculateHorizontalGrid(
            Control.ControlCollection controls, Rectangle bounds,
            int columnSize, int columnCount)
        {
            Rectangle[] controlsBounds = new Rectangle[controls.Count];
            int currentColumn = 0;
            int currentLineHeight = 0;
            int nextLineLocation = bounds.Top;

            for (int i = 0; i < controlsBounds.Length; i++)
            {
                Padding controlMargin = controls[i].Margin;

                currentColumn = i % columnCount;

                if (currentColumn == 0)
                {
                    nextLineLocation += currentLineHeight;
                    currentLineHeight = 0;
                }

                controlsBounds[i] = CalculateIconBounds(
                    controls[i],
                    bounds.Left + currentColumn * columnSize + controlMargin.Left,
                    nextLineLocation + controlMargin.Top,
                    columnSize);

                int totalControlHeight = controlsBounds[i].Height + controlMargin.Vertical;

                if (currentLineHeight < totalControlHeight)
                    currentLineHeight = totalControlHeight;
            }

            return controlsBounds;
        }

        protected virtual Rectangle[] CalculateVerticalGrid(
            Control.ControlCollection controls, Rectangle bounds,
            int columnSize, int columnCount)
        {
            Rectangle[] controlsBounds = new Rectangle[controls.Count];
            int completeLinesCount = (int)Math.Floor(controls.Count / (float)columnCount);
            int unfinishedLineSize = controls.Count % columnCount;

            int currentColumn = 0;
            int currentLine = 0;
            int currentControl = 0;
            int currentLineHeight = 0;
            int nextLineLocation = bounds.Top;

            for (int i = 0; i < controlsBounds.Length; i++)
            {
                currentLine = (int)Math.Floor(i / (float)columnCount);
                currentColumn = i % columnCount;
                currentControl = currentColumn * completeLinesCount + currentLine;

                if (currentColumn < unfinishedLineSize)
                    currentControl += currentColumn;
                else
                    currentControl += unfinishedLineSize;

                if (currentColumn == 0)
                {
                    nextLineLocation += currentLineHeight;
                    currentLineHeight = 0;
                }

                Padding controlMargin = controls[currentControl].Margin;

                controlsBounds[currentControl] = CalculateIconBounds(
                    controls[currentControl],
                    bounds.Left + currentColumn * columnSize + controlMargin.Left,
                    nextLineLocation + controlMargin.Top,
                    columnSize);

                int totalControlHeight = controlsBounds[currentControl].Height + controlMargin.Vertical;

                if (currentLineHeight < totalControlHeight)
                    currentLineHeight = totalControlHeight;
            }

            return controlsBounds;
        }

        protected virtual Rectangle[] CalculateHorizontalStaggeredGrid(
            Control.ControlCollection controls, Rectangle bounds,
            int columnSize, int columnCount)
        {
            Rectangle[] controlsBounds = new Rectangle[controls.Count];
            int[] columnsHeight = new int[columnCount];
            int currentColumn = 0;

            for (int i = 0; i < columnsHeight.Length; i++)
                columnsHeight[i] = bounds.Top;

            for (int i = 0; i < controlsBounds.Length; i++)
            {
                Padding controlMargin = controls[i].Margin;

                currentColumn = 0;

                for (int n = 0; n < columnCount; n++)
                {
                    if (columnsHeight[n] < columnsHeight[currentColumn]) currentColumn = n;
                }

                controlsBounds[i] = CalculateIconBounds(
                    controls[i],
                    bounds.Left + currentColumn * columnSize + controlMargin.Left,
                    columnsHeight[currentColumn] + controlMargin.Top,
                    columnSize);

                columnsHeight[currentColumn] += controlsBounds[i].Height + controlMargin.Vertical;
            }

            return controlsBounds;
        }

        /// <summary>
        /// In developing. It works incorrectly!
        /// </summary>
        protected virtual Rectangle[] CalculateVerticalStaggeredGrid(
            Control.ControlCollection controls, Rectangle bounds,
            int columnSize, int columnCount)
        {
            Rectangle[] controlsBounds = new Rectangle[controls.Count];
            int currentColumn = 0;
            int nextLineLocation = bounds.Top;
            int maxColumnHeight = 0;

            for (int i = 0; i < controls.Count; i++)
                maxColumnHeight += controls[i].Height + controls[i].Margin.Vertical;

            maxColumnHeight /= columnCount;
            maxColumnHeight += bounds.Top;

            for (int i = 0; i < controlsBounds.Length; i++)
            {
                Rectangle controlBounds = controls[i].Bounds;
                Padding controlMargin = controls[i].Margin;

                controlsBounds[i] = CalculateIconBounds(
                    controls[i],
                    bounds.Left + currentColumn * columnSize + controlMargin.Left,
                    nextLineLocation + controlMargin.Top,
                    columnSize - controlMargin.Horizontal);

                nextLineLocation = controlBounds.Bottom + controlMargin.Bottom;

                if (controlsBounds[i].Bottom >= maxColumnHeight)
                {
                    currentColumn++;
                    nextLineLocation = bounds.Top;
                }
            }

            return controlsBounds;
        }

        protected virtual Rectangle CalculateIconBounds(
            Control control, int locationX, int locationY, int columnSize)
        {
            return new Rectangle(
                locationX,
                locationY,
                columnSize - control.Margin.Horizontal,
                control.AutoSize == true
                    ? control.GetPreferredSize(new Size(control.Bounds.Width, 0)).Height
                    : control.Height
                );
        }
    }
}