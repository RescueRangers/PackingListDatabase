using System;
using System.Data;
using System.Linq;
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
        private MonthlyUsageReport _monthlyUsageReport;

        public MonthlyUsageReport MonthlyUsageReport
        {
            get => _monthlyUsageReport;
            set => Set(ref _monthlyUsageReport, value);
        }

        /// <summary>
        /// Sets and gets the Report property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public DataTable Report
        {
            get => _report;
            set => Set(nameof(Report), ref _report, value);
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

                MonthlyUsageReport = report;
            }, new DateTime(2018,1,1));

            CreateDataTable();
        }

        private void CreateDataTable()
        {
            var report = new DataTable(MonthlyUsageReport.ReportDate.ToString("yyyy-MM-dd"));

            var column = new DataColumn("Material", typeof(string));
            report.Columns.Add(column);
            column = new DataColumn("Unit", typeof(string));
            report.Columns.Add(column);

            foreach (var day in MonthlyUsageReport.Days)
            {
                column = new DataColumn($"{day.Date:yyyy-MM-dd}", typeof(float));
                report.Columns.Add(column);
            }

            //DataRow row;

            var materials = MonthlyUsageReport.Days[0].NetMaterialCount.Select(s => s.Material).ToList();

            foreach (var material in materials)
            {
                var row = report.NewRow();
                row["Material"] = material.MaterialName;
                row["Unit"] = material.Unit;
                
                foreach (var day in MonthlyUsageReport.Days)
                {
                    row[$"{day.Date:yyyy-MM-dd}"] = day.NetMaterialCount.Single(s => s.Material == material).Amount;
                }

                var rows = row.ItemArray.Where(o => o is float).Cast<float>().Where(fl => fl.Equals(0)).ToList();

                var count = rows.Count;

                if (count == report.Columns.Count - 2) continue;

                report.Rows.Add(row);
            }

            Report = report;
        }
    }
}
