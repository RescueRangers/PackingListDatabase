using System;
using System.Collections.ObjectModel;
using GalaSoft.MvvmLight;

namespace Packlists.Model
{
    public class ImportTransport : ObservableObject
    {
        private string _sender;

        /// <summary>
        /// Sets and gets the Sender property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public string Sender
        {
            get => _sender;
            set => Set(nameof(Sender), ref _sender, value);
        }

        private DateTime _importDate;

        /// <summary>
        /// Sets and gets the ImportDate property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public DateTime ImportDate
        {
            get => _importDate;
            set => Set(nameof(ImportDate), ref _importDate, value);
        }

        private ObservableCollection<MaterialAmount> _importedMaterials;

        /// <summary>
        /// Sets and gets the ImportedMaterials property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public ObservableCollection<MaterialAmount> ImportedMaterials
        {
            get => _importedMaterials;
            set => Set(nameof(ImportedMaterials), ref _importedMaterials, value);
        }

        public int ImportTransportId { get; set; }
    }
}
