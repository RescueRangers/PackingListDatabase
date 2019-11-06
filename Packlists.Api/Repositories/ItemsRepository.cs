using Dapper;
using Packilists.Shared.Data;
using Packlists.Api.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Packlists.Api.Repositories
{
    public class ItemsRepository : RepositoryBase, IItemsRepository
    {
        public ItemsRepository(Microsoft.Extensions.Configuration.IConfiguration config) : base(config)
        { }

        public async Task<IEnumerable<Item>> Get()
        {
            using var db = Connection;
            const string sql = @"SELECT ItemName, ItemId FROM [Items]";

            return await db.QueryAsync<Item>(sql);
        }

        public async Task<Item> GetById(int id)
        {
            using var db = Connection;
            const string sql = @"
SELECT A.[ItemId]
      ,A.[ItemName]
	  ,B.MaterialAmountId
	  ,B.Amount
	  ,B.Material_MaterialId
	  ,B.Item_ItemId
      ,C.MaterialId
	  ,C.MaterialName
	  ,C.Unit
  FROM [dbo].[Items] A
  INNER JOIN [dbo].[MaterialAmounts] B
  ON B.Item_ItemId = A.ItemId
  INNER JOIN [dbo].Materials C
  ON C.MaterialId = B.Material_MaterialId
  WHERE A.ItemId = @ItemId";

            var lookup = new Dictionary<int, Item>();

            var list = await db.QueryAsync<Item, MaterialAmount, Material, Item>(sql, (item, materialAmount, material) =>
            {
                Item itemEntry;
                if (!lookup.TryGetValue(item.ItemId, out itemEntry))
                {
                    itemEntry = item;
                    itemEntry.Materials = new List<MaterialAmount>();
                    lookup.Add(item.ItemId, itemEntry);
                }

                if (materialAmount.Material == null)
                {
                    materialAmount.Material = material;
                }

                itemEntry.Materials.Add(materialAmount);

               return itemEntry;
            }, new { ItemId = id }, splitOn: "ItemId, MaterialAmountId, MaterialId");

            return list.FirstOrDefault();
        }

        public async Task<bool> Insert(Item item)
        {
            using var db = Connection;
            const string sql = @"INSERT INTO [Items] (ItemName) VALUES (@ItemName)";

            var result = await db.ExecuteAsync(sql, new { ItemName = item.ItemName });

            return result != 0;
        }

        public async Task<bool> Update(int id, Item item)
        {
            using var db = Connection;
            const string sql = @"UPDATE [Items] SET ItemName = @ItemName WHERE ItemId = @ItemId";

            var result = await db.ExecuteAsync(sql, new { ItemName = item.ItemName, ItemId = id });

            return result != 0;
        }

        public async Task<bool> Delete(int id)
        {
            using var db = Connection;
            const string sql = @"DELETE FROM [Items] WHERE ItemId = @ItemId";

            var result = await db.ExecuteAsync(sql, new { ItemId = id });

            return result != 0;
        }
    }
}
