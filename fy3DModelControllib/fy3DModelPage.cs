using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Kitware.VTK;
using soy.Algorithms.Geometry;
using soy.Algorithms.Numerics;

namespace fy3DModelControllib
{
    /// <summary>
    /// 3D模型显示页面，添加进fy3DModelCanvas进行显示
    /// 
    /// Author:喻思羽
    /// Time：2015-2-17
    /// Place:枣阳
    /// </summary>
    internal partial class fy3DModelPage : UserControl
    {
        //显示模式
        fy3DModelPageMode m_PageMode = fy3DModelPageMode.Normal_Mode;
        //数据缓冲对象
        vtkImageData m_ImageData = null;
        //渲染对象
        vtkRenderer m_Renderer = null;

        #region 颜色与透明度

        vtkLookupTable m_LookupTable = null;
        vtkColorTransferFunction m_ColorTransferFunction = null;
        vtkPiecewiseFunction m_PiecewiseFunction = null;

        #endregion

        #region 坐标轴

        vtkRenderWindowInteractor m_RenderWindowInteractor = null;
        vtkAxesActor m_AxesActor = null;
        vtkOrientationMarkerWidget m_OrientationMarkerWidget = null;

        #endregion

        #region 相机参数

        double[] m_CameraFocalPoint = null;
        double[] m_CameraPoint = null;
        double[] m_CameraViewUp = null;

        #endregion

        #region -------------------普通模式-------------------

        public fy3DModelPage(ExGrid Grid, double[] CameraFocalPoint, double[] CameraPoint, double[] CameraViewUp)
        {
            InitializeComponent();

            m_PageMode = fy3DModelPageMode.Normal_Mode;

            m_CameraFocalPoint = CameraFocalPoint;
            m_CameraPoint = CameraPoint;
            m_CameraViewUp = CameraViewUp;

            m_ImageData = ConvertfyGrid2vtkImageData(Grid);
            //初始化渲染的颜色和透明度
            InitializeColor_Normal(Grid);
        }

        //普通模式_初始化颜色
        private void InitializeColor_Normal(ExGrid Grid)
        {
            m_LookupTable = vtkLookupTable.New();
            m_LookupTable.SetTableRange(Grid.Min, Grid.Max);
            //蓝色->红色
            m_LookupTable.SetHueRange(0.667, 0);
            m_LookupTable.SetNumberOfColors(100);
            m_LookupTable.Build();

            //设定标量值的颜色 
            m_ColorTransferFunction = vtkColorTransferFunction.New();
            for (int i = 0; i < 100; i += 10)
            {
                var color = m_LookupTable.GetTableValue(i);
                double Range = Grid.Max - Grid.Min;
                m_ColorTransferFunction.AddRGBPoint(Grid.Min + i * Range / 100.0, color[0], color[1], color[2]);
            }
            m_ColorTransferFunction.Build();

            //线性插值透明度  
            m_PiecewiseFunction = vtkPiecewiseFunction.New();
            m_PiecewiseFunction.AddPoint(Grid.Min, 1);
            m_PiecewiseFunction.AddPoint(Grid.Max, 1);
        }

        //普通模式_绘制
        private void DrawMode_Normal()
        {
            #region 体属性 vtkVolumeProperty
            //设定体数据的属性:不透明性和颜色值映射标量值 
            vtkVolumeProperty volumeProperty = vtkVolumeProperty.New();
            volumeProperty.SetColor(m_ColorTransferFunction);
            volumeProperty.SetScalarOpacity(m_PiecewiseFunction);
            //设置插值类型
            volumeProperty.SetInterpolationTypeToNearest();
            volumeProperty.SetDiffuse(0.7);
            volumeProperty.SetAmbient(0.01);
            volumeProperty.SetSpecular(0.5);
            volumeProperty.SetSpecularPower(100);
            #endregion

            //绘制方法:体射线投射
            vtkVolumeRayCastCompositeFunction compositeFunction = vtkVolumeRayCastCompositeFunction.New();

            #region 体数据映射器 vtkVolumeRayCastMapper
            //体数据映射器  
            vtkVolumeRayCastMapper volumeMapper = vtkVolumeRayCastMapper.New();
            volumeMapper.SetInput(m_ImageData);
            volumeMapper.SetVolumeRayCastFunction(compositeFunction);
            #endregion

            #region 体 vtkVolume
            //体
            vtkVolume volume = vtkVolume.New();
            volume.SetMapper(volumeMapper);
            volume.SetProperty(volumeProperty);
            #endregion

            //模型体放入Renerer
            m_Renderer.AddVolume(volume);
        }

