using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using GalaSoft.MvvmLight.Messaging;
using MvvmDialogs;
using MvvmDialogs.FrameworkDialogs.OpenFile;
using Packlists.ExcelImport;
using Packlists.Messages;
using Packlists.Model;
using Packlists.Model.Printing;

namespace Packlists.ViewModel
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
        private readonly IDialogService _dialogService;
        private readonly IPrintingService _printing;

        private ListCollectionView _packlistView;

        private Day _selectedDay;

        private Packliste _selectedPackliste;

        private ListCollectionView _itemsView;

        private ItemWithQty _selectedItem;

        private string _newYear;
        
        private string _searchFilter;

        private DateTime _selectedMonth;

        #region Properties


        /// <summary>
        /// Sets and gets the SelectedMonth property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// This property's value is broadcast by the MessengerInstance when it changes.
        /// </summary>
        public DateTime SelectedMonth
        {
            get => _selectedMonth;

            set
            {
                if (_selectedMonth == value)
                {
                    return;
                }

                AddFilter(value);
                _packlistView.Refresh();

                var oldValue = _selectedMonth;
                _selectedMonth = value;
                RaisePropertyChanged(nameof(SelectedMonth), oldValue, value, true);
            }
        }

        /// <summary>
        /// Sets and gets the NewYear property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public string NewYear
        {
            get => _newYear;
            set => Set(nameof(NewYear), ref _newYear, value);
        }

        /// <summary>
        /// Sets and gets the SearchFilter property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// This property's value is broadcast by the MessengerInstance when it changes.
        /// </summary>
        public string SearchFilter
        {
            get => _searchFilter;

            set
            {
                if (_searchFilter == value)
                {
                    return;
                }

                //AddFilter(value);
                //_packlistView.Refresh();
               

                var oldValue = _searchFilter;
                _searchFilter = value;
                RaisePropertyChanged(nameof(SearchFilter), oldValue, value, true);
            }
        }

        
        /// <summary>
        /// Sets and gets the SelectedItem property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// This property's value is broadcast by the MessengerInstance when it changes.
        /// </summary>
        public ItemWithQty SelectedItem
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
        /// Sets and gets the ItemsWithQties property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// This property's value is broadcast by the MessengerInstance when it changes.
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
        /// This property's value is broadcast by the MessengerInstance when it changes.
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
        /// This property's value is broadcast by the MessengerInstance when it changes.
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
        /// Sets and gets the YearsView property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// This property's value is broadcast by the MessengerInstance when it changes.
        /// </summary>
        public ListCollectionView PacklistView
        {
            get => _packlistView;

            set
            {
                if (_packlistView == value)
                {
                    return;
                }

                var oldValue = _packlistView;
                _packlistView = value;
                RaisePropertyChanged(nameof(PacklistView), oldValue, value, true);
            }
        }

        #endregion

        public ICommand AddEmptyPacklisteCommand { get; private set; }
        public ICommand AddItemToPacklisteCommand { get; private set; }
        public ICommand OpenItemsPanelCommand { get; set; }
        public ICommand SaveCommand { get; set; }
        public ICommand OpenMaterialsPanelCommand { get; set; }
        public ICommand ImportPacklisteCommand { get; set; }
        public ICommand PrintPacklisteCommand { get; set; }
        public ICommand PrintItemTableCommand { get; set; }
        public ICommand EditItemCommand { get; set; }

        
        /// <inheritdoc />
        /// <summary>
        /// Initializes a new instance of the MainViewModel class.
        /// </summary>
        public MainViewModel(IDataService dataService, IPrintingService printing, IDialogService dialogService)
        {
            LoadCommands();
            _dataService = dataService;
            _printing = printing;
            _dialogService = dialogService;
            _dataService.GetPacklists(
                (packlists, items, error) =>
                {
                    if (error != null)
                    {
                        // Report error here
                        return;
                    }

                    //_packlistView.Source = years;
                    _packlistView = (ListCollectionView) CollectionViewSource.GetDefaultView(packlists);
                    _itemsView = (ListCollectionView) CollectionViewSource.GetDefaultView(items);
                });


            PacklistView.SortDescriptions.Add(new SortDescription("PacklisteDate", ListSortDirection.Ascending));
            SelectedMonth = DateTime.Today;
        }

        private void LoadCommands()
        {
            AddEmptyPacklisteCommand = new RelayCommand(AddEmptyPackliste, () => SelectedDay != null);
            AddItemToPacklisteCommand = new RelayCommand(AddItemToPackliste, ()=> SelectedItem != null && SelectedItem.Quantity > 0);
            OpenItemsPanelCommand = new RelayCommand<MainViewModel>((mc) => OpenItemsPanel("ShowItemsPanel"), true);
            OpenMaterialsPanelCommand = new RelayCommand<MainViewModel>((mc) => OpenItemsPanel("ShowMaterialsPanel"), true);
            SaveCommand = new RelayCommand(Save, true);
            ImportPacklisteCommand = new RelayCommand(ImportPacklisteData, true);
            PrintPacklisteCommand = new RelayCommand(PrintPackliste,
                () => SelectedPackliste?.PacklisteData != null);
            EditItemCommand = new RelayCommand(EditItem, true);
            
        }

        private void EditItem()
        {
            MessengerInstance.Send(new NotificationMessage("ShowItemsPanel"));
            MessengerInstance.Send<FilterItemsMessage>(new FilterItemsMessage(SelectedItem.Item.ItemName));
            //MessengerInstance.Send(new NotificationMessage(SelectedItem.Item.ItemName));
        }

        private void PrintPackliste()
        {
            var printDialog = new PrintDialog();

            if (printDialog.ShowDialog() == true)
            {
                _printing.Print(SelectedPackliste.PacklisteData);
            }
        }

        private void AddYear()
        {
            PacklistView.AddNewItem(new Year(int.Parse(NewYear)));
        }

        private bool CanAddYear()
        {
            if (string.IsNullOrWhiteSpace(NewYear))
            {
                return false;
            }

            var isNumber = int.TryParse(NewYear, out var numbersResult);

            if (isNumber)
            {
                return (numbersResult > 1 && numbersResult <= 9999);
            }

            return false;
        }

        private void RemovePackliste()
        {
            SelectedDay.Packlists.Remove(SelectedPackliste);
        }

        private void ImportPacklisteData()
        {
            FileInfo excelFile;
            var openFileOptions = new OpenFileDialogSettings
            {
                Filter = "Excel files (*.xlsx)|*.xlsx",
                Title = "Open excel file with items",
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)
            };

            var success =_dialogService.ShowOpenFileDialog(this, openFileOptions);

            if (success == true)
            {
                excelFile = new FileInfo(openFileOptions.FileName);
            }
            else
            {
                return;
            }

            ImportPackliste.FromExcel((packliste, error) =>
            {
                if (error != null)
                {
                    _dialogService.ShowMessageBox(this, error.Message, "Error", MessageBoxButton.OK,
                        MessageBoxImage.Error);
                    return;
                }
                
                _dataService.Add(packliste);

            },excelFile, _dataService);

        }

        private void Save()
        {
            _dataService.SaveData();
        }

        private void OpenItemsPanel(string message)
        {
            MessengerInstance.Send(new NotificationMessage(message));
        }

        private void AddItemToPackliste()
        {
            SelectedPackliste.ItemsWithQties.Add(SelectedItem);
        }

        private void AddEmptyPackliste()
        {
            if(SelectedDay.Packlists == null) 
                SelectedDay.Packlists = new ObservableCollection<Packliste>();

            SelectedDay.Packlists.Add(new Packliste
            {
                ItemsWithQties = new ObservableCollection<ItemWithQty>(),
                PacklisteNumber = -1,
            });
        }

        private void AddFilter(DateTime value)
        {
            _packlistView.Filter = o =>
            {
                //if (string.IsNullOrWhiteSpace(_searchFilter)) return true;

                var result = o is Packliste packliste &&
                             (packliste.PacklisteDate.Year == value.Year &&
                              packliste.PacklisteDate.Month == value.Month);
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