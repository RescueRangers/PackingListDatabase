using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using GalaSoft.MvvmLight;
using MvvmDialogs;
using Packlists.Model;
using Packlists.Model.Printing;

namespace Packlists.ViewModel
{
    public class MonthlyUsageViewModel : ViewModelBase
    {
        private readonly IDataService _dataService;
        private readonly IDialogService _dialogService;
        private readonly IPrintingService _printing;

        private DataTable _report;

        /// <summary>
        /// Sets and gets the Report property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public DataTable Report
        {
            get
            {
                return _report;
            }
            set
            {
                Set(nameof(Report), ref _report, value);
            }
        }


        public MonthlyUsageViewModel(IDataService dataService, IDialogService dialogService, IPrintingService printing)
        {
            _dataService = dataService;
            _dialogService = dialogService;
            _printing = printing;

            _dataService.CreateMonthlyReport((report, exception) =>
            {
                if (exception != null)
                {
                    _dialogService.ShowMessageBox(this, exception.Message, "Error while creating the monthly report",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }

                Report = report;
            }, DateTime.Now);


        }


    }
}