        #endregion

        #region -------------------切片模式-------------------

        #region 变量

        //-------------------XOY切片(垂直Z轴)-------------------
        vtkImageReslice m_ImageResliceXOY = null;
        vtkTexture m_TextureXOY = null;
        vtkPlaneSource m_PlaneSourceXOY = null;
        vtkPolyDataMapper m_PlaneMapperXOY = null;
        vtkActor m_ActorXOY = null;

        //-------------------XOZ切片(垂直Y轴)-------------------
        vtkImageReslice m_ImageResliceXOZ = null;
        vtkTexture m_TextureXOZ = null;
        vtkPlaneSource m_PlaneSourceXOZ = null;
        vtkPolyDataMapper m_PlaneMapperXOZ = null;
        vtkActor m_ActorXOZ = null;

        //-------------------YOZ切片(垂直X轴)-------------------
        vtkImageReslice m_ImageResliceYOZ = null;
        vtkTexture m_TextureYOZ = null;
        vtkPlaneSource m_PlaneSourceYOZ = null;
        vtkPolyDataMapper m_PlaneMapperYOZ = null;
        vtkActor m_ActorYOZ = null;

        int m_XPos = 0;
        int m_YPos = 0;
        int m_ZPos = 0;

        #endregion

        public fy3DModelPage(ExGrid Grid, double[] CameraFocalPoint, double[] CameraPoint, double[] CameraViewUp,
            int XPos, int YPos, int ZPos)
        {
            InitializeComponent();

            m_PageMode = fy3DModelPageMode.Slice_Mode;

            m_CameraFocalPoint = CameraFocalPoint;
            m_CameraPoint = CameraPoint;
            m_CameraViewUp = CameraViewUp;

            m_XPos = XPos;
            m_YPos = YPos;
            m_ZPos = ZPos;

            m_ImageData = ConvertfyGrid2vtkImageData(Grid);

            InitializeColor_Slice(Grid);
        }

        //切片模式_初始化颜色
        private void InitializeColor_Slice(ExGrid Grid)
        {
            m_LookupTable = vtkLookupTable.New();
            m_LookupTable.SetTableRange(Grid.Min, Grid.Max);
            //蓝色->红色
            m_LookupTable.SetHueRange(0.667, 0);
            m_LookupTable.SetNumberOfColors(100);
            m_LookupTable.Build();

            //线性插值透明度  
            m_PiecewiseFunction = vtkPiecewiseFunction.New();
            m_PiecewiseFunction.AddPoint(Grid.Min, 1);
            m_PiecewiseFunction.AddPoint(Grid.Max, 1);
        }

