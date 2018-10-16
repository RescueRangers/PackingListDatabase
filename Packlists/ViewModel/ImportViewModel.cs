using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using MvvmDialogs;
using Packlists.Model;
using Packlists.Model.Printing;
using Packlists.Model.ProgressBar;

namespace Packlists.ViewModel
{
    public class ImportViewModel : ViewModelBase
    {
        private readonly IDataService _dataService;
        private readonly IDialogService _dialogService;
        private readonly IPrintingService _printing;
        private readonly IProgressDialogService _progressDialog;
        private List<ImportTransport> _importsList;

        private DateTime _selectedDate = DateTime.Now;
        private string _sender;
        private string _quantity;
        private DateTime _selectedMonth = DateTime.Now;
        private ImportTransport _selectedImport;
        private Material _selectedMaterial;
        private ListCollectionView _imports;
        private ListCollectionView _materials;
        private MaterialAmount _selectedMaterialAmount;
        
        #region Properties

        /// <summary>
        /// Sets and gets the SelectedMaterialAmount property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// This property's value is broadcasted by the MessengerInstance when it changes.
        /// </summary>
        public MaterialAmount SelectedMaterialAmount
        {
            get
            {
                return _selectedMaterialAmount;
            }

            set
            {
                if (_selectedMaterialAmount == value)
                {
                    return;
                }

                var oldValue = _selectedMaterialAmount;
                _selectedMaterialAmount = value;
                RaisePropertyChanged(nameof(SelectedMaterialAmount), oldValue, value, true);
            }
        }
        
        /// <summary>
        /// Sets and gets the SelectedDate property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// This property's value is broadcasted by the MessengerInstance when it changes.
        /// </summary>
        public DateTime SelectedDate
        {
            get => _selectedDate;

            set
            {
                if (_selectedDate == value)
                {
                    return;
                }

                var oldValue = _selectedDate;
                _selectedDate = value;
                RaisePropertyChanged(nameof(SelectedDate), oldValue, value, true);
            }
        }

        /// <summary>
        /// Sets and gets the Sender property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// This property's value is broadcasted by the MessengerInstance when it changes.
        /// </summary>
        public string Sender
        {
            get => _sender;

            set
            {
                if (_sender == value)
                {
                    return;
                }

                var oldValue = _sender;
                _sender = value;
                RaisePropertyChanged(nameof(Sender), oldValue, value, true);
            }
        }

