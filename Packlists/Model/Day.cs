using System;
using System.Collections.ObjectModel;
using GalaSoft.MvvmLight;

namespace Packlists.Model
{
    public class Day : ObservableObject
    {
        private ObservableCollection<MaterialAmount> _netMaterialCount = new ObservableCollection<MaterialAmount>();

        /// <summary>
        /// Sets and gets the NetMaterialCount property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public ObservableCollection<MaterialAmount> NetMaterialCount
        {
            get => _netMaterialCount;
            set => Set(nameof(NetMaterialCount), ref _netMaterialCount, value);
        }

        private DateTime _date;

        /// <summary>
        /// Sets and gets the Date property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public DateTime Date
        {
            get => _date;
            set => Set(nameof(Date), ref _date, value);
        }

        public override string ToString()
        {
            return Date.ToString();
        }
    }
}
