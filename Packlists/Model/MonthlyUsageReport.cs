using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using GalaSoft.MvvmLight;

namespace Packlists.Model
{
    public class MonthlyUsageReport : ObservableObject
    {
        private List<Material> _materials;
        private List<Day> _days;
        private DateTime _reportDate;

        [Key]
        public int ReportId { get; set; }

        [Required]
        public DateTime ReportDate
        {
            get => _reportDate;
            set => Set(ref _reportDate, value);
        }

        /// <summary>
        /// Sets and gets the Days property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public List<Day> Days
        {
            get => _days;
            set => Set(nameof(Days), ref _days, value);
        }

        [NotMapped]
        public List<Material> Materials
        {
            get => _materials;
            set => Set(nameof(Materials), ref _materials, value);
        }

        private MonthlyUsageReport()
        {}

        public MonthlyUsageReport(DateTime month, List<ImportTransport> imports, List<Packliste> exports, List<Material> materials, MonthlyUsageReport previousReport = null)
        {
            Materials = materials;
            if (Days == null || !Days.Any())
            {
                Days = GetDaysInMonth(month);
            }

            foreach (var t in Days)
            {
                var day = t.Date;

                var dailyImports = imports.Where(im => im.ImportDate == day).SelectMany(s => s.ImportedMaterials)
                    .GroupBy(g => g.Material).Select(g => new MaterialAmount
                        {Material = g.Key, Amount = g.Sum(s => s.Amount)}).ToList();
                var dailyExports = exports.Where(pac => pac.PacklisteDate == day)
                    .SelectMany(s => s.ItemsWithQties.SelectMany(itm => itm.Item.Materials)).GroupBy(g => g.Material)
                    .Select(g => new MaterialAmount
                        {Material = g.Key, Amount = g.Sum(s => s.Amount)}).ToList();

                for (var j = 0; j < _materials.Count; j++)
                {
                    var material = _materials[j];
                    var importedMaterial = dailyImports.SingleOrDefault(s => s.Material == material);
                    var exportedMaterial = dailyExports.SingleOrDefault(s => s.Material == material);

                    t.NetMaterialCount.Add(importedMaterial ?? new MaterialAmount {Material = material, Amount = 0});

                    if (exportedMaterial != null)
                    {
                        t.NetMaterialCount[j].Amount -= exportedMaterial.Amount;
                    }
                }
            }

            if (previousReport != null)
            {
                var lastMonthDay = previousReport.Days.Last();

                Days[0].NetMaterialCount.AddRange(lastMonthDay.NetMaterialCount);

                Days[0].NetMaterialCount = Days[0].NetMaterialCount.GroupBy(g => g.Material)
                    .Select(g => new MaterialAmount {Material = g.Key, Amount = g.Sum(s => s.Amount)}).ToList();

            }

            for (var i = 1; i < Days.Count; i++)
            {
                var day = Days[i];
                var previousDay = Days[i - 1];

                for (var index = 0; index < day.NetMaterialCount.Count; index++)
                {
                    day.NetMaterialCount[index].Amount += previousDay.NetMaterialCount[index].Amount;
                }
            }
            
        }

        private List<Day> GetDaysInMonth(DateTime month)
        {
            var days = new List<Day>();

            for (var i = 1; i <= DateTime.DaysInMonth(month.Year, month.Month); i++)
            {
                days.Add(new Day{Date = new DateTime(month.Year, month.Month, i)});
            }

            return days;
        }
    }
}
