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
            }));

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
                    if(cocParse == false) continue;

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
                
                //var test = worksheet.Cells.FirstOrDefault(c => c.Text.ToLower() == "nummer")?.Offset(0, 2).End.Column;

                var packlisteData = GetPacklisteData(worksheet, englishPackliste != null);

                var packlisteItems = (List<ItemWithQty>) GetItems(items, packlisteData, dataService);

                var rawUsage = CalculateUsage(packlisteItems);

                var results = packlisteItems.GroupBy(item => item.Item).Select(g =>
                    new ItemWithQty {Item = g.First().Item, Quantity = g.Sum(i => i.Quantity)}).ToList();

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
                        {Material = materialWithUsage.Material, Amount = qty * materialWithUsage.Amount});
                }
            }
            var results = rawUsage.GroupBy(m => m.Material).Select(g => new MaterialAmount{Material = g.Key, Amount = g.Sum(l => l.Amount)}).OrderBy(o => o.Material).ToList();

            return results;
        }

        private static IEnumerable<ItemWithQty> GetItems(ICollection<Item> items, Dictionary<Tuple<int, int>, object> packlisteData, IDataService dataService)
        {

            if (items == null)
            {
                throw new ArgumentNullException(nameof(items));
            }

            var packlisteItems = new List<ItemWithQty>();
            var quantityColumn = packlisteData.SingleOrDefault(c => c.Value != null && (c.Value.ToString().ToLower().Contains("qty") ||
                                                                                        c.Value.ToString().ToLower().Contains("antal"))).Key.Item2;

            var itemData = packlisteData.Where(d =>
                d.Key.Item2 == 1 && d.Value != null && (d.Value.ToString().ToLower().Contains("industri") == false &&
                                                        d.Value.ToString().ToLower().Contains("total") == false &&
                                                        d.Value.ToString().ToLower().Contains("item") == false &&
                                                        d.Value.ToString().ToLower().Contains("varenum") == false));

            foreach (var item in itemData)
            {
                var newItem = items.FirstOrDefault(itm => string.Equals(itm.ItemName, item.Value.ToString(), StringComparison.CurrentCultureIgnoreCase));
                ItemWithQty itemWithQty;

                float quantityValue;
                var quantityResult =
                    packlisteData.TryGetValue(new Tuple<int, int>(item.Key.Item1, quantityColumn),
                        out var quantity);

                if (quantityResult)
                {
                    var floatResult = float.TryParse(quantity.ToString(), out quantityValue);
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
                        ItemName = item.Value.ToString(),
                    };

                    dataService.Add(newItem);
                    items.Add(newItem);

                    itemWithQty = new ItemWithQty
                    {
                        Item = newItem,
                        Quantity = quantityValue
                    };

                    //dataService.SaveData();
                }
                else
                {
                    itemWithQty = new ItemWithQty
                    {
                        Item = newItem,
                        Quantity = quantityValue
                    };

                    //dataService.SaveData();
                }
                packlisteItems.Add(itemWithQty);
            }

            return packlisteItems;
        }

        private static Dictionary<Tuple<int, int>, object> GetPacklisteData(ExcelWorksheet worksheet, bool isInEnglish)
        {
            var packlisteData = new Dictionary<Tuple<int, int>, object>();
            
            var endRow = worksheet.Cells.First(c => c != null && c.Text == "ID").End.Row - 1;
            
            var columnDimension = worksheet.Dimension.End.Column;
            var rowDimension = worksheet.Dimension.End.Row;

            int[] dataRange;

            if (isInEnglish)
            {
                var totalsRow = worksheet.Cells[endRow + 1, 1, rowDimension, 1].First(c => c.Text.ToLower().Contains("totals")).End.Row;
                dataRange = worksheet.Cells[totalsRow + 3, 1, rowDimension, columnDimension]
                    .Where(c => c.End.Row % 2 == (totalsRow + 3) % 2 && c.End.Column == 1).Select(c => c.End.Row).ToArray();
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

                packlisteData.Add(new Tuple<int, int>(endRow + 1, column), value);
            }

            for (var row = 1; row <= endRow; row++)
            {
                for (var column = 1; column <= columnDimension; column++)
                {
                    var value = worksheet.Cells[row, column].Value;
                    
                    if (value == null) continue;
                    
                    packlisteData.Add(new Tuple<int, int>(row, column), value);
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

                    packlisteData.Add(new Tuple<int, int>(targetRow, column), worksheet.Cells[row, column].Value);
                }

                targetRow++;
            }

            return packlisteData;
        }
    }
}