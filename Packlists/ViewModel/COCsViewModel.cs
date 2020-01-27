using System;
using System.ComponentModel;
using System.IO;
using System.Threading;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using MvvmDialogs;
using MvvmDialogs.FrameworkDialogs.OpenFile;
using Packlists.Model;
using Packlists.Model.ProgressBar;

namespace Packlists.ViewModel
{
    public class COCsViewModel : ViewModelBase
    {
        private readonly IDataService _dataService;
        private readonly IDialogService _dialogService;
        private readonly IProgressDialogService _progressDialog;
        private FileInfo _excelFile;
        private ListCollectionView _cocs;
        private DateTime _selectedMonth = DateTime.Now;

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

        public ListCollectionView COCs
        {
            get => _cocs;
            set => Set(ref _cocs, value);
        }

        public ICommand ImportCOCsCommand { get; set; }

        public COCsViewModel(IDataService dataService, IDialogService dialogService, IProgressDialogService progressDialog)
        {
            _dataService = dataService;
            _dialogService = dialogService;
            _progressDialog = progressDialog;
            ImportCOCsCommand = new RelayCommand(ImportCOCs);
            LoadMonthlyData();
        }

        private void ImportCOCs()
        {
            var openFileOptions = new OpenFileDialogSettings
            {
                Filter = "Excel files (*.xlsx)|*.xlsx",
                Title = "Packliste in excel",
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)
            };

            var success = _dialogService.ShowOpenFileDialog(this, openFileOptions);

            if (success == true)
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
                WindowTitle = "Importing COCs"
            };
            _progressDialog.Execute(Import, options);
        }

        private async void Import(CancellationToken cancellationToken, IProgress<ProgressReport> progress)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var progressReport = new ProgressReport
            {
                IsIndeterminate = true,
                CurrentTask = "Importing COCs"
            };

            progress.Report(progressReport);

            var result = await ExcelImport.ImportCOCs.FromExcel(_excelFile, _dataService);

            if (string.Equals(result, "success", StringComparison.InvariantCultureIgnoreCase))
            {
                //LoadData();
                return;
            }
            _dialogService.ShowMessageBox(this, result, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        private void LoadMonthlyData()
        {
            _dataService.GetCOCs(((cocs, exception) =>
            {
                if (exception != null)
                {
                    //TODO: Report any errors here
                }
                _cocs = (ListCollectionView)CollectionViewSource.GetDefaultView(cocs);
                _cocs.SortDescriptions.Add(new SortDescription("CocNumber", ListSortDirection.Ascending));
                _cocs.Refresh();
            }), SelectedMonth);
        }

        private void AddFilter(DateTime value)
        {
            _cocs.Filter = o =>
            {
                //if (string.IsNullOrWhiteSpace(_searchFilter)) return true;

                var result = o is COC coc &&
                             (coc.InventoryDate.Year == value.Year &&
                              coc.InventoryDate.Month == value.Month);
                return result;
            };
            _cocs.Refresh();
        }
    }
}