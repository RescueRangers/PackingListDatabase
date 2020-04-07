using System;
using GalaSoft.MvvmLight;

namespace Packlists.Model
{
    public class COC : ObservableObject
    {
        private int _cocNumber;
        private ItemWithQty _item;
        private DateTime _inventoryDate;

        public DateTime InventoryDate
        {
            get => _inventoryDate;
            set => Set(ref _inventoryDate, value);
        }

        public ItemWithQty Item
        {
            get => _item;
            set => Set(ref _item, value);
        }

        public int CocNumber
        {
            get => _cocNumber;
            set => Set(ref _cocNumber, value);
        }

        public int CocId { get; set; }

        public override string ToString()
        {
            return $"{CocNumber}: {Item}";
        }
    }
}