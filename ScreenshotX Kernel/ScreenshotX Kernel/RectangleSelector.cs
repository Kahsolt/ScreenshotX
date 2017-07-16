using System.Drawing;

namespace ScreenshotX
{
    class RectangleSelector
    {
        /*预定义*/
        public const int QUADRANT_SIZE = 5;
        public const int QUADRANT_OFFSET = QUADRANT_SIZE / 2;

        /*字段*/
        public Rectangle RectangleFrame;//主矩形
        public Rectangle[] RectangleQuadrant;//8个象限点,左上到右下，列优先变换
        
        /*操作函数*/
        public RectangleSelector(Point PointStart, Point PointEnd)
        {
            //主矩形初始化
            Points2Rectangle(PointStart, PointEnd);

            //象限点初始化
            this.RectangleQuadrant = new Rectangle[8];
            Size RectangleQuadrantSize = new Size(QUADRANT_SIZE, QUADRANT_SIZE);//象限点大小
            this.RectangleQuadrant[0] = new Rectangle(new Point(RectangleFrame.X - QUADRANT_OFFSET, RectangleFrame.Y - QUADRANT_OFFSET), RectangleQuadrantSize);
            this.RectangleQuadrant[1] = new Rectangle(new Point(RectangleFrame.X + RectangleFrame.Width / 2 - QUADRANT_OFFSET, RectangleFrame.Y - QUADRANT_OFFSET), RectangleQuadrantSize);
            this.RectangleQuadrant[2] = new Rectangle(new Point(RectangleFrame.X + RectangleFrame.Width - QUADRANT_OFFSET, RectangleFrame.Y - QUADRANT_OFFSET), RectangleQuadrantSize);
            this.RectangleQuadrant[3] = new Rectangle(new Point(RectangleFrame.X - QUADRANT_OFFSET, RectangleFrame.Y + RectangleFrame.Height / 2 - QUADRANT_OFFSET), RectangleQuadrantSize);
            this.RectangleQuadrant[4] = new Rectangle(new Point(RectangleFrame.X + RectangleFrame.Width - QUADRANT_OFFSET, RectangleFrame.Y + RectangleFrame.Height / 2 - QUADRANT_OFFSET), RectangleQuadrantSize);
            this.RectangleQuadrant[5] = new Rectangle(new Point(RectangleFrame.X - QUADRANT_OFFSET, RectangleFrame.Y + RectangleFrame.Height - QUADRANT_OFFSET), RectangleQuadrantSize);
            this.RectangleQuadrant[6] = new Rectangle(new Point(RectangleFrame.X + RectangleFrame.Width / 2 - QUADRANT_OFFSET, RectangleFrame.Y + RectangleFrame.Height - QUADRANT_OFFSET), RectangleQuadrantSize);
            this.RectangleQuadrant[7] = new Rectangle(new Point(RectangleFrame.X + RectangleFrame.Width - QUADRANT_OFFSET, RectangleFrame.Y + RectangleFrame.Height - QUADRANT_OFFSET), RectangleQuadrantSize);
        }
        public void Change(Point PointStart, Point PointEnd)
        {
            //更新主矩形位置
            Points2Rectangle(PointStart, PointEnd);

            //更新象限点位置(重新推算)
            this.RectangleQuadrant[0].X = this.RectangleFrame.X                                     - QUADRANT_OFFSET;
            this.RectangleQuadrant[0].Y = this.RectangleFrame.Y                                     - QUADRANT_OFFSET;
            this.RectangleQuadrant[1].X = this.RectangleFrame.X + this.RectangleFrame.Width / 2     - QUADRANT_OFFSET;
            this.RectangleQuadrant[1].Y = this.RectangleFrame.Y                                     - QUADRANT_OFFSET;
            this.RectangleQuadrant[2].X = this.RectangleFrame.X + this.RectangleFrame.Width         - QUADRANT_OFFSET;
            this.RectangleQuadrant[2].Y = this.RectangleFrame.Y                                     - QUADRANT_OFFSET;
            this.RectangleQuadrant[3].X = this.RectangleFrame.X                                     - QUADRANT_OFFSET;
            this.RectangleQuadrant[3].Y = this.RectangleFrame.Y + this.RectangleFrame.Height / 2    - QUADRANT_OFFSET;
            this.RectangleQuadrant[4].X = this.RectangleFrame.X + this.RectangleFrame.Width         - QUADRANT_OFFSET;
            this.RectangleQuadrant[4].Y = this.RectangleFrame.Y + this.RectangleFrame.Height / 2    - QUADRANT_OFFSET;
            this.RectangleQuadrant[5].X = this.RectangleFrame.X                                     - QUADRANT_OFFSET;
            this.RectangleQuadrant[5].Y = this.RectangleFrame.Y + this.RectangleFrame.Height        - QUADRANT_OFFSET;
            this.RectangleQuadrant[6].X = this.RectangleFrame.X + this.RectangleFrame.Width / 2     - QUADRANT_OFFSET;
            this.RectangleQuadrant[6].Y = this.RectangleFrame.Y + this.RectangleFrame.Height        - QUADRANT_OFFSET;
            this.RectangleQuadrant[7].X = this.RectangleFrame.X + this.RectangleFrame.Width         - QUADRANT_OFFSET;
            this.RectangleQuadrant[7].Y = this.RectangleFrame.Y + this.RectangleFrame.Height        - QUADRANT_OFFSET;
        }
        public void Paint(Graphics g)
        {
            //画主矩形
            Pen p = new Pen(Color.Orange);
            p.DashStyle = System.Drawing.Drawing2D.DashStyle.Dash;
            g.DrawRectangle(p, this.RectangleFrame);

            //画象限点
            g.FillRectangles(Brushes.OrangeRed, RectangleQuadrant);
        }

        //工具函数
        private void Points2Rectangle(Point PointStart, Point PointEnd)
        {
            int width = PointEnd.X - PointStart.X;
            int height = PointEnd.Y - PointStart.Y;

            if (width > 0 && height > 0)//PointEnd在右下
                this.RectangleFrame = new Rectangle(PointStart.X, PointStart.Y, width, height);
            else if (width > 0 && height < 0)//PointEnd在右上
                this.RectangleFrame = new Rectangle(PointStart.X, PointEnd.Y, width, -height);
            else if (width < 0 && height > 0)//PointEnd在左下
                this.RectangleFrame = new Rectangle(PointEnd.X, PointStart.Y, -width, height);
            else ////PointEnd在左上
                this.RectangleFrame = new Rectangle(PointEnd.X, PointEnd.Y, -width, -height);
        }
    }
}
