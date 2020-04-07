using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Microsoft.Extensions.Configuration;
using Packilists.Shared.Data;
using Packlists.Api.Repositories.Interfaces;

namespace Packlists.Api.Repositories
{
    public class TestingRepository : RepositoryBase, ITestingRepository
    {
        public TestingRepository(IConfiguration config) : base(config)
        { }

        public async Task<TimeSpan> BenchMultipleQuerries(DateTime month)
        {
            var stopwatch = Stopwatch.StartNew();

            var startDate = $"{month.ToString("yyyyMM", CultureInfo.InvariantCulture)}01";
            var endDate = $"{month.ToString("yyyyMM", CultureInfo.InvariantCulture)}{DateTime.DaysInMonth(month.Year, month.Month)}";

            using (var db = Connection)
            {
                var packlists = await db.QueryAsync<Packliste>("SELECT * FROM Packlistes WHERE PacklisteDate >= @StartDate and PacklisteDate <= @EndDate", new { startDate, endDate }).ConfigureAwait(false);
                var packlisteIds = packlists.Select(x => x.PacklisteId).ToList();

                var itemsWithQty = await db.QueryAsync<ItemWithQty>("SELECT * FROM ItemsWithQty_View WHERE PacklisteId in @IDs", new { IDs = packlisteIds }).ConfigureAwait(false);
                var itemIds = itemsWithQty.Select(x => x.ItemId).ToList();

                var materials = await db.QueryAsync<MaterialAmount>("SELECT * FROM MaterialAmount_View WHERE ItemId in @IDs", new { IDs = itemIds }).ConfigureAwait(false);

                var rawUsage = await db.QueryAsync<MaterialAmount>("SELECT * FROM MaterialAmount_View WHERE PacklisteId in @IDs", new { IDs = packlisteIds }).ConfigureAwait(false);

                foreach (var item in itemsWithQty)
                {
                    item.Materials = materials.Where(x => x.ItemId == item.ItemId).ToList();
                }

                foreach (var packingList in packlists)
                {
                    packingList.ItemsWithQties = itemsWithQty.Where(x => x.PacklisteId == packingList.PacklisteId).ToList();
                    packingList.RawUsage = rawUsage.Where(x => x.PacklisteId == packingList.PacklisteId).ToList();
                }
                stopwatch.Stop();

                return stopwatch.Elapsed;
            }
        }

