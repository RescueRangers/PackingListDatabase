using System;

namespace Packilists.Shared.Data
{
    public class COC
    {
        public DateTime InventoryDate { get; set; }
        public float Quantity { get; set; }
        public int ItemWithQtyId { get; set; }
        public int ItemId { get; set; }
        public string ItemName { get; set; }
        public int CocNumber { get; set; }
        public int CocId { get; set; }

        public override string ToString()
        {
            return $"{CocNumber}: {ItemName}";
        }
    }
}
