using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Transactions;
using System.Windows;
using System.Windows.Data;
using GalaSoft.MvvmLight.Messaging;
using OfficeOpenXml.FormulaParsing.Utilities;
using Packlists.Messages;

namespace Packlists.Model
{
    public sealed class DataService : IDataService, IDisposable
    {
        //private ObservableCollection<Packliste> _packlists;
        private ObservableCollection<Item> _items;
        //private ObservableCollection<ImportTransport> _imports;
        private ObservableCollection<Material> _materials;
        //private ObservableCollection<COC> _cocs;
        private ObservableCollection<ItemWithQty> _itemsWithQty;
        private readonly PacklisteContext _packlisteContext;


        public DataService()
        {
            _packlisteContext = new PacklisteContext();
            //_packlists = new ObservableCollection<Packliste>(_packlisteContext.Packlistes.Include(i => i.ItemsWithQties).Include(r => r.RawUsage));
            _items = new ObservableCollection<Item>(_packlisteContext.Items.Include(m => m.Materials));
            _materials = new ObservableCollection<Material>(_packlisteContext.Materials);
            //_imports = new ObservableCollection<ImportTransport>(_packlisteContext.ImportTransports.Include(m => m.ImportedMaterials));
            //_cocs = new ObservableCollection<COC>(_packlisteContext.Cocs.Include(i => i.Item));
            _itemsWithQty = new ObservableCollection<ItemWithQty>(_packlisteContext.ItemWithQties.Include(i => i.Item));
        }

        
        public void GetPacklists(Action<ICollection<Packliste>, Exception> callback, DateTime month)
        {
            _packlisteContext.Packlistes.Where(p => p.PacklisteDate.Year == month.Year && p.PacklisteDate.Month == month.Month).Include(i => i.ItemsWithQties).Include(r => r.RawUsage).Load();

            callback(_packlisteContext.Packlistes.Local, null);
        }

        public void GetItems(Action<ICollection<Item>> callback)
        {
            if (_items == null)
            {
                _items = new ObservableCollection<Item>(_packlisteContext.Items.Include(m => m.Materials));
            }

            callback(_items);
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

        public void GetImports(Action<ICollection<ImportTransport>, Exception> callback, DateTime month)
        {
            //var imports = new ObservableCollection<ImportTransport>(_packlisteContext.ImportTransports.Where(i =>
            //    i.ImportDate.Year == month.Year && i.ImportDate.Month == month.Month).Include(m => m.ImportedMaterials).ToList());

            _packlisteContext.ImportTransports.Where(i =>
                i.ImportDate.Year == month.Year && i.ImportDate.Month == month.Month).Include(m => m.ImportedMaterials).Load();

            callback(_packlisteContext.ImportTransports.Local, null);
        }

        public void GetItemsWithQty(Action<ICollection<ItemWithQty>, Exception> callback)
        {
            if (_itemsWithQty == null || !_itemsWithQty.Any())
            {
                _itemsWithQty = new ObservableCollection<ItemWithQty>(_packlisteContext.ItemWithQties);
            }

            callback(_itemsWithQty, null);
        }

        public void GetMaterials(Action<ICollection<Material>> callback)
        {
            callback(_materials);
        }

        public void SaveData()
        {
            var deletedMaterials = _packlisteContext.Materials.ToArray().Except(_materials).ToArray();
            var deletedItems = _packlisteContext.Items.ToArray().Except(_items).ToArray();
            //var deletedPacklists = _packlisteContext.Packlistes.ToArray().Except(_packlists).ToArray();
            //var deletedImports = _packlisteContext.ImportTransports.ToArray().Except(_imports).ToArray();

            _packlisteContext.Materials.RemoveRange(deletedMaterials);
            _packlisteContext.Items.RemoveRange(deletedItems);
            //_packlisteContext.Packlistes.RemoveRange(deletedPacklists);
            //_packlisteContext.ImportTransports.RemoveRange(deletedImports);

            var changes = _packlisteContext.SaveChanges();

            Console.WriteLine(changes);

        }

        public void CreateMonthlyReport(Action<MonthlyUsageReport, Exception> callback, DateTime month)
        {
            var imports = _packlisteContext.ImportTransports.Where(i =>
                i.ImportDate.Year == month.Year && i.ImportDate.Month == month.Month).Include(m => m.ImportedMaterials).ToList();
            var packlists = _packlisteContext.Packlistes.Where(i =>
                i.PacklisteDate.Year == month.Year && i.PacklisteDate.Month == month.Month).Include(i => i.ItemsWithQties).Include(r => r.RawUsage).ToList();

            var report = new MonthlyUsageReport(month, imports, packlists, _materials.ToList());

            callback(report, null);
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
                
                //Application.Current.Dispatcher.Invoke(() =>
                //{
                //    _packlists.Add(packliste);
                //});
            }
            if (type == typeof(ImportTransport))
            {
                var transport = (ImportTransport) obj;

                _packlisteContext.ImportTransports.Add(transport);
                _packlisteContext.SaveChanges();

                //Application.Current.Dispatcher.Invoke(() =>
                //{
                //    _imports.Add(transport);
                //});
            }
            if (type == typeof(COC))
            {
                var coc = (COC) obj;

                _packlisteContext.Cocs.Add(coc);
                _packlisteContext.SaveChanges();

                //Application.Current.Dispatcher.Invoke(() =>
                //{
                //    _cocs.Add(coc);
                //});
            }

        }

        public void BulkAdd(object obj)
        {
            if (obj == null) return;

            var type = obj.GetType();

            if (type == typeof(List<COC>))
            {
                SaveData();

                var cocs = (IEnumerable<COC>) obj;

                _packlisteContext.Configuration.AutoDetectChangesEnabled = false;

                foreach (var coc in cocs)
                {
                    _packlisteContext.Cocs.Add(coc);
                    //Application.Current.Dispatcher.Invoke(() =>
                    //{
                    //    _cocs.Add(coc);
                    //});
                }
                _packlisteContext.Configuration.AutoDetectChangesEnabled = true;
            }

            if (type == typeof(List<Item>))
            {
                SaveData();

                var items = (IEnumerable<Item>) obj;

                _packlisteContext.Configuration.AutoDetectChangesEnabled = false;

                foreach (var coc in items)
                {
                    _packlisteContext.Items.Add(coc);
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        _items.Add(coc);
                    });
                }
                _packlisteContext.Configuration.AutoDetectChangesEnabled = true;
            }
        }
        
        public void GetCOCs(Action<ICollection<COC>, Exception> callback, DateTime month)
        {
            _packlisteContext.Cocs.Where(i =>
                i.InventoryDate.Year == month.Year && i.InventoryDate.Month == month.Month).Include(i => i.Item).Load();

            callback(_packlisteContext.Cocs.Local, null);
        }

        public void Dispose()
        {
            _packlisteContext.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}