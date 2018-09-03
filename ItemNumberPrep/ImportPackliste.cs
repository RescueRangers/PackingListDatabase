using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using OfficeOpenXml;

namespace ItemNumberPrep
{
    public static class ImportPackliste
    {
        public static Dictionary<Tuple<int, int>, object> FromExcel(string[] args)
        {
            var packliste = new Dictionary<Tuple<int, int>, object>();

            if (args.Length < 1 && !File.Exists(args[0])) 
                throw new ArgumentNullException(nameof(args), @"Please provide the path for an excel file");

            var excelFile = new FileInfo(args[0]);

            using (var excel = new ExcelPackage(excelFile))
            {
                var worksheet = excel.Workbook.Worksheets[1];

                var endRow = worksheet.Cells.First(c => c!=null && c.Text == "ID").End.Row;

                var columnDimension = worksheet.Dimension.End.Column;
                var rowDimension = worksheet.Dimension.End.Row;

                var dataRange = worksheet.Cells[endRow + 1, 1, rowDimension, columnDimension]
                    .Where(c => c.Address.Contains("A") && c.Text.Contains("A9B")).Select(c => c.End.Row);

                for (var row = 1; row <= endRow; row++)
                {
                    for (var column = 1; column <= columnDimension; column++)
                    {
                        packliste.Add(new Tuple<int, int>(row, column), worksheet.Cells[row, column].Value);
                    }
                }

                var targetRow = endRow + 2;

                foreach (var row in dataRange)
                {
                    for (var column = 1; column <= columnDimension; column++)
                    {
                        packliste.Add(new Tuple<int, int>(targetRow, column), worksheet.Cells[row, column].Value);
                    }

                    targetRow++;
                }

                return packliste;

            }
        }
    }
}