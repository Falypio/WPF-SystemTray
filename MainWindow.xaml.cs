using Google.Apis.Drive.v3.Data;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using WpfApp1.ViewModels;
using WpfApp1.Views;

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
        /// <summary>
        /// 关闭
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Storyboard_CurrentStateInvalidated(object sender, EventArgs e)
        {
            this.btnMenuOpen.Visibility = Visibility.Collapsed;
        }

        /// <summary>
        /// 显示
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Storyboard_Completed(object sender, EventArgs e)
        {
            this.btnMenuOpen.Visibility = Visibility.Visible;
        }

        /// <summary>
        /// 从主表
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnDataGrid_Click(object sender, RoutedEventArgs e)
        {
            DataGridInDataGrid dataGrid = new DataGridInDataGrid();
            dataGrid.ShowDialog();
        }
        /// <summary>
        /// 关闭
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Closed(object sender, EventArgs e)
        {
            notifyIcon.Dispose();
            System.Windows.Application.Current.Shutdown();
        }
        /// <summary>
        /// 打开示波器
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnScope_Click(object sender, RoutedEventArgs e)
        {
            ScopeView dataGrid = new ScopeView();
            dataGrid.ShowDialog();
        }
        /// <summary>
        /// 打开列头菜单
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnPrism_Click(object sender, RoutedEventArgs e)
        {
            MainPrism dataGrid = new MainPrism();
            dataGrid.ShowDialog();
        }

        /// <summary>
        /// 打开随机数
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnRandom_Click(object sender, RoutedEventArgs e)
        {

            RandomView dataGrid = new RandomView();
            dataGrid.ShowDialog();
        }
    }
}
