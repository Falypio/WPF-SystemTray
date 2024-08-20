using Prism.Events;
using Prism.Services.Dialogs;
using System.Windows;

namespace WpfApp1.Views
{
    /// <summary>
    /// Interaction logic for OscilloscopeView.xaml
    /// </summary>
    public partial class OscilloscopeView : Window
    {
        public OscilloscopeView(IEventAggregator eventAggregator)
        {
            InitializeComponent();
        }
    }
}
