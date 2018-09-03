using System;
using System.Collections.Generic;
using System.Linq;
using PacklistDatabase.Model;

namespace PacklistDatabase.Design
{
    public class DesignDataService : IDataService
    {
        public void GetYearsAndItems(Action<ICollection<Year>, ICollection<Item>, Exception> callback)
        {
            // Use this to create design time data

            var years = new List<Year>(new Year[]{new Year{YearNumber = 2017}, new Year{YearNumber = 2018}});
            var items = new List<Item>();
            callback(years, items, null);
        }

        public void GetItemsAndMaterials(Action<ICollection<Item>, ICollection<Material>, Exception> callback)
        {
            throw new NotImplementedException();
        }

        public void GetMaterials(Action<ICollection<Material>, Exception> callback)
        {
            throw new NotImplementedException();
        }

        
        public void SaveData()
        {
            throw new NotImplementedException();
        }
    }
}