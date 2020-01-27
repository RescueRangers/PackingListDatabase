using System.ComponentModel.DataAnnotations;
using GalaSoft.MvvmLight;

namespace Packlists.Model
{
    public class PacklisteData : ObservableObject
    {
        private string _data;
        private int _columnNumber;
        private int _rowNumber;

        public int RowNumber
        {
            get => _rowNumber;
            set => Set(ref _rowNumber, value);
        }

        public int ColumnNumber
        {
            get => _columnNumber;
            set => Set(ref _columnNumber, value);
        }

        public string Data
        {
            get => _data;
            set => Set(ref _data, value);
        }

        public virtual Packliste Packliste { get; set; }

        [Key]
        public int PacklisteDataId { get; set; }
    }
}