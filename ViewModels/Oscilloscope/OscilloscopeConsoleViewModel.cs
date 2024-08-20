using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using WpfApp1.Common.Themes;

namespace WpfApp1.ViewModels.Oscilloscope
{
    /// <summary>
    /// 示波器控制台
    /// </summary>
	public class OscilloscopeConsoleViewModel : BindableBase
    {
        private IListViewItemListener iListener = null!;

        public OscilloscopeConsoleViewModel(IListViewItemListener lisenter)
        {
            iListener = lisenter;
        }
	}
}
