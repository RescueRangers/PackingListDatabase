using System.Collections.ObjectModel;
using GalaSoft.MvvmLight;

namespace Packlists.Model
{
    public class Day : ObservableObject
    {
        private int _dayNumber;

        /// <summary>
        /// Sets and gets the DayNumber property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public int DayNumber
        {
            get => _dayNumber;
            set => Set(nameof(DayNumber), ref _dayNumber, value);
        }

        private ObservableCollection<Packliste> _packlists;

        /// <summary>
        /// Sets and gets the Packlists property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public ObservableCollection<Packliste> Packlists
        {
            get => _packlists;
            set => Set(nameof(Packlists), ref _packlists, value);
        }

        public int DayId { get; set; }

        public override string ToString()
        {
            return DayNumber.ToString();
        }
    }
}
