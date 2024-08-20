using Prism.Mvvm;
using System.Windows.Media;

namespace WpfApp1.Models
{
    public class PieSegmentModel : BindableBase
    {
        private Brush _color;
        private Brush _colorStroke = Brushes.White;
        private double _endAngle;

        private string _name;


        private double _startAngle;
        private double _value;



        public Brush ColorStroke
        {
            get => _colorStroke;
            set { SetProperty(ref _colorStroke, value); }
        }

        public Brush Color
        {
            get => _color;
            set { SetProperty(ref _color, value); }
        }

        public double Value
        {
            get => _value;
            set { SetProperty(ref _value, value); }
        }

        public string Name
        {
            get => _name;
            set { SetProperty(ref _name, value); }
        }

        public double StartAngle
        {
            get => _startAngle;
            set { SetProperty(ref _startAngle, value); }
        }

        public double EndAngle
        {
            get => _endAngle;
            set { SetProperty(ref _endAngle, value); }
        }
    }
}
