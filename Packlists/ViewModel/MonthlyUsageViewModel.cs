using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using MvvmDialogs;
using MvvmDialogs.FrameworkDialogs.MessageBox;
using OfficeOpenXml;
using Packlists.Model;
using Packlists.Model.Printing;

namespace Packlists.ViewModel
{
    public class MonthlyUsageViewModel : ViewModelBase
    {
        private readonly IDataService _dataService;
        private readonly IDialogService _dialogService;
        private readonly IPrintingService _printing;

        private float[,] _report;
        private MonthlyUsageReport _monthlyUsageReport;
        private List<string> _rowHeaders;
        private List<string> _columnHeaders;
        private DateTime _month;

        public MonthlyUsageReport MonthlyUsageReport
        {
            get => _monthlyUsageReport;
            set => Set(ref _monthlyUsageReport, value);
        }

        public DateTime Month
        {
            get { return _month; }
            set
            {
                if (_month != value)
                {
                    _month = value;
                    RaisePropertyChanged(nameof(Month));
                }
            }
        }

        public List<string> ColumnHeaders
        {
            get { return _columnHeaders; }
            set
            {
                if (_columnHeaders != value)
                {
                    _columnHeaders = value;
                    RaisePropertyChanged(nameof(ColumnHeaders));
                }
            }
        }

        public List<string> RowHeaders
        {
            get { return _rowHeaders; }
            set
            {
                if (_rowHeaders != value)
                {
                    _rowHeaders = value;
                    RaisePropertyChanged(nameof(RowHeaders));
                }
            }
        }

        public ICommand ExportToExcel { get; set; }
        public ICommand Generate { get; set; }

        /// <summary>
        /// Sets and gets the Report property.
        /// Changes to that property's value raise the PropertyChanged event.
        /// </summary>
        public float[,] Report
        {
            get => _report;
            set => Set(nameof(Report), ref _report, value);
        }

        public MonthlyUsageViewModel(IDataService dataService, IDialogService dialogService, IPrintingService printing)
        {
            _dataService = dataService;
            _dialogService = dialogService;
            _printing = printing;

            ExportToExcel = new RelayCommand(ToExcel);
            Generate = new RelayCommand(GenerateMonthlyReport);

            _month = DateTime.UtcNow;
        }

        private void ToExcel()
        {
            using (var excel = new ExcelPackage(new MemoryStream()))
            {
                var rowLength = Report.GetLength(0) + 2;
                var colLength = Report.GetLength(1) + 2;
                var ws = excel.Workbook.Worksheets.Add("Worksheet1");
                ws.Cells[2, 1, colLength, 1].LoadFromCollection(RowHeaders);

                for (int i = 0; i < Report.GetLength(0); i++)
                {
                    for (int j = 0; j < Report.GetLength(1); j++)
                    {
                        ws.Cells[1, j + 2].Value = ColumnHeaders[j];
                        ws.Column(j + 1).AutoFit();
                        ws.Cells[i + 2, j + 2].Value = Report[i, j];
                    }
                }

                ws.Cells[2, 2, rowLength, colLength].Style.Numberformat.Format = "0.00";
                ws.Column(colLength - 1).AutoFit();

                var address = new ExcelAddress(2, colLength - 1, rowLength - 1, colLength - 1);
                var cfRule1 = ws.ConditionalFormatting.AddGreaterThan(address);
                var cfRule2 = ws.ConditionalFormatting.AddLessThan(address);
                cfRule1.Style.Fill.BackgroundColor.Color = System.Drawing.Color.LawnGreen;
                cfRule1.Formula = "0";
                cfRule2.Style.Fill.BackgroundColor.Color = System.Drawing.Color.Crimson;
                cfRule2.Formula = "0";
                ws.View.FreezePanes(2, 2);

                var filename = $"{System.IO.Path.GetTempPath()}{Guid.NewGuid()}.xlsx";
                excel.SaveAs(new FileInfo(filename));
                Process.Start(filename);
            }
        }

