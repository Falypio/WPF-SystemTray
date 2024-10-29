using LiveChartsCore;
using LiveChartsCore.Defaults;
using LiveChartsCore.Drawing;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Extensions;
using LiveChartsCore.SkiaSharpView.Painting;
using LiveChartsCore.SkiaSharpView.Painting.Effects;
using Prism.Commands;
using Prism.Mvvm;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using System.Windows.Input;

namespace WpfApp1.ViewModels
{
	public class LiveChartViewModel : BindableBase
	{
        private static readonly SKColor s_gray = new(195, 195, 195);
        private static readonly SKColor s_gray1 = new(160, 160, 160);
        private static readonly SKColor s_gray2 = new(90, 90, 90);
        private static readonly SKColor s_dark3 = new(60, 60, 60);


        private readonly Random _random = new();
        private readonly List<DateTimePoint> _values = new();
        private readonly List<DateTimePoint> _values2 = new();
        private readonly List<DateTimePoint> _values3 = new();
        private readonly List<DateTimePoint> _values4 = new();
        private readonly List<DateTimePoint> _values5 = new();
        private readonly List<DateTimePoint> _values6 = new();
        private readonly List<DateTimePoint> _values7 = new();
        private readonly List<DateTimePoint> _values8 = new();
        private readonly DateTimeAxis _customAxis;

        private List<List<DateTimePoint>> _valuesMax = new List<List<DateTimePoint>>();

        public ObservableCollection<ISeries> Series { get; set; }

        public Axis[] XAxes { get; set; }

        public object Sync { get; } = new object();

        public bool IsReading { get; set; } = true;


        /// <summary>
        /// 饼状图
        /// </summary>
        public IEnumerable<ISeries> PieChart { get; set; } = new[] { 6, 5, 4, 3, 2 }.AsPieSeries((value, series) =>
        {
            // pushes out the slice with the value of 6 to 30 pixels.
            if (value != 6) return;

            series.Pushout = 30;
        });

        public DrawMarginFrame Frame { get; set; } = new()
        {
            Fill = new SolidColorPaint(s_dark3),
            Stroke = new SolidColorPaint
            {
                Color = s_gray,
                StrokeThickness = 1
            }
        };

        public Axis[] YAxes { get; set; } =
        {
            new Axis
            {
                Name = "Y axis",
                NamePaint = new SolidColorPaint(s_gray1),
                TextSize = 18,
                Padding = new Padding(5, 0, 15, 0),
                LabelsPaint = new SolidColorPaint(s_gray),
                SeparatorsPaint = new SolidColorPaint
                {
                    Color = s_gray,
                    StrokeThickness = 1,
                    PathEffect = new DashEffect(new float[] { 3, 3 })
                },
                SubseparatorsPaint = new SolidColorPaint
                {
                    Color = s_gray2,
                    StrokeThickness = 0.5f
                },
                SubseparatorsCount = 9,
                ZeroPaint = new SolidColorPaint
                {
                    Color = s_gray1,
                    StrokeThickness = 2
                },
                TicksPaint = new SolidColorPaint
                {
                    Color = s_gray,
                    StrokeThickness = 1.5f
                },
                SubticksPaint = new SolidColorPaint
                {
                    Color = s_gray,
                    StrokeThickness = 1
                }
            }
        };


        public ICommand StartCommand { get; set; }

        public ICommand StopCommand { get; set; }

