using ImTools;
using ScottPlot;
using ScottPlot.AxisPanels;
using ScottPlot.Plottables;
using ScottPlot.WPF;
using SkiaSharp;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;
using WpfApp1.Common;

namespace WpfApp1.Views.Oscilloscope
{
    /// <summary>
    /// ScottPlotView.xaml 的交互逻辑
    /// </summary>
    public partial class ScottPlotView : UserControl
    {
        #region 私有变量

        /// <summary>
        /// 波形数据是否开启
        /// </summary>
        private bool m_signalState = false;

        private int sutr = 0;

        private double m_frequency = 0;

        private System.Timers.Timer AddSystemTimer = new() { Interval = 10 };

        private DeviceTimer AddDeviceTimer = new DeviceTimer();

        private readonly DispatcherTimer UpdateDispatcherTimer = new() { Interval = TimeSpan.FromMilliseconds(10) };

        /// <summary>
        /// 每段信号最大容量
        /// </summary>
        private static int pointMaxCount = 10_000_000;

        private readonly IPalette PlottablePalette = new ScottPlot.Palettes.Category20();

        /// <summary>
        /// 通道列表
        /// </summary>
        private Dictionary<string, Signal> renderInfos = new Dictionary<string, Signal>();

        public List<ScottPlot.Plottables.DataLogger> DataLoggerList { get; set; } = new List<ScottPlot.Plottables.DataLogger>();

        public List<ScottPlot.Plottables.Signal> SignalList { get; set; } = new List<ScottPlot.Plottables.Signal>();

        public List<DataStreamer> DataStreamerList { get; set; } = new List<DataStreamer>();
        /// <summary>
        /// 曲线数据列表
        /// </summary>
        private Dictionary<string, double[]> CurveData = new Dictionary<string, double[]>();

        private Dictionary<string, CircularBuffer<double>> Buffers = new Dictionary<string, CircularBuffer<double>>();
        internal readonly ScottPlotYAxisCollection PlotYAxis;

        /// <summary>
        /// 高亮波形key
        /// </summary>
        private string lastHightlight = string.Empty;

        /// <summary>
        /// 标尺列表
        /// </summary>
        private Dictionary<string, AxisLine> axisLineMap = new Dictionary<string, AxisLine>();

        /// <summary>
        /// Y轴锁定范围参数
        /// </summary>
        private ScottPlot.AxisRules.LockedVertical lockY { get; set; }
        /// <summary>
        /// X轴锁定范围参数
        /// </summary>
        private ScottPlot.AxisRules.LockedHorizontal lockX { get; set; }

        /// <summary>
        /// XY最大范围
        /// </summary>
        private ScottPlot.AxisRules.MaximumSpan maximumSpan { get; set; }
        #endregion

        public ScottPlotView()
        {
            InitializeComponent();
            PlotYAxis = new ScottPlotYAxisCollection(WpfPlot1.Plot);
            ScottPlotConfiguration();
            ScottPlotStyle();
            StartScottPlot();

            //十字标记
            //var cross = WpfPlot1.Plot.Add.Crosshair(0, 0);
            //MouseMove += (s, e) => {
            //    Point p = e.GetPosition(WpfPlot1);
            //    ScottPlot.Pixel mousePixel = new(p.X * WpfPlot1.DisplayScale, p.Y * WpfPlot1.DisplayScale);
            //    ScottPlot.Coordinates coordinates = WpfPlot1.Plot.GetCoordinates(mousePixel);
            //    cross.Position = coordinates;
            //    WpfPlot1.Refresh();
            //};

            WpfPlot1.Refresh();
        }

        /// <summary>
        /// 示波器风格样式
        /// </summary>
        private void ScottPlotStyle()
        {
            WpfPlot1.Plot.ShowLegend();//辅助方法，里面有可用的属性
            // change figure colors
            WpfPlot1.Plot.FigureBackground.Color = Color.FromHex("#181818");
            WpfPlot1.Plot.DataBackground.Color = Color.FromHex("#1f1f1f");

            // change axis and grid colors
            WpfPlot1.Plot.Axes.Color(Color.FromHex("#d7d7d7"));
            WpfPlot1.Plot.Grid.MajorLineColor = Color.FromHex("#404040");

            // change legend colors
            WpfPlot1.Plot.Legend.BackgroundColor = Color.FromHex("#404040");
            WpfPlot1.Plot.Legend.FontColor = Color.FromHex("#d7d7d7");
            WpfPlot1.Plot.Legend.OutlineColor = Color.FromHex("#d7d7d7");
        }

