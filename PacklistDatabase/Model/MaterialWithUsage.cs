using GalaSoft.MvvmLight;

namespace PacklistDatabase.Model
{
    public class MaterialWithUsage : ObservableObject
    {
        /// <summary>
        /// The <see cref="Material" /> property's name.
        /// </summary>
        public const string MaterialPropertyName = "Material";

        private Material _material;

        /// <summary>
        /// Sets and gets the Material property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public Material Material
        {
            get => _material;
            set => Set(MaterialPropertyName, ref _material, value);
        }

        /// <summary>
        /// The <see cref="Usage" /> property's name.
        /// </summary>
        public const string UsagePropertyName = "Usage";

        private float _usage;

        /// <summary>
        /// Sets and gets the Usage property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public float Usage
        {
            get => _usage;
            set => Set(UsagePropertyName, ref _usage, value);
        }

        public int MaterialWithUsageId { get; set; }
    }
}