        public LiveChartViewModel()
        {
            StartCommand = new DelegateCommand(EcextStartCommand);
            StopCommand = new DelegateCommand(EcextStopCommand);
            Series = new ObservableCollection<ISeries>();
            for (int i = 0; i < 7; i++)
            {
                _valuesMax.Add(new List<DateTimePoint>());
                Series.Add(new LineSeries<DateTimePoint>
                {
                    Values = _valuesMax[i],
                    Fill = null,
                    GeometryFill = null,
                    GeometryStroke = null
                });
            }

            //Series = new ObservableCollection<ISeries>
            //{
            //    new LineSeries<DateTimePoint>
            //    {
            //        Values = _values,
            //        Fill = null,
            //        GeometryFill = null,
            //        GeometryStroke = null
            //    },
            //    new LineSeries<DateTimePoint>
            //    {
            //        Values = _values2,
            //        Fill = null,
            //        GeometryFill = null,
            //        GeometryStroke = null
            //    },
            //    new LineSeries<DateTimePoint>
            //    {
            //        Values = _values3,
            //        Fill = null,
            //        GeometryFill = null,
            //        GeometryStroke = null
            //    },
            //    new LineSeries<DateTimePoint>
            //    {
            //        Values = _values4,
            //        Fill = null,
            //        GeometryFill = null,
            //        GeometryStroke = null
            //    },
            //    new LineSeries<DateTimePoint>
            //    {
            //        Values = _values5,
            //        Fill = null,
            //        GeometryFill = null,
            //        GeometryStroke = null
            //    }
            //};

            _customAxis = new DateTimeAxis(TimeSpan.FromSeconds(1), Formatter)
            {
                CustomSeparators = GetSeparators(),
                AnimationsSpeed = TimeSpan.FromMilliseconds(0),
                SeparatorsPaint = new SolidColorPaint(SKColors.Black.WithAlpha(100)),
                //MinStep = 0,
              //MinZoomDelta = 1000,
                //MaxLimit = 1000,
            };

            XAxes = new Axis[] { _customAxis };

            _ = ReadData();
        }

        private void EcextStartCommand()
        {
            IsReading = true;
            _ = ReadData();
        }

        private void EcextStopCommand()
        {
            IsReading = false;
        }


        private async Task ReadData()
        {
            // to keep this sample simple, we run the next infinite loop 
            // in a real application you should stop the loop/task when the view is disposed 

            while (IsReading)
            {
                await Task.Delay(50);

                // Because we are updating the chart from a different thread 
                // we need to use a lock to access the chart data. 
                // this is not necessary if your changes are made in the UI thread. 
                lock (Sync)
                {
                    try
                    {
                        foreach (var values in _valuesMax)
                        {
                            values.Add(new DateTimePoint(DateTime.Now, _random.Next(0, 10)));
                            if (values.Count > 250) values.RemoveAt(0);
                        }
                        //_values.Add(new DateTimePoint(DateTime.Now, _random.Next(0, 10)));
                        //if (_values.Count > 250) _values.RemoveAt(0);

                        //_values2.Add(new DateTimePoint(DateTime.Now, _random.Next(0, 15)));
                        //if (_values2.Count > 250) _values2.RemoveAt(0);

                        //_values3.Add(new DateTimePoint(DateTime.Now, _random.Next(0, 20)));
                        //if (_values3.Count > 250) _values3.RemoveAt(0);

                        //_values4.Add(new DateTimePoint(DateTime.Now, _random.Next(0, 25)));
                        //if (_values4.Count > 250) _values4.RemoveAt(0);

                        //_values5.Add(new DateTimePoint(DateTime.Now, _random.Next(0, 30)));
                        //if (_values5.Count > 250)
                        //{
                        //    _values5.RemoveAt(0);
                        //}
                    }
                    catch (Exception ex)
                    {

                    }


                    // we need to update the separators every time we add a new point 
                    _customAxis.CustomSeparators = GetSeparators();
                }
            }
        }

        private double[] GetSeparators()
        {
            var now = DateTime.Now;

            return new double[]
            {
            now.AddSeconds(-25).Ticks,
            now.AddSeconds(-20).Ticks,
            now.AddSeconds(-15).Ticks,
            now.AddSeconds(-10).Ticks,
            now.AddSeconds(-5).Ticks,
            now.Ticks
            };
        }

        private static string Formatter(DateTime date)
        {
            var secsAgo = (DateTime.Now - date).TotalSeconds;

            return secsAgo < 1
                ? "now"
                : $"{secsAgo:N0}s ago";
        }
    }
}
