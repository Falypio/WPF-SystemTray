using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Markup;
using System.Windows;

namespace WpfApp1.Common
{
    /*
 *使用教程：
 * 添加头文件     xmlns:ser="clr-namespace:WpfApp1.Common;assembly=WpfApp1"
 * 绑定 {ser:BindableStaticResource {Binding MyResourceKey}}
 */
    /// <summary>
    /// 静态资源转换绑定
    /// </summary>
    public class BindableStaticResource : StaticResourceExtension
    {
        private static readonly DependencyProperty m_dummyProperty;

        static BindableStaticResource()
        {
            m_dummyProperty = DependencyProperty.RegisterAttached("Dummy",
                                                                typeof(Object),
                                                                typeof(DependencyObject),
                                                                new UIPropertyMetadata(null));
        }

        public Binding MyBinding { get; set; }

        public BindableStaticResource()
        {
        }

        public BindableStaticResource(Binding binding)
        {
            MyBinding = binding;
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            var target = (IProvideValueTarget)serviceProvider.GetService(typeof(IProvideValueTarget));
            var targetObject = (FrameworkElement)target.TargetObject;

            MyBinding.Source = targetObject.DataContext;
            var DummyDO = new DependencyObject();
            BindingOperations.SetBinding(DummyDO, m_dummyProperty, MyBinding);

            ResourceKey = DummyDO.GetValue(m_dummyProperty);
            return ResourceKey != null ? base.ProvideValue(serviceProvider) : null;
        }
        public new object ResourceKey
        {
            get
            {
                return base.ResourceKey;
            }
            set
            {
                if (value != null)
                {
                    base.ResourceKey = value;
                }
            }
        }
    }
}
