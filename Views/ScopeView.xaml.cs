using ImTools;
using ScottPlot;
using ScottPlot.Plottables;
using ScottPlot.WPF;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;
using WpfApp1.Models;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TaskbarClock;

namespace WpfApp1.Views
{
    /// <summary>
    /// ScopeView.xaml 的交互逻辑
    /// </summary>
    public partial class ScopeView : Window
    {
        public bool IsStart { get; set; } = false;

        System.Timers.Timer AddSystemTimer = new() { Interval = 1 };

        private readonly DispatcherTimer UpdateDispatcherTimer = new() { Interval = TimeSpan.FromMilliseconds(10) };

        double[] myData1 = new double[20000];
        double[] myData2 = new double[20000];

        public List<double[]> ValueList { get; set; } =  new List<double[]>();
        public List<ScottPlot.Plottables.Signal> SignalList { get; set; } = new List<ScottPlot.Plottables.Signal>();
        public List<ScottPlot.Plottables.DataStreamer> DataStreamerList { get; set; } = new List<ScottPlot.Plottables.DataStreamer>();

        ScottPlot.Plottables.Signal signalPlot1;
        ScottPlot.Plottables.Signal signalPlot2;

        readonly ScottPlot.Plottables.DataLogger Logger1;
        bool initflag = false;


        readonly ScottPlot.DataGenerators.RandomWalker Walker2 = new(1, multiplier: 1000);
        public ScopeView()
        {
            InitializeComponent();
            //this.Closing += CloseWindow;
            InitDynamicDataDisplay();

            StartScottPlot();
        }

        // 关闭窗口
        private void CloseWindow(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Process.GetCurrentProcess().Kill();
            //this.Close();
        }

        // 折线图动态数据展示
        private void InitDynamicDataDisplay()
        {
            for (int i = 0; i < 16; i++)
            {
                ValueList.Add(new double[20000]);
                var signal = this.WpfPlot1.Plot.Add.Signal(ValueList[i], 1);

                signal.LegendText = $"Lines{i + 1}";
                //signal.LegendText = Fonts.Detect($"测试{i + 1}");
                SignalList.Add(signal);
            }

            //for (int i = 0; i < 16; i++)
            //{
            //    ValueList.Add(new double[20000]);
            //    var dataStreamer = this.WpfPlot1.Plot.Add.DataStreamer(20000, 0.1);
            //    dataStreamer.LegendText = $"data{i + 1}";
            //    DataStreamerList.Add(dataStreamer);
            //}

            //signalPlot1 = this.WpfPlot1.Plot.Add.Signal(myData1, 0.1);
            //signalPlot1.LegendText = "Line 1";
            //signalPlot2 = this.WpfPlot1.Plot.Add.Signal(myData2, 0.1);
            //signalPlot2.LegendText = "Line 2";


            //WpfPlot1.Plot.SetAxisLimits(0, 2000, -100, 1000);//自定义Y轴与X轴范围

            //WpfPlot1.Configuration.LockVerticalAxis = true;//锁定垂直缩放(Y轴)
            //WpfPlot1.Configuration.LockHorizontalAxis = true;//锁定水平缩放(X轴)

            ScottPlotStyle();
            //Crosscursor();

            WpfPlot1.Refresh();

            //signalPlot1.IsVisible = true;
            //signalPlot2.IsVisible = true;

            initflag = true;
        }
        public ScottPlot.Plottables.Crosshair MyCrosshair;
        /// <summary>
        /// 十字光标
        /// </summary>
        private void Crosscursor()
        {

            MyCrosshair = WpfPlot1.Plot.Add.Crosshair(0, 0);
            MyCrosshair.IsVisible = false;
            MyCrosshair.MarkerShape = MarkerShape.None;
            MyCrosshair.MarkerSize = 15;
            WpfPlot1.MouseMove += (s, e) =>
            {
                if (IsStart)
                {
                    MyCrosshair.IsVisible = false;
                    Text.Text = $"No point selected";
                    return;
                }
            
                // 获取鼠标在数据空间中的位置
                // determine where the mouse is and get the nearest point
                Pixel mousePixel = new(e.GetPosition(WpfPlot1).X, e.GetPosition(WpfPlot1).Y);
                Coordinates mouseLocation = WpfPlot1.Plot.GetCoordinates(mousePixel);

                StringBuilder stringBuilder = new StringBuilder();
                foreach (var item in SignalList)
                {
                    DataPoint nearest = (bool)rbNearestXY.IsChecked
                    ? item.GetNearest(mouseLocation, WpfPlot1.Plot.LastRender)
                    : item.GetNearestX(mouseLocation, WpfPlot1.Plot.LastRender);


                    stringBuilder.AppendLine($"Selected Index={nearest.Index}, X={nearest.X:0.##}, Y={nearest.Y:0.##}");

                    // place the crosshair over the highlighted point
                    if (nearest.IsReal)
                    {
                        MyCrosshair.IsVisible = true;
                        MyCrosshair.Position = nearest.Coordinates;
                        WpfPlot1.Refresh();

                        stringBuilder.AppendLine($"Selected Index={nearest.Index}, X={nearest.X:0.##}, Y={nearest.Y:0.##}");
                        //Text.Text = $"Selected Index={nearest.Index}, X={nearest.X:0.##}, Y={nearest.Y:0.##}";
                    }
                    // hide the crosshair when no point is selected
                    if (!nearest.IsReal && MyCrosshair.IsVisible)
                    {
                        MyCrosshair.IsVisible = false;
                        WpfPlot1.Refresh();
                        stringBuilder.AppendLine($"No point selected");
                    }
                }

                Text.Text = stringBuilder.ToString();
            };
        }

