using System.ComponentModel;
using System.Windows.Data;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using MvvmDialogs;
using Packlists.Model;

namespace Packlists.ViewModel
{
    public class MaterialsViewModel : ViewModelBase
    {
        private readonly IDataService _dataService;
        private readonly IDialogService _dialogService;
        private ListCollectionView _materialView;

        #region Properties

        /// <summary>
        /// Sets and gets the MaterialView property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// This property's value is broadcast-ed by the MessengerInstance when it changes.
        /// </summary>
        public ListCollectionView MaterialView
        {
            get => _materialView;

            set
            {
                if (_materialView == value)
                {
                    return;
                }

                var oldValue = _materialView;
                _materialView = value;
                RaisePropertyChanged(nameof(MaterialView), oldValue, value, true);
            }
        }


        public ICommand SaveCommand { get; set; }
        
        #endregion

        public MaterialsViewModel(IDataService dataService, IDialogService dialogService)
        {
            _dataService = dataService;
            _dialogService = dialogService;

            _dataService.GetItems((items, materials, exception) =>
            {
                if (exception != null)
                {
                    _dialogService.ShowMessageBox(this, $"{exception.Message}\r\n{exception.StackTrace}");
                    return;
                }

                //_materials = new ObservableCollection<Material>(materials);
                MaterialView = (ListCollectionView) CollectionViewSource.GetDefaultView(materials);

            });

            MaterialView.SortDescriptions.Add(new SortDescription("MaterialName", ListSortDirection.Ascending));

            SaveCommand = new RelayCommand(Save, true);
        }

        private void Save()
        {
            _dataService.SaveData();
        }
    }
}
