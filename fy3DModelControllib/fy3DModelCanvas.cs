using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using Kitware.VTK;
using soy.Algorithms.Geometry;

namespace fy3DModelControllib
{
    public partial class fy3DModelCanvas : UserControl
    {
        [System.Runtime.InteropServices.DllImport("user32")]
        private static extern int SendMessage(IntPtr hwnd, int wMsg, int wParam, IntPtr lParam);
        private const int WM_SETREDRAW = 0xB;

        //相机的全局参数
        double[] CameraFocalPoint = null;
        double[] CameraPoint = null;
        double[] CameraViewUp = null;

        //fyGrid对象
        ExGrid m_Grid = null;

        //UpdatefyGrid消息委托
        public delegate void UpdatefyGridEventHandler();
        public event UpdatefyGridEventHandler UpdatefyGridEvent;

        //I方向网格数量
        public int ICount
        {
            get
            {
                return m_Grid.ICount;
            }
        }
        //J方向网格数量
        public int JCount
        {
            get
            {
                return m_Grid.JCount;
            }
        }
        //K方向网格数量
        public int KCount
        {
            get
            {
                return m_Grid.KCount;
            }
        }

        //最小值
        public double MinValue
        {
            get
            {
                return m_Grid.Min;
            }
        }
        //最大值
        public double MaxValue
        {
            get
            {
                return m_Grid.Max;
            }
        }

