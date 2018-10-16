using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using Newtonsoft.Json;
using Packlists.Converters;

namespace Packlists.Model
{
    public class Packliste : EditableModelBase<Packliste>
    {
        private int _packlisteNumber;

        private DateTime _packlisteDate;

        private string _destination;
        private ObservableCollection<MaterialAmount> _rawUsage;

        public ObservableCollection<MaterialAmount> RawUsage
        {
            get
            {
                //if (_rawUsage != null && _rawUsage.Count >= 1) return _rawUsage;
                //_rawUsage = CalculateUsage();
                return _rawUsage;
            }
            set => Set(ref _rawUsage, value);
        }

        /// <summary>
        /// Sets and gets the Destination property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public string Destination
        {
            get => _destination;
            set => Set(nameof(Destination), ref _destination, value);
        }

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

       private ObservableCollection<ItemWithQty> _itemsWithQties;

        /// <summary>
        /// Sets and gets the ItemsWithQties property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public ObservableCollection<ItemWithQty> ItemsWithQties
        {
            get => _itemsWithQties;
            set => Set(nameof(ItemsWithQties), ref _itemsWithQties, value);
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


        public void AddItem(ItemWithQty item)
        {
            if(ItemsWithQties == null) ItemsWithQties = new ObservableCollection<ItemWithQty>();
            ItemsWithQties.Add(item);
            RawUsage = CalculateUsage();
        }

        private ObservableCollection<MaterialAmount> CalculateUsage()
        {
            var rawUsage = new List<MaterialAmount>();

            if (ItemsWithQties == null || ItemsWithQties.Any() == false)
            {
                return new ObservableCollection<MaterialAmount>();
            }

            foreach (var itemWithQty in _itemsWithQties)
            {
                var qty = itemWithQty.Quantity;

                if (itemWithQty.Item.Materials == null) continue;

                foreach (var materialWithUsage in itemWithQty.Item.Materials)
                {
                    rawUsage.Add(new MaterialAmount
                        {Material = materialWithUsage.Material, Amount = qty * materialWithUsage.Amount});
                }
            }
            var results = rawUsage.GroupBy(m => m.Material).Select(g => new MaterialAmount{Material = g.Key, Amount = g.Sum(l => l.Amount)}).OrderBy(o => o.Material).ToList();

            return new ObservableCollection<MaterialAmount>(results);
        }

        public void RecalculateUsage()
        {
            RawUsage = CalculateUsage();
        }
    }
}