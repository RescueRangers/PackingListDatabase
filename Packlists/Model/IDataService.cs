using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Packlists.Model
{
    public interface IDataService
    {
        void GetPacklists(Action<ICollection<Packliste>, Exception> callback, DateTime month);

        Task<List<Packliste>> GetPacklistsWithoutData(DateTime month);

        void GetItems(Action<ICollection<Item>> callback);

        void GetItems(Action<ICollection<Item>, ICollection<Material>, Exception> callback);

        void GetImports(Action<ICollection<ImportTransport>, Exception> callback, DateTime month);

        Task<List<ImportTransport>> GetImports(DateTime month);

        void GetItemsWithQty(Action<ICollection<ItemWithQty>, Exception> callback);

        void GetMaterials(Action<ICollection<Material>> callback);

        void SaveData();

        void CreateMonthlyReport(Action<MonthlyUsageReport, Exception> callback, DateTime month);

        /// <summary>
        /// Adds an object to the database
        /// </summary>
        /// <param name="obj">Object can be an Item, Material, Packliste, Import or COC</param>
        void Add(object obj);

        void BulkAdd(object obj);

        void GetCOCs(Action<ICollection<COC>, Exception> callback, DateTime month);
    }
}