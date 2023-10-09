using ImTools;
using ScottPlot;
using ScottPlot.Plottable;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using System.Windows.Media;
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

        double[] myData1 = new double[2000];
        double[] myData2 = new double[2000];
        ScottPlot.Plottable.SignalPlot signalPlot1;
        ScottPlot.Plottable.SignalPlot signalPlot2;
        Task SPdataProcessing;
        CancellationTokenSource tokenSource;
        bool initflag = false;
        public ScopeView()
        {
            InitializeComponent();
            this.Closing += CloseWindow;
            InitDynamicDataDisplay();
        }


        // 关闭窗口
        private void CloseWindow(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Process.GetCurrentProcess().Kill();
            this.Close();
        }

        // 折线图动态数据展示
        private void InitDynamicDataDisplay()
        {

            signalPlot1 = this.WpfPlot1.Plot.AddSignal(myData1, 0.1, System.Drawing.Color.Black, label: "曲线1");
            signalPlot2 = this.WpfPlot1.Plot.AddSignal(myData2, 0.1, System.Drawing.Color.Magenta, label: "曲线2");


            //WpfPlot1.Plot.XAxis.Label("Time (milliseconds)");
            WpfPlot1.Plot.YAxis.Label("CCTV");
            WpfPlot1.Plot.XAxis2.Label("Title");
            WpfPlot1.Plot.SetAxisLimits(0, 2000, -100, 1000);//自定义Y轴与X轴范围
            //WpfPlot1.Configuration.LockVerticalAxis = true;//锁定垂直缩放(Y轴)
            //WpfPlot1.Configuration.LockHorizontalAxis = true;//锁定水平缩放(X轴)

            // One Axis Only
            //WpfPlot1.Plot.XAxis.Ticks(false);
            //WpfPlot1.Plot.XAxis.Line(false);
            //WpfPlot1.Plot.YAxis2.Line(false);
            //WpfPlot1.Plot.XAxis2.Line(false);


            WpfPlot1.Plot.Legend();

            WpfPlot1.Refresh();

            signalPlot1.IsVisible = true;
            signalPlot2.IsVisible = true;

            initflag = true;

            signalPlot1.OffsetX = 0;
            signalPlot2.OffsetX = 0;




            //var random = new Random();

            //var xs = new List<double>();
            //var ys = new List<double>();
            //plot.Render();

            //Task.Factory.StartNew(async () =>
            //{
            //    var index = 5;
            //    while (true)
            //    {
            //        xs.Add(index++);
            //        ys.Add(random.Next(1, 100));

            //        plot.Plot.Clear();
            //        plot.Plot.AddScatter(xs.ToArray(), ys.ToArray());

            //        Dispatcher.Invoke(() => plot.Render());

            //        await Task.Delay(300);
            //    }
            //});
        }

        public async void RefreshOPMUI()
        {
            while (!tokenSource.IsCancellationRequested)
            {
                Random rd = new Random();

                int v1 = rd.Next(-30, 200);
                int v2 = rd.Next(0, 1000);

                // double[] values = ScottPlot.DataGen.RandomWalk(rand, 2);
                Array.Copy(myData1, 1, myData1, 0, myData1.Length - 1);
                Array.Copy(myData2, 1, myData2, 0, myData2.Length - 1);

                myData1[myData1.Length - 1] = v1;
                myData2[myData2.Length - 1] = v2;
                Dispatcher.Invoke(new Action(delegate
                {
                    WpfPlot1.Refresh();
                    //WpfPlot1.Plot.AxisAutoY();
                    //WpfPlot1.Plot.AxisAutoX();//自动调整X轴范围
                }));


                //Thread.Sleep(50);
                await Task.Delay(50);
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
        /// <summary>
        /// 开始进行
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnStart_Click(object sender, RoutedEventArgs e)
        {
            tokenSource = new CancellationTokenSource();
            SPdataProcessing = new Task(() => RefreshOPMUI(), tokenSource.Token);
            SPdataProcessing.Start();

            Array.Copy(myData1, 1, myData1, 0, myData1.Length - 1);
            Array.Copy(myData2, 1, myData2, 0, myData2.Length - 1);

            myData1[myData1.Length - 1] = 50;
            myData2[myData2.Length - 1] = 20;
            WpfPlot1.Plot.AxisAutoY();
            WpfPlot1.Plot.AxisAutoX();
            WpfPlot1.Refresh();
        }

        /// <summary>
        /// 停止
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnStop_Click(object sender, RoutedEventArgs e)
        {
            tokenSource.Cancel();
        }
    }
}
