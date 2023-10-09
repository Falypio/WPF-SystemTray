using ScottPlot.Plottable;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace WpfApp1.Models
{
    /// <summary>
    /// 标尺类
    /// </summary>
    public class AxisLineVernier
    {
        /// <summary>
        /// 标尺参数(X轴或Y轴)
        /// </summary>
        public AxisLine  AxisLineModel { get; set; }

        /// <summary>
        /// 标尺颜色
        /// </summary>
        public Brush LineColor { get; set; }

        /// <summary>
        /// 标尺选中
        /// </summary>
        public bool AxisLineDisabled { get; set; }

        /// <summary>
        /// 标尺轴类型 (ture=X false=Y)
        /// </summary>
        public string AxisLineXY { get; set; }
    }
}
