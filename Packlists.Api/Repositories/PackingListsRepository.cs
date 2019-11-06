﻿using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Dapper;
using Microsoft.Extensions.Configuration;
using Packilists.Shared.Data;
using Packlists.Api.Repositories.Interfaces;

namespace Packlists.Api.Repositories
{
    public class PackingListsRepository : RepositoryBase, IPackingListsRepository
    {
        public PackingListsRepository(IConfiguration config) : base(config)
        { }

        public async Task<IEnumerable<Packliste>> Get()
        {
            // Select
            using (var db = Connection)
            {
                const string sql = @"SELECT Destination, PacklisteDate, PacklisteNumber, PacklisteId FROM [Packlistes]";

                return await db.QueryAsync<Packliste>(sql).ConfigureAwait(false);
            }
        }

        public async Task<Packliste> GetById(int id)
        {
            // Select
            using (var db = Connection)
            {
                const string sql = @"
SELECT A.[PacklisteId]
      ,A.[PacklisteDate]
      ,A.[PacklisteNumber]
      ,A.[Destination]
	  ,B.ItemWithQtyId
	  ,B.Item_ItemId
	  ,B.Quantity
	  ,C.ItemId
	  ,C.ItemName
	  ,D.MaterialAmountId
	  ,D.Amount
	  ,E.MaterialId
	  ,E.MaterialName
	  ,E.Unit
  FROM [dbo].[Packlistes] A
  INNER JOIN dbo.ItemWithQties B on B.Packliste_PacklisteId = A.PacklisteId
  INNER JOIN dbo.Items C on C.ItemId = B.Item_ItemId
  INNER Join MaterialAmounts D on D.Packliste_PacklisteId = A.PacklisteId
  INNER Join Materials E on E.MaterialId = D.Material_MaterialId
  WHERE A.PacklisteId = @Id";

                var packlisteLookup = new Dictionary<int, Packliste>();
                var itemWithQtyLookup = new Dictionary<int, ItemWithQty>();
                var materialAmountLookup = new Dictionary<int, MaterialAmount>();

                var list = await db.QueryAsync<Packliste, ItemWithQty, Item, MaterialAmount, Material, Packliste>(sql, (packliste, itemWithQty, item, materialAmount, material) =>
                {
                    Packliste packlisteEntry;
                    if (!packlisteLookup.TryGetValue(packliste.PacklisteId, out packlisteEntry))
                    {
                        packlisteEntry = packliste;
                        packlisteEntry.ItemsWithQties = new List<ItemWithQty>();
                        packlisteEntry.RawUsage = new List<MaterialAmount>();
                        packlisteEntry.PacklisteData = new List<PacklisteData>();
                        packlisteLookup.Add(packliste.PacklisteId, packlisteEntry);
                    }

                    if (!itemWithQtyLookup.TryGetValue(itemWithQty.ItemWithQtyId, out ItemWithQty itemWithQtyEntry))
                    {
                        itemWithQtyEntry = itemWithQty;
                        if (itemWithQtyEntry.Item == null)
                        {
                            itemWithQtyEntry.Item = item;
                        }
                        packlisteEntry.ItemsWithQties.Add(itemWithQtyEntry);

                        itemWithQtyLookup.Add(itemWithQty.ItemWithQtyId, itemWithQtyEntry);
                    }

                    if (!materialAmountLookup.TryGetValue(materialAmount.MaterialAmountId, out MaterialAmount materialAmountEntry))
                    {
                        materialAmountEntry = materialAmount;
                        if (materialAmountEntry.Material == null)
                        {
                            materialAmountEntry.Material = material;
                        }
                        packlisteEntry.RawUsage.Add(materialAmountEntry);

                        materialAmountLookup.Add(materialAmount.MaterialAmountId, materialAmountEntry);
                    }

                    return packlisteEntry;
                },
                new { id },
                splitOn: "PacklisteId, ItemWithQtyId, ItemId, MaterialAmountId, MaterialId").ConfigureAwait(false);

                return list.FirstOrDefault();
            }
        }

        public async Task<IEnumerable<PacklisteData>> GetPacklisteData(int id)
        {
            // Select
            using (var db = Connection)
            {
                const string sql = @"SELECT PacklisteDataId ,ColumnNumber ,RowNumber ,Data FROM [PacklisteDatas] WHERE Packliste_PacklisteId = @Packliste_PacklisteId";

                return await db.QueryAsync<PacklisteData>(sql, new { Packliste_PacklisteId = id }).ConfigureAwait(false);
            }
        }

        public async Task InsertPacklisteData(int id, List<PacklisteData> packlisteDatas)
        {
            // Insert
            using (var db = Connection)
            {
                var sql = $@"INSERT INTO [PacklisteDatas] (RowNumber, ColumnNumber, Data, Packliste_PacklisteId) VALUES (@RowNumber, @ColumnNumber, @Data, {id})";
                db.Open();

                using (var trans = db.BeginTransaction())
                {
                    try
                    {
                        await db.ExecuteAsync(sql, packlisteDatas, transaction: trans).ConfigureAwait(false);
                        trans.Commit();
                    }
                    catch (Exception)
                    {
                        trans.Rollback();
                        throw;
                    }
                }
            }
        }

        public async Task<bool> Insert(Packliste packliste)
        {
            // Insert
            using (var db = Connection)
            {
                const string sql = @"INSERT INTO [Packlistes] (Destination, PacklisteDate, PacklisteNumber) VALUES (@Destination, @PacklisteDate, @PacklisteNumber)";

                var result = await db.ExecuteAsync(sql, new { packliste.Destination, packliste.PacklisteDate, packliste.PacklisteNumber }).ConfigureAwait(false);

                return result != 0;
            }
        }

        public async Task<bool> Update(int id, Packliste packliste)
        {
            // Update
            using (var db = Connection)
            {
                const string sql = @"UPDATE [Packlistes] SET Destination = @Destination, PacklisteDate = @PacklisteDate, PacklisteNumber = @PacklisteNumber WHERE PacklisteId = @PacklisteId";

                var result = await db.ExecuteAsync(sql, new { packliste.Destination, packliste.PacklisteDate, packliste.PacklisteNumber, PacklisteId = id }).ConfigureAwait(false);

                return result != 0;
            }
        }

        public async Task<bool> Delete(int id)
        {
            // Delete
            using (var db = Connection)
            {
                const string sql = @"DELETE FROM [Packlistes] WHERE PacklisteId = @PacklisteId";

                var result = await db.ExecuteAsync(sql, new { PacklisteId = id }).ConfigureAwait(false);

                return result != 0;
            }
        }
    }
}