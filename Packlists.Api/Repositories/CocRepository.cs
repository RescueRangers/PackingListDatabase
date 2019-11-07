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
    public class CocRepository : RepositoryBase, ICocRepository
    {
        public CocRepository(IConfiguration config) : base(config)
        { }

        public async Task<IEnumerable<COC>> Get()
        {
            // Select
            using (var db = Connection)
            {
                const string sql = @"
SELECT [CocId]
      ,[InventoryDate]
      ,[CocNumber]
      ,[ItemWithQtyId]
      ,[Quantity]
      ,[ItemId]
      ,[ItemName]
  FROM [db_owner].[COC_View]";

                return await db.QueryAsync<COC>(sql).ConfigureAwait(false);
            }
        }

        public async Task<COC> GetById(int id)
        {
            // Select
            using (var db = Connection)
            {
                const string sql = @"
SELECT [CocId]
      ,[InventoryDate]
      ,[CocNumber]
      ,[ItemWithQtyId]
      ,[Quantity]
      ,[ItemId]
      ,[ItemName]
  FROM [db_owner].[COC_View]
  WHERE [CocId] = @CocId";

                return await db.QueryFirstOrDefaultAsync<COC>(sql, new { CocId = id }).ConfigureAwait(false);
            }
        }

        public async Task<bool> Insert(COC coc)
        {
            // Insert
            using (var db = Connection)
            {
                const string sql = @"INSERT INTO [COCs] (InventoryDate, CocNumber) VALUES (@InventoryDate, @CocNumber)";

                var result = await db.ExecuteAsync(sql, new { InventoryDate = coc.InventoryDate, CocNumber = coc.CocNumber, CocId = coc.CocId }).ConfigureAwait(false);

                return result != 0;
            }
        }

        public async Task<bool> Update(int id, COC coc)
        {
            // Update
            using (var db = Connection)
            {
                const string sql = @"UPDATE [COCs] SET InventoryDate = @InventoryDate, CocNumber = @CocNumber WHERE CocId = @CocId";

                var result = await db.ExecuteAsync(sql, new { InventoryDate = coc.InventoryDate, CocNumber = coc.CocNumber, CocId = id }).ConfigureAwait(false);

                return result != 0;
            }
        }

        public async Task<bool> Delete(int id)
        {
            // Delete
            using (var db = Connection)
            {
                const string sql = @"DELETE FROM [COCs] WHERE CocId = @CocId";

                var result = await db.ExecuteAsync(sql, new { CocId = id }).ConfigureAwait(false);

                return result != 0;
            }
        }
    }
}
