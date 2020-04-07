using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OfficeOpenXml;
using Packlists.Model;

namespace Packlists.ExcelImport
{
    public static class ImportPackliste
    {
        public static Task<(Packliste, string)> FromCOCs(FileInfo excelFile, IDataService dataService)
        {
            var cOCs = new List<COC>();
            var packliste = new Packliste();
            var cocNumbers = new List<int>();
            var items = new List<ItemWithQty>();
            var packlisteItems = new List<ItemWithQty>();

            var dateResult = DateTime.TryParse(excelFile.Name.Substring(0, 10), CultureInfo.InvariantCulture,
                DateTimeStyles.None, out var packDate);
            if (!dateResult) packDate = DateTime.Now;

            dataService.GetCOCs(((cocs, exception) =>
            {
                if (exception != null)
                {
                    throw exception;
                }

                cOCs = cocs.ToList();
            }), packDate);

            dataService.GetItemsWithQty(((itemWithQties, exception) =>
            {
                if (exception != null)
                {
                    throw exception;
                }

                items = new List<ItemWithQty>(itemWithQties);
            }));

            using (var excel = new ExcelPackage(excelFile))
            {
                var worksheet = excel.Workbook.Worksheets[1];

                var endRow = worksheet.Dimension.End.Row;

                for (var i = 1; i <= endRow; i++)
                {
                    var cocParse = int.TryParse(worksheet.Cells[i, 1].Value.ToString(), out var cocNumber);
                    if (cocParse == false) continue;

                    var coc = cOCs.SingleOrDefault(c => c.CocNumber == cocNumber);
                    if (coc == null)
                    {
                        cocNumbers.Add(cocNumber);
                        continue;
                    }

                    var item = items.SingleOrDefault(s => s.ItemWithQtyId == coc.Item.ItemWithQtyId);
                    if (item == null) return Task.FromResult((packliste, $"Something went wrong: {coc.Item.Item}, {cocNumber}"));

                    packlisteItems.Add(item);
                }
            }

            packliste.PacklisteDate = packDate;
            packliste.Destination = "Tarm";
            packliste.ItemsWithQties = new ObservableCollection<ItemWithQty>(packlisteItems);
            packliste.PacklisteNumber = -1;
            packliste.RawUsage = new ObservableCollection<MaterialAmount>(CalculateUsage(packlisteItems));

            if (cocNumbers.Any())
            {
                var message = new StringBuilder("Missing COCs:\n");
                foreach (var cocNumber in cocNumbers)
                {
                    message.AppendLine(cocNumber.ToString());
                }
                return Task.FromResult((packliste, message.ToString()));
            }

            return Task.FromResult((packliste, "Success"));
        }

        public static void FromExcel(Action<Packliste, Exception> callback, FileInfo excelFile, IDataService dataService)
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

                var location = worksheet.Cells[7, 2].Text;

                var englishPackliste = worksheet.Cells.FirstOrDefault(c =>
                    string.Equals(c.Text, "number", StringComparison.InvariantCultureIgnoreCase))?.Address;

                var packlisteNumber = 0;

                if (englishPackliste != null)
                {
                    packlisteNumber = worksheet.Cells[englishPackliste].Offset(0, 1).Value != null
                        ? int.Parse(worksheet.Cells[englishPackliste].Offset(0, 1).Text)
                        : int.Parse(worksheet.Cells[englishPackliste].Offset(0, 2).Text);
                }

                if (englishPackliste == null)
                {
                    var nonEnglishPackliste = worksheet.Cells.FirstOrDefault(c =>
                        string.Equals(c.Text, "nummer", StringComparison.InvariantCultureIgnoreCase))?.Address;

                    packlisteNumber = worksheet.Cells[nonEnglishPackliste].Offset(0, 1).Value != null
                        ? int.Parse(worksheet.Cells[nonEnglishPackliste].Offset(0, 1).Text)
                        : int.Parse(worksheet.Cells[nonEnglishPackliste].Offset(0, 2).Text);
                }

                var packlisteData = GetPacklisteData(worksheet, englishPackliste != null);

                var packlisteItems = (List<ItemWithQty>)GetItems(items, packlisteData, dataService);

                var rawUsage = CalculateUsage(packlisteItems);

                var results = packlisteItems.GroupBy(item => item.Item).Select(g =>
                    new ItemWithQty { Item = g.First().Item, Quantity = g.Sum(i => i.Quantity) }).ToList();

                var packliste = new Packliste
                {
                    PacklisteData = packlisteData,
                    ItemsWithQties = new ObservableCollection<ItemWithQty>(results),
                    PacklisteNumber = packlisteNumber,
                    PacklisteDate = packDate,
                    Destination = location,
                    RawUsage = new ObservableCollection<MaterialAmount>(rawUsage)
                };
                callback(packliste, null);
            }
        }

        private static List<MaterialAmount> CalculateUsage(List<ItemWithQty> itemsWithQties)
        {
            var rawUsage = new List<MaterialAmount>();

            if (itemsWithQties == null || itemsWithQties.Any() == false)
            {
                return new List<MaterialAmount>();
            }

            foreach (var itemWithQty in itemsWithQties)
            {
                var qty = itemWithQty.Quantity;

                if (itemWithQty.Item.Materials == null) continue;

                foreach (var materialWithUsage in itemWithQty.Item.Materials)
                {
                    rawUsage.Add(new MaterialAmount
                    { Material = materialWithUsage.Material, Amount = qty * materialWithUsage.Amount });
                }
            }
            return rawUsage.GroupBy(m => m.Material).Select(g => new MaterialAmount { Material = g.Key, Amount = g.Sum(l => l.Amount) }).OrderBy(o => o.Material).ToList();
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

                var qResult =
                    float.TryParse(
                        packlisteData.Find(f => f.RowNumber == item.RowNumber && f.ColumnNumber == quantityColumn)?
                            .Data, out quantityValue);

                if (!qResult)
                {
                    continue;
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

        private static List<PacklisteData> GetPacklisteData(ExcelWorksheet worksheet, bool isInEnglish)
        {
            var packlisteData = new List<PacklisteData>();

            var endRow = worksheet.Cells.First(c => c != null && c.Text == "ID").End.Row - 1;

            var columnDimension = worksheet.Dimension.End.Column;
            var rowDimension = worksheet.Dimension.End.Row;

            int[] dataRange;

            if (isInEnglish)
            {
                dataRange = worksheet.Cells[endRow + 3, 3, rowDimension, 3]
                    .Where(c => !string.IsNullOrWhiteSpace(c.Text)).Select(c => c.End.Row).ToArray();
            }
            else
            {
                dataRange = worksheet.Cells[endRow + 1, 1, rowDimension, columnDimension]
                    .Where(c => c.Address.Contains("A") && c.Text.ToLower().Contains("totaler")).Select(c => c.End.Row + 3).ToArray();
            }

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