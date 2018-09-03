using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight.Messaging;
using PacklistDatabase.Model;

namespace PacklistDatabase.Messages
{
    public class ItemsMessage
    {
        public ICollection<Item> Items { get; set; }
    }
}
