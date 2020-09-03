using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Packilists.Shared.Data;
using PacklistsWebUI.Repositories.Interfaces;
using Syncfusion.Blazor;
using Syncfusion.Blazor.Data;

namespace PacklistsWebUI.DataAdaptors
{
    public class MaterialAmountAdaptor : DataAdaptor
    {
        private IMaterialsRepository _materialsRepository;

        public MaterialAmountAdaptor(IMaterialsRepository materialsRepository)
        {
            _materialsRepository = materialsRepository;
        }

        public override async Task<object> ReadAsync(DataManagerRequest dm, string key = null)
        {
            var hasKey = dm.Params.TryGetValue("ItemId", out var itemId);

            if (!hasKey) return null;

            var items = await _materialsRepository.GetItemMaterial(Convert.ToInt32(itemId)).ConfigureAwait(false);
            if (dm.Sorted != null)
            {
                items = DataOperations.PerformSorting(items, dm.Sorted);
            }
            if (dm.Search != null)
            {
                items = DataOperations.PerformSearching(items, dm.Search);
            }
            return dm.RequiresCounts ? new DataResult { Result = items, Count = items.Count() } : (object)items;
        }
    }
}