        private void AdjustLimits()
        {
            WpfPlot1.Plot.Axes.SetLimits(pointMaxCount - 1000000, pointMaxCount + 10, -10, 100);//自定义初始Y轴与X轴范围
            //WpfPlot1.Plot.Axes.SetLimits(-100, 10, -10, 100);//自定义初始Y轴与X轴范围
        }

        private void ScottPlotConfiguration()
        {
            WpfPlot1.Plot.Font.Set(SKFontManager.Default.MatchCharacter('汉').FamilyName);
            AdjustLimits();
            WpfPlot1.Plot.Axes.AntiAlias(false);//抗锯齿
            //WpfPlot1.Plot.Axes.DateTimeTicksBottom();//删除所有低轴，添加时间轴
            //WpfPlot1.Plot.Axes.Bottom.TickLabelStyle.IsVisible = false;//隐藏底部X轴

            //WpfPlot1.Plot.Axes.ZoomOutX(10);
            //WpfPlot1.Plot.Axes.ZoomOutY(10);

            //添加XY最小范围
            ScottPlot.AxisRules.MinimumSpan spanMin = new(xAxis: WpfPlot1.Plot.Axes.Bottom, yAxis: WpfPlot1.Plot.Axes.Left, xSpan: 10, ySpan: 10);
            WpfPlot1.Plot.Axes.Rules.Add(spanMin);
            //添加XY最大范围
            maximumSpan = new(xAxis: WpfPlot1.Plot.Axes.Bottom, yAxis: WpfPlot1.Plot.Axes.Left, xSpan: 10000000, ySpan: 2000);
            WpfPlot1.Plot.Axes.Rules.Add(maximumSpan);
             
            //WpfPlot1.Plot.Axes.Rules.Clear();
            //最小缩放
            //ScottPlot.AxisRules.MinimumBoundary ruleMin = new(
            //xAxis: WpfPlot1.Plot.Axes.Bottom,
            //yAxis: WpfPlot1.Plot.Axes.Left,
            //limits: new AxisLimits(-10, 10, -10, 10));
            //WpfPlot1.Plot.Axes.Rules.Add(ruleMin);

            //最大缩放
            //ScottPlot.AxisRules.MaximumBoundary ruleMax = new(
            //xAxis: WpfPlot1.Plot.Axes.Bottom,
            //yAxis: WpfPlot1.Plot.Axes.Left,
            //limits: new AxisLimits(-10, pointMaxCount, -100, 2000));
            //WpfPlot1.Plot.Axes.Rules.Add(ruleMax);

            AxisLimits limits = WpfPlot1.Plot.Axes.GetLimits();
            lockX = new(WpfPlot1.Plot.Axes.Bottom, limits.Left, limits.Right);
            lockY = new(WpfPlot1.Plot.Axes.Left, limits.Bottom, limits.Top);

            // WpfPlot1.Plot.Axes.Left.MinimumSize = 500;
            // WpfPlot1.Plot.Axes.Bottom.MaximumSize = pointMaxCount * 2;
            //WpfPlot1.Plot.Axes.Bottom.Max = pointMaxCount;

            ScottPlot.TickGenerators.NumericAutomatic tickGenX = new();
            tickGenX.MinimumTickSpacing = 100;//最小刻度间距
            tickGenX.TickDensity = 1;//刻度密度
            tickGenX.TargetTickCount = 10;//只存在10个刻度间距
            WpfPlot1.Plot.Axes.Bottom.MinimumSize = 1;
            WpfPlot1.Plot.Axes.Bottom.TickGenerator = tickGenX;

            ScottPlot.TickGenerators.NumericAutomatic tickGenY = new();
            tickGenY.MinimumTickSpacing = 50;
            tickGenY.TickDensity = 1;
            WpfPlot1.Plot.Axes.Left.TickGenerator = tickGenY;
            //WpfPlot1.Plot.Axes.Left.MinimumSize = 1;

            WpfPlot1.Plot.Font.Automatic();//设置字体
        }

        /// <summary>
        /// 添加更新示波器刷新率线程
        /// </summary>
        private void StartScottPlot()
        {
            UpdateAddSystemTimer();
            UpdateDispatcherTimer.Tick += (s, e) =>
            {
                // Locking the sync object does not seem to be required
                // when data is changed using the dispatcher timer in WPF apps
                //ChangeDataLength();
                WpfPlot1.Refresh();
            };
        }
        private Stopwatch stopwatch = Stopwatch.StartNew();
        readonly ScottPlot.DataGenerators.RandomWalker Walker2 = new(1, multiplier: 1000);
        private int ccc = 0;
        private int CurveDataLength = 0;
        private double multiplier = 2 * Math.PI / 1000;

