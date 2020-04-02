using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Linq;
using OfficeOpenXml;
using Packlists.Model;

namespace Packlists.ExcelImport
{
    public static class CreateFromPackliste
    {
        public static void FromExcel(Action<Packliste, Exception> callback, FileInfo excelFile,
            IDataService dataService)
        {
            var dateResult = DateTime.TryParse(excelFile.Name.Substring(0, 10), CultureInfo.InvariantCulture,
                DateTimeStyles.None, out var packDate);

            List<Item> items = null;

            dataService.GetItems(((itemss, materials, exception) =>
            {
                if (exception != null)
                {
                    throw exception;
                }

                items = itemss.ToList();
            }));

            if (!dateResult) packDate = DateTime.Now;

            using (var excel = new ExcelPackage(excelFile))
            {
                var worksheet = excel.Workbook.Worksheets[1];

                var englishPackliste = worksheet.Cells.FirstOrDefault(c =>
                    string.Equals(c.Text, "number", StringComparison.InvariantCultureIgnoreCase))?.Address;

                var packlisteData = GetPacklisteData(worksheet);

                var packlisteItems = (List<ItemWithQty>)GetItems(items, packlisteData, dataService);

                var results = packlisteItems.GroupBy(item => item.Item).Select(g =>
                    new ItemWithQty { Item = g.First().Item, Quantity = g.Sum(i => i.Quantity) }).ToList();

                var packliste = new Packliste
                {
                    ItemsWithQties = new ObservableCollection<ItemWithQty>(results),
                    PacklisteNumber = -1,
                    PacklisteDate = packDate,
                    Destination = "Tarm"
                };
                callback(packliste, null);
            }
        }

        private static IEnumerable<ItemWithQty> GetItems(ICollection<Item> items, List<PacklisteData> packlisteData, IDataService dataService)
        {
            if (items == null)
            {
                throw new ArgumentNullException(nameof(items));
            }

            var packlisteItems = new List<ItemWithQty>();
            var quantityColumn = packlisteData.Find(c => c.Data != null &&
                (c.Data.IndexOf("qty", StringComparison.OrdinalIgnoreCase) >= 0 ||
                c.Data.IndexOf("antal", StringComparison.OrdinalIgnoreCase) >= 0)).ColumnNumber;

            var itemData = packlisteData.Where(d =>
                d.ColumnNumber == 3 && (d.Data?.IndexOf("industri", StringComparison.OrdinalIgnoreCase) < 0 &&
                                                        d.Data.IndexOf("total", StringComparison.OrdinalIgnoreCase) < 0 &&
                                                        d.Data.IndexOf("item", StringComparison.OrdinalIgnoreCase) < 0 &&
                                                        d.Data.IndexOf("id", StringComparison.OrdinalIgnoreCase) < 0 &&
                                                        d.Data.IndexOf("varenum", StringComparison.OrdinalIgnoreCase) < 0));

            foreach (var item in itemData)
            {
                var newItem = items.FirstOrDefault(itm => string.Equals(itm.ItemName, item.Data, StringComparison.CurrentCultureIgnoreCase));
                ItemWithQty itemWithQty;

                float quantityValue;
                var quantityResult =
                    float.TryParse(
                        packlisteData.Find(f => f.RowNumber == item.RowNumber && f.ColumnNumber == quantityColumn)?
                            .Data, out quantityValue);

                if (quantityResult)
                {
                    var floatResult = float.TryParse(quantityValue.ToString(), out quantityValue);
                    if (!floatResult)
                    {
                        quantityValue = 0f;
                    }
                }
                else
                {
                    quantityValue = 0f;
                }

                if (newItem == null)
                {
                    newItem = new Item
                    {
                        ItemName = item.Data
                    };

                    dataService.Add(newItem);
                    items.Add(newItem);

                    itemWithQty = new ItemWithQty
                    {
                        Item = newItem,
                        Quantity = quantityValue
                    };
                }
                else
                {
                    itemWithQty = new ItemWithQty
                    {
                        Item = newItem,
                        Quantity = quantityValue
                    };
                }
                packlisteItems.Add(itemWithQty);
            }

            return packlisteItems;
        }

        private static List<PacklisteData> GetPacklisteData(ExcelWorksheet worksheet)
        {
            var packlisteData = new List<PacklisteData>();

            var endRow = worksheet.Cells.First(c => c != null && c.Text == "ID").End.Row - 1;

            var columnDimension = worksheet.Dimension.End.Column;
            var rowDimension = worksheet.Dimension.End.Row;

            int[] dataRange = worksheet.Cells[endRow + 3, 3, rowDimension, 3]
                    .Where(c => !string.IsNullOrWhiteSpace(c.Text)).Select(c => c.End.Row).ToArray();

            //Add table headers to the last row of pack list header
            for (var column = 1; column <= columnDimension; column++)
            {
                var value = worksheet.Cells[dataRange[0] - 2, column].Value;

                if (value == null) continue;

                packlisteData.Add(new PacklisteData { ColumnNumber = column, RowNumber = endRow + 1, Data = value.ToString() });
            }

            for (var row = 1; row <= endRow; row++)
            {
                for (var column = 1; column <= columnDimension; column++)
                {
                    var value = worksheet.Cells[row, column].Value;

                    if (value == null) continue;

                    packlisteData.Add(new PacklisteData { ColumnNumber = column, RowNumber = row, Data = value.ToString() });
                }
            }

            //Offset the target row by 3 for visual distinction from header
            var targetRow = endRow + 3;

            foreach (var row in dataRange)
            {
                for (var column = 1; column <= columnDimension; column++)
                {
                    var value = worksheet.Cells[row, column].Value;

                    if (value == null) continue;

                    packlisteData.Add(new PacklisteData { ColumnNumber = column, RowNumber = targetRow, Data = value.ToString() });
                }

                targetRow++;
            }

            return packlisteData;
        }
    }
}