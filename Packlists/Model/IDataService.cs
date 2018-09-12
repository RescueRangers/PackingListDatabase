using System;
using System.Collections.Generic;

namespace Packlists.Model
{
    public interface IDataService
    {
        void GetPacklists(Action<ICollection<Packliste>, ICollection<Item>, Exception> callback);
        void GetItems(Action<ICollection<Item>, ICollection<Material>, Exception> callback);
        void AddItems(IEnumerable<Item> items);
        void GetImports(Action<ICollection<ImportTransport>, ICollection<Material>, Exception> callback);
        void SaveData();
        
        /// <summary>
        /// Adds an object to the database
        /// </summary>
        /// <param name="obj">Object can be an Item, Material, Packliste or Import</param>
        void Add(object obj);
    }
}