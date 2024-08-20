using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WpfApp1.Models;

namespace WpfApp1.Views.Oscilloscope.DataGridLinkage
{
    /// <summary>
    /// DataGridControl1.xaml 的交互逻辑
    /// </summary>
    public partial class DataGridControl : UserControl
    {

        public static readonly DependencyProperty VerticalOffsetProperty =
        DependencyProperty.Register("VerticalOffset", typeof(double), typeof(DataGridControl), new PropertyMetadata(0.0));

        public double VerticalOffset
        {
            get { return (double)GetValue(VerticalOffsetProperty); }
            set { SetValue(VerticalOffsetProperty, value); }
        }



        private void DataGrid1_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            VerticalOffset = e.VerticalOffset;
        }


        public DataGridControl()
        {
            InitializeComponent();
            DataGrid1.ItemsSource = Questions;
        }

        ObservableCollection<Question> Questions = new ObservableCollection<Question>();//主表组
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            string[] CharStr = new string[4] { "1", "2", "3", "4" };
            for (int i = 0; i < 5; i++)
            {
                ObservableCollection<ChoseItem> ChoseItems = new ObservableCollection<ChoseItem>();//从表组               
                for (int j = 0; j < 4; j++)
                {
                    ChoseItem item = new ChoseItem();//从表类
                    item.ChoseName = CharStr[j];
                    item.ChoseContent = "内容";
                    ChoseItems.Add(item);
                }
                Questions.Add(new Question(i, "__第" + (i + 1).ToString() + "列", ChoseItems));
            }

        }

        /// <summary>
        /// 展开从表
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Expander_Expanded(object sender, RoutedEventArgs e)
        {
            DataGridRow row = FindVisualParent<DataGridRow>(sender as Expander);
            row.DetailsVisibility = System.Windows.Visibility.Visible;
        }
        /// <summary>
        /// 关闭从表
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Expander_Collapsed(object sender, RoutedEventArgs e)
        {
            DataGridRow row = FindVisualParent<DataGridRow>(sender as Expander);
            row.DetailsVisibility = System.Windows.Visibility.Collapsed;
        }

        /// <summary>
        /// 构造数据获取
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="child"></param>
        /// <returns></returns>
        public T FindVisualParent<T>(DependencyObject child) where T : DependencyObject
        {
            DependencyObject parentObject = VisualTreeHelper.GetParent(child);
            if (parentObject == null) return null;
            T parent = parentObject as T;
            if (parent != null)
                return parent;
            else
                return FindVisualParent<T>(parentObject);
        }
    }
}
