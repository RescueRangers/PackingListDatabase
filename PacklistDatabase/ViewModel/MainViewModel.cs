using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Data;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using GalaSoft.MvvmLight.Messaging;
using GalaSoft.MvvmLight.Views;
using PacklistDatabase.Messages;
using PacklistDatabase.Model;

namespace PacklistDatabase.ViewModel
{
    /// <summary>
    /// This class contains properties that the main View can data bind to.
    /// <para>
    /// See http://www.mvvmlight.net
    /// </para>
    /// </summary>
    public class MainViewModel : ViewModelBase
    {
        private readonly IDataService _dataService;

        private ICollection<Year> _years;
        private ICollection<Item> _items;

        private ListCollectionView _yearsView;

        private Year _selectedYear;

        private Month _selectedMonth;

        private Day _selectedDay;

        private Packliste _selectedPackliste;

        private ListCollectionView _itemsView;

        private Item _selectedItem;

        private string _searchFilter;

        #region Properties

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
                _yearsView.Refresh();
               

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

        /// <summary>
        /// Sets and gets the SelectedPackliste property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// This property's value is broadcasted by the MessengerInstance when it changes.
        /// </summary>
        public Packliste SelectedPackliste
        {
            get => _selectedPackliste;

            set
            {
                if (_selectedPackliste == value)
                {
                    return;
                }

                var oldValue = _selectedPackliste;
                _selectedPackliste = value;
                RaisePropertyChanged(nameof(SelectedPackliste), oldValue, value, true);
            }
        }

        /// <summary>
        /// Sets and gets the SelectedDay property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// This property's value is broadcasted by the MessengerInstance when it changes.
        /// </summary>
        public Day SelectedDay
        {
            get => _selectedDay;

            set
            {
                if (_selectedDay == value)
                {
                    return;
                }

                var oldValue = _selectedDay;
                _selectedDay = value;
                RaisePropertyChanged(nameof(SelectedDay), oldValue, value, true);
            }
        }

        /// <summary>
        /// Sets and gets the SelectedMonth property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// This property's value is broadcasted by the MessengerInstance when it changes.
        /// </summary>
        public Month SelectedMonth
        {
            get => _selectedMonth;

            set
            {
                if (_selectedMonth == value)
                {
                    return;
                }

                var oldValue = _selectedMonth;
                _selectedMonth = value;
                RaisePropertyChanged(nameof(SelectedMonth), oldValue, value, true);
            }
        }

        /// <summary>
        /// Sets and gets the SelectedYear property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// This property's value is broadcasted by the MessengerInstance when it changes.
        /// </summary>
        public Year SelectedYear
        {
            get => _selectedYear;

            set
            {
                if (_selectedYear == value)
                {
                    return;
                }

                var oldValue = _selectedYear;
                _selectedYear = value;
                RaisePropertyChanged(nameof(SelectedYear), oldValue, value, true);
            }
        }

        /// <summary>
        /// Sets and gets the YearsView property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// This property's value is broadcasted by the MessengerInstance when it changes.
        /// </summary>
        public ListCollectionView YearsView
        {
            get => _yearsView;

            set
            {
                if (_yearsView == value)
                {
                    return;
                }

                var oldValue = _yearsView;
                _yearsView = value;
                RaisePropertyChanged(nameof(YearsView), oldValue, value, true);
            }
        }

        #endregion

        public ICommand AddEmptyPacklisteCommand { get; private set; }
        public ICommand AddItemToPacklisteCommand { get; private set; }
        public ICommand OpenItemsPanelCommand { get; set; }
        public ICommand OpenMaterialsPanelCommand { get; set; } 

        
        /// <summary>
        /// Initializes a new instance of the MainViewModel class.
        /// </summary>
        public MainViewModel(IDataService dataService)
        {
            LoadCommands();
            _dataService = dataService;
            _dataService.GetYearsAndItems(
                (years, items, error) =>
                {
                    if (error != null)
                    {
                        // Report error here
                        return;
                    }

                    //_yearsView.Source = years;
                    _years = years;
                    _yearsView = (ListCollectionView) CollectionViewSource.GetDefaultView(years);
                    _items = items;
                    _itemsView = (ListCollectionView) CollectionViewSource.GetDefaultView(_items);
                });
            
        }

        private void LoadCommands()
        {
            AddEmptyPacklisteCommand = new RelayCommand(AddEmptyPackliste, () => SelectedDay != null);
            AddItemToPacklisteCommand = new RelayCommand(AddItemToPackliste, ()=> SelectedItem != null && SelectedItem.Quantity > 0);
            OpenItemsPanelCommand = new RelayCommand<MainViewModel>((mc) => OpenItemsPanel("ShowItemsPanel"), true);
            OpenMaterialsPanelCommand = new RelayCommand<MainViewModel>((mc) => OpenItemsPanel("ShowMaterialsPanel"), true);

        }

        private void OpenItemsPanel(string message)
        {
            MessengerInstance.Send(new NotificationMessage(message));
        }

        private void AddItemToPackliste()
        {
            throw new NotImplementedException();
        }

        private void AddEmptyPackliste()
        {
            if(SelectedDay.Packlists == null) SelectedDay.Packlists = new ObservableCollection<Packliste>();

            SelectedDay.Packlists.Add(new Packliste
            {
                Items = new ObservableCollection<Item>(new[]{new Item()}),
                PacklisteNumber = -1
            });
        }

        private void AddFilter(string value)
        {
            _yearsView.Filter = o =>
            {
                if (string.IsNullOrWhiteSpace(_searchFilter)) return true;

                var result = o is Year year && year.YearNumber.ToString().ToLower().Contains(value.ToLower());
                return result;
            };
            
        }

        ////public override void Cleanup()
        ////{
        ////    // Clean up if needed

        ////    base.Cleanup();
        ////}
    }
}