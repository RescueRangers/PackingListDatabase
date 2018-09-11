using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using GalaSoft.MvvmLight;
using Newtonsoft.Json;
using Packlists.Converters;
using Packlists.Model;

namespace Packlists.Model
{
    public class Packliste : EditableModelBase<Packliste>
    {
        private int _packlisteNumber;

        private DateTime _packlisteDate;

        private string _destination;

        private ObservableCollection<Tuple<Material, float, string>> _rawUsage;

        /// <summary>
        /// Sets and gets the Material property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        [NotMapped]public ObservableCollection<Tuple<Material, float, string>> RawUsage
        {
            get
            {
                if (_rawUsage != null && _rawUsage.Count >= 1) return _rawUsage;
                _rawUsage = CalculateUsage();
                return _rawUsage;

            }
            set => Set(nameof(RawUsage), ref _rawUsage, value);
        }

        private ObservableCollection<Tuple<Material, float, string>> CalculateUsage()
        {
            if (ItemsWithQties == null || ItemsWithQties.Any() == false)
            {
                return new ObservableCollection<Tuple<Material, float, string>>();
            }

            var totalUsage = new ObservableCollection<Tuple<Material, float, string>>();

            foreach (var itemWithQty in _itemsWithQties)
            {
                var qty = itemWithQty.Quantity;

                if(itemWithQty.Item.Materials == null) continue;
                
                foreach (var materialWithUsage in itemWithQty.Item.Materials)
                {
                    totalUsage.Add(new Tuple<Material, float, string>(materialWithUsage.Material,
                        qty * materialWithUsage.Usage, materialWithUsage.Material.Unit));
                }
            }

            var results = totalUsage.GroupBy(m => m.Item1).Select(g => Tuple.Create(g.Key, g.Sum(l => l.Item2), g.First().Item3)).OrderBy(o => o.Item1).ToList();

            return new ObservableCollection<Tuple<Material, float, string>>(results);
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
            ItemsWithQties.Add(item);
            RawUsage = CalculateUsage();
        }
        

    }
}


public class RawUsage : ObservableObject
{
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

    private ObservableCollection<(Material, float)> _material;

    /// <summary>
    /// Sets and gets the Material property.
    /// Changes to that property's value raise the PropertyChanged event. 
    /// </summary>
    public ObservableCollection<(Material, float)> Material
    {
        get => _material;
        set => Set(nameof(Material), ref _material, value);
    }

    public RawUsage(ItemWithQty item)
    {
        foreach (var materialWithUsage in item.Item.Materials)
        {
            var material = materialWithUsage.Material;
            var usage = materialWithUsage.Usage * item.Quantity;

            _material.Add((material, usage));

        }
    }
}

