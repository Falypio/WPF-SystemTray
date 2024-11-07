using Prism.Events;
using Prism.Services.Dialogs;
using ScottPlot;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using WpfApp1.Common.EventMessages;

namespace WpfApp1.Views
{
    /// <summary>
    /// Interaction logic for OscilloscopeView.xaml
    /// </summary>
    public partial class OscilloscopeView : Window
    {
        private readonly IEventAggregator aggregator;
        public OscilloscopeView()
        {
            this.aggregator = App.EventAggregator;
            EventAggregatorSubscribe(this.aggregator);
            InitializeComponent();
            LockY.Click += LockY_Click;
            LockX.Click += LockX_Click;
        }

        private void EventAggregatorSubscribe(IEventAggregator aggregator)
        {
            aggregator.GetEvent<AddChanelEvent>().Subscribe(OnChannelAddedCallback);
            aggregator.GetEvent<DelChanelEvent>().Subscribe(OnChannelDeledCallback);
            aggregator.GetEvent<SelectChannelEvent>().Subscribe(OnChannelSelectedCallback);
        }

        private void OnChannelAddedCallback(ItemAddedRecord record)
        {
            Plot.GenerateSignal(record.id.ToString(), record.name);
        }

        private void OnChannelDeledCallback(int id)
        {
            Plot.RemoveSignal(id.ToString());
        }
        private void OnChannelSelectedCallback(int id)
        {
            Plot.ChannelSelected(id.ToString());
        }

        private void BtnStart_Click(object sender, RoutedEventArgs e)
        {
            Plot.TimerStart();
        }

        private void BtnEnd_Click(object sender, RoutedEventArgs e)
        {
            Plot.TimerStop();
        }

        private void Frequency_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if(Frequency.SelectedItem is ComboBoxItem selectedItem)
            {
                string selectedValue = selectedItem.Content.ToString();

                Plot.UpdateAddSystemTimer(selectedValue);
            }
        }

        private void LockX_Click(object sender, RoutedEventArgs e)
        {
            if (sender is ToggleButton btn)
            {
                Plot.LockHorizontal((bool)btn.IsChecked);
            }
        }

        private void LockY_Click(object sender, RoutedEventArgs e)
        {
            if (sender is ToggleButton btn)
            {
                Plot.LockVertical((bool)btn.IsChecked);
            }
        }
    }
}
