using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Packlists.Messages
{
    public class FilterItemsMessage
    {
        public string ItemName { get; private set; }

        public FilterItemsMessage(string itemName)
        {
            ItemName = itemName;
        }
    }
}
