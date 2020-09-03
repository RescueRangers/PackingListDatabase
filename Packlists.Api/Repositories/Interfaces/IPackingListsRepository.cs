using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Packilists.Shared.Data;

namespace Packlists.Api.Repositories.Interfaces
{
    public interface IPackingListsRepository
    {
        Task<bool> Delete(int id);
        Task<IEnumerable<Packliste>> Get();
        Task<Packliste> GetById(int id);
        Task<bool> Insert(Packliste packliste);
        Task<bool> Update(int id, Packliste packliste);
        Task<IEnumerable<PacklisteData>> GetPacklisteData(int id);
        Task InsertPacklisteData(int id, List<PacklisteData> packlisteDatas);
        Task<IEnumerable<Packliste>> GetByMonth(DateTime month);
        Task<IEnumerable<Packliste>> GetByPacklisteNumber(int packlisteNumber);
        Task<IEnumerable<ItemWithQty>> GetPacklisteItems(int id);
    }
}