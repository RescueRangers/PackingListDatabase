using System.Collections.Generic;
using System.Collections.ObjectModel;
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
        private IDataService _dataService;
        private IDialogService _dialogService = new DialogService();
        private ObservableCollection<Material> _materials;

        private ListCollectionView _materialView;

        /// <summary>
        /// Sets and gets the MaterialView property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// This property's value is broadcasted by the MessengerInstance when it changes.
        /// </summary>
        public ListCollectionView MaterialView
        {
            get
            {
                return _materialView;
            }

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

        public MaterialsViewModel(IDataService dataService)
        {
            _dataService = dataService;

            _dataService.GetItems((items, materials, error) =>
            {
                if (error != null)
                {
                    //TODO: Report error
                    return;
                }

                //_materials = new ObservableCollection<Material>(materials);
                MaterialView = (ListCollectionView) CollectionViewSource.GetDefaultView(materials);

            });

            SaveCommand = new RelayCommand(Save, true);
        }

        private void Save()
        {
            _dataService.SaveData();
        }
    }
}
