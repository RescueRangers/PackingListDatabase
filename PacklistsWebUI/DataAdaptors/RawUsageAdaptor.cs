using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PacklistsWebUI.Repositories.Interfaces;
using Syncfusion.Blazor;
using Syncfusion.Blazor.Data;

namespace PacklistsWebUI.DataAdaptors
{
    public class RawUsageAdaptor : DataAdaptor
    {
        private IMaterialsRepository _materialsRepository;

        public RawUsageAdaptor(IMaterialsRepository materialsRepository)
        {
            _materialsRepository = materialsRepository;
        }

        public override async Task<object> ReadAsync(DataManagerRequest dm, string key = null)
        {
            object packlisteId = 0;
            var hasKey = dm?.Params?.TryGetValue("PacklisteId", out packlisteId);

            if (!hasKey.HasValue) return null;

            var items = await _materialsRepository.GetRawUsage(Convert.ToInt32(packlisteId)).ConfigureAwait(false);
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
