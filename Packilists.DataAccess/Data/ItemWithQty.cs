namespace Packilists.Shared.Data
{
    public class ItemWithQty
    {
        /// <summary>
        /// Sets and gets the Quantity property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public float Quantity { get; set; }

        /// <summary>
        /// Sets and gets the Item property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public Item Item { get; set; }

        public int ItemWithQtyId { get; set; }
    }
}
