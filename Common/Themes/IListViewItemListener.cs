using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp1.Common.Themes
{
    public interface IListViewItemListener
    {
        /// <summary>
        /// 选中
        /// </summary>
        /// <param name="key"></param>
        void OnSelectChanged(int key);
        /// <summary>
        /// 添加
        /// </summary>
        /// <param name="key"></param>
        /// <param name="name"></param>
        void OnAdded(int key, string name);

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="key"></param>
        void OnRemoved(int key);

        /// <summary>
        /// 清空
        /// </summary>
        void OnCleared();

        /// <summary>
        /// 是否隐藏
        /// </summary>
        /// <param name="visbility"></param>
        /// <param name="key"></param>
        void OnVisibilityChanged(bool visbility, int key);
    }
}
