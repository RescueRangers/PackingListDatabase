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
        private ObservableCollection<Packliste> _packlists;
        private ObservableCollection<Item> _items;
        private ObservableCollection<ImportTransport> _imports;
        private ObservableCollection<Material> _materials;
        private ObservableCollection<COC> _cocs;
        private ObservableCollection<ItemWithQty> _itemsWithQty;
        private readonly PacklisteContext _packlisteContext;


        public DataService()
        {
            _packlisteContext = new PacklisteContext();
            _packlists = new ObservableCollection<Packliste>(_packlisteContext.Packlistes.Include(i => i.ItemsWithQties).Include(r => r.RawUsage));
            _items = new ObservableCollection<Item>(_packlisteContext.Items.Include(m => m.Materials));
            _materials = new ObservableCollection<Material>(_packlisteContext.Materials);
            _imports = new ObservableCollection<ImportTransport>(_packlisteContext.ImportTransports.Include(m => m.ImportedMaterials));
            _cocs = new ObservableCollection<COC>(_packlisteContext.Cocs.Include(i => i.Item));
            _itemsWithQty = new ObservableCollection<ItemWithQty>(_packlisteContext.ItemWithQties.Include(i => i.Item));
        }

        
        public void GetPacklists(Action<ICollection<Packliste>, ICollection<Item>, Exception> callback)
        {
            if (_packlists == null)
            {
                _packlists = new ObservableCollection<Packliste>(_packlisteContext.Packlistes.Include(i => i.ItemsWithQties).Include(r => r.RawUsage));
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

        public void GetItemsWithQty(Action<ICollection<ItemWithQty>, Exception> callback)
        {
            if (_itemsWithQty == null || !_itemsWithQty.Any())
            {
                _itemsWithQty = new ObservableCollection<ItemWithQty>(_packlisteContext.ItemWithQties);
            }

            callback(_itemsWithQty, null);
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

        public void CreateMonthlyReport(Action<ListCollectionView, Exception> callback, DateTime month)
        {
            var days = GetDaysInMonth(month);
            
            foreach (var t in days)
            {
                var day = t.Date;

                var imports = _imports.Where(im => im.ImportDate == day).SelectMany(s => s.ImportedMaterials)
                    .GroupBy(g => g.Material).Select(g => new MaterialAmount
                        {Material = g.Key, Amount = g.Sum(s => s.Amount)}).ToList();
                var exports = _packlists.Where(pac => pac.PacklisteDate == day)
                    .SelectMany(s => s.ItemsWithQties.SelectMany(itm => itm.Item.Materials)).GroupBy(g => g.Material)
                    .Select(g => new MaterialAmount
                        {Material = g.Key, Amount = g.Sum(s => s.Amount)}).ToList();

                for (var j = 0; j < _materials.Count; j++)
                {
                    var material = _materials[j];
                    var importedMaterial = imports.SingleOrDefault(s => s.Material == material);
                    var exportedMaterial = exports.SingleOrDefault(s => s.Material == material);

                    t.NetMaterialCount.Add(importedMaterial ?? new MaterialAmount {Material = material, Amount = 0});

                    if (exportedMaterial != null)
                    {
                        t.NetMaterialCount[j].Amount -= exportedMaterial.Amount;
                    }
                }
            }

            for (var i = 1; i < days.Count; i++)
            {
                var day = days[i];
                var previousDay = days[i - 1];

                for (var index = 0; index < day.NetMaterialCount.Count; index++)
                {
                    day.NetMaterialCount[index].Amount += previousDay.NetMaterialCount[index].Amount;
                }
            }


            var result = new DataTable($"{month:yyyy-MM-dd}");
            var column = new DataColumn("Materials")
                {DataType = typeof(string), Unique = true, AutoIncrement = false, ReadOnly = true};
            result.Columns.Add(column);
            column = new DataColumn("Unit")
            {
                DataType = typeof(string),
                Unique = false,
                AutoIncrement = false,
                ReadOnly = true
            };
            result.Columns.Add(column);

            foreach (var day in days)
            {
                column = new DataColumn($"{day.Date:yyyy-MM-dd}")
                {
                    DataType = typeof(float),
                    Unique = false,
                    AutoIncrement = false,
                    ReadOnly = true
                };
                result.Columns.Add(column);
            }

            foreach (var material in _materials)
            {
                var row = result.NewRow();
                row["Materials"] = material.MaterialName;
                row["Unit"] = material.Unit;
                
                foreach (var day in days)
                {
                    row[$"{day.Date:yyyy-MM-dd}"] = day.NetMaterialCount.Single(s => s.Material == material).Amount;
                }

                var rows = row.ItemArray.Where(o => o is float).Cast<float>().Where(fl => fl.Equals(0)).ToList();

                var count = rows.Count;

                if (count == result.Columns.Count - 2) continue;

                result.Rows.Add(row);
            }

            var tempListview = new ListCollectionView(result.DefaultView);


            callback(tempListview, null);
        }

        private List<Day> GetDaysInMonth(DateTime month)
        {
            var days = new List<Day>();

            for (var i = 1; i <= DateTime.DaysInMonth(month.Year, month.Month); i++)
            {
                days.Add(new Day{Date = new DateTime(month.Year, month.Month, i)});
            }

            return days;
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
            if (type == typeof(COC))
            {
                var coc = (COC) obj;

                _packlisteContext.Cocs.Add(coc);
                _packlisteContext.SaveChanges();

                Application.Current.Dispatcher.Invoke(() =>
                {
                    _cocs.Add(coc);
                });
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
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        _cocs.Add(coc);
                    });
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
        
        public void GetCOCs(Action<ICollection<COC>, Exception> callback)
        {
            if (_cocs == null)
            {
                _cocs = new ObservableCollection<COC>(_packlisteContext.Cocs.Include(i => i.Item));
            }

            callback(_cocs, null);
        }

        public void Dispose()
        {
            _packlisteContext.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}