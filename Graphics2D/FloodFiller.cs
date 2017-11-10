using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace Graphics2D
{
    public sealed class FloodFiller : System.ComponentModel.INotifyPropertyChanged, IDisposable
    {
        private long _times;
        public long Times
        {
            get
            {
                return _times;
            }
            private set
            {
                _times = value;
                TotalTimesString = "填充次数:" + _times;
            }
        }

        private string _tts;

        public string TotalTimesString
        {
            get
            {
                return _tts;
            }
            set
            {
                _tts = value;
                PropertyChanged(this, new PropertyChangedEventArgs("TotalTimesString"));
            }
        }

        public bool HasStarted
        {
            get;
            private set;
        }

        private Stack<int[]> stack = new Stack<int[]>();

        private MyBitmap image;

        public Color newColor;

        private Color emptyColor;

        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        public FloodFiller(WriteableBitmap source, Color newColor, Color emptyColor)
        {
            image = new MyBitmap(source);
            this.newColor = newColor;
            this.emptyColor = emptyColor;
            Times = 0;
        }

        private bool setStartPoint(int x, int y)
        {
            if (HasStarted || stack.Count > 0)
            {
                return false;
            }
            else
            {
                if (x < 0) x = 0;
                if (x >= image.Width) x = image.Width;
                if (y < 0) y = 0;
                if (y >= image.Width) y = image.Width;

                if (isEmptyColor(image.GetPixel(x, y)))
                {
                    HasStarted = true;
                    stack.Push(new int[] { x, y });
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        public void setStartPoint(double x, double y, double actualWidth, double actualHeight)
        {
            setStartPoint((int)Math.Round(image.Width * x / actualWidth), (int)Math.Round(image.Height * y / actualHeight));
        }

        /// <summary>
        /// 填充一次
        /// </summary>
        /// <returns>是否填充完毕</returns>
        public bool fillOnce()
        {
            if (stack.Count > 0)
            {
                Times++;

                int[] pixel = stack.Pop();
                int x = pixel[0];
                int y = pixel[1];

                if (x >= image.Width || x < 0 || y >= image.Height || y < 0)
                    return false;

                if (isEmptyColor(image.GetPixel(x, y)))
                {
                    image.SetPixel(x, y, newColor);
                    stack.Push(new int[] { x - 1, y });
                    stack.Push(new int[] { x + 1, y });
                    stack.Push(new int[] { x, y + 1 });
                    stack.Push(new int[] { x, y - 1 });
                }
                return false;
            }
            else
                return true;
        }

        private bool isEmptyColor(Color thisColor)
        {
            return thisColor.A == emptyColor.A && thisColor.R == emptyColor.R && thisColor.G == emptyColor.G && thisColor.B == emptyColor.B;
        }

        public void Dispose()
        {
            if (image != null)
                ((IDisposable)image).Dispose();
        }
    }

    class MyBitmap : IDisposable
    {
        public int Width { get; set; }
        public int Height { get; set; }

        private byte[] bgraValues;

        public Color GetPixel(int x, int y)
        {
            byte b = bgraValues[x * 4 + y * data.Stride];
            byte g = bgraValues[x * 4 + y * data.Stride + 1];
            byte r = bgraValues[x * 4 + y * data.Stride + 2];
            byte a = bgraValues[x * 4 + y * data.Stride + 3];

            return Color.FromArgb(a, r, g, b);
        }

        public void SetPixel(int x, int y, Color c)
        {
            bgraValues[x * 4 + y * data.Stride] = c.B;
            bgraValues[x * 4 + y * data.Stride + 1] = c.G;
            bgraValues[x * 4 + y * data.Stride + 2] = c.R;
            bgraValues[x * 4 + y * data.Stride + 3] = c.A;

            byte[] ColorData = { c.B, c.G, c.R, c.A }; // B G R A

            System.Windows.Int32Rect rect = new System.Windows.Int32Rect(x, y, 1, 1);

            writeableBitmap.WritePixels(rect, ColorData, 4, 0);
        }

        public void Dispose()
        {
            if (bitmap != null)
                bitmap.Dispose();
        }

        Bitmap bitmap;
        BitmapData data;

        WriteableBitmap writeableBitmap;

        private double DpiX;
        private double DpiY;
        private System.Windows.Media.PixelFormat Format;
        private BitmapPalette Palette;

        public MyBitmap(WriteableBitmap bitmapImage)
        {
            Width = bitmapImage.PixelWidth;
            Height = bitmapImage.PixelHeight;
            DpiX = bitmapImage.DpiX;
            DpiY = bitmapImage.DpiY;
            Format = bitmapImage.Format;
            Palette = bitmapImage.Palette;

            writeableBitmap = bitmapImage;
            //
            bitmap = new Bitmap(bitmapImage.PixelWidth, bitmapImage.PixelHeight, PixelFormat.Format32bppPArgb);

            data = bitmap.LockBits(new Rectangle(Point.Empty, bitmap.Size), ImageLockMode.ReadWrite, PixelFormat.Format32bppPArgb);

            bitmapImage.CopyPixels(System.Windows.Int32Rect.Empty, data.Scan0, data.Height * data.Stride, data.Stride);

            IntPtr ptr = data.Scan0;
            int bytes = data.Stride * data.Height;
            bgraValues = new byte[bytes];
            System.Runtime.InteropServices.Marshal.Copy(ptr, bgraValues, 0, bytes);

            bitmap.UnlockBits(data);
        }
    }
}
