using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Media;
using WpfApp1.Common;

namespace WpfApp1.ViewModels
{
    public class MainPrismViewModel : BindableBase
    {
        public CollapsedOfMarginLeftCommand  collapsedOfMarginLeftCommand { get; set; }

        public VisibleOfMarginLeftCommand visibleOfMarginLeftCommand { get; set; }
        private ObservableCollection<MenuCless> m_menuClessList;
        public ObservableCollection<MenuCless>  MenuClessList
        {
            get { return m_menuClessList; }
            set { SetProperty(ref m_menuClessList, value); }
        }
        public MainPrismViewModel()
        {
            collapsedOfMarginLeftCommand = new CollapsedOfMarginLeftCommand();
            visibleOfMarginLeftCommand = new VisibleOfMarginLeftCommand();
            MenuClessList = new ObservableCollection<MenuCless>();
            for (int i = 0; i < 6; i++)
            {
                MenuCless menuCless = new MenuCless();
                menuCless.MenuName = $"菜单{i}";
                menuCless.MyResourceKey = "ChangeOverImage";
                menuCless.MenuColor = Brushes.White;
                MenuClessList.Add(menuCless);
            }
        }
    }

    public class MenuCless : BindableBase
    {
        private string m_menuName;
        public string MenuName
        {
            get { return m_menuName; }
            set { SetProperty(ref m_menuName, value); }
        }
        private string m_myResourceKey;
        public string MyResourceKey
        {
            get { return m_myResourceKey; }
            set { SetProperty(ref m_myResourceKey, value); }
        }
        private Brush m_menuColor;
        public Brush MenuColor
        {
            get { return m_menuColor; }
            set { SetProperty(ref m_menuColor, value); }
        }
    }
}
