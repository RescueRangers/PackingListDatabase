using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using OfficeOpenXml;
using Packlists.Model;

namespace Packlists.ExcelImport
{
    public static class ImportPackliste
    {
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

                items = (List<Item>)itemss;

            }));

            if (!dateResult) packDate = DateTime.Now;

            using (var excel = new ExcelPackage(excelFile))
            {
                var worksheet = excel.Workbook.Worksheets[1];

                var englishPackliste =
                    int.TryParse(
                        worksheet.Cells.FirstOrDefault(c => c.Text.ToLower() == "number")?.Offset(0, 1).Text,
                        out var packlisteNumber);

                if (!englishPackliste)
                    int.TryParse(
                        worksheet.Cells.FirstOrDefault(c => c.Text.ToLower() == "nummer")?.Offset(0, 1).Text,
                        out packlisteNumber);

                var packlisteData = GetPacklisteData(worksheet, englishPackliste);

                var packlisteItems = (List<Item>) GetItems(items, packlisteData);

                var results = packlisteItems.GroupBy(item => item.ItemName).Select(g =>
                    new Item {ItemName = g.First().ItemName, Quantity = g.Sum(i => i.Quantity), Materials = g.First().Materials}).ToList();

                var packliste = new Packliste
                {
                    PacklisteData = packlisteData,
                    Items = results,
                    PacklisteNumber = packlisteNumber
                };
                callback(packliste, null);
            }
            
        }

        private static IEnumerable<Item> GetItems(ICollection<Item> items, Dictionary<Tuple<int, int>, object> packlisteData)
        {

            if (items == null)
            {
                throw new ArgumentNullException(nameof(items));
            }

            var packlisteItems = new List<Item>();
            var quantityColumn = packlisteData.SingleOrDefault(c => c.Value != null && (c.Value.ToString().ToLower().Contains("qty") ||
                                                                                        c.Value.ToString().ToLower().Contains("antal"))).Key.Item2;

            var itemData = packlisteData.Where(d =>
                d.Key.Item2 == 1 && d.Value != null && (d.Value.ToString().ToLower().Contains("industri") == false &&
                                                        d.Value.ToString().ToLower().Contains("total") == false &&
                                                        d.Value.ToString().ToLower().Contains("item") == false));

            foreach (var item in itemData)
            {
                var newItem = items.FirstOrDefault(itm => string.Equals(itm.ItemName, item.Value.ToString(), StringComparison.CurrentCultureIgnoreCase));
                
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
                        Quantity = quantityValue
                    };
                }
                else
                {
                    newItem.Quantity = quantityValue;
                }
                packlisteItems.Add(newItem);
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
                    .Where(c => c.End.Row % 2 == (totalsRow + 3) % 2).Select(c => c.End.Row).ToArray();
            }
            else
            {
                dataRange = worksheet.Cells[endRow + 1, 1, rowDimension, columnDimension]
                    .Where(c => c.Address.Contains("A") && c.Text.ToLower().Contains("totaler")).Select(c => c.End.Row + 3).ToArray();
            }
            
            
            //Add table headers to the last row of pack liste header
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