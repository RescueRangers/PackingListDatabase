using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using OfficeOpenXml;
using Packlists.Model;

namespace Packlists.ExcelImport
{
    public class ImportCOCs
    {
        public static Task<string> FromExcel(FileInfo excelFile, IDataService dataService)
        {
            List<Item> items = null;

            dataService.GetItems(((itemNumbers, materials, exception) =>
            {
                if (exception != null)
                {
                    throw exception;
                }

                items = itemNumbers.ToList();
            }));

            var cocs = new List<COC>();

            //try
            //{
            using (var excel = new ExcelPackage(excelFile))
            {
                var stopwatch = new Stopwatch();

                stopwatch.Start();

                var worksheet = excel.Workbook.Worksheets[1];
                var endRow = worksheet.Dimension.End.Row;

                var qty = 0;

                for (var i = 1; i <= endRow; i++)
                {
                    var qtyParse = int.TryParse(worksheet.Cells[i, 3].Value.ToString(), out var cellQty);
                    if (qtyParse == false) continue;

                    if (i != endRow && worksheet.Cells[i, 1].Value.ToString() == worksheet.Cells[i + 1, 1].Value.ToString())
                    {
                        if (qty == 0) qty = cellQty;

                        var qtyBelowParse = int.TryParse(worksheet.Cells[i, 3].Value.ToString(), out var qtyBelow);
                        if (qtyBelowParse == false) continue;

                        qty += qtyBelow;
                        continue;
                    }

                    if (qty == 0) qty = cellQty;

                    var cocNumberParse = int.TryParse(worksheet.Cells[i, 1].Value.ToString(), out var cocNumber);
                    if (cocNumberParse == false)
                    {
                        qty = 0;
                        continue;
                    }

                    var dateParse =
                        DateTime.TryParse(worksheet.Cells[i, 4].Value.ToString(), out var inventoryDate);
                    if (dateParse == false) inventoryDate = DateTime.Now;

                    var itemNumber = worksheet.Cells[i, 2].Value.ToString();

                    var newItem = items.FirstOrDefault(itm => string.Equals(itm.ItemName,
                        itemNumber, StringComparison.CurrentCultureIgnoreCase));
                    ItemWithQty itemWithQty;

                    if (newItem == null)
                    {
                        newItem = new Item
                        {
                            ItemName = itemNumber
                        };

                        dataService.Add(newItem);
                        items.Add(newItem);

                        itemWithQty = new ItemWithQty
                        {
                            Item = newItem,
                            Quantity = qty
                        };
                    }
                    else
                    {
                        itemWithQty = new ItemWithQty
                        {
                            Item = newItem,
                            Quantity = qty
                        };
                    }

                    var coc = new COC
                    {
                        Item = itemWithQty,
                        CocNumber = cocNumber,
                        InventoryDate = inventoryDate
                    };

                    qty = 0;
                    cocs.Add(coc);
                }

                dataService.BulkAdd(cocs);

                stopwatch.Stop();

                Console.WriteLine(stopwatch.Elapsed);

                var message = $"Success";

                dataService.SaveData();

                return Task.FromResult(result: message);
            }
            //}
            //catch (NullReferenceException exception)
            //{
            //    Console.WriteLine(exception);
            //    throw;
            //    Task.FromResult(result: Malformed excel file + Environment.NewLine exception.Message);
            //}
        }
    }
}