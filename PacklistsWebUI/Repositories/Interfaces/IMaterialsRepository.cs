using System.Collections.Generic;
using System.Threading.Tasks;
using Packilists.Shared.Data;

namespace PacklistsWebUI.Repositories.Interfaces
{
    public interface IMaterialsRepository
    {
        Task<bool> Delete(int id);

        Task<IEnumerable<Material>> Get();

        Task<Material> GetById(int id);

        Task<bool> Insert(Material material);

        Task<bool> Update(int id, Material material);

        Task<bool> InsertMaterialAmount(MaterialAmount material);
        Task<IEnumerable<MaterialAmount>> GetItemMaterial(int itemId);
        Task<IEnumerable<MaterialAmount>> GetRawUsage(int packlisteId);
    }
}