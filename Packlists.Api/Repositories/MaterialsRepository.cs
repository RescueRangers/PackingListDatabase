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

        public async Task Insert(Material material)
        {
            // Insert
            using var db = Connection;
            const string sql = @"INSERT INTO [Materials] (Unit, MaterialName) VALUES (@Unit, @MaterialName)";

            await db.ExecuteAsync(sql, new { Unit = material.Unit, MaterialName = material.MaterialName }).ConfigureAwait(false);
        }

        public async Task Update(Material material)
        {
            // Update
            using (var db = Connection)
            {
                const string sql = @"UPDATE [Materials] SET Unit = @Unit, MaterialName = @MaterialName WHERE MaterialId = @MaterialId";

                await db.ExecuteAsync(sql, new { Unit = material.Unit, MaterialName = material.MaterialName, MaterialId = material.MaterialId }).ConfigureAwait(false);
            }
        }

        public async Task Delete(Material material)
        {
            // Delete
            using (var db = Connection)
            {
                const string sql = @"DELETE FROM [Materials] WHERE MaterialId = @MaterialId";

                await db.ExecuteAsync(sql, new { material.Unit }).ConfigureAwait(false);
            }
        }
    }
}
