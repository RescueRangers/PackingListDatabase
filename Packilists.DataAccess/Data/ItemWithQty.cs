using System.Collections.Generic;

namespace Packilists.Shared.Data
{
    public class ItemWithQty
    {
        public string ItemName { get; set; }
        public float Quantity { get; set; }
        public int ItemWithQtyId { get; set; }
        public int ItemId { get; set; }
        public int PacklisteId { get; set; }
        public Item Item { get; set; }
        public List<MaterialAmount> Materials { get; set; }
    }
}