        private async void GenerateMonthlyReport()
        {
            var values = new List<float>();
            var labels = new List<DateTime>();
            var titles = new List<string>();
            var packlists = new List<Packliste>();
            var imports = new List<ImportTransport>();
            var materials = new List<Material>();

            try
            {
                packlists = await _dataService.GetPacklistsWithoutData(_month);
                imports = await _dataService.GetImports(_month);

                materials = packlists.SelectMany(p => p.RawUsage.Select(pr => pr.Material)).Distinct()
                    .Union(imports.SelectMany(i => i.ImportedMaterials.Select(ii => ii.Material))).Distinct().ToList();
                materials.Sort();
            }
            catch (Exception ex)
            {
                _dialogService.ShowMessageBox(this, new MessageBoxSettings
                {
                    Button = MessageBoxButton.OK,
                    Caption = "Error while creating the report.",
                    Icon = MessageBoxImage.Error,
                    MessageBoxText = ex.Message
                });
            }

            labels = packlists.Select(p => p.PacklisteDate).Union(imports.Select(i => i.ImportDate)).ToList();
            labels.Sort();

            var columns = labels.Count + 1;
            var rows = materials.Count;

            var report = new float[rows, columns];

            for (var i = 0; i < labels.Count; i++)
            {
                var packlistsDate = packlists.Where(p => p.PacklisteDate == labels[i]);
                var importsDate = imports.Where(wi => wi.ImportDate == labels[i]);

                if (packlistsDate.Any())
                {
                    var rawMaterial = new List<MaterialAmount>();

                    foreach (var item in packlistsDate)
                    {
                        rawMaterial.AddRange(item.RawUsage);
                    }

                    if (importsDate.Any())
                    {
                        var rawImportedMaterial = new List<MaterialAmount>();
                        foreach (var item in importsDate)
                        {
                            rawImportedMaterial.AddRange(item.ImportedMaterials);
                        }

                        for (var j = 0; j < materials.Count; j++)
                        {
                            var importedMaterial = rawImportedMaterial.Where(r => r.Material.Equals(materials[j]));
                            var packlisteMaterial = rawMaterial.Where(r => r.Material.Equals(materials[j]));

                            Console.WriteLine(j);

                            var importedAmounts = importedMaterial.Sum(s => s.Amount);
                            var packlisteAmounts = packlisteMaterial.Sum(s => s.Amount);

                            report[j, i] = importedAmounts - packlisteAmounts;
                        }
                    }
                    else
                    {
                        for (var j = 0; j < materials.Count; j++)
                        {
                            var packlisteMaterial = rawMaterial.Where(r => r.Material.Equals(materials[j]));

                            Console.WriteLine(j);

                            var packlisteAmounts = packlisteMaterial.Sum(s => s.Amount);

                            report[j, i] = -packlisteAmounts;
                        }
                    }
                }
                else if (importsDate.Any())
                {
                    var rawImportedMaterial = new List<MaterialAmount>();
                    foreach (var item in importsDate)
                    {
                        rawImportedMaterial.AddRange(item.ImportedMaterials);
                    }

                    for (var j = 0; j < materials.Count; j++)
                    {
                        var importedMaterial = rawImportedMaterial.Where(r => r.Material.Equals(materials[j]));

                        Console.WriteLine(j);

                        report[j, i] = importedMaterial.Sum(s => s.Amount);
                    }
                }
            }

            //Add last column with the sums

            for (int i = 0; i < materials.Count; i++)
            {
                report[i, columns - 1] = SumRow(report, i);
            }

            Report = report;
            RowHeaders = new List<string>(materials.Select(s => s.MaterialName));
            ColumnHeaders = new List<string>(labels.Select(s => s.ToString("yyyy-MM-dd")))
            {
                "Sum"
            };
        }

        public void GenerateMonthlyReport(DateTime month)
        {
            var values = new List<float>();
            var labels = new List<DateTime>();
            var titles = new List<string>();

            var packlists = new List<Packliste>();
            var imports = new List<ImportTransport>();

            _dataService.GetPacklists(((packlist, exception) =>
            {
                if (exception != null) return;

                packlists = packlist.ToList();
            }), month);

            _dataService.GetImports(((transports, exception) =>
            {
                if (exception != null) return;

                imports = transports.ToList();
            }), month);

            var materials = packlists.SelectMany(p => p.RawUsage.Select(pr => pr.Material)).Distinct()
                .Union(imports.SelectMany(i => i.ImportedMaterials.Select(ii => ii.Material))).Distinct().ToList();

            labels = packlists.Select(p => p.PacklisteDate).Union(imports.Select(i => i.ImportDate)).ToList();
            labels.Sort();

            var columns = labels.Count + 1;
            var rows = materials.Count;

            var report = new float[rows, columns];

            for (var i = 0; i < labels.Count; i++)
            {
                var packlistsDate = packlists.Where(p => p.PacklisteDate == labels[i]);
                var importsDate = imports.Where(wi => wi.ImportDate == labels[i]);

                if (packlistsDate.Any())
                {
                    var rawMaterial = new List<MaterialAmount>();

                    foreach (var item in packlistsDate)
                    {
                        rawMaterial.AddRange(item.RawUsage);
                    }

                    if (importsDate.Any())
                    {
                        var rawImportedMaterial = new List<MaterialAmount>();
                        foreach (var item in importsDate)
                        {
                            rawImportedMaterial.AddRange(item.ImportedMaterials);
                        }

                        for (var j = 0; j < materials.Count; j++)
                        {
                            var importedMaterial = rawImportedMaterial.Where(r => r.Material == materials[j]);
                            var packlisteMaterial = rawMaterial.Where(r => r.Material == materials[j]);

                            var importedAmounts = importedMaterial.Sum(s => s.Amount);
                            var packlisteAmounts = packlisteMaterial.Sum(s => s.Amount);

                            report[j, i] = importedAmounts - packlisteAmounts;
                        }
                    }
                    else
                    {
                        for (var j = 0; j < materials.Count; j++)
                        {
                            var packlisteMaterial = rawMaterial.Where(r => r.Material == materials[j]);

                            var packlisteAmounts = packlisteMaterial.Sum(s => s.Amount);

                            report[j, i] = -packlisteAmounts;
                        }
                    }
                }
                else if (importsDate.Any())
                {
                    var rawImportedMaterial = new List<MaterialAmount>();
                    foreach (var item in importsDate)
                    {
                        rawImportedMaterial.AddRange(item.ImportedMaterials);
                    }

                    for (var j = 0; j < materials.Count; j++)
                    {
                        var importedMaterial = rawImportedMaterial.Where(r => r.Material == materials[j]);

                        report[j, i] = importedMaterial.Sum(s => s.Amount);
                    }
                }
            }

            //Add last column with the sums

            for (int i = 0; i < materials.Count; i++)
            {
                report[i, columns - 1] = SumRow(report, i);
            }

            Report = report;
            RowHeaders = new List<string>(materials.Select(s => s.MaterialName));
            ColumnHeaders = new List<string>(labels.Select(s => s.ToString("yyyy-MM-dd")))
            {
                "Sum"
            };
        }

        public float SumRow(float[,] matrix, int row)
        {
            float sum = 0;
            for (int i = 0; i < matrix.GetLength(1); i++)
            {
                sum += matrix[row, i];
            }
            return sum;
        }
    }
}