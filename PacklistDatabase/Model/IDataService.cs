using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PacklistDatabase.Model
{
    public interface IDataService
    {
        void GetYearsAndItems(Action<ICollection<Year>, ICollection<Item>, Exception> callback);
        void GetItemsAndMaterials(Action<ICollection<Item>, ICollection<Material>, Exception> callback);
        void GetMaterials(Action<ICollection<Material>, Exception> callback);

        void SaveData();
    }
}