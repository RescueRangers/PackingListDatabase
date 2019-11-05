using System.Collections.Generic;
using System.Threading.Tasks;
using Packilists.Shared.Data;

namespace Packlists.Api.Repositories.Interfaces
{
    public interface IMaterialsRepository
    {
        Task Delete(Material material);
        Task<IEnumerable<Material>> Get();
        Task<Material> GetById(int id);
        Task Insert(Material material);
        Task Update(Material material);
    }
}