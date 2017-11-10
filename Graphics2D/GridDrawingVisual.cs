using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace Graphics2D
{
    class GridDrawingVisual : FrameworkElement
    {
        private VisualCollection _children;

        public readonly int size = 5;
        public readonly int widthN = 600;
        public readonly int heightN = 400;

        public readonly int maxX;
        public readonly int maxY;

        public GridDrawingVisual()
        {
            maxX = size * widthN;
            maxY = size * heightN;

            _children = new VisualCollection(this);
            _children.Add(CreateDrawingVisualGrid());
        }

        public void DrawRect(int x, int y)
        {
            _children.Add(CreateRect(x, y));
        }

        public void DrawRects(int[][] rects)
        {
            _children.Add(CreateRects(rects));
        }

        private DrawingVisual CreateDrawingVisualGrid()
        {
            DrawingVisual visual = new DrawingVisual();

            using (DrawingContext c = visual.RenderOpen())
            {
                Pen p = new Pen(Brushes.DarkGray, 1);

                //竖线
                for (int i = 0; i <= widthN * size; i += size)
                {
                    c.DrawLine(p, new Point(i, 0), new Point(i, maxY));
                }

                //横线
                for (int j = 0; j <= heightN * size; j += size)
                {
                    c.DrawLine(p, new Point(0, j), new Point(maxX, j));
                }
            }

            return visual;
        }

        private DrawingVisual CreateRect(int x, int y)
        {
            DrawingVisual visual = new DrawingVisual();

            using (DrawingContext c = visual.RenderOpen())
            {
                Pen p = new Pen(Brushes.Black, 1);
                Brush brush = new SolidColorBrush(Colors.Black);

                Rect rect = new Rect(x * size, (heightN - y - 1) * size, size, size);

                c.DrawRectangle(brush, null, rect);
            }

            return visual;
        }

        private DrawingVisual CreateRects(int[][] rects)
        {
            DrawingVisual visual = new DrawingVisual();

            using (DrawingContext c = visual.RenderOpen())
            {
                Pen p = new Pen(Brushes.Black, 1);
                Brush brush = new SolidColorBrush(Colors.Black);

                Rect r = new Rect(0, 0, size, size);

                foreach (int[] rect in rects)
                {
                    if (rect == null) continue;

                    if (rect.Length != 2) return null;

                    r.X = rect[0] * size;
                    r.Y = (heightN - rect[1] - 1) * size;
                    c.DrawRectangle(brush, null, r);
                }
            }

            return visual;
        }

        public void Reset()
        {
            if (_children.Count > 1)
                _children.RemoveRange(1, _children.Count - 1);
        }

        protected override int VisualChildrenCount
        {
            get { return _children.Count; }
        }

        protected override Visual GetVisualChild(int index)
        {
            if (index < 0 || index >= _children.Count)
            {
                throw new ArgumentOutOfRangeException();
            }

            return _children[index];
        }
    }
}
