using System.Collections.Generic;
using System.Threading.Tasks;
using Packilists.Shared.Data;

namespace Packlists.Api.Repositories.Interfaces
{
    public interface ICocRepository
    {
        Task<bool> Delete(int id);
        Task<IEnumerable<COC>> Get();
        Task<COC> GetById(int id);
        Task<bool> Insert(COC coc);
        Task<bool> Update(int id, COC coc);
    }
}