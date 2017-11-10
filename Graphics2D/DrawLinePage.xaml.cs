using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace Graphics2D
{
    /// <summary>
    /// DrawLine.xaml 的交互逻辑
    /// </summary>
    public partial class DrawLinePage : Page
    {
        private readonly int MIN_LINES_COUNT = 10;
        private readonly int MAX_LINES_COUNT = 50;

        DispatcherTimer timer;

        private double timerInterval;

        private bool mouseDown;
        private Point mouseXY;

        private double min = 0.1, max = 8.0;//最小/最大放大倍数

        private GridDrawingVisual visualGrid;

        private MyLine[] lines;

        private bool hasStarted;

        public DrawLinePage()
        {
            InitializeComponent();

            visualGrid = new GridDrawingVisual();

            gridDrawingCanvas.Children.Add(visualGrid);//画网格最快的方法

            setSpeed();
        }

        //画网格最慢，因此不用
        //private void DrawGrid()
        //{
        //    const int size = 5;
        //    const int widthN = 600;
        //    const int heightN = 400;

        //    int maxX = size * widthN;
        //    int maxY = size * heightN;
        //    GeometryGroup group = new GeometryGroup();

        //    LineGeometry line;

        //    //竖线
        //    for (int i = 0; i < widthN * size; i += size)
        //    {
        //        line = new LineGeometry(new Point(i, 0), new Point(i, maxY));
        //        line.Freeze();
        //        group.Children.Add(line);
        //    }

        //    //横线
        //    for (int j = 0; j < heightN * size; j += size)
        //    {
        //        line = new LineGeometry(new Point(0, j), new Point(maxX, j));
        //        line.Freeze();
        //        group.Children.Add(line);
        //    }
        //    gridPath.Data = group;

        //    gridPath.Data.Freeze();
        //}

        //画网格也较慢，因此不用
        //private void DrawGridByStreamGeometry()
        //{
        //    const int size = 5;
        //    const int widthN = 600;
        //    const int heightN = 400;

        //    int maxX = size * widthN;
        //    int maxY = size * heightN;

        //    StreamGeometry geometry = new StreamGeometry();

        //    geometry.FillRule = FillRule.EvenOdd;

        //    using (StreamGeometryContext c = geometry.Open())
        //    {
        //        //竖线
        //        for (int i = 0; i < widthN * size; i += size)
        //        {
        //            c.BeginFigure(new Point(i, 0), false, false);
        //            c.LineTo(new Point(i, maxY), true, true);
        //        }

        //        //横线
        //        for (int j = 0; j < heightN * size; j += size)
        //        {
        //            c.BeginFigure(new Point(0, j), false, false);
        //            c.LineTo(new Point(maxX, j), true, true);
        //        }
        //    }

        //    geometry.Freeze();

        //    gridPath.Data = geometry;
        //}

        //public void DrawingBrushExample()
        //{
        //    int size = 5;

        //    GeometryGroup gridlines = new GeometryGroup();
        //    gridlines.Children.Add(
        //        new LineGeometry(new Point(0, 0), new Point(0, size))
        //        );
        //    gridlines.Children.Add(
        //        new LineGeometry(new Point(0, 0), new Point(size, 0))
        //        );
        //    gridlines.Children.Add(
        //        new LineGeometry(new Point(size, 0), new Point(size, size))
        //        );
        //    gridlines.Children.Add(
        //        new LineGeometry(new Point(0, size), new Point(size, size))
        //        );

        //    GeometryDrawing aGeometryDrawing = new GeometryDrawing();
        //    aGeometryDrawing.Geometry = new RectangleGeometry(new Rect(new Size(size, size)));

        //    //aGeometryDrawing.Brush =
        //    //    new LinearGradientBrush(
        //    //        Colors.Blue,
        //    //        Color.FromRgb(204, 204, 255),
        //    //        new Point(0, 0),
        //    //        new Point(1, 1));
        //    aGeometryDrawing.Brush = new SolidColorBrush();

        //    aGeometryDrawing.Pen = new Pen(Brushes.DarkGray, 1);

        //    DrawingBrush patternBrush = new DrawingBrush(aGeometryDrawing);
        //    patternBrush.Viewport = new Rect(0, 0, size, size);
        //    patternBrush.TileMode = TileMode.Tile;
        //    patternBrush.Freeze();

        //    //
        //    // Create an object to paint.
        //    //
        //    Rectangle paintedRectangle = new Rectangle();
        //    paintedRectangle.Stretch = Stretch.None;
        //    paintedRectangle.Width = 600;
        //    paintedRectangle.Height = 400;
        //    paintedRectangle.Fill = patternBrush;

        //    //
        //    // Place the image inside a border and
        //    // add it to the page.
        //    //

        //    //Border exampleBorder = new Border();
        //    //exampleBorder.Child = paintedRectangle;
        //    //exampleBorder.BorderBrush = Brushes.Gray;
        //    //exampleBorder.BorderThickness = new Thickness(1);
        //    //exampleBorder.HorizontalAlignment = HorizontalAlignment.Left;
        //    //exampleBorder.VerticalAlignment = VerticalAlignment.Top;
        //    //exampleBorder.Margin = new Thickness(10);

        //    //this.Margin = new Thickness(20);
        //    //this.Background = Brushes.White;
        //    //this.Content = exampleBorder;
        //    lineGrid.Children.Add(paintedRectangle);
        //}


        //以下四个事件处理函数用于使网格能够移动和缩放

        private void gridDrawingCanvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            mouseDown = true;
            mouseXY = e.GetPosition(null);
        }

        private void gridDrawingCanvas_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            mouseDown = false;
        }

        private void gridDrawingCanvas_MouseMove(object sender, MouseEventArgs e)
        {
            if (mouseDown)
            {
                //Domousemove(gridDrawingCanvas, e);
                if (e.LeftButton != MouseButtonState.Pressed) return;

                TranslateTransform transform = (gridDrawingCanvas.RenderTransform as TransformGroup).Children[1] as TranslateTransform;

                var position = e.GetPosition(null);
                transform.X -= mouseXY.X - position.X;
                transform.Y -= mouseXY.Y - position.Y;
                mouseXY = position;
            }
        }

        private void gridDrawingCanvas_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            var pointToContentFake = e.GetPosition(gridDrawingCanvas);

            var delta = e.Delta * 0.001;

            var group = gridDrawingCanvas.RenderTransform as TransformGroup;

            var point = gridDrawingCanvas.RenderTransform.Transform(pointToContentFake);

            var transform = group.Children[0] as ScaleTransform;
            if (transform.ScaleX + delta < min) return;
            if (transform.ScaleX + delta > max) return;
            transform.ScaleX += delta;
            transform.ScaleY += delta;
            var transform1 = group.Children[1] as TranslateTransform;
            transform1.X = -1 * ((pointToContentFake.X * transform.ScaleX) - point.X);
            transform1.Y = -1 * ((pointToContentFake.Y * transform.ScaleY) - point.Y);
        }

        //以上四个事件处理函数用于使网格能够移动和缩放

        private void btnPauseResume_Click(object sender, RoutedEventArgs e)
        {
            if (hasStarted)
            {
                if (timer != null)
                {
                    if (timer.IsEnabled)
                    {
                        btnPauseResume.Content = "继续";
                        timer.Stop();
                    }
                    else
                    {
                        btnPauseResume.Content = "暂停";
                        timer.Start();
                    }
                }
            }
            else
            {
                visualGrid.Reset();
                startDrawingNewLines();
            }
        }

        private void btnResetGrid_Click(object sender, RoutedEventArgs e)
        {
            var group = gridDrawingCanvas.RenderTransform as TransformGroup;
            var st = group.Children[0] as ScaleTransform;
            st.ScaleX = 1;
            st.ScaleY = 1;
            var tt = group.Children[1] as TranslateTransform;
            tt.X = 0;
            tt.Y = 0;
        }

        private void DowheelZoom(TransformGroup group, Point point, double delta)
        {
            var pointToContent = group.Inverse.Transform(point);
            var st = group.Children[0] as ScaleTransform;
            if (st.ScaleX + delta < min) return;
            if (st.ScaleX + delta > max) return;
            st.ScaleX += delta;
            st.ScaleY += delta;
            var tt = group.Children[1] as TranslateTransform;
            tt.X = -1 * ((pointToContent.X * st.ScaleX) - point.X);
            tt.Y = -1 * ((pointToContent.Y * st.ScaleY) - point.Y);
        }

        private void speedSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            setSpeed();
        }

        private void setSpeed()
        {
            if (speedSlider == null)
            {
                timerInterval = 1000;
            }

            int v = (int)speedSlider.Value;

            timerInterval = speedSlider.Maximum - v + 1;

            if (timer != null) timer.Interval = TimeSpan.FromMilliseconds(timerInterval);
        }

        private void startDrawingNewLines()
        {
            hasStarted = true;
            lines = MyLine.getRandomLines(MIN_LINES_COUNT, MAX_LINES_COUNT, visualGrid.widthN, visualGrid.heightN);
            // lines = new MyLine[] { new MyLine(new MyPoint(181, 373), new MyPoint(427, 4)) };
            if (timer == null || !timer.IsEnabled) startTimer();
        }

        private void startTimer()
        {
            if (timer == null)
            {
                timer = new DispatcherTimer();
                timer.Tick += Timer_Tick;
            }
            timer.Interval = TimeSpan.FromMilliseconds(timerInterval);
            timer.Start();
            hasStarted = true;
            btnPauseResume.Content = "暂停";
        }

        private void pauseTimer()
        {
            if (timer != null) timer.Stop();
            btnPauseResume.Content = "开始";
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            drawNext();
        }

        private void finishDrawingAllLines()
        {
            if (timer != null) timer.Stop();
            hasStarted = false;
            btnPauseResume.Content = "重新\n开始";
        }

        /// <summary>
        /// 进行下一步计算，并将结果显示出来
        /// </summary>
        private void drawNext()
        {
            if (lines == null) return;

            bool allComplete = true;

            int[][] rects = new int[lines.Length][];

            int i = 0;
            foreach (var line in lines)
            {
                MyPoint p = line.getNextPoint();
                if (p != null)
                {
                    if (allComplete)
                        allComplete = false;
                    rects[i] = new int[] { p.X, p.Y };
                }
                i++;
            }

            drawRects(rects);

            if (allComplete) finishDrawingAllLines();
        }

        private void btnDrawNext_Click(object sender, RoutedEventArgs e)
        {
            if (hasStarted && timer != null && timer.IsEnabled)
            {
                btnPauseResume.Content = "继续";
                timer.Stop();
            }
            drawNext();
        }

        private void drawRect(int x, int y)
        {
            if (visualGrid != null) visualGrid.DrawRect(x, y);
        }

        private void drawRects(int[][] rects)
        {
            if (visualGrid != null) visualGrid.DrawRects(rects);
        }
    }
}