        public async Task<TimeSpan> BenchMultiMapping(DateTime month)
        {
            var stopwatch = Stopwatch.StartNew();

            var startDate = $"{month.ToString("yyyyMM", CultureInfo.InvariantCulture)}01";
            var endDate = $"{month.ToString("yyyyMM", CultureInfo.InvariantCulture)}{DateTime.DaysInMonth(month.Year, month.Month)}";

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
      ,F.MaterialAmountId
      ,F.Amount
      ,G.MaterialId
	  ,G.MaterialName
	  ,G.Unit
  FROM [dbo].[Packlistes] A
  Left Outer JOIN dbo.ItemWithQties B on B.Packliste_PacklisteId = A.PacklisteId
  Left Outer JOIN dbo.Items C on C.ItemId = B.Item_ItemId
  Left Outer JOIN MaterialAmounts D on D.Packliste_PacklisteId = A.PacklisteId
  Left Outer JOIN Materials E on E.MaterialId = D.Material_MaterialId
  Left Outer JOIN MaterialAmounts F on F.Item_ItemId = C.ItemId
  Left Outer JOIN Materials G on G.MaterialId = F.Material_MaterialId
  WHERE A.PacklisteDate >= @StartDate and A.PacklisteDate <= @EndDate";

                var packlisteLookup = new Dictionary<int, Packliste>();
                var itemWithQtyLookup = new Dictionary<int, ItemWithQty>();
                var materialAmountLookup = new Dictionary<int, MaterialAmount>();
                var itemMaterialAmountLookup = new Dictionary<int, MaterialAmount>();

                var ret = await db.QueryAsync<Packliste, ItemWithQty, Item, MaterialAmount, Material, MaterialAmount, Material, Packliste>(sql, (packliste, itemWithQty, item, materialAmount, material, itemMaterialAmount, itemMaterial) =>
                {
                    Packliste packlisteEntry;
                    if (!packlisteLookup.TryGetValue(packliste.PacklisteId, out packlisteEntry))
                    {
                        packlisteEntry = packliste;
                        packlisteEntry.ItemsWithQties = new List<ItemWithQty>();
                        packlisteEntry.RawUsage = new List<MaterialAmount>();
                        packlisteEntry.PacklisteData = new List<PacklisteData>();
                        packlisteLookup.Add(packlisteEntry.PacklisteId, packlisteEntry);
                    }

                    if (itemWithQty != null && !itemWithQtyLookup.TryGetValue(itemWithQty.ItemWithQtyId, out ItemWithQty itemWithQtyEntry))
                    {
                        itemWithQtyEntry = itemWithQty;
                        if (itemWithQtyEntry.Item == null)
                        {
                            itemWithQtyEntry.Item = item;
                            itemWithQtyEntry.Item.Materials = new List<MaterialAmount>();
                        }
                        packlisteEntry.ItemsWithQties.Add(itemWithQtyEntry);

                        itemWithQtyLookup.Add(itemWithQty.ItemWithQtyId, itemWithQtyEntry);
                    }
                    else
                    {
                        return packlisteEntry;
                    }

                    if (itemMaterialAmount != null && !itemMaterialAmountLookup.TryGetValue(itemMaterialAmount.MaterialAmountId, out var itemMaterialAmountEntry))
                    {
                        itemMaterialAmountEntry = itemMaterialAmount;
                        if (itemMaterialAmountEntry.Material == null)
                        {
                            itemMaterialAmountEntry.Material = itemMaterial;
                        }
                        itemMaterialAmountLookup.Add(itemMaterialAmountEntry.MaterialAmountId, itemMaterialAmountEntry);
                        itemWithQtyEntry.Item.Materials.Add(itemMaterialAmountEntry);
                    }
                    else if (itemMaterialAmount != null)
                    {
                        if (itemMaterialAmount.Material == null)
                        {
                            itemMaterialAmount.Material = itemMaterial;
                        }
                        itemWithQtyEntry.Item.Materials.Add(itemMaterialAmount);
                    }

                    if (materialAmount != null && !materialAmountLookup.TryGetValue(materialAmount.MaterialAmountId, out MaterialAmount materialAmountEntry))
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
                new { StartDate = startDate, EndDate = endDate },
                splitOn: "PacklisteId, ItemWithQtyId, ItemId, MaterialAmountId, MaterialId, MaterialAmountId, MaterialId").ConfigureAwait(false);

                var dist = ret.Distinct();

                stopwatch.Stop();

                return stopwatch.Elapsed;
            }
        }

        public async Task<TimeSpan> BenchItemMultiQuery(int id)
        {
            var stopwatch = Stopwatch.StartNew();

            using var db = Connection;

            var item = await db.QueryFirstOrDefaultAsync<Item>("SELECT * FROM Items WHERE ItemId = @Id", new { id}).ConfigureAwait(false);

            var materials = await db.QueryAsync<MaterialAmount>("SELECT * FROM MaterialAmount_View WHERE ItemId = @Id", new { id }).ConfigureAwait(false);

            if (materials == null)
            {
                stopwatch.Stop();
                return stopwatch.Elapsed;
            }

            item.Materials = materials.ToList();

            stopwatch.Stop();
            return stopwatch.Elapsed;
        }

        public async Task<TimeSpan> BenchItemMultiMapping(int id)
        {
            var stopwatch = Stopwatch.StartNew();

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
  LEFT OUTER JOIN [dbo].[MaterialAmounts] B
  ON B.Item_ItemId = A.ItemId
  LEFT OUTER JOIN [dbo].Materials C
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

                if (materialAmount == null)
                {
                    return itemEntry;
                }

                if (materialAmount.Material == null)
                {
                    materialAmount.Material = material;
                }

                itemEntry.Materials.Add(materialAmount);

                return itemEntry;
            }, new { ItemId = id }, splitOn: "ItemId, MaterialAmountId, MaterialId").ConfigureAwait(false);

            stopwatch.Stop();

            return stopwatch.Elapsed;
        }
    }
}
