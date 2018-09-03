using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Packlists.Model
{
    public interface IDataService
    {
        void GetPacklists(Action<ICollection<Packliste>, ICollection<Item>, Exception> callback);
        void GetItems(Action<ICollection<Item>, ICollection<Material>, Exception> callback);
        void AddItems(IEnumerable<Item> items);
        void SaveData();
        
        /// <summary>
        /// Adds an object to the database.
        /// </summary>
        /// <param name="obj">obj can be an Item or a Material</param>
        void Add(object obj);
    }
}