        /// <summary>
        /// 模拟参数
        /// </summary>
        public void RefreshOPMUI()
        {
            double phase = stopwatch.Elapsed.TotalSeconds;
            ccc++;
            
            Parallel.ForEach(CurveData.Values, new ParallelOptions { MaxDegreeOfParallelism = Environment.ProcessorCount }, item =>
            {
                item[CurveDataLength - 1] = Math.Sin(ccc * multiplier + phase);
            });
            CurveDataLength--;
            if (CurveDataLength == 1)
            {
                CurveDataLength = pointMaxCount;
            }
        }

        public void UpdateAddSystemTimer(string frequency = "2000")
        {
            //频率 2000 Hz 的时间是 0.5 毫秒
            //频率 4000 Hz 的时间是 0.25 毫秒
            //频率 8000 Hz 的时间是 0.125 毫秒
            //频率 16000 Hz 的时间是 0.0625 毫秒
            m_frequency = 1000.0 / Convert.ToInt32(frequency);
        }

        private Stopwatch stopwatchStart = new Stopwatch();
        /// <summary>
        /// 计时开始
        /// </summary>
        public void TimerStart()
        {
            if (renderInfos.Count == 0) return;

            AdjustLimits();
            ClearArrayValues();
            //AddSystemTimer.Start();
            m_signalState = true;
            SetRenderInfo(m_signalState);

            UpdateDispatcherTimer.Start();
            AddDeviceTimer.Start(m_frequency);

            Task.Factory.StartNew(() =>
            {
                while (m_signalState)
                {
                    if (AddDeviceTimer.IsTimeout())
                    {
                        lock (WpfPlot1.Plot.Sync)
                        {
                            RefreshOPMUI();
                        }
                        AddDeviceTimer.Start(m_frequency);

                        //sutr++;
                        //if (sutr % 2000 == 0)
                        //{
                        //    stopwatchStart.Stop();
                        //    long elapsedMicroseconds = stopwatchStart.ElapsedTicks / (Stopwatch.Frequency / 1000000);
                        //    Debug.WriteLine(elapsedMicroseconds);
                        //    stopwatchStart.Restart();
                        //}
                    }
                }
            });
        }

        /// <summary>
        /// 将double值安全地转换为uint。
        /// </summary>
        /// <param name="doubleValue">double值。</param>
        /// <returns>转换后的uint值。</returns>
        public uint ConvertToUint(double milliseconds)
        {
            // 检查double值是否在uint的范围内
            if (milliseconds < 0 || milliseconds * 1000 > uint.MaxValue)
            {
                throw new OverflowException("毫秒值超出微秒的范围");
            }

            // 将毫秒值转换为微秒值
            double microseconds = milliseconds * 1000.0;

            // 取整（可以选择不同的取整方式）
            uint result = (uint)Math.Round(microseconds);

            return result;
        }

        /// <summary>
        /// 停止计时
        /// </summary>
        public void TimerStop()
        {
            //AddSystemTimer.Stop();
            AddDeviceTimer.Stop();
            UpdateDispatcherTimer.Stop();
            m_signalState = false;
            sutr = 0;
            SetRenderInfo(m_signalState);
        }

        private void SetRenderInfo(bool state)
        {
            if (state)
            {
                maximumSpan.XSpan = 1000000;
                maximumSpan.YSpan = 2000;
            }
            else
            {
                maximumSpan.XSpan = 10000000;
                maximumSpan.YSpan = 5000;
            }
            //foreach (var item in renderInfos.Values)
            //{
            //    item.ManageAxisLimits = state;
            //}
        }

        private void ClearArrayValues()
        {
            CurveDataLength = pointMaxCount;
            foreach (var pair in CurveData)
            {
                Array.Fill(pair.Value, 0.0);
            }

            //foreach (var item in renderInfos.Values)
            //{
            //    item.Data.Coordinates.Clear();
            //}
        }

        #region 通道

        /// <summary>
        /// 添加通道
        /// </summary>
        internal void GenerateSignal(string key, string name)
        {
            //创建波形图实例        
            double[] ys = new double[pointMaxCount];

            RemoveSignal(key);

            //string name = string.Format(;"通道{0}", key);
            var color = PlottablePalette.GetColor(renderInfos.Count);

            //var dataLogger = WpfPlot1.Plot.Add.DataLogger();
            //dataLogger.ViewSlide(20000);
            //dataLogger.ManageAxisLimits = true;
            //dataLogger.Data.YOffset = renderInfos.Count() * 10;
            //renderInfos.TryAdd(key, dataLogger);

            Signal plot = WpfPlot1.Plot.Add.Signal(ys, 1);
            plot.LegendText = name;
            plot.Data.YOffset = renderInfos.Count() * 10;
            renderInfos.TryAdd(key, plot);
            CurveData.TryAdd(key, ys);
            //Buffers.TryAdd(key, new CircularBuffer<double>(ys));

            //SignalConst<double> signalConst = WpfPlot1.Plot.Add.SignalConst(ys,1);
            //signalConst.LegendText = name;
            //renderInfos.TryAdd(key, signalConst);
            //CurveData.Add(key, ys);

            //PlotAddAxis(key, plot);

            //WpfPlot1.Plot.Axes.AutoScale();
            //WpfPlot1.Plot.Font.Automatic();
            AdjustLimits();
            WpfPlot1.Refresh();
        }

