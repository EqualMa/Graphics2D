using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace Graphics2D
{
    /// <summary>
    /// FloodFillPage.xaml 的交互逻辑
    /// </summary>
    public sealed partial class FloodFillPage : Page, IDisposable
    {
        DispatcherTimer timer;

        private System.Drawing.Color newColor = System.Drawing.Color.FromArgb(246, 179, 82);

        private FloodFiller filler;

        public WriteableBitmap CurrentBitmapSource { get; private set; }

        private int times;

        private double timerInterval;

        public FloodFillPage()
        {
            InitializeComponent();

            CurrentBitmapSource = new WriteableBitmap((BitmapSource)Application.Current.FindResource("OriginalBitmap"));

            fillImage.Source = CurrentBitmapSource;

            textBlock.Text = "请点击图片中的白色位置开始填充。";

            setSpeed();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            fillManyTimes(times);
        }

        private void fillManyTimes(int n)
        {
            if (filler == null) return;

            for (int i = 0; i < times; i++)
                if (filler.fillOnce())
                {
                    completeFill();
                    return;
                }
        }

        private void completeFill()
        {
            textBlock.Text = "填充完毕，总共填充 " + filler.Times + "次。";
            timer.Stop();
            btnPauseResume.Content = "";
            btnPauseResume.IsEnabled = false;
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
            btnPauseResume.Content = "暂停";
        }

        private void pauseTimer()
        {
            if (timer != null)
            {
                timer.Stop();
                btnPauseResume.Content = "开始";
            }
        }

        private void btnPauseResume_Click(object sender, RoutedEventArgs e)
        {
            if (timer == null || !timer.IsEnabled) startTimer();
            else pauseTimer();
        }

        private void fillImage_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (filler == null)
            {
                filler = new FloodFiller(CurrentBitmapSource, newColor, System.Drawing.Color.White);

                Binding bd = new Binding();
                bd.Source = filler;
                bd.Path = new PropertyPath("TotalTimesString");
                textBlock.SetBinding(TextBlock.TextProperty, bd);
            }
            if (!filler.HasStarted)
            {
                System.Windows.Point p = e.GetPosition(fillImage);

                filler.setStartPoint(p.X, p.Y, fillImage.ActualWidth, fillImage.ActualHeight);

                startTimer();

                btnPauseResume.IsEnabled = true;
            }
        }

        private void btnColorPicker_Click(object sender, RoutedEventArgs e)
        {
            popupColorPicker.IsOpen = !popupColorPicker.IsOpen;
        }

        private void RadioButton_Checked(object sender, RoutedEventArgs e)
        {
            System.Windows.Media.Color c = ((SolidColorBrush)((Grid)((RadioButton)sender).Parent).Background).Color;
            newColor = System.Drawing.Color.FromArgb(c.A, c.R, c.G, c.B);
            if (filler != null) filler.newColor = newColor;
        }

        private void setSpeed()
        {
            if (speedSlider == null)
            {
                times = 1;
                timerInterval = 0.1;
            }

            int v = (int)speedSlider.Value;

            if (v <= 10)
            {
                times = 1;
                timerInterval = 1 / v;

                if (timer != null) timer.Interval = TimeSpan.FromMilliseconds(timerInterval);
            }
            else
            {
                times = v;
                timerInterval = 0.1;

                if (timer != null) timer.Interval = TimeSpan.FromMilliseconds(timerInterval);
            }
        }

        private void reset()
        {
            CurrentBitmapSource = new WriteableBitmap((BitmapSource)Application.Current.FindResource("OriginalBitmap"));

            fillImage.Source = CurrentBitmapSource;

            if (filler != null)
            {
                filler.Dispose();
                filler = null;
            }
            if (timer != null)
                timer.Stop();

            textBlock.Text = "请点击图片中的白色位置开始填充。";
            btnPauseResume.Content = "";
            btnPauseResume.IsEnabled = false;
        }

        private void speedSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            setSpeed();
        }

        private void btnReset_Click(object sender, RoutedEventArgs e)
        {
            reset();
        }

        public void Dispose()
        {
            if (filler != null)
                filler.Dispose();
        }
    }
}
