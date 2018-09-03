using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using Packlists.Messages;
using Packlists.Model;
using Packlists.Model.ProgressBar;

namespace Packlists.ViewModel
{
    public class ItemsViewModel : ViewModelBase
    {
        private IDataService _dataService;
        private ObservableCollection<Item> _items;
        private ObservableCollection<Material> _materials;
        private readonly IDialogService _dialogService;
        private readonly IProgressDialogService _progressDialog;
        private FileInfo _excelFile;

        private ListCollectionView _itemsView;

        private Item _selectedItem;

        private string _searchFilter;

        private string _newItemName;

        private ListCollectionView _materialsView;

       private MaterialWithUsage _selectedMaterial;

        /// <summary>
        /// Sets and gets the SelectedItem property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public MaterialWithUsage SelectedMaterial
        {
            get
            {
                return _selectedMaterial;
            }
            set
            {
                Set(nameof(SelectedMaterial), ref _selectedMaterial, value);
            }
        }

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

        public ICommand AddNewItemCommand { get; private set; }
        public ICommand AddMaterialCommand { get; private set; }
        public ICommand OpenMaterialsPanelCommand { get; private set; } 
        public ICommand SaveCommand { get; private set; }
        public ICommand RemoveMaterialCommand { get; private set; }
        public ICommand ImportItemsCommand { get; private set; }
        public ICommand ClosingCommand { get; set; }


        public ItemsViewModel(IDataService dataService, IDialogService dialogService, IProgressDialogService progressDialog)
        {
            _dataService = dataService;
            _dialogService = dialogService;
            _progressDialog = progressDialog;
            //AddFilter(string.Empty);

            MessengerInstance.Register<UpdateItemsModelMessage>(this, OnMessageReceived);

            LoadData();
            LoadCommands();
            AddFilter(string.Empty);
            ItemsView.Refresh();
        }

        private void OnMessageReceived(UpdateItemsModelMessage obj)
        {
            LoadData();
        }

        private void LoadCommands()
        {
            AddNewItemCommand = new RelayCommand(AddNewItem, () => !string.IsNullOrWhiteSpace(NewItemName));
            AddMaterialCommand = new RelayCommand(AddMaterial, () => SelectedItem != null);
            SaveCommand = new RelayCommand(Save, true);
            RemoveMaterialCommand = new RelayCommand<object>(RemoveMaterial, true);
            ImportItemsCommand = new RelayCommand(ImportItems, true);
        }

        private void ImportItems()
        {
            FileInfo excelFile;
            var openFileOptions = new OpenFileDialogSettings
            {
                Filter = "Excel files (*.xlsx)|*.xlsx",
                Title = "Open excel file with items",
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)
            };

            var succes = _dialogService.ShowOpenFileDialog(this, openFileOptions);

            if (succes == true)
            {
                _excelFile = new FileInfo(openFileOptions.FileName);
            }
            else
            {
                return;
            }

            var options = new ProgressDialogOptions
            {
                Label = "Current task: ",
                WindowTitle = "Importing items"
            };
            _progressDialog.Execute(Import, options);
            //await Import();

        }

        private async void Import(CancellationToken cancellationToken, IProgress<ProgressReport> progress)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var progressReport = new ProgressReport
            {
                IsIndeterminate = true,
                CurrentTask = "Importing items"
            };

            progress.Report(progressReport);

            var added = await ExcelImport.ImportItems.FromExcel((items, error) =>
            {
                if (error != null)
                {
                    _dialogService.ShowMessageBox(this, error.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                //_dataService.AddItems(items);

                LoadData();

            }, _excelFile, _dataService);

            if (!string.IsNullOrWhiteSpace(added))
            {
                Application.Current.Dispatcher.Invoke(delegate
                {
                    _dialogService.ShowMessageBox(this, added, "Import operation.", MessageBoxButton.OK,
                        MessageBoxImage.Information);
                });
            }

        }

        private void LoadData()
        {
            _dataService.GetItems((items, materials, exception) =>
            {
                if (exception != null)
                {
                    //TODO: Report error
                }

                //_materials = new ObservableCollection<Material>(materials);
                //_items = new ObservableCollection<Item>(items);
                MaterialsView = (ListCollectionView)CollectionViewSource.GetDefaultView(materials);
                //_materialsView.Refresh();
                ItemsView = (ListCollectionView)CollectionViewSource.GetDefaultView(items);
                //_itemsView.Refresh();
            });
        }

        private void RemoveMaterial(object o)
        {
            if (o != null)
            {
                SelectedItem.Materials.Remove((MaterialWithUsage) o);
            }
            SelectedItem.Materials.Remove(SelectedMaterial);
            //ItemsView.Refresh();
        }

        private void Save()
        {

            var options = new ProgressDialogOptions
            {
                Label = "Please wait",
                WindowTitle = "Saving changes"
            };
            _progressDialog.Execute(SaveItems, options);

            //_dataService.SaveData();
        }

        private void SaveItems(CancellationToken cancellationToken, IProgress<ProgressReport> progress)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var progressReport = new ProgressReport
            {
                IsIndeterminate = true,
                CurrentTask = "Saving..."
            };

            progress.Report(progressReport);

            Application.Current.Dispatcher.Invoke(delegate
            {
                _dataService.SaveData();
            });


        }
        private void AddMaterial()
        {
            if(SelectedItem.Materials == null)
                SelectedItem.Materials = new ObservableCollection<MaterialWithUsage>();

            SelectedItem.Materials.Add(new MaterialWithUsage());
        }

        private void AddNewItem()
        {
            //_items.Add(new Item{ItemName = _newItemName});
            ItemsView.AddNewItem(new Item {ItemName = _newItemName});
            //if (ItemsView.AddNew() is Item newItem) newItem.ItemName = _newItemName;
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
        //    base.Cleanup();
        //}
        
    }
}
