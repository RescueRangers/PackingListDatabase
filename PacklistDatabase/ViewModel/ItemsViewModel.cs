using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using GalaSoft.MvvmLight.Messaging;
using PacklistDatabase.Messages;
using PacklistDatabase.Model;

namespace PacklistDatabase.ViewModel
{
    public class ItemsViewModel : ViewModelBase
    {
        private IDataService _dataService;
        private ICollection<Item> _items;
        private ICollection<Material> _materials;

        private ListCollectionView _itemsView;

        private Item _selectedItem;

        private string _searchFilter;

        private string _newItemName;

        private ListCollectionView _materialsView;

        /// <summary>
        /// Sets and gets the Materials property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// This property's value is broadcasted by the MessengerInstance when it changes.
        /// </summary>
        public ListCollectionView MaterialsView
        {
            get
            {
                return _materialsView;
            }

            set
            {
                if (_materialsView == value)
                {
                    return;
                }

                var oldValue = _materialsView;
                _materialsView = value;
                RaisePropertyChanged(nameof(MaterialsView), oldValue, value, true);
            }
        }

        /// <summary>
        /// Sets and gets the NewItemName property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// This property's value is broadcasted by the MessengerInstance when it changes.
        /// </summary>
        public string NewItemName
        {
            get
            {
                return _newItemName;
            }

            set
            {
                if (_newItemName == value)
                {
                    return;
                }

                var oldValue = _newItemName;
                _newItemName = value;
                RaisePropertyChanged(nameof(NewItemName), oldValue, value, true);
            }
        }

        /// <summary>
        /// Sets and gets the SearchFilter property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// This property's value is broadcasted by the MessengerInstance when it changes.
        /// </summary>
        public string SearchFilter
        {
            get
            {
                return _searchFilter;
            }

            set
            {
                if (_searchFilter == value)
                {
                    return;
                }

                AddFilter(value);
                _itemsView.Refresh();

                var oldValue = _searchFilter;
                _searchFilter = value;
                RaisePropertyChanged(nameof(SearchFilter), oldValue, value, true);
            }
        }

        /// <summary>
        /// Sets and gets the SelectedItem property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// This property's value is broadcasted by the MessengerInstance when it changes.
        /// </summary>
        public Item SelectedItem
        {
            get => _selectedItem;

            set
            {
                if (_selectedItem == value)
                {
                    return;
                }

                var oldValue = _selectedItem;
                _selectedItem = value;
                RaisePropertyChanged(nameof(SelectedItem), oldValue, value, true);
            }
        }

        /// <summary>
        /// Sets and gets the Items property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// This property's value is broadcasted by the MessengerInstance when it changes.
        /// </summary>
        public ListCollectionView ItemsView
        {
            get => _itemsView;

            set
            {
                if (_itemsView == value)
                {
                    return;
                }

                var oldValue = _itemsView;
                _itemsView = value;
                RaisePropertyChanged(nameof(ItemsView), oldValue, value, true);
            }
        }

        public ICommand AddNewItemCommand { get; set; }
        public ICommand AddMaterialCommand { get; set; }
        public ICommand OpenMaterialsPanelCommand { get; set; } 


        public ItemsViewModel(IDataService dataService)
        {
            _dataService = dataService;
            _dataService.GetItemsAndMaterials((items, materials, error) =>
            {
                if (error != null)
                {
                    //TODO: Report error
                }

                _materials = materials;
                _items = items;
                _materialsView = (ListCollectionView) CollectionViewSource.GetDefaultView(_materials);
                _itemsView = (ListCollectionView) CollectionViewSource.GetDefaultView(_items);
            });

            AddNewItemCommand = new RelayCommand(AddNewItem, ()=> !string.IsNullOrWhiteSpace(NewItemName));
            AddMaterialCommand = new RelayCommand(AddMaterial, () => SelectedItem != null);
        }

        private void AddMaterial()
        {
            if(SelectedItem.Materials == null)
                SelectedItem.Materials = new ObservableCollection<MaterialWithUsage>();

            SelectedItem.Materials.Add(new MaterialWithUsage());
        }

        private void AddNewItem()
        {
            if(_items == null) _items = new List<Item>();
            _items.Add(new Item{ItemName = _newItemName});
        }

        private void AddFilter(string value)
        {
            _itemsView.Filter = o =>
            {
                if (string.IsNullOrWhiteSpace(value)) return true;

                var result = o is Item item && item.ItemName.ToLower().Contains(value.ToLower());
                return result;
            };
            
        }

        //public override void Cleanup()
        //{
        //    // Clean up if needed
        //    _items = null;
        //    base.Cleanup();
        //}
    }
}
