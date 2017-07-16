using System;
using System.Collections.Specialized;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;

namespace ScreenshotX
{
    public partial class Main : Form
    {
        /*常量*/
        public const int INTERVAL = 200;//区分右键单双击的时间间隔，默认200ms

        /*状态*/
        private bool isAreaSelected;//已经选定子区域

        /*字段*/
        private Image Screenshots;//原截屏
        private Image Screenshots_Grey;//暗色原截屏
        private RectangleSelector RectangleSelected;//编辑框

        private Point PointStart;//起笔点
        private Point PointEnd;//收笔点

        /*构析函数*/
        public Main()
        {
            InitializeComponent();
            SetDoubleBuffer();//绘图优化防止闪屏

            //初始化窗体控件属性
            this.Location = new Point(0, 0);
            this.Width = Screen.PrimaryScreen.Bounds.Width;
            this.Height = Screen.PrimaryScreen.Bounds.Height;

            this.timer.Interval = INTERVAL;
        }

        /*Event - 选图编辑事件响应*/
        public void Screenshot()
        {
            //截取全屏截图
            Screenshots = new Bitmap(Screen.PrimaryScreen.Bounds.Width, Screen.PrimaryScreen.Bounds.Height);
            Graphics g = Graphics.FromImage(Screenshots);
            g.CopyFromScreen(0, 0, 0, 0, new Size(Screen.PrimaryScreen.Bounds.Width, Screen.PrimaryScreen.Bounds.Height), CopyPixelOperation.MergeCopy);
            g.Dispose();

            //拷贝成暗色的全屏截图
            Screenshots_Grey = (Bitmap)Screenshots.Clone();
            g = Graphics.FromImage(Screenshots_Grey);
            g.FillRectangle(new SolidBrush(Color.FromArgb(127, 0, 0, 0)), 0, 0, Screen.PrimaryScreen.Bounds.Width, Screen.PrimaryScreen.Bounds.Height);
            g.Dispose();

            this.BackgroundImage = Screenshots_Grey;
            this.Show();
        }
        private void Main_MouseMove(object sender, MouseEventArgs e)
        {
            if(e.Button==MouseButtons.Left)
            {
                PointEnd = e.Location;
                RectangleSelected.Change(PointStart, PointEnd);

                Bitmap Screenshot_Edit = (Bitmap)this.Screenshots_Grey.Clone();
                Graphics g = Graphics.FromImage(Screenshot_Edit);
                Size currentSelectedSize = new Size(PointEnd.X - PointStart.X, PointEnd.Y - PointStart.Y);
                //选定区域去灰色
                g.DrawImage(Screenshots, new Rectangle(PointStart.X, PointStart.Y, currentSelectedSize.Width, currentSelectedSize.Height), new Rectangle(PointStart.X, PointStart.Y, currentSelectedSize.Width, currentSelectedSize.Height), GraphicsUnit.Pixel);
                //画RectangleSelector
                RectangleSelected.Paint(g);
                g.Dispose();

                this.BackgroundImage = Screenshot_Edit;
            }
        }
        private void Main_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                GC.Collect();