        //切片模式_绘制
        private void DrawMode_Slice()
        {
            int[] xyz = m_ImageData.GetDimensions();
            double[] sp = m_ImageData.GetSpacing();

            double[] pos = new double[] { m_XPos, m_YPos, m_ZPos };

            #region -------------------XOY切片(垂直Z轴)-------------------

            m_ImageResliceXOY = vtkImageReslice.New();
            m_ImageResliceXOY.SetInput(m_ImageData);
            m_ImageResliceXOY.SetResliceAxesDirectionCosines(
                1, 0, 0,
                0, 1, 0,
                0, 0, 1
                );
            m_ImageResliceXOY.InterpolateOn();
            m_ImageResliceXOY.SetInterpolationModeToNearestNeighbor();
            m_ImageResliceXOY.SetResliceAxesOrigin(pos[0], pos[1], pos[2]);
            m_ImageResliceXOY.SetOutputDimensionality(2);
            m_ImageResliceXOY.Update();

            m_TextureXOY = vtkTexture.New();
            m_TextureXOY.InterpolateOff();
            m_TextureXOY.SetInput(m_ImageResliceXOY.GetOutput());
            m_TextureXOY.SetLookupTable(m_LookupTable);
            m_TextureXOY.MapColorScalarsThroughLookupTableOn();

            //---------------------set plane position----------
            m_PlaneSourceXOY = vtkPlaneSource.New();
            m_PlaneSourceXOY.SetXResolution(xyz[0]);
            m_PlaneSourceXOY.SetYResolution(xyz[1]);
            m_PlaneSourceXOY.SetOrigin(0, 0, 0);
            m_PlaneSourceXOY.SetPoint1((xyz[0] - 1) * sp[0], 0, 0);
            m_PlaneSourceXOY.SetPoint2(0, (xyz[1] - 1) * sp[1], 0);
            m_PlaneSourceXOY.Push(pos[2]);

            //---------------------pipeline--------------------
            m_PlaneMapperXOY = vtkPolyDataMapper.New();
            m_PlaneMapperXOY.SetInput(m_PlaneSourceXOY.GetOutput());

            m_ActorXOY = vtkActor.New();
            m_ActorXOY.SetMapper(m_PlaneMapperXOY);
            m_ActorXOY.SetTexture(m_TextureXOY);

            #endregion

            #region -------------------XOZ切片(垂直Y轴)-------------------

            m_ImageResliceXOZ = vtkImageReslice.New();
            m_ImageResliceXOZ.SetInput(m_ImageData);
            m_ImageResliceXOZ.SetResliceAxesDirectionCosines(
                            1, 0, 0,
                            0, 0, -1,
                            0, 1, 0
                );
            m_ImageResliceXOZ.InterpolateOn();
            m_ImageResliceXOZ.SetInterpolationModeToNearestNeighbor();
            m_ImageResliceXOZ.SetResliceAxesOrigin(pos[0], pos[1], pos[2]);
            m_ImageResliceXOZ.SetOutputDimensionality(2);
            m_ImageResliceXOZ.Update();

            m_TextureXOZ = vtkTexture.New();
            m_TextureXOZ.SetInput(m_ImageResliceXOZ.GetOutput());
            m_TextureXOZ.SetLookupTable(m_LookupTable);
            m_TextureXOZ.MapColorScalarsThroughLookupTableOn();

            //---------------------set plane position--------------------
            m_PlaneSourceXOZ = vtkPlaneSource.New();
            m_PlaneSourceXOZ.SetXResolution(xyz[0]);
            m_PlaneSourceXOZ.SetYResolution(xyz[2]);
            m_PlaneSourceXOZ.SetOrigin(0, 0, (xyz[2] - 1) * sp[2]);
            m_PlaneSourceXOZ.SetPoint1((xyz[0] - 1) * sp[0], 0, (xyz[2] - 1) * sp[2]);
            m_PlaneSourceXOZ.SetPoint2(0, 0, 0);
            m_PlaneSourceXOZ.Push(pos[1]);

            //---------------------pipeline------------------------------
            m_PlaneMapperXOZ = vtkPolyDataMapper.New();
            m_PlaneMapperXOZ.SetInput(m_PlaneSourceXOZ.GetOutput());

            m_ActorXOZ = vtkActor.New();
            m_ActorXOZ.SetMapper(m_PlaneMapperXOZ);
            m_ActorXOZ.SetTexture(m_TextureXOZ);

            #endregion

            #region -------------------YOZ切片(垂直X轴)-------------------

            m_ImageResliceYOZ = vtkImageReslice.New();
            m_ImageResliceYOZ.SetInput(m_ImageData);
            m_ImageResliceYOZ.SetResliceAxesDirectionCosines(
                            0, 0, -1,
                            0, 1, 0,
                            1, 0, 0
                );
            m_ImageResliceYOZ.InterpolateOn();
            m_ImageResliceYOZ.SetInterpolationModeToNearestNeighbor();
            m_ImageResliceYOZ.SetResliceAxesOrigin(pos[0], pos[1], pos[2]);
            m_ImageResliceYOZ.SetOutputDimensionality(2);
            m_ImageResliceYOZ.Update();

            m_TextureYOZ = vtkTexture.New();
            m_TextureYOZ.SetInput(m_ImageResliceYOZ.GetOutput());
            m_TextureYOZ.SetLookupTable(m_LookupTable);
            m_TextureYOZ.MapColorScalarsThroughLookupTableOn();

            //---------------------set plane position--------------------
            m_PlaneSourceYOZ = vtkPlaneSource.New();
            m_PlaneSourceYOZ.SetXResolution(xyz[2]);
            m_PlaneSourceYOZ.SetYResolution(xyz[1]);
            m_PlaneSourceYOZ.SetOrigin(0, 0, (xyz[2] - 1) * sp[2]);
            m_PlaneSourceYOZ.SetPoint1(0, 0, 0);
            m_PlaneSourceYOZ.SetPoint2(0, (xyz[1] - 1) * sp[1], (xyz[2] - 1) * sp[2]);
            m_PlaneSourceYOZ.Push(pos[0]);

            //---------------------pipeline------------------------------
            m_PlaneMapperYOZ = vtkPolyDataMapper.New();
            m_PlaneMapperYOZ.SetInput(m_PlaneSourceYOZ.GetOutput());

            m_ActorYOZ = vtkActor.New();
            m_ActorYOZ.SetMapper(m_PlaneMapperYOZ);
            m_ActorYOZ.SetTexture(m_TextureYOZ);

            #endregion

            m_Renderer.AddActor(m_ActorXOY);
            m_Renderer.AddActor(m_ActorXOZ);
            m_Renderer.AddActor(m_ActorYOZ);
        }

