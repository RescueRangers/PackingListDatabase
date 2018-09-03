using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;

namespace PacklistDatabase.Model
{
    public class Item : ObservableObject
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

        /// <summary>
            /// The <see cref="Materials" /> property's name.
            /// </summary>
        public const string MaterialsPropertyName = "Materials";

        private ObservableCollection<MaterialWithUsage> _materials;

        /// <summary>
        /// Sets and gets the Materials property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public ObservableCollection<MaterialWithUsage> Materials
        {
            get => _materials;
            set => Set(nameof(Materials), ref _materials, value);
        }

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

        public int ItemId { get; set; }
    }
}
