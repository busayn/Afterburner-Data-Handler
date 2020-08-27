using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AfterburnerDataHandler.FlatControls
{
    class VectorIcons
    {
        public static Point[] Arrow(Rectangle rect, ArrowDirection direction)
        {
            Point[] arrow;

            switch (direction)
            {
                case ArrowDirection.Left:
                    arrow = LeftArrow(rect);
                    break;
                case ArrowDirection.Up:
                    arrow = UpArrow(rect);
                    break;
                case ArrowDirection.Right:
                    arrow = RightArrow(rect);
                    break;
                case ArrowDirection.Down:
                    arrow = DownArrow(rect);
                    break;
                default:
                    arrow = new Point[0];
                    break;
            }

            return arrow;
        }

        public static Point[] LeftArrow(Rectangle rect)
        {
            return LeftArrow(rect, rect.Width < rect.Height ? rect.Width : rect.Height);
        }

        public static Point[] LeftArrow(Rectangle rect, int size)
        {
            Point[] points = new Point[3];
            Point center = new Point(rect.X + rect.Width / 2, rect.Y + rect.Height / 2);
            int mainRadius = size / 2;
            int secondRadius = (int)(mainRadius * 0.5);

            points[0] = new Point(center.X - secondRadius, center.Y);
            points[1] = new Point(center.X + secondRadius, center.Y - mainRadius);
            points[2] = new Point(center.X + secondRadius, center.Y + mainRadius);

            return points;
        }

        public static Point[] UpArrow(Rectangle rect)
        {
            return UpArrow(rect, rect.Width < rect.Height ? rect.Width : rect.Height);
        }

        public static Point[] UpArrow(Rectangle rect, int size)
        {
            Point[] points = new Point[3];
            Point center = new Point(rect.X + rect.Width / 2, rect.Y + rect.Height / 2);
            int mainRadius = size / 2;
            int secondRadius = (int)(mainRadius * 0.5);

            points[0] = new Point(center.X, center.Y - secondRadius);
            points[1] = new Point(center.X + mainRadius, center.Y + secondRadius);
            points[2] = new Point(center.X - mainRadius, center.Y + secondRadius);

            return points;
        }

        public static Point[] RightArrow(Rectangle rect)
        {
            return RightArrow(rect, rect.Width < rect.Height ? rect.Width : rect.Height);
        }

        public static Point[] RightArrow(Rectangle rect, int size)
        {
            Point[] points = new Point[3];
            Point center = new Point(rect.X + rect.Width / 2, rect.Y + rect.Height / 2);
            int mainRadius = size / 2;
            int secondRadius = (int)(mainRadius * 0.5);

            points[0] = new Point(center.X + secondRadius, center.Y);
            points[1] = new Point(center.X - secondRadius, center.Y + mainRadius);
            points[2] = new Point(center.X - secondRadius, center.Y - mainRadius);

            return points;
        }

        public static Point[] DownArrow(Rectangle rect)
        {
            return DownArrow(rect, rect.Width < rect.Height ? rect.Width : rect.Height);
        }

        public static Point[] DownArrow(Rectangle rect, int size)
        {
            Point[] points = new Point[3];
            Point center = new Point(rect.X + rect.Width / 2, rect.Y + rect.Height / 2);
            int mainRadius = size / 2;
            int secondRadius = (int)(mainRadius * 0.5);

            points[0] = new Point(center.X, center.Y + secondRadius);
            points[1] = new Point(center.X - mainRadius, center.Y - secondRadius);
            points[2] = new Point(center.X + mainRadius, center.Y - secondRadius);

            return points;
        }

        public static Point[] Checkmark(Rectangle rect)
        {
            Point[] points = new Point[3];
            Point center = new Point(rect.X + rect.Width / 2, rect.Y + rect.Height / 2);
            int radius = (rect.Width < rect.Height ? rect.Width : rect.Height) / 2;

            points[0] = new Point(center.X - radius, center.Y - (int)(radius * 0.3));
            points[1] = new Point(center.X - (int)(radius * 0.3), center.Y + (int)(radius * 0.5));
            points[2] = new Point(center.X + radius, center.Y - radius);

            return points;
        }
    }
}
