using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;

namespace PacklistDatabase.Model
{
    public class Packliste : ObservableObject
    {
        private int _packlisteNumber;

        /// <summary>
        /// Sets and gets the PacklisteNumber property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// If the number is -1 the packliste is a virtual construct created form the bodies of our enemies.
        /// </summary>
        public int PacklisteNumber
        {
            get => _packlisteNumber;
            set => Set(nameof(PacklisteNumber), ref _packlisteNumber, value);
        }

       private ICollection<Item> _items;

        /// <summary>
        /// Sets and gets the Items property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public ICollection<Item> Items
        {
            get => _items;
            set => Set(nameof(Items), ref _items, value);
        }

        private string _packlisteHeader;

        /// <summary>
        /// Sets and gets the PacklisteHeader property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public string PacklisteHeader
        {
            get => _packlisteHeader;
            set => Set(nameof(PacklisteHeader), ref _packlisteHeader, value);
        }

        public int PacklisteId { get; set; }

        public override string ToString()
        {
            return PacklisteNumber == -1 ? "EmptyPackliste" : PacklisteNumber.ToString();
        }
    }
}
