using Hardcodet.Wpf.TaskbarNotification;
using Prism.Events;
using Prism.Ioc;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace WpfApp1
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {

        // 静态属性，用于全局访问 IEventAggregator 实例
        public static IEventAggregator EventAggregator { get; private set; }

        public App()
        {
            // 在应用启动时初始化 EventAggregator 实例
            EventAggregator = new EventAggregator();
        }
        protected override void OnStartup(StartupEventArgs e)
        {
            _taskbar = (TaskbarIcon)FindResource("Taskbar");
            base.OnStartup(e);
        }
        private TaskbarIcon _taskbar;
    }
}