        /// <summary>
        /// 添加对应通道Y轴
        /// </summary>
        /// <param name="key"></param>
        /// <param name="plot"></param>
        private void PlotAddAxis(string key, Signal plot)
        {
            //PlotYAxis.YAxisAdd(key, plot);
            WpfPlot1.Refresh();
        }


        /// <summary>
        /// 选择通道
        /// </summary>
        /// <param name="key"></param>
        internal void ChannelSelected(string key)
        {
            try
            {
                if (lastHightlight.Equals(key) || !renderInfos.ContainsKey(key)) return;

                if (!string.IsNullOrEmpty(lastHightlight) && renderInfos.ContainsKey(lastHightlight))
                {
                    renderInfos[lastHightlight].MarkerSize = 3;
                    renderInfos[lastHightlight].LineWidth = 1;
                    //PlotYAxis.YAxisVisible(lastHightlight, false);
                    //PlotYAxis.YAxisLockLimits(lastHightlight, true);
                }

                renderInfos[key].MarkerSize = 8;
                renderInfos[key].LineWidth = 3;

                double yMin = WpfPlot1.Plot.Axes.GetLimits(renderInfos[key].Axes).YRange.Min;
                double yMax = WpfPlot1.Plot.Axes.GetLimits(renderInfos[key].Axes).YRange.Max;

                WpfPlot1.Plot.Axes.SetLimitsY(yMin, yMax);

                //PlotYAxis.YAxisVisible(key, true);
                //PlotYAxis.YAxisLockLimits(key, false);
                lastHightlight = key;

                WpfPlot1.Refresh();
            }
            catch (Exception ex)
            {

            }
           
        }

        /// <summary>
        /// 移除通道
        /// </summary>
        /// <param name="key"></param>
        internal void RemoveSignal(string key)
        {
            if (renderInfos.ContainsKey(key))
            {
                var info = renderInfos[key];

                // if (renderInfos.TryRemove(key, out var target))
                {
                    WpfPlot1.Plot.Remove(info);
                    //PlotYAxis.YAxisDelete(key);
                }
                renderInfos.Remove(key);
                CurveData.Remove(key);
                //Buffers.Remove(key);
                //CurveData.TryRemove(key,out _);
            }
        }

        /// <summary>
        /// 清空通道
        /// </summary>
        internal void ClearSignals()
        {
            ClearAxisLine();
            foreach (var item in renderInfos)
            {
                WpfPlot1.Plot.Remove(item.Value);
            }
            renderInfos?.Clear();
        }

        #endregion

        #region 标尺

        /// <summary>
        /// 清空标尺
        /// </summary>
        internal void ClearAxisLine()
        {
            foreach (var item in axisLineMap)
            {
                WpfPlot1.Plot.Remove(item.Value);
            }
            axisLineMap?.Clear();
        }
        #endregion

        /// <summary>
        /// 锁定Y轴
        /// </summary>
        /// <param name="state"></param>
        public void LockVertical(bool state)
        {
            //WpfPlot1.Plot.Axes.Rules.Clear();
            if (state)
            {
                AxisLimits limits = WpfPlot1.Plot.Axes.GetLimits();
                lockY.Max = limits.Top;
                lockY.YMin = limits.Bottom;
                if (!WpfPlot1.Plot.Axes.Rules.Contains(lockY)) WpfPlot1.Plot.Axes.Rules.Add(lockY);
            }
            else
            {
                WpfPlot1.Plot.Axes.Rules.Remove(lockY);
            }
            //WpfPlot1.Plot.Axes.Link(WpfPlot1, y: state);
        }

