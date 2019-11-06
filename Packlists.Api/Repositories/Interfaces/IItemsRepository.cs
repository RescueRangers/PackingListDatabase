using System.Collections.Generic;
using System.Threading.Tasks;
using Packilists.Shared.Data;

namespace Packlists.Api.Repositories.Interfaces
{
    public interface IItemsRepository
    {
        Task<bool> Delete(int id);
        Task<IEnumerable<Item>> Get();
        Task<Item> GetById(int id);
        Task<bool> Insert(Item item);
        Task<bool> Update(int id, Item item);
    }
}