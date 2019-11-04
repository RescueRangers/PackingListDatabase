using System;

namespace Packilists.DataAccess.Data
{
    public class COC
    {
        public DateTime InventoryDate { get; set; }

        public ItemWithQty Item { get; set; }

        public int CocNumber { get; set; }

        public int CocId { get; set; }

        public override string ToString()
        {
            return $"{CocNumber}: {Item}";
        }
    }
}