        /// <summary>
        /// 锁定X轴
        /// </summary>
        /// <param name="state"></param>
        public void LockHorizontal(bool state)
        {
            //WpfPlot1.Plot.Axes.Rules.Clear();
            if (state)
            {
                AxisLimits limits = WpfPlot1.Plot.Axes.GetLimits();
                lockX.XMax = limits.Right;
                lockX.XMin = limits.Left;
                if (!WpfPlot1.Plot.Axes.Rules.Contains(lockX)) WpfPlot1.Plot.Axes.Rules.Add(lockX);
            }
            else
            {
                WpfPlot1.Plot.Axes.Rules.Remove(lockX);
            }
            //WpfPlot1.Plot.Axes.Link(WpfPlot1, x: state);
        }
    }

    /// <summary>
    /// Y轴类
    /// </summary>
    internal class ScottPlotYAxisCollection
    {
        private readonly Plot Plot;

        /// <summary>
        /// Y轴列表
        /// </summary>
        private Dictionary<string, LeftAxis> YaxisList = new Dictionary<string, LeftAxis>();

        public ScottPlotYAxisCollection(Plot plot)
        {
            this.Plot = plot;

        }

        /// <summary>
        /// 读取Y轴索引
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public LeftAxis YAxisGetAxisIndex(string key)
        {
            if (string.IsNullOrEmpty(key))
            {
                return new LeftAxis();
            }
            return YaxisList[key];
        }

        /// <summary>
        /// 添加对应通道Y轴
        /// </summary>
        /// <param name="key">唯一ID</param>
        /// <param name="plot">通道类</param>
        public void YAxisAdd(string key, Signal plot)
        {
            var yAxis = Plot.Axes.AddLeftAxis();
            yAxis.IsVisible = false;
            yAxis.Color(plot.Color);
            plot.Axes.XAxis = Plot.Axes.Bottom; // standard X axis
            plot.Axes.YAxis = yAxis; // custom Y axis

            YaxisList.Add(key, yAxis);
        }

        /// <summary>
        /// 删除对应通道Y轴
        /// </summary>
        /// <param name="key"></param>
        public void YAxisDelete(string key)
        {
            Plot.Axes.Remove(YaxisList[key]);
            YaxisList.Remove(key);
        }

        /// <summary>
        /// 是否锁定子Y轴
        /// </summary>
        /// <param name="key"></param>
        /// <param name="states"></param>
        public void YAxisLockLimits(string key, bool states)
        {
            //YaxisList[key].LockLimits(states);
        }

        /// <summary>
        /// 显示子Y轴显示隐藏
        /// </summary>
        /// <param name="key"></param>
        /// <param name="states"></param>
        public void YAxisVisible(string key, bool states)
        {
            YaxisList[key].IsVisible = states;
        }

        /// <summary>
        /// 校准子Y轴与主Y轴对应
        /// </summary>
        /// <param name="key"></param>
        public void YAxisCalibration(string key)
        {
            //double yMin = Plot.GetAxisLimits().YMin;
            //double yMax = Plot.GetAxisLimits().YMax;
            double yMin = Plot.Axes.GetLimits().YRange.Min;
            double yMax = Plot.Axes.GetLimits().YRange.Max;

            //Plot.SetAxisLimitsY(yMin, yMax, yAxisIndex: YaxisList[key].AxisIndex);

            Plot.Axes.SetLimitsY(yMin, yMax, YaxisList[key]);
        }
    }

    // 环形缓冲区类
    public class CircularBuffer<T> where T : struct
    {
        //private T[] _buffer;
        private int _head;
        private int _tail;
        private int _count;

        private T[] _buffer;

        /// <summary>
        /// // 提供对 _buffer 的只读访问
        /// </summary>
        public T[] Buffer
        {
            get { return _buffer; }
            set { _buffer = value; }
        }
        public CircularBuffer(int capacity)
        {
            _buffer = new T[capacity];
            _head = 0;
            _tail = 0;
            _count = 0;
        }

        public CircularBuffer(T[] capacity)
        {
            _buffer = capacity;
            _head = 0;
            _tail = 0;
            _count = 0;
        }

        public void Add(T value)
        {
            _buffer[_head] = value;
            _head = (_head + 1) % _buffer.Length;
            if (_count < _buffer.Length)
            {
                _count++;
            }
            else
            {
                _tail = (_tail + 1) % _buffer.Length;
            }
        }

        public T[] ToArray()
        {
            T[] result = new T[_count];
            if (_head >= _tail)
            {
                Array.Copy(_buffer, _tail, result, 0, _count);
            }
            else
            {
                Array.Copy(_buffer, _tail, result, 0, _buffer.Length - _tail);
                Array.Copy(_buffer, 0, result, _buffer.Length - _tail, _head);
            }
            return result;
        }

        public void ToArrayFill()
        {
            Array.Fill(_buffer, default(T));
            _head = 0;
            _tail = 0;
            _count = 0;
        }
    }

}
