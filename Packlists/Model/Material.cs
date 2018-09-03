namespace Packlists.Model
{
    public class Material : EditableModelBase<Material>
    {
        private string _materialName;

        private string _unit;

        /// <summary>
        /// Sets and gets the Unit property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public string Unit
        {
            get => _unit;
            set => Set(nameof(Unit), ref _unit, value);
        }

        /// <summary>
        /// Sets and gets the MaterialName property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public string MaterialName
        {
            get => _materialName;
            set => Set(nameof(MaterialName), ref _materialName, value);
        }

        ///// <summary>
        ///// The <see cref="MaterialUsage" /> property's name.
        ///// </summary>
        //public const string MaterialUsagePropertyName = "MaterialUsage";

        //private float _materialUsage;

        public int MaterialId { get; set; }

        public override string ToString()
        {
            return MaterialName;
        }
    }


}
