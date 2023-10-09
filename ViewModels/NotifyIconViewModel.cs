using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Input;
using WpfApp1.Common;

namespace WpfApp1.ViewModels
{
    public class NotifyIconViewModel
    {
        /// <summary>
        /// 如果窗口没显示，就显示窗口
        /// </summary>
        public ICommand ShowWindowCommand
        {
            get
            {
                return new DelegateCommand
                {
                    //CanExecuteFunc = () => Application.Current.MainWindow.Visibility == Visibility.Hidden,
                    CommandAction = () =>
                    {
                        //Application.Current.MainWindow = new MainWindow();
                        Application.Current.MainWindow.Visibility = Visibility.Visible;
                    }
                };
            }
        }
        /// <summary>
        /// 隐藏窗口
        /// </summary>
        public ICommand HideWindowCommand
        {
            get
            {
                return new DelegateCommand
                {
                    CommandAction = () => Application.Current.MainWindow.Visibility = Visibility.Hidden,
                    //CanExecuteFunc = () => Application.Current.MainWindow.Visibility != Visibility.Hidden
                };
            }
        }
        /// <summary>
        /// 关闭软件
        /// </summary>
        public ICommand ExitApplicationCommand
        {
            get
            {
                return new DelegateCommand { CommandAction = () => Application.Current.Shutdown() };
            }
        }
    }
}