        /// <summary>
        /// 示波器风格样式
        /// </summary>
        private void ScottPlotStyle()
        {
            WpfPlot1.Plot.ShowLegend();
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

        private Random m_random = new Random();

        private void ChangeDataLength()
        {
            int v1 = m_random.Next(-30, 200);
            int v2 = m_random.Next(0, 1000);

            Array.Copy(myData1, 1, myData1, 0, myData1.Length - 1);
            Array.Copy(myData2, 1, myData2, 0, myData2.Length - 1);

            myData1[myData1.Length - 1] = v1;
            myData2[myData2.Length - 1] = v2;

            for (int i = 0; i < ValueList.Count; i++)
            {
                var item = ValueList[i];
                int tt = m_random.Next(0, 1000);

                Array.Copy(item, 1, item, 0, item.Length - 1);
                item[item.Length - 1] = tt;

                //DataStreamerList[i].AddRange(item);
            }

        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            if (initflag == false)
            {
                return;
            }
            if (checkBox.IsChecked == true)
            {
                signalPlot1.IsVisible = true;
            }
            else
            {
                signalPlot1.IsVisible = false;
            }
        }

        private void CheckBox1_Checked(object sender, RoutedEventArgs e)
        {
            if (initflag == false)
            {
                return;
            }
            if (checkBox1.IsChecked == true)
            {
                signalPlot2.IsVisible = true;
            }
            else
            {
                signalPlot2.IsVisible = false;
            }
        }

        private void plot_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            //e.Handled = true;
        }

        private void StartScottPlot()
        {
            AddSystemTimer.Elapsed += (s, e) =>
            {
                // Changing data length will throw an exception if it occurs mid-render.
                // Operations performed while the sync object will occur outside renders.
                lock (WpfPlot1.Plot.Sync)
                {
                    ChangeDataLength();
                }
               // WpfPlot1.Refresh();
            };

            UpdateDispatcherTimer.Tick += (s, e) =>
            {
                // Locking the sync object does not seem to be required
                // when data is changed using the dispatcher timer in WPF apps
                //ChangeDataLength();
                WpfPlot1.Refresh();
            };
        }

        /// <summary>
        /// 开始进行
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnStart_Click(object sender, RoutedEventArgs e)
        {
            AddSystemTimer.Start();
            UpdateDispatcherTimer.Start();
            IsStart = true;

            Array.Copy(myData1, 1, myData1, 0, myData1.Length - 1);
            Array.Copy(myData2, 1, myData2, 0, myData2.Length - 1);

            myData1[myData1.Length - 1] = 50;
            myData2[myData2.Length - 1] = 20;
            WpfPlot1.Plot.Axes.AutoScaleY();
            WpfPlot1.Plot.Axes.AutoScaleX();
            WpfPlot1.Refresh();
        }

        /// <summary>
        /// 停止
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnStop_Click(object sender, RoutedEventArgs e)
        {
           // tokenSource.Cancel();
            AddSystemTimer.Stop();
            UpdateDispatcherTimer.Stop();
            IsStart = false;
        }
    }
}
