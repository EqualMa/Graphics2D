using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Graphics2D
{
    class MyLine
    {
        private MyPoint start;
        private MyPoint end;

        private readonly bool kAbsIsSmallerThanOne = false;//k的绝对值小于等于1
        private readonly int kComparedToZero;//如果k为正数则此值为正数；如果k为负数则此值为负数；如果k为0则此值为0。

        /// <summary>
        /// 当前要绘制的点，如果还未开始为null，如果全部完成为结束点
        /// </summary>
        private MyPoint currentPoint = null;

        private int ei = -1;

        private readonly int deltaX;
        private readonly int deltaY;

        public MyLine(MyPoint start, MyPoint end)
        {
            //判断k绝对值是否小于等于1
            if (Math.Abs(end.Y - start.Y) <= Math.Abs(end.X - start.X))
                kAbsIsSmallerThanOne = true;

            //如果k绝对值小于等于1，直接初始化起点和终点。否则，交换x和y坐标
            if (kAbsIsSmallerThanOne)
            {
                this.start = start;
                this.end = end;
            }
            else
            {
                this.start = start.Reverse;
                this.end = end.Reverse;
            }

            //保证起点的x值比终点x值小（如果不是，则交换起点和终点）
            if (this.start.X > this.end.X)
            {
                var temp = this.start;
                this.start = this.end;
                this.end = temp;
            }

            //保证终点的y值比起点y值大（如果不是，则关于x轴做对称）
            if (this.start.Y > this.end.Y)
            {
                kComparedToZero = -1;
                this.start = this.start.YInverse;
                this.end = this.end.YInverse;
            }
            else if (this.start.Y == this.end.Y)
            {
                kComparedToZero = 0;
            }
            else
            {
                kComparedToZero = 1;
            }

            deltaX = this.end.X - this.start.X;
            deltaY = this.end.Y - this.start.Y;
            ei = 2 * deltaY - deltaX;
        }

        public MyPoint getNextPoint()
        {
            //还未开始
            if (currentPoint == null)
            {
                currentPoint = start;
            }

            //已经全部计算完毕，返回null
            else if (currentPoint.X == end.X && currentPoint.Y == end.Y) return null;

            //已经开始，但是还没计算完毕
            else
            {
                if (ei >= 0)
                {
                    ei = ei + 2 * deltaY - 2 * deltaX;
                    currentPoint = new MyPoint(currentPoint.X + 1, currentPoint.Y + 1);
                }
                else
                {
                    ei = ei + 2 * deltaY;
                    currentPoint = new MyPoint(currentPoint.X + 1, currentPoint.Y);
                }
            }

            MyPoint result = currentPoint;
            if (kComparedToZero < 0) result = result.YInverse;
            if (!kAbsIsSmallerThanOne) result = result.Reverse;
            return result;
        }

        public static MyLine[] getRandomLines(int minCount, int maxCount, int maxWidth, int maxHeight)
        {
            int count;
            Random random = new Random();

            if (minCount <= 1) minCount = 1;
            if (maxCount <= minCount) count = minCount;
            else count = random.Next(minCount, maxCount + 1);

            MyLine[] lines = new MyLine[count];
            int sx, sy, ex, ey;
            for (int i = 0; i < count; i++)
            {
                sx = random.Next(maxWidth);
                ex = random.Next(maxWidth);
                sy = random.Next(maxHeight);
                ey = random.Next(maxHeight);

                lines[i] = new MyLine(new MyPoint(sx, sy), new MyPoint(ex, ey));
            }

            return lines;
        }
    }

    class MyPoint
    {
        public int X;
        public int Y;

        public MyPoint(int x, int y)
        {
            X = x;
            Y = y;
        }

        public MyPoint Reverse
        {
            get
            {
                return new MyPoint(Y, X);
            }
        }

        public MyPoint YInverse
        {
            get
            {
                return new MyPoint(X, -Y);
            }
        }
    }
}
