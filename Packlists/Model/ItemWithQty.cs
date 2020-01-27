namespace Packlists.Model
{
    public class ItemWithQty : EditableModelBase<ItemWithQty>
    {
        private Item _item;

        private float _quantity;

        /// <summary>
        /// Sets and gets the Quantity property.
        /// Changes to that property's value raise the PropertyChanged event.
        /// </summary>
        public float Quantity
        {
            get => _quantity;
            set => Set(nameof(Quantity), ref _quantity, value);
        }

        /// <summary>
        /// Sets and gets the Item property.
        /// Changes to that property's value raise the PropertyChanged event.
        /// </summary>
        public Item Item
        {
            get => _item;
            set => Set(nameof(Item), ref _item, value);
        }

        public int ItemWithQtyId { get; set; }
    }
}