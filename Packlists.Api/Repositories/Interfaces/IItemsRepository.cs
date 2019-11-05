using System.Collections.Generic;
using System.Threading.Tasks;
using Packilists.Shared.Data;

namespace Packlists.Api.Repositories.Interfaces
{
    public interface IItemsRepository
    {
        Task Delete(int id);
        Task<IEnumerable<Item>> Get();
        Task<Item> GetById(int id);
        Task Insert(Item item);
        Task Update(Item item);
    }
}