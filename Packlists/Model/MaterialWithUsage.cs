using GalaSoft.MvvmLight;

namespace Packlists.Model
{
    public class MaterialWithUsage : EditableModelBase<MaterialWithUsage>
    {
        private Material _material;

        /// <summary>
        /// Sets and gets the Material property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public Material Material
        {
            get => _material;
            set => Set(nameof(Material), ref _material, value);
        }

        private float _usage;

        /// <summary>
        /// Sets and gets the Usage property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public float Usage
        {
            get => _usage;
            set => Set(nameof(Usage), ref _usage, value);
        }

        public int MaterialWithUsageId { get; set; }

        public override string ToString()
        {
            return Material.MaterialName;
        }
    }
}