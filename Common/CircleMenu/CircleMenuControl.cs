using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Animation;
using System.Windows;
using System.Windows.Controls.Primitives;

namespace WpfApp1.Common.CircleMenu
{
    /// <summary>
    /// 圆形菜单控制器
    /// </summary>
    [TemplatePart(Name = PART_PartCenterBtn)]
    [TemplatePart(Name = PART_PartContainer)]
    [TemplatePart(Name = PART_PartPanelPresenter)]
    [TemplateVisualState(GroupName = "CommonStates", Name = VisualStateInitial)]
    [TemplateVisualState(GroupName = "CommonStates", Name = VisualStateExpanded)]
    [TemplateVisualState(GroupName = "CommonStates", Name = VisualStateCollapsed)]
    public class CircleMenuControl : ItemsControl
    {
        private const string PART_PartCenterBtn = "PART_CenterBtn";
        private const string PART_PartContainer = "PART_Container";
        private const string PART_PartPanelPresenter = "PART_PanelPresenter";
        public const string VisualStateInitial = "Initial";
        public const string VisualStateExpanded = "Expanded";
        public const string VisualStateCollapsed = "Collapsed";
        double distance = 10;
        double distanceNew = 50;//边沿间距
        private bool move = false;
        private Point lastPos;
        private Point newPos;
        private Point oldPos;
        static CircleMenuControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(CircleMenuControl), new FrameworkPropertyMetadata(typeof(CircleMenuControl)));
        }

        #region dependency property

        public static readonly DependencyProperty AngleProperty = DependencyProperty.Register(
            "Angle", typeof(double), typeof(CircleMenuControl), new PropertyMetadata(360d));

        public double Angle
        {
            get { return (double)GetValue(AngleProperty); }
            set { SetValue(AngleProperty, value); }
        }

        #endregion

        private Button? _centerBtn;
        private Grid? _container;
        private CircleMenuPanel? _circleMenuPanel;
        private CircleMenuItemsPresenter? _circleMenuItemsPresenter;

        public override void OnApplyTemplate()
        {
            if (_centerBtn != null)
            {
                _centerBtn.PreviewMouseLeftButtonDown -= centerBtn_Click;
                //_centerBtn.MouseLeave -= CircleMenuControl_MouseLeave;
            }

            base.OnApplyTemplate();

            _centerBtn = GetTemplateChild(PART_PartCenterBtn) as Button;
            _container = GetTemplateChild(PART_PartContainer) as Grid;
            _circleMenuItemsPresenter = GetTemplateChild(PART_PartPanelPresenter) as CircleMenuItemsPresenter;

            if (_centerBtn != null)
            {
                _centerBtn.PreviewMouseLeftButtonDown += centerBtn_Click;
                //_centerBtn.MouseLeave += CircleMenuControl_MouseLeave;
            }
            this.Loaded += CircleMenuControl_Loaded;
            this.MouseLeave += CircleMenuControl_MouseLeave;
        }


        /// <summary>
        /// 鼠标离开
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CircleMenuControl_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (this.Parent != null && this.Parent is FrameworkElement)
            {

                FrameworkElement parent = this.Parent as FrameworkElement;
                parent.PreviewMouseMove -= Parent_PreviewMouseMove;
                parent.PreviewMouseUp -= Parent_PreviewMouseUp;
            }
        }

        private void CircleMenuControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (this.Parent != null && this.Parent is FrameworkElement)
            {
                FrameworkElement parent = this.Parent as FrameworkElement;
                double left = parent.ActualWidth - this.ActualWidth - this.distanceNew;
                double top = parent.ActualHeight - this.ActualHeight - this.distanceNew;
                this.Margin = new Thickness(this.distanceNew, top, left, 0);
                VisualStateManager.GoToState(this, VisualStateExpanded, false);//展开
            }
        }
        /// <summary>
        /// 暂时不用
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void centerBtn_Click(object sender, RoutedEventArgs e)
        {
            var dsd = (System.Windows.Input.MouseButtonEventArgs)e;
            if (dsd.ClickCount > 1)
            {

            }
            else
            {
                if (this.Parent != null && this.Parent is FrameworkElement)
                {
                    FrameworkElement parent = this.Parent as FrameworkElement;
                    move = true;
                    lastPos = dsd.GetPosition(parent);
                    oldPos = lastPos;
                    parent.PreviewMouseMove += Parent_PreviewMouseMove;
                    parent.PreviewMouseUp += Parent_PreviewMouseUp;
                }
                return;
            }
            //第一个参数是<VisualStateManager>所在元素的父元素，本控件中为Grid的父级，即控件本身
            switch (_circleMenuItemsPresenter.Status)
            {
                case CircleMenuStatus.Expanded:
                    VisualStateManager.GoToState(this, VisualStateCollapsed, false);//缩回
                    break;
                case CircleMenuStatus.Initial:
                case CircleMenuStatus.Collapsed:
                    VisualStateManager.GoToState(this, VisualStateExpanded, false);//展开
                    break;
            }
            //如果只是在控件内部更改Panel状态可以直接设置ItemPresenter的Status
            //使用VisualStateManager是为了可以在外部通过更改状态更新面板
        }
        #region 事件方法
        private void Parent_PreviewMouseUp(object sender, MouseButtonEventArgs ee)
        {
            FrameworkElement parent = this.Parent as FrameworkElement;
            if (move)
            {
                move = false;

                Point pos = ee.GetPosition(parent);
                newPos = pos;
                double left = this.Margin.Left + pos.X - this.lastPos.X;
                double top = this.Margin.Top + pos.Y - this.lastPos.Y;
                double right = parent.ActualWidth - left - this.ActualWidth;
                double bottom = parent.ActualHeight - top - this.ActualHeight;

                if (left < distance && top < distance) //左上
                {
                    left = this.distanceNew;
                    top = this.distanceNew;
                }
                else if (left < distance && bottom < distance) //左下
                {
                    left = this.distanceNew;
                    top = parent.ActualHeight - this.ActualHeight - this.distanceNew;
                }
                else if (right < distance && top < distance) //右上
                {
                    left = parent.ActualWidth - this.ActualWidth - this.distanceNew;
                    top = this.distanceNew;
                }
                else if (right < distance && bottom < distance) //右下
                {
                    left = parent.ActualWidth - this.ActualWidth - this.distanceNew;
                    top = parent.ActualHeight - this.ActualHeight - this.distanceNew;
                }
                else if (left < distance && top > distance && bottom > distance) //左
                {
                    left = this.distanceNew;
                    top = this.Margin.Top;
                }
                else if (right < distance && top > distance && bottom > distance) //右
                {
                    left = parent.ActualWidth - this.ActualWidth - this.distanceNew;
                    top = this.Margin.Top;
                }
                else if (top < distance && left > distance && right > distance) //上
                {
                    left = this.Margin.Left;
                    top = this.distanceNew;
                }
                else if (bottom < distance && left > distance && right > distance) //下
                {
                    left = this.Margin.Left;
                    top = parent.ActualHeight - this.ActualHeight - this.distanceNew;
                }

                ThicknessAnimation marginAnimation = new ThicknessAnimation();
                marginAnimation.From = this.Margin;
                marginAnimation.To = new Thickness(left, top, 0, 0);
                marginAnimation.Duration = TimeSpan.FromMilliseconds(200);

                Storyboard story = new Storyboard();
                story.FillBehavior = FillBehavior.Stop;
                story.Children.Add(marginAnimation);
                Storyboard.SetTargetName(marginAnimation, "gMenu");
                Storyboard.SetTargetProperty(marginAnimation, new PropertyPath("(0)", Grid.MarginProperty));

                //story.Begin(this);                        

                this.Margin = new Thickness(left, top, 0, 0);
            }
        }

        private void Parent_PreviewMouseMove(object sender, MouseEventArgs ee)
        {
            FrameworkElement parent = this.Parent as FrameworkElement;
            if (move)
            {
                Point pos = ee.GetPosition(parent);
                double left = this.Margin.Left + pos.X - this.lastPos.X;
                double top = this.Margin.Top + pos.Y - this.lastPos.Y;
                this.Margin = new Thickness(left, top, 0, 0);

                lastPos = ee.GetPosition(parent);
            }
        }
        #endregion
        #region route event

        //route event

        //inner menu click
        public static readonly RoutedEvent SubMenuClickEvent =
            ButtonBase.ClickEvent.AddOwner(typeof(CircleMenuControl));

        public event RoutedEventHandler SubMenuClick
        {
            add { AddHandler(ButtonBase.ClickEvent, value, false); }
            remove { RemoveHandler(ButtonBase.ClickEvent, value); }
        }

        #endregion

    }
}
