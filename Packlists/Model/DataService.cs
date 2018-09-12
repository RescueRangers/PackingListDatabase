using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.Linq;
using System.Windows;
using GalaSoft.MvvmLight.Messaging;
using Packlists.Messages;

namespace Packlists.Model
{
    public sealed class DataService : IDataService, IDisposable
    {
        private ObservableCollection<Packliste> _packlists;
        private ObservableCollection<Item> _items;
        private ObservableCollection<ImportTransport> _imports;

        private ObservableCollection<Material> _materials;
        //private ObservableCollection<MaterialWithUsage> _materialWithUsage;
        private readonly PacklisteContext _packlisteContext;


        public DataService()
        {
            _packlisteContext = new PacklisteContext();
            _packlists = new ObservableCollection<Packliste>(_packlisteContext.Packlistes.Include(i => i.ItemsWithQties));
            _items = new ObservableCollection<Item>(_packlisteContext.Items.Include(m => m.Materials));
            _materials = new ObservableCollection<Material>(_packlisteContext.Materials);
            _imports = new ObservableCollection<ImportTransport>(_packlisteContext.ImportTransports.Include(m => m.ImportedMaterials));
        }

        
        public void GetPacklists(Action<ICollection<Packliste>, ICollection<Item>, Exception> callback)
        {
            if (_packlists == null)
            {
                _packlists = new ObservableCollection<Packliste>(_packlisteContext.Packlistes.Include(i => i.ItemsWithQties));
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
            _items = new ObservableCollection<Item>(_packlisteContext.Items.Include("Materials.Material"));
            _materials = new ObservableCollection<Material>(_packlisteContext.Materials);
            Messenger.Default.Send(new UpdateItemsModelMessage());
        }

        public void GetImports(Action<ICollection<ImportTransport>, ICollection<Material>, Exception> callback)
        {
            if (_imports == null)
            {
                _imports = new ObservableCollection<ImportTransport>(_packlisteContext.ImportTransports.Include(m => m.ImportedMaterials));
            }

            if (_materials == null)
            {
                _materials = new ObservableCollection<Material>(_packlisteContext.Materials);
            }

            callback(_imports, _materials, null);
        }

        public void SaveData()
        {
            var deletedMaterials = _packlisteContext.Materials.ToArray().Except(_materials).ToArray();
            var deletedItems = _packlisteContext.Items.ToArray().Except(_items).ToArray();
            var deletedPacklists = _packlisteContext.Packlistes.ToArray().Except(_packlists).ToArray();
            var deletedImports = _packlisteContext.ImportTransports.ToArray().Except(_imports).ToArray();

            _packlisteContext.Materials.RemoveRange(deletedMaterials);
            _packlisteContext.Items.RemoveRange(deletedItems);
            _packlisteContext.Packlistes.RemoveRange(deletedPacklists);
            _packlisteContext.ImportTransports.RemoveRange(deletedImports);

            var changes = _packlisteContext.SaveChanges();

            Console.WriteLine(changes);

        }


        /// <inheritdoc />
        /// <summary>
        /// Adds an object to the database
        /// </summary>
        /// <param name="obj">Object can be an Item, Material, Packliste or Import</param>
        public void Add(object obj)
        {
            if (obj == null) return;

            var type = obj.GetType();

            if (type == typeof(Item))
            {
                var item = (Item) obj;
                _packlisteContext.Items.Add(item);
                _packlisteContext.SaveChanges();

                Application.Current.Dispatcher.Invoke(() =>
                {
                    _items.Add(item);
                });

                
            }
            if (type == typeof(Material))
            {
                var material = (Material) obj;
                _packlisteContext.Materials.Add(material);
                _packlisteContext.SaveChanges();
                
                Application.Current.Dispatcher.Invoke(() =>
                {
                    _materials.Add(material);
                });
            }
            if (type == typeof(Packliste))
            {
                var packliste = (Packliste) obj;

                _packlisteContext.Packlistes.Add(packliste);
                _packlisteContext.SaveChanges();
                
                Application.Current.Dispatcher.Invoke(() =>
                {
                    _packlists.Add(packliste);
                });
            }

            if (type == typeof(ImportTransport))
            {
                var transport = (ImportTransport) obj;

                _packlisteContext.ImportTransports.Add(transport);
                _packlisteContext.SaveChanges();

                Application.Current.Dispatcher.Invoke(() =>
                {
                    _imports.Add(transport);
                });
            }

        }

        public void Dispose()
        {
            _packlisteContext.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}