        /// <summary>
        /// Sets and gets the Quantity property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// This property's value is broadcasted by the MessengerInstance when it changes.
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
        /// Sets and gets the SelectedMonth property.
        /// Changes to that property's value raise the PropertyChanged event. 
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
                LoadMonthlyData();
                RaisePropertyChanged(nameof(SelectedMonth), oldValue, value, true);
            }
        }

        

        /// <summary>
        /// Sets and gets the SelectedImport property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public ImportTransport SelectedImport
        {
            get => _selectedImport;
            set => Set(nameof(SelectedImport), ref _selectedImport, value);
        }

        /// <summary>
        /// Sets and gets the SelectedMaterial property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public Material SelectedMaterial
        {
            get => _selectedMaterial;
            set => Set(nameof(SelectedMaterial), ref _selectedMaterial, value);
        }

        /// <summary>
        /// Sets and gets the Imports property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public ListCollectionView Imports
        {
            get => _imports;
            set => Set(nameof(Imports), ref _imports, value);
        }

        /// <summary>
        /// Sets and gets the Materials property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public ListCollectionView Materials
        {
            get => _materials;
            set
            {
                if (_materials == value)
                {
                    return;
                }

                var oldValue = _materials;
                _materials = value;
                RaisePropertyChanged(nameof(Materials), oldValue, value, true);
            }
        }

        #region Commands

        public ICommand AddImportedMaterialCommand { get; set; }
        public ICommand ImportedMaterialEnterCommand { get; set; }
        public ICommand AddImportCommand { get; set; }
        public ICommand CloseCommand { get; set; }
        public ICommand ImportsEnterCommand { get; set; }
        public ICommand RemoveMaterialAmountCommand { get; set; }
        public ICommand PrintMonthlyReportCommand { get; set; }

        #endregion

        #endregion

        public ImportViewModel(IPrintingService printing, IDialogService dialogService, IDataService dataService, IProgressDialogService progressDialog)
        {
            _printing = printing;
            _dialogService = dialogService;
            _dataService = dataService;
            _progressDialog = progressDialog;

            _dataService.GetMaterials(((materials) =>
            {
                _materials = (ListCollectionView)CollectionViewSource.GetDefaultView(materials);
                _materials.SortDescriptions.Add(new SortDescription("MaterialName", ListSortDirection.Ascending));

            }));

            LoadCommands();

        }

        #region Commands

        private void LoadCommands()
        {
            AddImportedMaterialCommand = new RelayCommand(AddImportedMaterial, CanAddMaterial);
            AddImportCommand = new RelayCommand(AddImport, CanAddImport);
            CloseCommand = new RelayCommand(Close, true);
            ImportedMaterialEnterCommand = new RelayCommand<Key>(ImportedMaterialEnter, true);
            ImportsEnterCommand = new RelayCommand<Key>(ImportsEnter);
            RemoveMaterialAmountCommand = new RelayCommand(RemoveMaterialAmount, () => SelectedMaterialAmount != null);
            PrintMonthlyReportCommand = new RelayCommand(PrintMonthlyReport, CanPrintMonthly);
        }

        #region PrintMonthly

        private bool CanPrintMonthly()
        {
            if(Imports == null) return false;
            return Imports.Count > 1;
        }

        private void PrintMonthlyReport()
        {
            _importsList = Imports.OfType<ImportTransport>().ToList();

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

            var printingResult = await _printing.PrintMonthlyImportReport(_importsList);

            if (!string.IsNullOrWhiteSpace(printingResult))
            {
                Application.Current.Dispatcher.Invoke(delegate
                {
                    _dialogService.ShowMessageBox(this, printingResult, "Printing", MessageBoxButton.OK,
                        MessageBoxImage.Information);
                });
            }
        }

        #endregion

        private void RemoveMaterialAmount()
        {
            SelectedImport.ImportedMaterials.Remove(SelectedMaterialAmount);
        }

        private void ImportsEnter(Key key)
        {
            if (!CanAddImport()) return;
            if (key == Key.Enter)
                AddImport();
        }

        private void ImportedMaterialEnter(Key key)
        {
            if (!CanAddMaterial()) return;
            if (key == Key.Enter) 
                AddImportedMaterial();
        }

        private void Close()
        {
            _dataService.SaveData();
        }

        private void AddImport()
        {
            var import = new ImportTransport
            {
                ImportDate = SelectedDate,
                Sender = Sender,
                ImportedMaterials = new ObservableCollection<MaterialAmount>()
            };
            _dataService.Add(import);
        }

        private bool CanAddImport()
        {
            return !string.IsNullOrWhiteSpace(Sender);
        }

        private void AddImportedMaterial()
        {
            var materialAmount = new MaterialAmount
            {
                Material = SelectedMaterial,
                Amount = float.Parse(Quantity, CultureInfo.InvariantCulture)
            };

            SelectedImport.ImportedMaterials.Add(materialAmount);
            Quantity = string.Empty;
        }

        private bool CanAddMaterial()
        {
            if (string.IsNullOrWhiteSpace(Quantity)) return false;
            if (SelectedMaterial == null) return false;
            if (SelectedImport == null) return false;

            var qtyResult = float.TryParse(Quantity, NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out var qty);

            return qtyResult;
        }

        #endregion

        private void LoadMonthlyData()
        {
            _dataService.GetImports(((transports, exception) =>
            {
                if (exception != null)
                {
                    //TODO: Report any errors here
                }

                _imports = (ListCollectionView)CollectionViewSource.GetDefaultView(transports);
                _imports.SortDescriptions.Add(new SortDescription("ImportDate", ListSortDirection.Ascending));

            }), _selectedMonth);
        }

        private void AddFilter(DateTime value)
        {
            if (_imports == null) return;

            _imports.Filter = o =>
            {
                //if (string.IsNullOrWhiteSpace(_searchFilter)) return true;

                var result = o is ImportTransport import &&
                             (import.ImportDate.Year == value.Year &&
                              import.ImportDate.Month == value.Month);
                return result;
            };
            _imports.Refresh();
        }
    }
}
