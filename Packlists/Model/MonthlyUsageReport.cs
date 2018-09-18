using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;

namespace Packlists.Model
{
    public class MonthlyUsageReport : ObservableObject
    {
        private ObservableCollection<Day> _days;

        /// <summary>
        /// Sets and gets the Days property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public ObservableCollection<Day> Days
        {
            get
            {
                return _days;
            }
            set
            {
                Set(nameof(Days), ref _days, value);
            }
        }
        
        private ObservableCollection<Material> _materials;

        /// <summary>
        /// Sets and gets the Materials property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public ObservableCollection<Material> Materials
        {
            get
            {
                return _materials;
            }
            set
            {
                Set(nameof(Materials), ref _materials, value);
            }
        }
    }
}
