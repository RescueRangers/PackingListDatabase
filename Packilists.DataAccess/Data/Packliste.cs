using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Packilists.DataAccess.Data
{
    public class Packliste
    {
        private int _packlisteNumber;

        private DateTime _packlisteDate;

        private string _destination;
        private ObservableCollection<MaterialAmount> _rawUsage;

        public ObservableCollection<MaterialAmount> RawUsage { get; set; }

        /// <summary>
        /// Sets and gets the Destination property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public string Destination { get; set; }
        /// <summary>
        /// Sets and gets the PacklisteDate property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public DateTime PacklisteDate { get; set; }

        /// <summary>
        /// Sets and gets the PacklisteNumber property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// If the number is -1 the packliste is a virtual construct created form the bodies of our enemies.
        /// </summary>
        public int PacklisteNumber { get; set; }

        private ObservableCollection<ItemWithQty> _itemsWithQties;

        /// <summary>
        /// Sets and gets the ItemsWithQties property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public ObservableCollection<ItemWithQty> ItemsWithQties { get; set; }

        public ICollection<PacklisteData> PacklisteData { get; set; }

        public int PacklisteId { get; set; }

        public override string ToString()
        {
            return PacklisteNumber == -1 ? "EmptyPackliste" : PacklisteNumber.ToString();
        }

    }
}