        #endregion

        #region -------------------镂空模式-------------------

        //fyGrid数据的范围
        fyTransparentRange m_Range = null;
        //用户设置的范围（将进行预处理）
        List<fyTransparentRange> m_UserRanges = null;
        //最终使用的镂空
        List<fyTransparentRange> m_IntersectRanges = null;

        //镂空使用的变量
        //透明度值
        double Transparency_Yes = 0.015;//透明的
        double Transparency_Not = 1;//不透明的
        //控制变量
        double EPSILON = 0.000000001;//最小值E

        public fy3DModelPage(ExGrid Grid, double[] CameraFocalPoint, double[] CameraPoint, double[] CameraViewUp,
            List<fyTransparentRange> TransparentRanges)
        {
            InitializeComponent();

            m_PageMode = fy3DModelPageMode.HollowOut_Mode;

            m_CameraFocalPoint = CameraFocalPoint;
            m_CameraPoint = CameraPoint;
            m_CameraViewUp = CameraViewUp;

            m_ImageData = ConvertfyGrid2vtkImageData(Grid);

            m_Range = new fyTransparentRange();
            m_Range.Min = Grid.Min;
            m_Range.Max = Grid.Max;

            m_UserRanges = TransparentRanges;

            //初始化渲染的颜色和透明度
            InitializeColor_HollowOut(Grid);

        }

        //镂空模式_初始化颜色
        private void InitializeColor_HollowOut(ExGrid Grid)
        {
            m_LookupTable = vtkLookupTable.New();
            m_LookupTable.SetTableRange(Grid.Min, Grid.Max);
            //蓝色->红色
            m_LookupTable.SetHueRange(0.666667, 0);
            m_LookupTable.SetNumberOfColors(100);
            m_LookupTable.Build();

            //设定标量值的颜色 
            m_ColorTransferFunction = vtkColorTransferFunction.New();
            for (int i = 0; i < 100; i += 10)
            {
                var color = m_LookupTable.GetTableValue(i);
                double Range = Grid.Max - Grid.Min;
                m_ColorTransferFunction.AddRGBPoint(Grid.Min + i * Range / 100.0, color[0], color[1], color[2]);
            }
            m_ColorTransferFunction.Build();

            //线性插值透明度
            m_PiecewiseFunction = vtkPiecewiseFunction.New();
            // [,)
            InitRanges();

            foreach (var range in m_IntersectRanges)
            {
                double Min = range.Min;
                double Max = range.Max;
                ////由于VTK的特性，对Max进行修正
                //如果透明
                if (range.Transparent)
                {
                    m_PiecewiseFunction.AddSegment(Min, Transparency_Yes, Max, Transparency_Yes);
                }
                //不透明
                else
                {
                    Min = Min + EPSILON;
                    Max = Max - EPSILON;
                    m_PiecewiseFunction.AddSegment(Min, Transparency_Not, Max, Transparency_Not);
                }
            }
        }

