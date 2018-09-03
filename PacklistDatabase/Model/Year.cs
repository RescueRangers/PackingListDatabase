using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;

namespace PacklistDatabase.Model
{
    public class Year : ObservableObject, IEditableObject
    {
        private int _previousYearNumber;

        private int _yearNumber;

        /// <summary>
        /// Sets and gets the YearNumber property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public int YearNumber
        {
            get => _yearNumber;
            set => Set(nameof(YearNumber), ref _yearNumber, value);
        }

        private ObservableCollection<Month> _months = new ObservableCollection<Month>();

        /// <summary>
        /// Sets and gets the Months property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public ObservableCollection<Month> Months
        {
            get => _months;
            set => Set(nameof(Months), ref _months, value);
        }

        private bool _isEdited = false;

        /// <summary>
        /// Sets and gets the IsEdited property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public bool IsEdited
        {
            get => _isEdited;
            set => Set(nameof(IsEdited), ref _isEdited, value);
        }

        public int YearId { get; set; }

        public Year()
        {
            if (YearNumber < 1) return;
            PopulateMonths();
            IsEdited = true;

        }

        private void PopulateMonths()
        {
            for (var i = 1; i <= 12; i++)
            {
                _months.Add(new Month(i, _yearNumber));
            }
        }

        public override string ToString()
        {
            return YearNumber.ToString();
        }

        public void BeginEdit()
        {
            _previousYearNumber = YearNumber;
        }

        public void EndEdit()
        {
            if (_months.Count < 12 && YearNumber > 0)
            {
                PopulateMonths();
            }

            IsEdited = true;

            _previousYearNumber = 0;
        }

        public void CancelEdit()
        {
            YearNumber = _previousYearNumber;
        }
    }
}
