using Google.Apis.Drive.v3.Data;
using System;
using System.Windows;
using System.Windows.Forms;

namespace WpfApp1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        NotifyIcon notifyIcon = new NotifyIcon();
        public MainWindow()
        {
            InitializeComponent();
            this.DataContext = new NotifyIconViewModel();//把方法绑定上去
        }
        private void Storyboard_CurrentStateInvalidated(object sender, EventArgs e)
        {
            this.btnMenuOpen.Visibility = Visibility.Collapsed;
        }

        private void Storyboard_Completed(object sender, EventArgs e)
        {
            this.btnMenuOpen.Visibility = Visibility.Visible;
        }

        private void btnDataGrid_Click(object sender, RoutedEventArgs e)
        {
            DataGridInDataGrid dataGrid = new DataGridInDataGrid();
            dataGrid.ShowDialog();
        }
        private void Window_Closed(object sender, EventArgs e)
        {
            notifyIcon.Dispose();
            System.Windows.Application.Current.Shutdown();
        }
    }
}
