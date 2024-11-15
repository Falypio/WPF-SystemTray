﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using WpfApp1.Models;

namespace WpfApp1.Views
{
    /// <summary>
    /// DataGridInDataGrid.xaml 的交互逻辑
    /// </summary>
    public partial class DataGridInDataGrid : Window
    {
        public static readonly DependencyProperty SharedVerticalOffsetProperty =
        DependencyProperty.Register("SharedVerticalOffset", typeof(double), typeof(DataGridInDataGrid), new PropertyMetadata(0.0, OnSharedVerticalOffsetChanged));

        public double SharedVerticalOffset
        {
            get { return (double)GetValue(SharedVerticalOffsetProperty); }
            set { SetValue(SharedVerticalOffsetProperty, value); }
        }

        private static void OnSharedVerticalOffsetChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var mainWindow = d as DataGridInDataGrid;
            if (mainWindow != null)
            {
                var newOffset = (double)e.NewValue;
                // 获取Page1中的DataGrid的ScrollViewer并滚动到新的位置
                ScrollViewer svPage1 = GetDescendantScrollViewer(mainWindow.Control1.DataGrid1);
                if (svPage1 != null)
                {
                    svPage1.ScrollToVerticalOffset(newOffset);
                }

                // 获取Page2中的DataGrid的ScrollViewer并滚动到新的位置
                ScrollViewer svPage2 = GetDescendantScrollViewer(mainWindow.Control2.DataGrid1);
                if (svPage2 != null)
                {
                    svPage2.ScrollToVerticalOffset(newOffset);
                }
            }
        }

        private static ScrollViewer GetDescendantScrollViewer(DependencyObject element)
        {
            if (element == null) return null;

            if (element is ScrollViewer) return (ScrollViewer)element;

            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(element); i++)
            {
                var child = VisualTreeHelper.GetChild(element, i);

                var result = GetDescendantScrollViewer(child);
                if (result != null) return result;
            }

            return null;
        }
        public DataGridInDataGrid()
        {
            InitializeComponent();
            datagrid1.ItemsSource = Questions;
            listBox.ItemsSource = Questions;

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

        private void ToggleButton_Checked(object sender, RoutedEventArgs e)
        {

        }

        private void ToggleButton_Unchecked(object sender, RoutedEventArgs e)
        {

        }
        /// <summary>
        /// 从表增加滚轮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void datagrid3_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (!e.Handled)
            {
                // 内层ListBox拦截鼠标滚轮事件
                e.Handled = true;

                // 激发一个鼠标滚轮事件，冒泡给外层ListBox接收到
                var eventArg = new MouseWheelEventArgs(e.MouseDevice, e.Timestamp, e.Delta);
                eventArg.RoutedEvent = UIElement.MouseWheelEvent;
                eventArg.Source = sender;
                var parent = ((Control)sender).Parent as UIElement;
                parent.RaiseEvent(eventArg);
            }
        }

    }
}
