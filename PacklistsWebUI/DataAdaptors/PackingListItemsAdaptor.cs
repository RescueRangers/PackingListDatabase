using System;
using System.Linq;
using System.Threading.Tasks;
using Packilists.Shared.Data;
using PacklistsWebUI.Repositories.Interfaces;
using Syncfusion.Blazor;
using Syncfusion.Blazor.Data;

namespace PacklistsWebUI.DataAdaptors
{
    public class PackingListItemsAdaptor : DataAdaptor
    {
        private IPackingListsRepository _packingListsRepository;

        public PackingListItemsAdaptor(IPackingListsRepository packingListsRepository)
        {
            _packingListsRepository = packingListsRepository;
        }

        public override async Task<object> ReadAsync(DataManagerRequest dm, string key = null)
        {
            var hasKey = dm.Params.TryGetValue("PacklisteId", out var packlisteId);

            if (!hasKey) return null;
            var items = await _packingListsRepository.GetPacklisteItems(Convert.ToInt32(packlisteId)).ConfigureAwait(false);

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