using Prism.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp1.Common.EventMessages
{
    internal record struct ItemAddedRecord(int id, string name);
    internal class AddChanelEvent : PubSubEvent<ItemAddedRecord>
    {

    }
}
