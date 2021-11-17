using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace WpfApp1
{
    /// <summary>
    /// HumanWork.xaml 的交互逻辑
    /// </summary>
    public partial class HumanWork : Window
    {
        public string Company_code;
        // public string Select_item;    
        public ObservableCollection<Facility> Items { get; set; }
        private delegate void ThreadDelegate(); //申明一个专用来调用更改线程函数的委托    
        public DispatcherTimer ShowTimer;
        public HumanWork(string company_code)
        {
            this.Company_code = company_code;
            InitializeComponent();
            this.WindowState = WindowState.Maximized;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //datagrid数据的加载    
            Items = new ObservableCollection<Facility>();
            DataSet dsFacility = new DataSet();
            DataSet dsFacilityDetail  = new DataSet();
            int count = dsFacility.Tables[0].Rows.Count;
            for (int i = 0; i < count; i++)
            {
                Items.Add(new Facility(Company_code, i, dsFacility, dsFacilityDetail));
            }
            dataGrid1.Items.Clear();
            dataGrid1.ItemsSource = Items;
            dataGrid1.Items.Refresh();
            dataGrid1.SelectedValuePath = "facility_type";
        }

        //展开，收缩子表的方法     
        private void Expander_Expanded(object sender, RoutedEventArgs e)
        {
            DataGridRow row = FindVisualParent<DataGridRow>(sender as Expander);
            row.DetailsVisibility = System.Windows.Visibility.Visible;
        }

        private void Expander_Collapsed(object sender, RoutedEventArgs e)
        {
            DataGridRow row = FindVisualParent<DataGridRow>(sender as Expander);
            row.DetailsVisibility = System.Windows.Visibility.Collapsed;
        }
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
        //下面的方法曾让我教头烂额，感叹微软的控件封装的太牛逼了，处理起来有点变态    
        /// <summary>    
        /// 找到行明细中嵌套的控件名称    
        /// </summary>    
        /// <typeparam name="T"></typeparam>    
        /// <param name="parent"></param>    
        /// <param name="name"></param>    
        /// <returns></returns>    
        public T FindVisualChildByName<T>(DependencyObject parent, string name) where T : DependencyObject
        {
            if (parent != null)
            {
                for (int i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
                {
                    var child = VisualTreeHelper.GetChild(parent, i) as DependencyObject;
                    string controlName = child.GetValue(Control.NameProperty) as string;
                    if (controlName == name)
                    {
                        return child as T;
                    }
                    else
                    {
                        T result = FindVisualChildByName<T>(child, name);
                        if (result != null)
                            return result;
                    }
                }
            }
            return null;
        }

        //父表的事件处理驱动    
        private void dataGrid1_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (dataGrid1.CurrentCell.Column != null && dataGrid1.CurrentCell.Column.Header != null)
            {
                string facility_type = (dataGrid1.Columns[1].GetCellContent(dataGrid1.CurrentCell.Item) as TextBlock).Text;
                string head = dataGrid1.CurrentCell.Column.Header.ToString();
                //这边可以根据实际写自己的一些方法    
            }
        }

        //字表,也就是rowdetailtemplate中的datagrid 的事件处理方法，其中有调用上面的约束泛型类型的方法。    
        private void dataGrid2_MouseUp(object sender, MouseButtonEventArgs e)
        {
            //下面的两行代码很关键    
            DataGridRow dgr = (DataGridRow)dataGrid1.ItemContainerGenerator.ContainerFromIndex(this.dataGrid1.SelectedIndex);
            DataGrid dg = FindVisualChildByName<DataGrid>(dgr, "dataGrid2") as DataGrid;
            //获取鼠标点击的行每一单元格的值   
            string facility_type = (dataGrid1.Columns[1].GetCellContent(dataGrid1.CurrentCell.Item) as TextBlock).Text;
            string building_code = (dg.Columns[1].GetCellContent(dg.CurrentCell.Item) as TextBlock).Text;
            string floor = (dg.Columns[2].GetCellContent(dg.CurrentCell.Item) as TextBlock).Text;
            string dept_code = dg.SelectedValue.ToString();
            string head = dg.CurrentCell.Column.Header.ToString();
            //获得一些信息后你可以自己写方法实现需要的功能    
        }
    }
    // 定义集合类用于存放栏位值    
    public class Facility
    {
        //static string FACILITY_TYPE;    
        public string Facility_type { get; set; }
        public string Building_code { get; set; }
        public string Floor { get; set; }
        public string Dept_code { get; set; }
        public string Count_all { get; set; }
        public string Count_no { get; set; }
        public string Count_yes { get; set; }
        public ObservableCollection<Facility_Detail> Details { get; set; }
        public Facility(string company_code, int row_index, DataSet dsfacilitySum, DataSet dsFacilityDetail)
        {

            Facility_type = dsfacilitySum.Tables[0].Rows[row_index]["facility_type"].ToString();
            Count_all = dsfacilitySum.Tables[0].Rows[row_index]["count_all"].ToString();
            Count_no = dsfacilitySum.Tables[0].Rows[row_index]["count_no"].ToString();
            Count_yes = dsfacilitySum.Tables[0].Rows[row_index]["count_yes"].ToString();
            Details = new ObservableCollection<Facility_Detail>();
            //详细信息    
            System.Data.DataTable dtDetail = new System.Data.DataTable();
            DataRow[] rowDetail = dsFacilityDetail.Tables[0].Select("facility_type='" + Facility_type + "'");
            dtDetail = dsFacilityDetail.Tables[0].Clone();
            foreach (DataRow dr in rowDetail)
            {
                Details.Add(new Facility_Detail()
                {
                    facility_type = Facility_type,
                    building_code = dr["building_code"].ToString(),
                    floor = dr["floor"].ToString(),
                    dept_code = dr["dept_code"].ToString(),
                    dept_name = dr["dept_name"].ToString(),
                    count_all = dr["count_all"].ToString(),
                    count_no = dr["count_no"].ToString(),
                    count_yes = dr["count_yes"].ToString()
                });
            }
        }
    }
    public class Facility_Detail
    {   //定义属性 
        public string facility_type { get; set; }
        public string building_code { get; set; }
        public string floor { get; set; }
        public string dept_code { get; set; }
        public string dept_name { get; set; }
        public string count_all { get; set; }
        public string count_no { get; set; }
        public string count_yes { get; set; }
    }
}
