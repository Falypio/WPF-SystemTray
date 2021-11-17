using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace WpfApp1.Models
{
    /// <summary>
    /// 主表
    /// </summary>
    public class Question
    {
        public int questionID//ID号
        { get; set; }
        public string questionName//内容:
        { get; set; }
        public ObservableCollection<ChoseItem> choseItems//表头集合
        { get; set; }
        public Question(Int32 _id, string _questionname, ObservableCollection<ChoseItem> _choseitems)//构造函数
        {
            questionID = _id;
            questionName = _questionname;
            choseItems = _choseitems;
        }
    }
    //从表类
    public class ChoseItem
    {
        public string ChoseName//表头编号或名称,比如:A,B,C,D之类
        { get; set; }
        public string ChoseContent//内容
        { get; set; }
    }
}
