using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Linq;
using GalaSoft.MvvmLight.Messaging;
using Org.BouncyCastle.Asn1.X509.Qualified;
using Packlists.Messages;
using Z.EntityFramework.Extensions;

namespace Packlists.Model
{
    public sealed class DataService : IDataService, IDisposable
    {
        private ObservableCollection<Packliste> _packlists;
        private ObservableCollection<Item> _items;

        private ObservableCollection<Material> _materials;
        //private ObservableCollection<MaterialWithUsage> _materialWithUsage;
        private readonly PacklisteContext _packlisteContext;


        public DataService()
        {
            _packlisteContext = new PacklisteContext();
            _packlists = new ObservableCollection<Packliste>(_packlisteContext.Packlistes.Include(i => i.Items));
            _items = new ObservableCollection<Item>(_packlisteContext.Items.Include(m => m.Materials));
            _materials = new ObservableCollection<Material>(_packlisteContext.Materials);
        }

        
        public void GetPacklists(Action<ICollection<Packliste>, ICollection<Item>, Exception> callback)
        {
            if (_packlists == null)
            {
                _packlists = new ObservableCollection<Packliste>(_packlisteContext.Packlistes.Include(i => i.Items));
            }

            if (_items == null)
            {
                _items = new ObservableCollection<Item>(_packlisteContext.Items.Include(m => m.Materials));
            }

            callback(_packlists, _items, null);
        }

        public void GetItems(Action<ICollection<Item>, ICollection<Material>, Exception> callback)
        {
            if (_items == null)
            {
                _items = new ObservableCollection<Item>(_packlisteContext.Items.Include(m => m.Materials));
            }

            if (_materials == null)
            {
                _materials = new ObservableCollection<Material>(_packlisteContext.Materials);
            }

            callback(_items, _materials, null);
        }

        public void AddItems(IEnumerable<Item> items)
        {
            _packlisteContext.BulkInsert(items, options => options.IncludeGraph = true);
            //_packlisteContext.BulkMerge(items, options => options.IncludeGraph = true);
            //_packlisteContext.Items.AddOrUpdate(items.ToArray());
            _items = new ObservableCollection<Item>(_packlisteContext.Items.Include("Materials.Material"));
            _materials = new ObservableCollection<Material>(_packlisteContext.Materials);
            Messenger.Default.Send(new UpdateItemsModelMessage());
        }

        public void SaveData()
        {
            //if (sender.ToString().Contains("Materials"))
            //{
                
            //}
            //if (sender.ToString().Contains("Items"))
            //{
                
            //}
            //if (sender.ToString().Contains("Main"))
            //{
                
            //}

            var deletedMaterials = _packlisteContext.Materials.ToArray().Except(_materials).ToArray();
            var deletedItems = _packlisteContext.Items.ToArray().Except(_items).ToArray();
            //var deletedMaterialsWithUsag = _packlisteContext.MaterialsWithUsage.Except(_materialWithUsage);
            var deletedPacklists = _packlisteContext.Packlistes.ToArray().Except(_packlists).ToArray();

            _packlisteContext.Materials.RemoveRange(deletedMaterials);
            _packlisteContext.Items.RemoveRange(deletedItems);
            //_packlisteContext.MaterialsWithUsage.RemoveRange(deletedMaterialsWithUsag);
            _packlisteContext.Packlistes.RemoveRange(deletedPacklists);

            var changes = _packlisteContext.SaveChangesAsync();

            Console.WriteLine(changes);

        }


        /// <inheritdoc />
        /// <summary>
        /// Adds an object to the database
        /// </summary>
        /// <param name="obj">Object can be an Item or a Material</param>
        public void Add(object obj)
        {
            if (obj == null) return;

            var type = obj.GetType();

            if (type == typeof(Item))
            {
                var item = (Item) obj;

                _packlisteContext.Items.Add(item);
                _items.Add(item);
            }
            if (type == typeof(Material))
            {
                var material = (Material) obj;
                _packlisteContext.Materials.Add(material);
                _materials.Add(material);
            }

        }

        public void Dispose()
        {
            _packlisteContext.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}