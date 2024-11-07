using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using WpfApp1.Common.Themes;

namespace WpfApp1.ViewModels.Oscilloscope
{
    /// <summary>
    /// 通道列表
    /// </summary>
	public class ChannelListViewModel : BindableBase
	{
        private IListViewItemListener channelListener = null!;

        private ObservableCollection<ChannelModel> channelModels;
        public ObservableCollection<ChannelModel>  ChannelModels
        {
            get { return channelModels; }
            set { SetProperty(ref channelModels, value); }
        }

        private ChannelModel selectedChannel;
        public ChannelModel SelectedChannel
        {
            get { return selectedChannel; }
            set
            {
                if (SetProperty(ref selectedChannel, value, nameof(SelectedChannel)))
                {
                    if (selectedChannel != null)
                        channelListener.OnSelectChanged(selectedChannel.ChannelUid);

                }
            }
        }

        public ChannelListViewModel(IListViewItemListener lisenter)
        {
            channelListener = lisenter;

            ChannelModels = new ObservableCollection<ChannelModel>();

        }

        private ICommand channelAddCommand;
        public ICommand ChannelAddCommand => channelAddCommand ??= new DelegateCommand(ExecuteChannelAddCommand);

        private void ExecuteChannelAddCommand()
        {
            if (ChannelModels.Count() >= 16)
            {
                Console.WriteLine("最多16通道");
                return;
            }
            ChannelModel channel = new ChannelModel(ChannelModels.Count() + 1, $"通道{ChannelModels.Count()+1}");
            //channel.ChannelUid = this.GetHashCode()
            ChannelModels.Add(channel);
            channelListener.OnAdded(channel.ChannelUid, channel.ChannelName);
        }

        private ICommand channelDelCommand;
        public ICommand ChannelDelCommand => channelDelCommand ??= new DelegateCommand<object>(ExecuteChannelDelCommand);

        private void ExecuteChannelDelCommand(object id)
        {
            var sds = ChannelModels.Where(x => x.ChannelUid.Equals(id)).FirstOrDefault();
            SelectedChannel = sds;
            channelListener?.OnRemoved(SelectedChannel.ChannelUid);
            ChannelModels.Remove(SelectedChannel);
        }

    }
    public class ChannelModel : BindableBase
    {
        private int channelUid;
        public int ChannelUid
        {
            get { return channelUid; }
            set { SetProperty(ref channelUid, value); }
        }

        private string channelName;
        public string ChannelName
        {
            get { return channelName; }
            set { SetProperty(ref channelName, value); }
        }

        public ChannelModel()
        {
                
        }

        public ChannelModel(int channelUid, string channelName)
        {
            this.channelUid = channelUid;
            this.channelName = channelName;
        }
    }
}
