using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
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
using Packlists.Model.ProgressBar;

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
        private readonly IProgressDialogService _progressDialog;
        private List<FileInfo> _excelFiles;
        private List<Packliste> _packlists = new List<Packliste>();

        private ListCollectionView _packlistView;

        private Packliste _selectedPackliste;

        private ListCollectionView _itemsView;

        private ItemWithQty _selectedItemWithQty;

        private string _searchFilter;

        private DateTime _selectedMonth = DateTime.Today;

        private Item _selectedItem;

        private string _quantity;

        #region Properties

        /// <summary>
        /// Sets and gets the Quantity property.
        /// Changes to that property's value raise the PropertyChanged event.
        /// This property's value is broadcast by the MessengerInstance when it changes.
        /// </summary>
        public string Quantity
        {
            get => _quantity;

            set
            {
                if (_quantity == value)
                {
                    return;
                }

                var oldValue = _quantity;
                _quantity = value;
                RaisePropertyChanged(nameof(Quantity), oldValue, value, true);
            }
        }

        /// <summary>
        /// Sets and gets the SelectedItem property.
        /// Changes to that property's value raise the PropertyChanged event.
        /// This property's value is broadcast by the MessengerInstance when it changes.
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

                var oldValue = _selectedMonth;
                _selectedMonth = value;
                RaisePropertyChanged(nameof(SelectedMonth), oldValue, value, true);
                LoadMonthlyData();
            }
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
        public ItemWithQty SelectedItemWithQty
        {
            get => _selectedItemWithQty;

            set
            {
                if (_selectedItemWithQty == value)
                {
                    return;
                }

                var oldValue = _selectedItemWithQty;
                _selectedItemWithQty = value;
                RaisePropertyChanged(nameof(SelectedItemWithQty), oldValue, value, true);
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

        #region Commands

        public ICommand AddItemToPacklisteCommand { get; private set; }
        public ICommand OpenItemsPanelCommand { get; set; }
        public ICommand SaveCommand { get; set; }
        public ICommand OpenMaterialsPanelCommand { get; set; }
        public ICommand ImportPacklisteCommand { get; set; }
        public ICommand PrintPacklisteCommand { get; set; }
        public ICommand PrintItemTableCommand { get; set; }
        public ICommand EditItemCommand { get; set; }
        public ICommand CreateTarmPacklisteCommand { get; set; }
        public ICommand PrintMonthlyCommand { get; set; }
        public ICommand OpenImportPanelCommand { get; set; }
        public ICommand OpenReportPanelCommand { get; set; }
        public ICommand OpenCocsPanelCommand { get; set; }
        public ICommand PacklisteFromCOCsCommand { get; set; }
        public ICommand RecalculateUsageCommand { get; set; }
        public ICommand PrintMissingPacklistNumbersCommand { get; set; }
        public ICommand RefreshItemsCommand { get; set; }

        #endregion Commands

        #endregion Properties

        /// <inheritdoc />
        /// <summary>
        /// Initializes a new instance of the MainViewModel class.
        /// </summary>
        public MainViewModel(IDataService dataService, IPrintingService printing, IDialogService dialogService, IProgressDialogService progressDialog)
        {
            LoadCommands();
            _dataService = dataService;
            _printing = printing;
            _dialogService = dialogService;
            _progressDialog = progressDialog;

            _dataService.GetItems((items) =>
            {
                _itemsView = (ListCollectionView)CollectionViewSource.GetDefaultView(items);
            });

            LoadMonthlyData();
        }

        private void LoadMonthlyData()
        {
            var existingPacklistsCount = _packlistView?.OfType<Packliste>().Count(p => p.PacklisteDate.Year == SelectedMonth.Year && p.PacklisteDate.Month == SelectedMonth.Month);

            if (existingPacklistsCount != null && existingPacklistsCount > 0) return;

            _dataService.GetPacklists(
                (packlists, error) =>
                {
                    if (error != null)
                    {
                        //TODO: Report error here
                        return;
                    }

                    _packlistView = (ListCollectionView)CollectionViewSource.GetDefaultView(packlists);
                }, SelectedMonth);

            PacklistView.SortDescriptions.Add(new SortDescription("PacklisteDate", ListSortDirection.Ascending));
            PacklistView.SortDescriptions.Add(new SortDescription("PacklisteNumber", ListSortDirection.Ascending));
        }

        #region Commands

        private void LoadCommands()
        {
            AddItemToPacklisteCommand = new RelayCommand(AddItemToPackliste, CanAddItemToPackliste);
            OpenItemsPanelCommand = new RelayCommand<MainViewModel>((mc) => OpenItemsPanel("ShowItemsPanel"), true);
            OpenMaterialsPanelCommand = new RelayCommand<MainViewModel>((mc) => OpenItemsPanel("ShowMaterialsPanel"), true);
            SaveCommand = new RelayCommand(Save, true);
            ImportPacklisteCommand = new RelayCommand(ImportPacklisteData, true);
            PrintPacklisteCommand = new RelayCommand(PrintPackliste,
                CanPrintPackliste);
            EditItemCommand = new RelayCommand(EditItem, true);
            CreateTarmPacklisteCommand = new RelayCommand(CreateTarmPackliste, true);
            PrintItemTableCommand = new RelayCommand(PrintItemTable, () => SelectedPackliste != null);
            PrintMonthlyCommand = new RelayCommand(PrintMonthlyReport, CanPrintMonthly);
            OpenImportPanelCommand = new RelayCommand<MainViewModel>((mc) => OpenItemsPanel("ShowImportPanel"), true);
            OpenReportPanelCommand = new RelayCommand<MainViewModel>((mc) => OpenItemsPanel("ShowMonthlyReport"), true);
            OpenCocsPanelCommand = new RelayCommand<MainViewModel>((mc) => OpenItemsPanel("ShowCOCs"), true);
            PacklisteFromCOCsCommand = new RelayCommand(PacklisteFromCOCs);
            RecalculateUsageCommand = new RelayCommand(() =>
            {
                SelectedPackliste.RecalculateUsage();
            }, () => SelectedPackliste != null);
            PrintMissingPacklistNumbersCommand = new RelayCommand(PrintingMissingPacklistNumbers, true);
            RefreshItemsCommand = new RelayCommand(Refresh, true);
        }

        private void Refresh()
        {
            _dataService.GetPacklists(
                (packlists, error) =>
                {
                    if (error != null)
                    {
                        //TODO: Report error here
                        return;
                    }
                    _packlistView = (ListCollectionView)CollectionViewSource.GetDefaultView(packlists);
                }, SelectedMonth);

            PacklistView.SortDescriptions.Add(new SortDescription("PacklisteDate", ListSortDirection.Ascending));
            PacklistView.SortDescriptions.Add(new SortDescription("PacklisteNumber", ListSortDirection.Ascending));
        }

        private void PrintingMissingPacklistNumbers()
        {
            _packlists = PacklistView.OfType<Packliste>().ToList();
            var packlistNumbers = _packlists
                .Where(p => p.PacklisteDate.Year == SelectedMonth.Year && p.PacklisteDate.Month == SelectedMonth.Month)
                .Select(p => p.PacklisteNumber).Except(new List<int> { 1, -1, 0 });

            var missingNumbers = Enumerable.Range(packlistNumbers.Min(), packlistNumbers.Max() - packlistNumbers.Min()).Except(packlistNumbers);

            _printing.PrintListOfNumbers(missingNumbers);
        }

        private async void PacklisteFromCOCs()
        {
            GetExcelFiles();

            if (_excelFiles == null || !_excelFiles.Any()) return;

            foreach (var excelFile in _excelFiles)
            {
                var packliste = await ImportPackliste.FromCOCs(excelFile, _dataService);

                if (packliste.Item2 == "Success")
                {
                    _dataService.Add(packliste.Item1);
                }
                else
                {
                    _dialogService.ShowMessageBox(this, packliste.Item2);
                }
            }
        }

        #region PrintMonthlyReport

        private bool CanPrintMonthly()
        {
            if (PacklistView == null) return false;
            return PacklistView.Count > 1;
        }

        private void PrintMonthlyReport()
        {
            _packlists = PacklistView.OfType<Packliste>().ToList();

            var options = new ProgressDialogOptions
            {
                Label = "Current task: ",
                WindowTitle = "Printing"
            };
            _progressDialog.Execute(PrintMonthly, options);
        }

        private async void PrintMonthly(CancellationToken cancellationToken, IProgress<ProgressReport> progress)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var progressReport = new ProgressReport
            {
                IsIndeterminate = true,
                CurrentTask = "Printing monthly report"
            };

            progress.Report(progressReport);

            var printingResult = await _printing.PrintMonthlyReport(_packlists);

            if (printingResult != "Printing successful")
            {
                Application.Current.Dispatcher.Invoke(delegate
                {
                    _dialogService.ShowMessageBox(this, printingResult, "Printing", MessageBoxButton.OK,
                        MessageBoxImage.Error);
                });
            }
            _packlists.Clear();
        }

        #endregion PrintMonthlyReport

        #region PrintItemTable

        private void PrintItemTable()
        {
            var options = new ProgressDialogOptions
            {
                Label = "Current task: ",
                WindowTitle = "Printing"
            };
            _progressDialog.Execute(PrintItemTableWithProgress, options);
        }

        private async void PrintItemTableWithProgress(CancellationToken cancellationToken, IProgress<ProgressReport> progress)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var progressReport = new ProgressReport
            {
                IsIndeterminate = true,
                CurrentTask = "Printing item table"
            };

            progress.Report(progressReport);

            await _printing.PrintItemTable(SelectedPackliste);
        }

        #endregion PrintItemTable

        private void CreateTarmPackliste()
        {
            FileInfo excelFile;
            var openFileOptions = new OpenFileDialogSettings
            {
                Filter = "Excel files (*.xlsx)|*.xlsx",
                Title = "Packliste in excel",
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)
            };

            var success = _dialogService.ShowOpenFileDialog(this, openFileOptions);

            if (success == true)
            {
                excelFile = new FileInfo(openFileOptions.FileName);
            }
            else
            {
                return;
            }

            CreateFromPackliste.FromExcel((packliste, error) =>
            {
                if (error != null)
                {
                    _dialogService.ShowMessageBox(this, error.Message, "Error", MessageBoxButton.OK,
                        MessageBoxImage.Error);
                    return;
                }

                _dataService.Add(packliste);
            }, excelFile, _dataService);
        }

        private void EditItem()
        {
            if (SelectedItemWithQty == null) return;
            MessengerInstance.Send(new NotificationMessage("ShowItemsPanel"));
            MessengerInstance.Send<FilterItemsMessage>(new FilterItemsMessage(SelectedItemWithQty.Item.ItemName));
        }

        #region PrintPackliste

        private bool CanPrintPackliste()
        {
            if (SelectedPackliste?.PacklisteData == null)
            {
                return false;
            }

            return SelectedPackliste.PacklisteData.Count >= 1;
        }

        private void PrintPackliste()
        {
            _printing.Print(SelectedPackliste.PacklisteData);
        }

        #endregion PrintPackliste

        #region ImportPackingList

        private void ImportPacklisteData()
        {
            GetExcelFiles();

            if (_excelFiles == null) return;

            var options = new ProgressDialogOptions
            {
                Label = "Current task: ",
                WindowTitle = "Importing packlists"
            };
            _progressDialog.Execute(ImportPacklisteWithProgress, options);
        }

        private void ImportPacklisteWithProgress(CancellationToken cancellationToken, IProgress<ProgressReport> progress)
        {
            cancellationToken.ThrowIfCancellationRequested();

            for (var i = 0; i < _excelFiles.Count; i++)
            {
                var progressReport = new ProgressReport
                {
                    IsIndeterminate = false,
                    MaxTaskNumber = _excelFiles.Count - 1,
                    CurrentTaskNumber = i,
                    CurrentTask = _excelFiles[i].Name
                };

                progress.Report(progressReport);

                var i1 = i;

                void Action()
                {
                    ImportPackliste.FromExcel((packliste, error) =>
                    {
                        if (error != null)
                        {
                            _dialogService.ShowMessageBox(this, error.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                            return;
                        }

                        _packlists.Add(packliste);
                        //_dataService.Add(packliste);
                    }, _excelFiles[i1], _dataService);
                }

                var t1 = Task.Factory.StartNew(Action, cancellationToken);
                t1.Wait(cancellationToken);
            }

            var progressReport2 = new ProgressReport
            {
                IsIndeterminate = true,
                CurrentTask = "Saving changes"
            };

            progress.Report(progressReport2);

            _dataService.BulkAdd(_packlists);

            _packlists.Clear();
        }

        #endregion ImportPackingList

        private void Save()
        {
            _dataService.SaveData();
        }

        private void OpenItemsPanel(string message)
        {
            MessengerInstance.Send(new NotificationMessage(message));
        }

        #region AddItemToPackliste

        private bool CanAddItemToPackliste()
        {
            if (string.IsNullOrWhiteSpace(Quantity)) return false;
            if (SelectedItem == null) return false;

            var qtyResult = float.TryParse(Quantity, out var qty);

            return qtyResult;
        }

        private void AddItemToPackliste()
        {
            var itemWithQty = new ItemWithQty
            {
                Item = SelectedItem,
                Quantity = float.Parse(Quantity)
            };

            SelectedPackliste.AddItem(itemWithQty);
        }

        #endregion AddItemToPackliste

        #endregion Commands

        private void GetExcelFiles()
        {
            _excelFiles = new List<FileInfo>();
            var openFileOptions = new OpenFileDialogSettings
            {
                Filter = "Excel files (*.xlsx)|*.xlsx",
                Title = "Packliste in excel",
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                Multiselect = true
            };

            var success = _dialogService.ShowOpenFileDialog(this, openFileOptions);

            if (success == true)
            {
                _excelFiles.AddRange(openFileOptions.FileNames.Select(fileName => new FileInfo(fileName)));
            }
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
            _packlistView.Refresh();
        }

        ////public override void Cleanup()
        ////{
        ////    // Clean up if needed

        ////    base.Cleanup();
        ////}
    }
}