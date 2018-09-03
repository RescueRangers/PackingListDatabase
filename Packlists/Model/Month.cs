using System;
using System.Collections.Generic;
using GalaSoft.MvvmLight;

namespace Packlists.Model
{
    public class Month : ObservableObject
    {
        private int _monthNumber ;

        /// <summary>
        /// Sets and gets the MonthNumber property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public int MonthNumber
        {
            get => _monthNumber;
            set => Set(nameof(MonthNumber), ref _monthNumber, value);
        }

       private string _monthName ;

        /// <summary>
        /// Sets and gets the MonthName property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public string MonthName
        {
            get => _monthName;
            set => Set(nameof(MonthName), ref _monthName, value);
        }

       private ICollection<Day> _days = new List<Day>();

        /// <summary>
        /// Sets and gets the Days property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public ICollection<Day> Days
        {
            get => _days;
            set => Set(nameof(Days), ref _days, value);
        }

        public int MonthId { get; set; }

        public Month(int monthNumber, int year)
        {
            for (var i = 1; i <= DateTime.DaysInMonth(year, monthNumber); i++)
            {
                _days.Add(new Day{DayNumber = i});
            }

            _monthName = new DateTime(year, monthNumber, 1).ToString("MMMM");
        }

        private Month(){}

        public override string ToString()
        {
            return MonthName;
        }
    }
}
