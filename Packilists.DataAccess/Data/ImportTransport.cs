using System;
using System.Collections.ObjectModel;

namespace Packilists.DataAccess.Data
{
    public class ImportTransport
    {
        /// <summary>
        /// Sets and gets the Sender property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public string Sender { get; set; }

        /// <summary>
        /// Sets and gets the ImportDate property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public DateTime ImportDate { get; set; }

        /// <summary>
        /// Sets and gets the ImportedMaterials property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public ObservableCollection<MaterialAmount> ImportedMaterials { get; set; }

        public int ImportTransportId { get; set; }
    }
}
