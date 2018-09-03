using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Linq;

namespace PacklistDatabase.Model
{
    public class DataService : IDataService, IDisposable
    {
        private ICollection<Year> _years;
        private ICollection<Item> _items;
        private ICollection<Material> _materials;
        private readonly PacklisteContext _packlisteContext;


        public DataService()
        {
            _packlisteContext = new PacklisteContext();
        }

        public void GetYearsAndItems(Action<ICollection<Year>, ICollection<Item>, Exception> callback)
        {
            if (_years == null)
                _years = _packlisteContext.Years.Include("Months.Days.Packlists").ToList();
            if (_items == null) _items = _packlisteContext.Items.ToList();
            
           callback(_years, _items, null);
        }

        public void GetItemsAndMaterials(Action<ICollection<Item>, ICollection<Material>, Exception> callback)
        {
            if (_items == null) _items = _packlisteContext.Items.ToList();
            if (_materials == null) _materials = _packlisteContext.Materials.ToList();
            callback(_items, _materials, null);
        }

        public void GetMaterials(Action<ICollection<Material>, Exception> callback)
        {
            if (_materials == null) _materials = _packlisteContext.Materials.ToList();
            callback(_materials, null);
        }

        public void SaveData()
        {
            //_packlisteContext.Materials = _materials as DbSet<Material>;
            //_packlisteContext.SaveChanges();
            //_packlisteContext.Materials.AddRange(_materials);
            //_packlisteContext.Materials.AddOrUpdate(_materials.ToArray());
            //_packlisteContext.ChangeTracker.DetectChanges();
            //_packlisteContext.Materials.AddOrUpdate();
        }


        public void Dispose()
        {
            _packlisteContext?.Dispose();
        }
    }
}