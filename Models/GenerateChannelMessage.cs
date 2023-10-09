using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp1.Models
{
    public class GenerateChannelMessage
    {
        public WaveSignalRenderInfo Signal { get; set; }
        /// <summary>
        /// 数据间隔
        /// </summary>
        public int Interval { get; set; }
    }
}