        void InitRanges()
        {
            //实例化透明度交集
            m_IntersectRanges = new List<fyTransparentRange>();

            //fyGrid数据的范围
            List<Interval> AllIntervals = new List<Interval>();
            AllIntervals.Add(new Interval(m_Range.Min, m_Range.Max));

            //用户设置的范围
            List<Interval> UserIntervals = new List<Interval>();
            foreach (var range in m_UserRanges)
            {
                double Min = range.Min;
                double Max = range.Max;
                //如果range的Min等于Max，即范围仅仅是一个点，那么对其进行修正
                //对Min加EPSILON，同时对Max减EPSILON，使其成为一个有效的范围
                if (range.Min == range.Max)
                {
                    Min = Min + EPSILON;
                    Max = Max - EPSILON;
                }
                UserIntervals.Add(new Interval(Min, Max));
            }

            //用户设置的范围补集
            List<Interval> UserIntervals_补集 = Interval.Complement(UserIntervals);

            //计算透明的范围
            var IntersectIntervals_透明 = Interval.Intersection(AllIntervals, UserIntervals);
            //计算不透明的范围
            var IntersectIntervals_不透明 = Interval.Intersection(AllIntervals, UserIntervals_补集);

            foreach (var interval in IntersectIntervals_透明)
            {
                if (interval.lowerbound == interval.upperbound) continue;
                fyTransparentRange range = new fyTransparentRange();
                range.Min = interval.lowerbound;
                range.Max = interval.upperbound;
                range.Transparent = true;
                m_IntersectRanges.Add(range);
            }
            foreach (var interval in IntersectIntervals_不透明)
            {
                if (interval.lowerbound == interval.upperbound) continue;
                fyTransparentRange range = new fyTransparentRange();
                range.Min = interval.lowerbound;
                range.Max = interval.upperbound;
                range.Transparent = false;
                m_IntersectRanges.Add(range);
            }
            //进行排序
            m_IntersectRanges = m_IntersectRanges.OrderBy(a => a.Min).ThenBy(a => a.Max).ToList();
        }

        //镂空模式_绘制
        private void DrawMode_HollowOut()
        {
            #region 体属性 vtkVolumeProperty
            //设定体数据的属性:不透明性和颜色值映射标量值 
            vtkVolumeProperty volumeProperty = vtkVolumeProperty.New();
            volumeProperty.SetColor(m_ColorTransferFunction);
            volumeProperty.SetScalarOpacity(m_PiecewiseFunction);
            //设置插值类型
            volumeProperty.SetInterpolationTypeToNearest();
            volumeProperty.SetDiffuse(0.7);
            volumeProperty.SetAmbient(0.01);
            volumeProperty.SetSpecular(0.5);
            volumeProperty.SetSpecularPower(100);
            #endregion

            //绘制方法:体射线投射
            vtkVolumeRayCastCompositeFunction compositeFunction = vtkVolumeRayCastCompositeFunction.New();

            #region 体数据映射器 vtkVolumeRayCastMapper
            //体数据映射器  
            vtkVolumeRayCastMapper volumeMapper = vtkVolumeRayCastMapper.New();
            volumeMapper.SetInput(m_ImageData);
            volumeMapper.SetVolumeRayCastFunction(compositeFunction);
            #endregion

            #region 体 vtkVolume
            //体
            vtkVolume volume = vtkVolume.New();
            volume.SetMapper(volumeMapper);
            volume.SetProperty(volumeProperty);
            #endregion

            //模型体放入Renerer
            m_Renderer.AddVolume(volume);
        }

        #endregion

        private void fy3DModelPage_Load(object sender, EventArgs e)
        {
            #region FileOutputWindow收集异常或错误

            vtkFileOutputWindow fileOutputWindow = vtkFileOutputWindow.New();
            fileOutputWindow.SetFileName("output.txt");
            vtkOutputWindow.SetInstance(fileOutputWindow);

            #endregion

            //绘制
            m_Renderer = renderWindowControl1.RenderWindow.GetRenderers().GetFirstRenderer();
            //设置背景颜色
            m_Renderer.SetBackground(0, 0, 0);
            //清空所有演员对象
            m_Renderer.GetViewProps().RemoveAllItems();

            //创建外框线
            vtkActor OutlineActor = BuildOutlineActor(m_ImageData);
            m_Renderer.AddActor(OutlineActor);

            //绘制坐标轴
            DrawAxes();

            //绘制色标
            vtkScalarBarActor ScaleBarActor = BuildScalarBar();
            m_Renderer.AddActor(ScaleBarActor);

            switch (m_PageMode)
            {
                case fy3DModelPageMode.Normal_Mode:
                    DrawMode_Normal();
                    break;
                case fy3DModelPageMode.Slice_Mode:
                    DrawMode_Slice();
                    break;
                case fy3DModelPageMode.HollowOut_Mode:
                    DrawMode_HollowOut();
                    break;
            }

            //设置Camera对象
            vtkCamera Camera = vtkCamera.New();
            if (m_CameraFocalPoint != null)
            {
                Camera.SetFocalPoint(m_CameraFocalPoint[0], m_CameraFocalPoint[1], m_CameraFocalPoint[2]);
                Camera.SetPosition(m_CameraPoint[0], m_CameraPoint[1], m_CameraPoint[2]);
                Camera.SetViewUp(m_CameraViewUp[0], m_CameraViewUp[1], m_CameraViewUp[2]);
                Camera.ComputeViewPlaneNormal();
            }
            else
            {
                Camera = m_Renderer.GetActiveCamera();
            }
            m_Renderer.SetActiveCamera(Camera);

            m_Renderer.ResetCamera();
            renderWindowControl1.RenderWindow.Render();
        }

