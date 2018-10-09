using System.Collections.ObjectModel;

namespace Packlists.Model
{
    public class Item : EditableModelBase<Item>
    {
        private string _itemName;

        /// <summary>
        /// Sets and gets the ItemName property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public string ItemName
        {
            get => _itemName;
            set => Set(nameof(ItemName), ref _itemName, value);
        }

        private ObservableCollection<MaterialAmount> _materials;

        /// <summary>
        /// Sets and gets the Materials property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public ObservableCollection<MaterialAmount> Materials
        {
            get => _materials;
            set => Set(nameof(Materials), ref _materials, value);
        }

        public int ItemId { get; set; }

        public override string ToString()
        {
            return ItemName;
        }
    }
}