                isAreaSelected = false;
                PointStart = e.Location;
                PointEnd = new Point(PointStart.X + 1, PointStart.Y + 1);//至少1个像素
                RectangleSelected = new RectangleSelector(PointStart, PointEnd);
            }
        }
        private void Main_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                isAreaSelected = true;
                PointEnd = e.Location;
                RectanglePointsAdjust(PointStart, PointEnd);
            }
        }

        /*Event - 软件运行控制*/
        private void Main_Load(object sender, EventArgs e)
        {
            Screenshot();
        }
        private void Main_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                if (!isAreaSelected)
                    this.BackgroundMode();
                else
                {
                    if (RectangleSelected.RectangleFrame.Contains(e.Location))
                    {
                        //保存到剪贴板
                        Size currentSelectedSize = new Size(PointEnd.X - PointStart.X, PointEnd.Y - PointStart.Y);
                        Bitmap Screenshot_Save = new Bitmap(currentSelectedSize.Width, currentSelectedSize.Height);
                        using (Graphics g = Graphics.FromImage(Screenshot_Save))
                        {
                            //裁剪选定区域
                            g.DrawImage(Screenshots, new Rectangle(0, 0, currentSelectedSize.Width, currentSelectedSize.Height), new Rectangle(PointStart.X < PointEnd.X ? PointStart.X : PointEnd.X, PointStart.Y < PointEnd.Y ? PointStart.Y : PointEnd.Y, currentSelectedSize.Width, currentSelectedSize.Height), GraphicsUnit.Pixel);
                        }
                        Clipboard.SetImage(Screenshot_Save);//复制到剪贴板

                        timer.Start();//倒计时关闭
                    }
                    else
                    {
                        DialogResult res = MessageBox.Show("截图尚未保存，确定要退出吗？", "放弃？", MessageBoxButtons.OKCancel);
                        if (res == DialogResult.OK)
                            BackgroundMode();
                    }
                }
            }
        }
        private void Main_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                if (!isAreaSelected)
                    this.BackgroundMode();
                else
                {
                    if (RectangleSelected.RectangleFrame.Contains(e.Location))
                    {
                        //保存到文件
                        Size currentSelectedSize = new Size(PointEnd.X - PointStart.X, PointEnd.Y - PointStart.Y);
                        Bitmap Screenshot_Save = new Bitmap(currentSelectedSize.Width, currentSelectedSize.Height);
                        using (Graphics g = Graphics.FromImage(Screenshot_Save))
                        {
                            //裁剪选定区域
                            g.DrawImage(Screenshots, new Rectangle(0, 0, currentSelectedSize.Width, currentSelectedSize.Height), new Rectangle(PointStart.X < PointEnd.X ? PointStart.X : PointEnd.X, PointStart.Y < PointEnd.Y ? PointStart.Y : PointEnd.Y, currentSelectedSize.Width, currentSelectedSize.Height), GraphicsUnit.Pixel);
                        }

                        string filename = Environment.GetEnvironmentVariable("TMP") + "/ScreenshotX截图_" + DateTime.Now.ToString("yy年MM月dd日hh时mm分ss秒") + ".png";
                        Screenshot_Save.Save(filename, ImageFormat.Png);
                        StringCollection file = new StringCollection();
                        file.Add(filename);
                        Clipboard.SetFileDropList(file);

                        this.BackgroundMode();
                    }
                    else
                    {
                        DialogResult res = MessageBox.Show("截图尚未保存，确定要退出吗？", "放弃？", MessageBoxButtons.OKCancel);
                        if (res == DialogResult.OK)
                            BackgroundMode();
                    }
                }
            }
        }
        private void notifyIcon_DoubleClick(object sender, EventArgs e)
        {
            Screenshot();
        }
        private void toolStripMenuItem_About_Click(object sender, EventArgs e)
        {
            string info = "--ScreenshotX 轻量化的截图内核--\n\n" +
                          "说明：\n" +
                          "右键单击选定区域，复制到剪贴板；\n" +
                          "右键双击选定区域，保存为临时文件，可在资源管理器中右键粘贴；\n" +
                          "右键单击选定外区域，放弃本次截图。\n\n" +
                          "作者：Kahsolt\n" +
                          "发布时间：2016年7月31日";
            MessageBox.Show(info, "关于ScreenshotX Kernel...");
        }
        private void toolStripMenuItem_Screenshot_Click(object sender, EventArgs e)
        {
            Screenshot();
        }
        private void toolStripMenuItem_Exit_Click(object sender, EventArgs e)
        {
            notifyIcon.Visible = false;
            Application.Exit();
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            timer.Stop();
            BackgroundMode();
        }

        //工具函数
        private void SetDoubleBuffer()//开启双缓冲
        {
            this.SetStyle(ControlStyles.ResizeRedraw, true);
            this.SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            this.SetStyle(ControlStyles.UserPaint, true);
            this.SetStyle(ControlStyles.SupportsTransparentBackColor, true);
        }
        private void BackgroundMode()//转为后台待命
        {
            this.FreeResources();
            this.Hide();
        }
        private void FreeResources()//资源销毁还原
        {
            this.isAreaSelected = false;
            this.Screenshots.Dispose();
            this.Screenshots_Grey.Dispose();
            this.RectangleSelected = null;
            GC.Collect();
        }

        private void RectanglePointsAdjust(Point PointStart, Point PointEnd)
        {
            int width = PointEnd.X - PointStart.X;
            int height = PointEnd.Y - PointStart.Y;

            if (width > 0 && height > 0)//PointEnd在右下
                return;
            else if (width > 0 && height < 0)//PointEnd在右上
            {
                this.PointStart.Y = PointEnd.Y;
                this.PointEnd.Y = PointStart.Y;
            }
            else if (width < 0 && height > 0)//PointEnd在左下
            {
                this.PointStart.X = PointEnd.X;
                this.PointEnd.X = PointStart.X;
            }
            else ////PointEnd在左上
            {
                this.PointStart.X = PointEnd.X;
                this.PointEnd.X = PointStart.X;
                this.PointStart.Y = PointEnd.Y;
                this.PointEnd.Y = PointStart.Y;
            }
        }

    }
}
