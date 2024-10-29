using ScottPlot;
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
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ScottPlotExtension
{
    /// <summary>
    /// ScottPlotView.xaml 的交互逻辑
    /// </summary>
    public partial class ScottPlotView : UserControl
    {
        #region 自定义属性

        public string Title
        {
            get { return (string)GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Title.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TitleProperty =
            DependencyProperty.Register(
                nameof(Title),
                typeof(string),
                typeof(ScottPlotView),
                new PropertyMetadata(nameof(ScottPlotView), new PropertyChangedCallback(OnTitlePropertyChangedCallback)));


        private static void OnTitlePropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ScottPlotView view)
            {
                view.PlotComponent.Plot.Title((string)e.NewValue);
            }
        }

        public double YMin
        {
            get { return (double)GetValue(YMinProperty); }
            set { SetValue(YMinProperty, value); }
        }

        // Using a DependencyProperty as the backing store for YYMin.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty YMinProperty =
            DependencyProperty.Register(nameof(YMin), typeof(double), typeof(ScottPlotView), new PropertyMetadata(-5D, new PropertyChangedCallback(OnYMinPropertyChangedCallback)));

        private static void OnYMinPropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ScottPlotView view)
            {
                var limits = view.GetLeftAxesLimit();

                double min = (double)e.NewValue;

                if (min < limits.max)
                {
                    view.SetLeftAxesLimit(min, limits.max);
                    view.Refresh();
                }
            }
        }

        public double YMax
        {
            get { return (double)GetValue(YMaxProperty); }
            set { SetValue(YMaxProperty, value); }
        }

        // Using a DependencyProperty as the backing store for YYMin.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty YMaxProperty =
            DependencyProperty.Register(nameof(YMax), typeof(double), typeof(ScottPlotView), new PropertyMetadata(5D, new PropertyChangedCallback(OnYMaxPropertyChangedCallback)));

        private static void OnYMaxPropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ScottPlotView view)
            {
                var limits = view.GetLeftAxesLimit();

                double max = (double)e.NewValue;

                if (max > limits.min)
                {
                    view.SetLeftAxesLimit(limits.min, max);
                    view.Refresh();
                }
            }
        }


        public double XMin
        {
            get { return (double)GetValue(XMinProperty); }
            set { SetValue(XMinProperty, value); }
        }

        // Using a DependencyProperty as the backing store for YYMin.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty XMinProperty =
            DependencyProperty.Register(nameof(XMin), typeof(double), typeof(ScottPlotView), new PropertyMetadata(-1D, new PropertyChangedCallback(OnXMinPropertyChangedCallback)));

        private static void OnXMinPropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ScottPlotView view)
            {
                var limits = view.GetButtomAxesLimit();
                view.SetButtomAxesLimit((double)e.NewValue, limits.max);
            }
        }

        public double XMax
        {
            get { return (double)GetValue(XMaxProperty); }
            set { SetValue(XMaxProperty, value); }
        }

        // Using a DependencyProperty as the backing store for YYMin.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty XMaxProperty =
            DependencyProperty.Register(nameof(XMax), typeof(double), typeof(ScottPlotView), new PropertyMetadata(5D, new PropertyChangedCallback(OnXMaxPropertyChangedCallback)));

        private static void OnXMaxPropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ScottPlotView view)
            {
                var limits = view.GetButtomAxesLimit();
                view.SetButtomAxesLimit(limits.min, (double)e.NewValue);
            }
        }

        /// <summary>
        /// 轴自动
        /// </summary>
        public bool IsAutoAxis
        {
            get { return (bool)GetValue(IsAutoAxisProperty); }
            set { SetValue(IsAutoAxisProperty, value); }
        }
        public static readonly DependencyProperty IsAutoAxisProperty =
            DependencyProperty.Register(nameof(IsAutoAxis), typeof(bool), typeof(ScottPlotView), new PropertyMetadata(true));

        public bool XAxisAuto
        {
            get { return (bool)GetValue(XAxisAutoProperty); }
            set { SetValue(XAxisAutoProperty, value); }
        }

        // Using a DependencyProperty as the backing store for XAxisAuto.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty XAxisAutoProperty =
            DependencyProperty.Register(nameof(XAxisAuto), typeof(bool), typeof(ScottPlotView), new PropertyMetadata(false));

        public bool YAxisAuto
        {
            get { return (bool)GetValue(YAxisAutoProperty); }
            set { SetValue(YAxisAutoProperty, value); }
        }

        // Using a DependencyProperty as the backing store for XAxisAuto.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty YAxisAutoProperty =
            DependencyProperty.Register(nameof(YAxisAuto), typeof(bool), typeof(ScottPlotView), new PropertyMetadata(false));
        /// <summary>
        /// Y轴锁定缩放
        /// </summary>
        public bool LockVerticalAxis
        {
            get { return (bool)GetValue(LockVerticalAxisProperty); }
            set
            {
                SetValue(LockVerticalAxisProperty, value);
            }
        }
        public static readonly DependencyProperty LockVerticalAxisProperty =
    DependencyProperty.Register(nameof(LockVerticalAxis), typeof(bool), typeof(ScottPlotView), new PropertyMetadata(false, new PropertyChangedCallback(OnLockVerticalAxisPropertyChangedCallback)));
        private static void OnLockVerticalAxisPropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ScottPlotView view)
            {
                // view.Axes.Bottom. = (bool)e.NewValue;
            }
        }

        /// <summary>
        /// X轴锁定缩放
        /// </summary>
        public bool LockHorizontalAxis
        {
            get { return (bool)GetValue(LockHorizontalAxisProperty); }
            set { SetValue(LockHorizontalAxisProperty, value); }
        }

        public static readonly DependencyProperty LockHorizontalAxisProperty =
    DependencyProperty.Register(nameof(LockHorizontalAxis), typeof(bool), typeof(ScottPlotView), new PropertyMetadata(false, new PropertyChangedCallback(OnLockHorizontalAxisPropertyChangedCallback)));
        private static void OnLockHorizontalAxisPropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ScottPlotView view)
            {
                // view.component.Configuration.LockHorizontalAxis = (bool)e.NewValue;
            }
        }


        public bool RealtimeRendering
        {
            get { return (bool)GetValue(RealtimeRenderingProperty); }
            set { SetValue(RealtimeRenderingProperty, value); }
        }

        // Using a DependencyProperty as the backing store for RealtimeRendering.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty RealtimeRenderingProperty =
            DependencyProperty.Register(nameof(RealtimeRendering), typeof(bool), typeof(ScottPlotView), new PropertyMetadata(false, new PropertyChangedCallback(OnRealtimeRenderingPropertyChangedCallback)));

        private static void OnRealtimeRenderingPropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            // throw new NotImplementedException();
        }

        public int InitPointLength
        {
            get { return (int)GetValue(InitPointLengthProperty); }
            set { SetValue(InitPointLengthProperty, value); }
        }

        // Using a DependencyProperty as the backing store for InitPointLength.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty InitPointLengthProperty =
            DependencyProperty.Register(nameof(InitPointLength), typeof(int), typeof(ScottPlotView), new PropertyMetadata(300));


        /// <summary>
        /// x轴坐标点间隔
        /// </summary>
        public double Frame
        {
            get { return (double)GetValue(FrameProperty); }
            set { SetValue(FrameProperty, value); }
        }

        public static readonly DependencyProperty FrameProperty =
            DependencyProperty.Register(nameof(Frame), typeof(double), typeof(ScottPlotView), new PropertyMetadata(0d));

        /// <summary>
        /// 窗体是否激活
        /// </summary>
        public bool IsActive
        {
            get { return (bool)GetValue(IsActiveProperty); }
            set { SetValue(IsActiveProperty, value); }
        }



        public static readonly DependencyProperty IsActiveProperty =
            DependencyProperty.Register(nameof(IsActive), typeof(bool), typeof(ScottPlotView), new PropertyMetadata(true));




        public float ZoomFactor
        {
            get { return (float)GetValue(ZoomFactorProperty); }
            set { SetValue(ZoomFactorProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ZoomFactor.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ZoomFactorProperty =
            DependencyProperty.Register("ZoomFactor", typeof(float), typeof(ScottPlotView), new PropertyMetadata(10F));

        public double ZoomMax
        {
            get { return (double)GetValue(ZoomMaxProperty); }
            set { SetValue(ZoomMaxProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ZoomMax.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ZoomMaxProperty =
            DependencyProperty.Register("ZoomMax", typeof(double), typeof(ScottPlotView), new PropertyMetadata(10D));


        public double ZoomMin
        {
            get { return (double)GetValue(ZoomMinProperty); }
            set { SetValue(ZoomMinProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ZoomMin.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ZoomMinProperty =
            DependencyProperty.Register("ZoomMin", typeof(double), typeof(ScottPlotView), new PropertyMetadata(-10D));

        /// <summary>
        /// X轴滑条参数 MAX:10 MIN:0 TickFrequency="1"
        /// </summary>
        public double SliderValue
        {
            get { return (double)GetValue(SliderValueProperty); }
            set { SetValue(SliderValueProperty, value); }
        }

        public static readonly DependencyProperty SliderValueProperty =
            DependencyProperty.Register(nameof(SliderValue), typeof(double), typeof(ScottPlotView), new PropertyMetadata(0D, SliderValuePropertyChangedCallback));
        private static void SliderValuePropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ScottPlotView view)
            {
                double newValue = (double)e.NewValue;

                double oldValue = (double)e.OldValue;

                //SetAxesZoomFactor(view, newValue, oldValue);

                SetAxesZoomOut(view, newValue, oldValue);

                view.Refresh();
            }
        }

        #endregion

        #region 自定义事件

        /// <summary>
        /// 水平标尺被拖拽路由事件
        /// </summary>
        public static readonly RoutedEvent HorizontalLineDraggedEvent = EventManager.RegisterRoutedEvent(
            nameof(HorizontalLineDragged), RoutingStrategy.Bubble,
            typeof(RoutedPropertyChangedEventHandler<(string, IEnumerable<AxisLineDraggedEventArgs>)>), typeof(ScottPlotView));

        public event RoutedPropertyChangedEventHandler<(string, IEnumerable<AxisLineDraggedEventArgs>)> HorizontalLineDragged
        { add => AddHandler(HorizontalLineDraggedEvent, value); remove => RemoveHandler(HorizontalLineDraggedEvent, value); }

        /// <summary>
        /// 垂直标尺被拖拽路由事件
        /// </summary>
        public static readonly RoutedEvent VerticalLineDraggedEvent = EventManager.RegisterRoutedEvent(
       nameof(VerticalLineDragged), RoutingStrategy.Bubble,
       typeof(RoutedPropertyChangedEventHandler<(string, IEnumerable<AxisLineDraggedEventArgs>)>), typeof(ScottPlotView));

        public event RoutedPropertyChangedEventHandler<(string, IEnumerable<AxisLineDraggedEventArgs>)> VerticalLineDragged
        { add => AddHandler(VerticalLineDraggedEvent, value); remove => RemoveHandler(VerticalLineDraggedEvent, value); }

        /// <summary>
        /// 垂直标尺添加路由事件
        /// </summary>
        public static readonly RoutedEvent VerticalLineAddEvent = EventManager.RegisterRoutedEvent(
       nameof(VerticalLineAdd), RoutingStrategy.Bubble,
       typeof(RoutedPropertyChangedEventHandler<AxisLineAddEventArgs>), typeof(ScottPlotView));

        public event RoutedPropertyChangedEventHandler<AxisLineAddEventArgs> VerticalLineAdd
        { add => AddHandler(VerticalLineAddEvent, value); remove => RemoveHandler(VerticalLineDraggedEvent, value); }

        public static readonly RoutedEvent ToolTipCreatedEvent = EventManager.RegisterRoutedEvent(
         nameof(ToolTipCreated), RoutingStrategy.Bubble,
        typeof(RoutedPropertyChangedEventHandler<ScottPlotMark>), typeof(ScottPlotView));

        public event RoutedPropertyChangedEventHandler<ScottPlotMark> ToolTipCreated
        { add => AddHandler(ToolTipCreatedEvent, value); remove => RemoveHandler(ToolTipCreatedEvent, value); }

        #endregion

        public (double min, double max) GetAxesLimit(IAxis axis) => (axis.Min, axis.Max);
        public (double min, double max) GetTopAxesLimit() => GetAxesLimit(Axes.Top);
        public (double min, double max) GetButtomAxesLimit() => GetAxesLimit(Axes.Bottom);
        public (double min, double max) GetLeftAxesLimit() => GetAxesLimit(Axes.Left);
        public (double min, double max) GetRightAxesLimit() => GetAxesLimit(Axes.Right);
        public void SetAxesLimit(double x_min, double x_max, double y_min, double y_max) => Axes.SetLimits(x_min, x_max, y_min, y_max);
        public void SetLeftAxesLimit(double min, double max) => Axes.SetLimitsY(min, max);
        public void SetButtomAxesLimit(double min, double max) => Axes.SetLimitsX(min, max);


        private AxisManager Axes => PlotComponent.Plot.Axes;

        private readonly System.Timers.Timer reanderTimer = new() { Interval = 15 };
        public ScottPlotView()
        {
            InitializeComponent();
            InitData(PlotComponent.Plot);

            reanderTimer.Elapsed += (s, e) =>
            {
                Refresh();
            };
        }

        private void InitData(Plot plot)
        {
            plot.ShowLegend();
            // change figure colors
            plot.FigureBackground.Color = Color.FromHex("#181818");
            plot.DataBackground.Color = Color.FromHex("#1f1f1f");

            // change axis and grid colors
            plot.Axes.Color(Color.FromHex("#d7d7d7"));
            plot.Grid.MajorLineColor = Color.FromHex("#404040");

            // change legend colors
            plot.Legend.BackgroundColor = Color.FromHex("#404040");
            plot.Legend.FontColor = Color.FromHex("#d7d7d7");
            plot.Legend.OutlineColor = Color.FromHex("#d7d7d7");

            plot.Axes.Bottom.Min = 0;
        }

        public void Refresh()
        {
            PlotComponent.Refresh();
        }

        public void StartRender()
        {
            reanderTimer.Start();
        }

        public void StopRender()
        {
            reanderTimer.Stop();
        }

        private static void SetAxesZoomOut(ScottPlotView view, double newValue, double oldValue)
        {
            if (double.IsNaN(newValue) || double.IsNaN(oldValue)) return;

            double center = view.ZoomMax - view.ZoomMin;

            if (center <= 0) throw new Exception("ZoomMax ，ZoomMin 设置异常");

            float factor = view.ZoomFactor;

            double scale = (newValue - oldValue) * factor;

            double multiple = (center + scale) / center;

            view.Axes.Bottom.Range.ZoomOut(multiple);
        }
    }

    public record struct AxisLineAddEventArgs(string key, System.Windows.Media.Color color, System.Windows.Controls.Orientation value);

    public record struct AxisLineDraggedEventArgs(string key, double value);

    public record struct ScottPlotMark(string key, double x, double y, string text);
}
