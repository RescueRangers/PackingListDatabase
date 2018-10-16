using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using GalaSoft.MvvmLight;

namespace Packlists.Model
{
    public class Day : ObservableObject
    {
        private List<MaterialAmount> _netMaterialCount = new List<MaterialAmount>();

        public List<MaterialAmount> NetMaterialCount
        {
            get => _netMaterialCount;
            set => Set(nameof(NetMaterialCount), ref _netMaterialCount, value);
        }

        private DateTime _date;

        [Required]
        public DateTime Date
        {
            get => _date;
            set => Set(nameof(Date), ref _date, value);
        }

        public int DayId { get; set; }

        public override string ToString()
        {
            return Date.ToString();
        }
    }
}