        public fy3DModelCanvas()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 更新fyGrid
        /// </summary>
        /// <param name="Grid"></param>
        public void UpdatefyGrid(ExGrid Grid, double ValueOfNull = -9999.9999)
        {
            if (Grid == null)
            {
                MessageBox.Show("来自fy3DModelCanvas的警告\nfyGrid是Null！", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            //对Grid的null值进行处理
            for (int i = 0; i < Grid.CellCount; i++)
            {
                //如果Cell是null，则赋予为ValueOfNull
                Grid.SetCell(i, Grid.GetCell(i) == null ? ValueOfNull : Grid.GetCell(i));
            }
            m_Grid = Grid;
            //发出数据更新消息
            UpdatefyGridEvent();
        }

        //普通模式观察模型
        public void LookAt_NormalMode(fy3DModelViewMode ViewMode)
        {
            switch (ViewMode)
            {
                case fy3DModelViewMode.FaceViewMode:
                    CameraFocalPoint = new double[] { 0, 0, 0 };
                    CameraPoint = new double[] { 1, 0, 0 };
                    CameraViewUp = new double[] { 0, 0, 1 };
                    break;
                case fy3DModelViewMode.TopViewMode:
                    CameraFocalPoint = new double[] { 0, 0, 0 };
                    CameraPoint = new double[] { 0, 0, 1 };
                    CameraViewUp = new double[] { 0, 1, 0 };
                    break;
                case fy3DModelViewMode.SideViewMode:
                    CameraFocalPoint = new double[] { 0, 0, 0 };
                    CameraPoint = new double[] { 0, 1, 0 };
                    CameraViewUp = new double[] { 0, 0, 1 };
                    break;
                case fy3DModelViewMode.None:
                    if (this.Controls.Count == 1)
                    {
                        (this.Controls[0] as fy3DModelPage).GetCameraParameters(ref CameraFocalPoint, ref CameraPoint, ref CameraViewUp);
                    }
                    break;
            }

            GC.Collect();

            SendMessage(this.Handle, WM_SETREDRAW, 0, IntPtr.Zero);//禁止重绘

            this.Controls.Clear();
            // 重新布局
            fy3DModelPage control = new fy3DModelPage(m_Grid, CameraFocalPoint, CameraPoint, CameraViewUp);
            control.Size = this.Size;
            control.Dock = DockStyle.Fill;
            control.Location = new System.Drawing.Point(0, 0);
            this.Controls.Add(control);

            SendMessage(this.Handle, WM_SETREDRAW, 1, IntPtr.Zero);//取消禁止
            GC.Collect();

            this.Refresh();
        }

        //切片模式观察模型
        public void LookAt_SliceMode(int XPos, int YPos, int ZPos, fy3DModelViewMode ViewMode)
        {
            switch (ViewMode)
            {
                case fy3DModelViewMode.FaceViewMode:
                    CameraFocalPoint = new double[] { 0, 0, 0 };
                    CameraPoint = new double[] { 1, 0, 0 };
                    CameraViewUp = new double[] { 0, 0, 1 };
                    break;
                case fy3DModelViewMode.TopViewMode:
                    CameraFocalPoint = new double[] { 0, 0, 0 };
                    CameraPoint = new double[] { 0, 0, 1 };
                    CameraViewUp = new double[] { 0, 1, 0 };
                    break;
                case fy3DModelViewMode.SideViewMode:
                    CameraFocalPoint = new double[] { 0, 0, 0 };
                    CameraPoint = new double[] { 0, 1, 0 };
                    CameraViewUp = new double[] { 0, 0, 1 };
                    break;
                case fy3DModelViewMode.None:
                    if (this.Controls.Count == 1)
                    {
                        (this.Controls[0] as fy3DModelPage).GetCameraParameters(ref CameraFocalPoint, ref CameraPoint, ref CameraViewUp);
                    }
                    break;
            }

            GC.Collect();

            SendMessage(this.Handle, WM_SETREDRAW, 0, IntPtr.Zero);//禁止重绘

            this.Controls.Clear();
            // 重新布局
            fy3DModelPage control = new fy3DModelPage(m_Grid, CameraFocalPoint, CameraPoint, CameraViewUp, XPos, YPos, ZPos);
            control.Size = this.Size;
            control.Dock = DockStyle.Fill;
            control.Location = new System.Drawing.Point(0, 0);
            this.Controls.Add(control);
            this.Validate();
            SendMessage(this.Handle, WM_SETREDRAW, 1, IntPtr.Zero);//取消禁止
            //刷新，必须滴
            this.Refresh();

            GC.Collect();
        }

        //镂空模式观察模型
        public void LookAt_HollowOutMode(List<fyTransparentRange> TransparentRanges, fy3DModelViewMode ViewMode)
        {
            switch (ViewMode)
            {
                case fy3DModelViewMode.FaceViewMode:
                    CameraFocalPoint = new double[] { 0, 0, 0 };
                    CameraPoint = new double[] { 1, 0, 0 };
                    CameraViewUp = new double[] { 0, 0, 1 };
                    break;
                case fy3DModelViewMode.TopViewMode:
                    CameraFocalPoint = new double[] { 0, 0, 0 };
                    CameraPoint = new double[] { 0, 0, 1 };
                    CameraViewUp = new double[] { 0, 1, 0 };
                    break;
                case fy3DModelViewMode.SideViewMode:
                    CameraFocalPoint = new double[] { 0, 0, 0 };
                    CameraPoint = new double[] { 0, 1, 0 };
                    CameraViewUp = new double[] { 0, 0, 1 };
                    break;
                case fy3DModelViewMode.None:
                    if (this.Controls.Count == 1)
                    {
                        (this.Controls[0] as fy3DModelPage).GetCameraParameters(ref CameraFocalPoint, ref CameraPoint, ref CameraViewUp);
                    }
                    break;
            }

            GC.Collect();

            SendMessage(this.Handle, WM_SETREDRAW, 0, IntPtr.Zero);//禁止重绘

            this.Controls.Clear();
            // 重新布局
            fy3DModelPage control = new fy3DModelPage(m_Grid, CameraFocalPoint, CameraPoint, CameraViewUp, TransparentRanges);
            control.Size = this.Size;
            control.Dock = DockStyle.Fill;
            control.Location = new System.Drawing.Point(0, 0);
            this.Controls.Add(control);

            SendMessage(this.Handle, WM_SETREDRAW, 1, IntPtr.Zero);//取消禁止

            GC.Collect();

            this.Refresh();
        }

    }
}
