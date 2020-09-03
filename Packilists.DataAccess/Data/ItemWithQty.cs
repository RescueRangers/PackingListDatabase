using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Packilists.Shared.Data
{
    public class ItemWithQty
    {
        [Required(ErrorMessage = "Field should not be empty")]
        public string ItemName { get; set; }
        [Required(ErrorMessage = "Field should not be empty")]
        public float Quantity { get; set; }
        public int ItemWithQtyId { get; set; }
        public int ItemId { get; set; }
        public int PacklisteId { get; set; }
        public Item Item { get; set; }
        public List<MaterialAmount> Materials { get; set; }
        public bool IsEmpty { get; set; }
    }
}
