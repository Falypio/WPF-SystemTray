using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using WpfApp1.Common.Themes;

namespace WpfApp1.ViewModels.Oscilloscope
{
    /// <summary>
    /// 滑动标尺列表
    /// </summary>
	public class VernierListViewModel : BindableBase
    {
        private IListViewItemListener iListener = null!;

        public VernierListViewModel(IListViewItemListener lisenter)
        {
            iListener = lisenter;
        }
	}
}
