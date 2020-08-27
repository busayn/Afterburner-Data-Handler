using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AfterburnerDataHandler.FlatControls
{
    public class DrawingUtils
    {
        private static readonly ContentAlignment leftdDir = ContentAlignment.TopLeft | ContentAlignment.MiddleLeft | ContentAlignment.BottomLeft;
        private static readonly ContentAlignment centerDir = ContentAlignment.TopCenter | ContentAlignment.MiddleCenter | ContentAlignment.BottomCenter;
        private static readonly ContentAlignment topDir = ContentAlignment.TopLeft | ContentAlignment.TopCenter | ContentAlignment.TopRight;
        private static readonly ContentAlignment middleDir = ContentAlignment.MiddleLeft | ContentAlignment.MiddleCenter | ContentAlignment.MiddleRight;

        public static TextFormatFlags ContentAlignmentToTextFormatFlags(ContentAlignment align)
        {
            TextFormatFlags flags = new TextFormatFlags();

            if ((align & leftdDir) != 0)
                flags |= TextFormatFlags.Left;
            else if ((align & centerDir) != 0)
                flags |= TextFormatFlags.HorizontalCenter;
            else
                flags |= TextFormatFlags.Right;

            if ((align & topDir) != 0)
                flags |= TextFormatFlags.Top;
            else if ((align & middleDir) != 0)
                flags |= TextFormatFlags.VerticalCenter;
            else
                flags |= TextFormatFlags.Bottom;

            return flags;
        }

        public static void FillRectangle(Graphics graphics, Color color, Rectangle rect)
        {
            if (graphics == null) return;

            using (SolidBrush brush = new SolidBrush(color))
            {
                graphics.FillRectangle(brush, rect);
            }
        }

        public static void DrawOnePixelBorder(Graphics graphics, Color color, Rectangle rect)
        {
            if (graphics == null) return;

            using (Pen pen = new Pen(color))
            {
                pen.Alignment = System.Drawing.Drawing2D.PenAlignment.Left;
                pen.Width = 1;
                graphics.DrawRectangle(pen, rect.X, rect.Y, rect.Width - 1, rect.Height - 1);
            }
        }

        public static void DrawIcon(Graphics graphics, Rectangle rect, Image icon)
        {
            if (graphics == null || icon == null) return;

            InterpolationMode lastInterpolationMode = graphics.InterpolationMode;
            graphics.InterpolationMode = InterpolationMode.HighQualityBilinear;

            graphics.DrawImage(icon, CreateRectFromAspect(rect, icon.Size));
            graphics.InterpolationMode = lastInterpolationMode;
        }

        public static void DrawIcon(Graphics graphics, Rectangle rect, Image icon, ImageAttributes attributes)
        {
            if (graphics == null || icon == null) return;

            InterpolationMode lastInterpolationMode = graphics.InterpolationMode;
            graphics.InterpolationMode = InterpolationMode.HighQualityBilinear;

            graphics.DrawImage(
                icon,
                CreateRectFromAspect(rect, icon.Size),
                0,
                0,
                icon.Width,
                icon.Height,
                GraphicsUnit.Pixel,
                attributes);

            graphics.InterpolationMode = lastInterpolationMode;
        }

        public static void DrawIcon(Graphics graphics, Rectangle rect, Image icon, Color multiplyColor)
        {
            if (graphics == null || icon == null) return;

            if (multiplyColor == Color.White)
            {
                DrawIcon(graphics, rect, icon);
                return;
            }

            using (ImageAttributes attributes = new ImageAttributes())
            {
                ColorMatrix cm = new ColorMatrix
                {
                    Matrix00 = multiplyColor.R / 255f,
                    Matrix11 = multiplyColor.G / 255f,
                    Matrix22 = multiplyColor.B / 255f
                };

                attributes.SetColorMatrix(cm);


                InterpolationMode lastInterpolationMode = graphics.InterpolationMode;
                graphics.InterpolationMode = InterpolationMode.HighQualityBilinear;

                graphics.DrawImage(
                    icon,
                    CreateRectFromAspect(rect, icon.Size),
                    0,
                    0,
                    icon.Width,
                    icon.Height,
                    GraphicsUnit.Pixel,
                    attributes);

                graphics.InterpolationMode = lastInterpolationMode;
            }
        }

        public static Rectangle CreateRectFromAspect(Rectangle bounds, Size aspect)
        {
            if (bounds.Width == 0 ||
                bounds.Height == 0 ||
                aspect.Width == 0 ||
                aspect.Height == 0)
                return bounds;

            Rectangle newRect = new Rectangle();
            float boundsAspect = (float)bounds.Width / (float)bounds.Height;
            float targetAspect = (float)aspect.Width / (float)aspect.Height;

            if (boundsAspect > targetAspect)
            {
                newRect.Width = (int)(bounds.Height * targetAspect);
                newRect.Height = bounds.Height;
                newRect.X = (int)(bounds.X + (bounds.Width - newRect.Width) * 0.5);
                newRect.Y = bounds.Y;
            }
            else
            {
                newRect.Width = bounds.Width;
                newRect.Height = (int)(bounds.Width * (1 / targetAspect));
                newRect.X = bounds.X;
                newRect.Y = (int)(bounds.Y + (bounds.Height - newRect.Height) * 0.5);
            }

            return newRect;
        }
    }
}
