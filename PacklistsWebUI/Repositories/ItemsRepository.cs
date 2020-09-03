using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Packilists.Shared.Data;
using PacklistsWebUI.Repositories.Interfaces;

namespace PacklistsWebUI.Repositories
{
    public class ItemsRepository : RepositoryBase, IItemsRepository
    {
        public ItemsRepository(Microsoft.Extensions.Configuration.IConfiguration config) : base(config)
        { }

        public async Task<IEnumerable<Item>> Get()
        {
            using var db = Connection;
            const string sql = @"SELECT ItemName, ItemId, IsEmpty FROM [Items]";

            return await db.QueryAsync<Item>(sql).ConfigureAwait(false);
        }

        public async Task<Item> GetById(int id)
        {
            using var db = Connection;

            var item = await db.QueryFirstOrDefaultAsync<Item>("SELECT * FROM Items WHERE ItemId = @Id", new { id }).ConfigureAwait(false);

            var materials = await db.QueryAsync<MaterialAmount>("SELECT * FROM MaterialAmount_View WHERE ItemId = @Id", new { id }).ConfigureAwait(false);

            if (materials == null)
            {
                return item;
            }

            item.Materials = materials.ToList();
            return item;
        }

        public async Task<bool> Insert(Item item)
        {
            using var db = Connection;
            const string sql = @"INSERT INTO [Items] (ItemName, IsEmpty) VALUES (@ItemName, @IsEmpty)";

            var result = await db.ExecuteAsync(sql, new { item.ItemName, item.IsEmpty }).ConfigureAwait(false);

            return result != 0;
        }

        public async Task<bool> Update(int id, Item item)
        {
            using var db = Connection;
            const string sql = @"UPDATE [Items] SET ItemName = @ItemName, IsEmpty = @IsEmpty WHERE ItemId = @ItemId";

            var result = await db.ExecuteAsync(sql, new { item.ItemName, item.IsEmpty, ItemId = id }).ConfigureAwait(false);

            return result != 0;
        }

        public async Task<bool> Delete(int id)
        {
            using var db = Connection;
            const string sql = @"DELETE FROM [Items] WHERE ItemId = @ItemId";

            var result = await db.ExecuteAsync(sql, new { ItemId = id }).ConfigureAwait(false);

            return result != 0;
        }
    }
}