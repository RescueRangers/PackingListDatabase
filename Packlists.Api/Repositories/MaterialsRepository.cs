using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Microsoft.Extensions.Configuration;
using Packilists.Shared.Data;
using Packlists.Api.Repositories.Interfaces;

namespace Packlists.Api.Repositories
{
    public class MaterialsRepository : RepositoryBase, IMaterialsRepository
    {
        public MaterialsRepository(IConfiguration config) : base(config)
        { }

        public async Task<IEnumerable<Material>> Get()
        {
            // Select
            using var db = Connection;
            const string sql = @"SELECT Unit, MaterialName, MaterialId FROM [Materials]";

            return await db.QueryAsync<Material>(sql).ConfigureAwait(false);
        }

        public async Task<Material> GetById(int id)
        {
            // Select
            using var db = Connection;
            const string sql = @"SELECT Unit, MaterialName, MaterialId FROM [Materials] WHERE MaterialId = @MaterialId";

            return await db.QueryFirstOrDefaultAsync<Material>(sql, new { MaterialId = id }).ConfigureAwait(false);
        }

        public async Task<bool> Insert(Material material)
        {
            // Insert
            using var db = Connection;
            const string sql = @"INSERT INTO [Materials] (Unit, MaterialName) VALUES (@Unit, @MaterialName)";

            var result = await db.ExecuteAsync(sql, new { Unit = material.Unit, MaterialName = material.MaterialName }).ConfigureAwait(false);

            return result != 0;
        }

        public async Task<bool> InsertMaterialAmount(MaterialAmount material)
        {
            using var db = Connection;
            const string sql = @"INSERT INTO [MaterialAmounts] (Amount, Material_MaterialId, Item_ItemId) VALUES (@Amount, @MaterialId, @ItemId)";

            var result = await db.ExecuteAsync(sql, new { material.Amount, material.MaterialId, material.ItemId }).ConfigureAwait(false);

            return result != 0;
        }

        public async Task<bool> Update(int id, Material material)
        {
            // Update
            using (var db = Connection)
            {
                const string sql = @"UPDATE [Materials] SET Unit = @Unit, MaterialName = @MaterialName WHERE MaterialId = @MaterialId";

                var result = await db.ExecuteAsync(sql, new { Unit = material.Unit, MaterialName = material.MaterialName, MaterialId = id }).ConfigureAwait(false);

                return result != 0;
            }
        }

        public async Task<bool> Delete(int id)
        {
            // Delete
            using (var db = Connection)
            {
                const string sql = @"DELETE FROM [Materials] WHERE MaterialId = @MaterialId";

                var result = await db.ExecuteAsync(sql, new { MaterialId = id }).ConfigureAwait(false);

                return result != 0;
            }
        }
    }
}
