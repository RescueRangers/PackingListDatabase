using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Packilists.Shared.Data;
using PacklistsWebUI.Repositories.Interfaces;
using Syncfusion.Blazor;
using Syncfusion.Blazor.Data;

namespace PacklistsWebUI.DataAdaptors
{
    public class PackingListAdaptor : DataAdaptor
    {
        private IPackingListsRepository _packingListsRepository;

        public PackingListAdaptor(IPackingListsRepository packingListsRepository)
        {
            _packingListsRepository = packingListsRepository;
        }

        public override async Task<object> RemoveAsync(DataManager dataManager, object data, string keyField, string key)
        {
            if(data is int id)
            {
                return await _packingListsRepository.Delete(id).ConfigureAwait(false);
            }
            return false;
        }

        public override async Task<object> ReadAsync(DataManagerRequest dm, string key = null)
        {
            var hasKey = dm.Params.TryGetValue("Month", out var packingMonth);

            if (!hasKey) return null;

            var packingLists = await _packingListsRepository.GetByMonth((DateTime)packingMonth).ConfigureAwait(false);
            var count = packingLists.Count();
            IEnumerable dataSource = packingLists;

            var dataObject = new DataResult();

            if (dm.Take != 0)
            {
                dataSource = DataOperations.PerformTake(dataSource, dm.Take);
            }
            if (dm.Group != null)
            {
                foreach (var group in dm.Group)
                {
                    dataSource = DataUtil.Group<Packliste>(dataSource, group, dm.Aggregates, 0, dm.GroupByFormatter);
                }

                dataObject.Result = dataSource;
                dataObject.Count = count;

                return dm.RequiresCounts ? dataObject : (object)dataSource;
            }
            if (dm.Sorted != null)
            {
                dataSource = DataOperations.PerformSorting(dataSource, dm.Sorted);
            }
            if (dm.Search != null)
            {
                dataSource = DataOperations.PerformSearching(dataSource, dm.Search);
            }
            if (dm.Where != null)
            {
                foreach (var whereFilter in dm.Where)
                {
                    dataSource = DataOperations.PerformFiltering(dataSource, whereFilter.predicates, whereFilter.Condition);
                }
            }

            return dm.RequiresCounts ? new DataResult { Result = dataSource, Count = count } : (object)dataSource;
        }
    }
}
