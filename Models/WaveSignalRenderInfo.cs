
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp1.Models
{
    public class WaveSignalRenderInfo
    {
        public string name;
        public double[] ys;
        public int renderedIndex;
        public int capacity;
        //public SignalPlot? plot;

        public WaveSignalRenderInfo(string name, int capacity)
        {
            this.name = name;
            this.capacity = capacity;
            ys = new double[capacity];
            renderedIndex = 0;
        }
    }
}