        //把fyGrid转为vtkImageData
        private vtkImageData ConvertfyGrid2vtkImageData(ExGrid Grid)
        {
            vtkImageData ImageData = vtkImageData.New();
            ImageData.SetScalarTypeToUnsignedChar();

            ImageData.SetNumberOfScalarComponents(1);
            ImageData.SetDimensions(Grid.ICount, Grid.JCount, Grid.KCount);
            ImageData.SetSpacing(1, 1, 1);
            ImageData.AllocateScalars();
            byte[] data = new byte[Grid.CellCount];

            for (int i = 0; i < Grid.CellCount; i++)
            {
                data[i] = (byte)Grid.GetCell(i).Value;
            }
            System.Runtime.InteropServices.Marshal.Copy(data, 0, (IntPtr)ImageData.GetScalarPointer(), data.Length);
            return ImageData;
        }

        //绘制坐标轴
        private void DrawAxes()
        {
            m_RenderWindowInteractor = this.renderWindowControl1.RenderWindow.GetInteractor();
            m_AxesActor = new vtkAxesActor();
            m_OrientationMarkerWidget = new vtkOrientationMarkerWidget();
            m_OrientationMarkerWidget.SetOrientationMarker(m_AxesActor);
            m_OrientationMarkerWidget.SetInteractor(m_RenderWindowInteractor);
            m_OrientationMarkerWidget.On();
            m_OrientationMarkerWidget.SetInteractive(0);
        }

        //绘制色标
        private vtkScalarBarActor BuildScalarBar()
        {
            vtkScalarBarActor scalarBar = vtkScalarBarActor.New();
            scalarBar.SetLookupTable(m_LookupTable);
            //scalarBar.SetTitle("Color Scale");
            scalarBar.GetLabelTextProperty().SetFontSize(2);
            scalarBar.GetLabelTextProperty().SetFontFamilyToTimes();
            scalarBar.GetLabelTextProperty().SetItalic(0);
            scalarBar.GetPositionCoordinate().SetCoordinateSystemToNormalizedViewport();
            //设置位置（笛卡尔坐标系）
            scalarBar.GetPositionCoordinate().SetValue(0.01, 0.5);
            //设置色标的标注数目
            scalarBar.SetNumberOfLabels(5);
            scalarBar.SetOrientationToVertical();
            scalarBar.SetWidth(0.12);
            scalarBar.SetHeight(0.5);

            return scalarBar;
        }

        //绘制外边框
        private vtkActor BuildOutlineActor(vtkImageData ImageData)
        {
            //创建Outline
            vtkOutlineFilter OutlineFilter = vtkOutlineFilter.New();
            OutlineFilter.SetInput(ImageData);
            vtkPolyDataMapper OutlineMapper = vtkPolyDataMapper.New();
            OutlineMapper.SetInputConnection(OutlineFilter.GetOutputPort());
            vtkActor outlineActor = vtkActor.New();
            outlineActor.SetMapper(OutlineMapper);
            outlineActor.GetProperty().SetColor(0.5, 0.5, 0.5);

            return outlineActor;
        }

        //读取Camera参数，传递给主窗体
        public void GetCameraParameters(ref double[] CameraFocalPoint, ref double[] CameraPosition, ref double[] CameraViewUp)
        {
            vtkCamera Camera = renderWindowControl1.RenderWindow.GetRenderers().GetFirstRenderer().GetActiveCamera();
            CameraFocalPoint = Camera.GetFocalPoint();
            CameraPosition = Camera.GetPosition();
            CameraViewUp = Camera.GetViewUp();
        }


    }
}
