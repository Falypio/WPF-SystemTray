using System;
using System.Windows;
using System.Windows.Controls;

namespace WpfApp1.Common.CircleMenu
{
    /// <summary>
    /// 圆形菜单属性重写
    /// </summary>
    public class CircleMenuItemsPresenter : ItemsPresenter
    {
        public static readonly DependencyProperty StatusProperty = DependencyProperty.Register(
            "Status", typeof(CircleMenuStatus), typeof(CircleMenuItemsPresenter), new PropertyMetadata(default(CircleMenuStatus)));

        public CircleMenuStatus Status
        {
            get { return (CircleMenuStatus)GetValue(StatusProperty); }
            set { SetValue(StatusProperty, value); }
        }

        public static readonly DependencyProperty AngleProperty = DependencyProperty.Register(
            "Angle", typeof(Double), typeof(CircleMenuItemsPresenter), new PropertyMetadata(360d));

        public double Angle
        {
            get { return (Double)GetValue(AngleProperty); }
            set { SetValue(AngleProperty, value); }
        }
    }
}
