using System;
using System.Collections.Generic;
using System.Data;

namespace Packlists.Model
{
    public interface IDataService
    {
        void GetPacklists(Action<ICollection<Packliste>, ICollection<Item>, Exception> callback);
        void GetItems(Action<ICollection<Item>, ICollection<Material>, Exception> callback);
        void AddItems(IEnumerable<Item> items);
        void GetImports(Action<ICollection<ImportTransport>, ICollection<Material>, Exception> callback);
        void GetItemsWithQty(Action<ICollection<ItemWithQty>, Exception> callback);
        void SaveData();
        void CreateMonthlyReport(Action<DataTable, Exception> callback, DateTime month);
        
        /// <summary>
        /// Adds an object to the database
        /// </summary>
        /// <param name="obj">Object can be an Item, Material, Packliste, Import or COC</param>
        void Add(object obj);
        void BulkAdd(object obj);


        void GetCOCs(Action<ICollection<COC>, Exception> callback);
    }
}