using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using GalaSoft.MvvmLight;
using Newtonsoft.Json;
using Packlists.Converters;

namespace Packlists.Model
{
    public class Packliste : ObservableObject
    {
        private int _packlisteNumber;

        private DateTime _packlisteDate;

        /// <summary>
        /// Sets and gets the PacklisteDate property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public DateTime PacklisteDate
        {
            get => _packlisteDate;
            set => Set(nameof(PacklisteDate), ref _packlisteDate, value);
        }

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

        private Dictionary<Tuple<int, int>,object> _packlisteData;

        /// <summary>
        /// Sets and gets the PacklisteData property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        [NotMapped]public Dictionary<Tuple<int, int>,object> PacklisteData
        {
            get => _packlisteData;
            set => Set(nameof(PacklisteData), ref _packlisteData, value);
        }
        
        /// <summary>
        /// Json serialized packlist.
        /// </summary>
        public string PacklisteDataAsJson
        {
            get => JsonConvert.SerializeObject(PacklisteData);
            set
            {
                if(value != null)
                    PacklisteData = JsonConvert.DeserializeObject<Dictionary<Tuple<int, int>, object>>(value, new ValueTupleConverter());
            } 
        }

        public int PacklisteId { get; set; }

        public override string ToString()
        {
            return PacklisteNumber == -1 ? "EmptyPackliste" : PacklisteNumber.ToString();
        }

        
    }
}
