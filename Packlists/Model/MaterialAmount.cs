using System.ComponentModel.DataAnnotations.Schema;
using GalaSoft.MvvmLight;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Logical;

namespace Packlists.Model
{
    public class MaterialAmount : ObservableObject
    {
        private float _amount;

        /// <summary>
        /// Sets and gets the Amount property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public float Amount
        {
            get => _amount;
            set => Set(nameof(Amount), ref _amount, value);
        }

        private Material _material;
        private bool _isSelected;

        /// <summary>
        /// Sets and gets the Material property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public Material Material
        {
            get => _material;
            set => Set(nameof(Material), ref _material, value);
        }

        [NotMapped]public bool IsSelected
        {
            get => _isSelected;
            set => Set(ref _isSelected, value);
        }

        public int MaterialAmountId { get; set; }
    }
}
