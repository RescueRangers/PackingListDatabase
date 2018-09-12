using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using MvvmDialogs;
using Packlists.Model;
using Packlists.Model.Printing;

namespace Packlists.ViewModel
{
    public class ImportViewModel : ViewModelBase
    {
        private readonly IDataService _dataService;
        private readonly IDialogService _dialogService;
        private readonly IPrintingService _printing;

        /// <summary>
        /// The <see cref="SelectedDate" /> property's name.
        /// </summary>
        public const string SelectedDatePropertyName = "SelectedDate";

        private DateTime _selectedDate = DateTime.Now;

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
                RaisePropertyChanged(SelectedDatePropertyName, oldValue, value, true);
            }
        }

        private string _sender;

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

        private string _quantity;

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

        private DateTime _selectedMonth = DateTime.Now;

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
                _imports.Refresh();

                var oldValue = _selectedMonth;
                _selectedMonth = value;
                RaisePropertyChanged(nameof(SelectedMonth), oldValue, value, true);
            }
        }
        
        private ImportTransport _selectedImport;

        /// <summary>
        /// Sets and gets the SelectedImport property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public ImportTransport SelectedImport
        {
            get => _selectedImport;
            set => Set(nameof(SelectedImport), ref _selectedImport, value);
        }

        private Material _selectedMaterial;

        /// <summary>
        /// Sets and gets the SelectedMaterial property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public Material SelectedMaterial
        {
            get => _selectedMaterial;
            set => Set(nameof(SelectedMaterial), ref _selectedMaterial, value);
        }

        private ListCollectionView _imports;

        /// <summary>
        /// Sets and gets the Imports property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public ListCollectionView Imports
        {
            get => _imports;
            set => Set(nameof(Imports), ref _imports, value);
        }

        private ListCollectionView _materials;

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

        public ICommand AddImportedMaterialCommand { get; set; }
        public ICommand ImportedMaterialEnterCommand { get; set; }
        public ICommand AddImportCommand { get; set; }
        public ICommand CloseCommand { get; set; }
        public ICommand ImportsEnterCommand { get; set; }   


        public ImportViewModel(IPrintingService printing, IDialogService dialogService, IDataService dataService)
        {
            _printing = printing;
            _dialogService = dialogService;
            _dataService = dataService;

            _dataService.GetImports(((transports, materials, exception) =>
            {
                if (exception != null)
                {
                    //TODO: Report any errors here
                }

                _imports = (ListCollectionView)CollectionViewSource.GetDefaultView(transports);
                _materials = (ListCollectionView)CollectionViewSource.GetDefaultView(materials);

                _imports.SortDescriptions.Add(new SortDescription("ImportDate", ListSortDirection.Ascending));
                _materials.SortDescriptions.Add(new SortDescription("MaterialName", ListSortDirection.Ascending));

            }));

            LoadCommands();

        }

        private void LoadCommands()
        {
            AddImportedMaterialCommand = new RelayCommand(AddImportedMaterial, CanAddMaterial);
            AddImportCommand = new RelayCommand(AddImport, CanAddImport);
            CloseCommand = new RelayCommand(Close, true);
            ImportedMaterialEnterCommand = new RelayCommand<Key>(ImportedMaterialEnter, true);
            ImportsEnterCommand = new RelayCommand<Key>(ImportsEnter, true);
        }

        private void ImportsEnter(Key key)
        {
            if (key == Key.Enter)
                AddImport();
        }

        private void ImportedMaterialEnter(Key key)
        {
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
        }

        private bool CanAddMaterial()
        {
            if (string.IsNullOrWhiteSpace(Quantity)) return false;
            if (SelectedMaterial == null) return false;
            if (SelectedImport == null) return false;

            var qtyResult = float.TryParse(Quantity, NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out var qty);

            return qtyResult;
        }

        private void AddFilter(DateTime value)
        {
            _imports.Filter = o =>
            {
                //if (string.IsNullOrWhiteSpace(_searchFilter)) return true;

                var result = o is ImportTransport import &&
                             (import.ImportDate.Year == value.Year &&
                              import.ImportDate.Month == value.Month);
                return result;
            };
            
        }
    }
}
