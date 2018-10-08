﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using MigraDoc.DocumentObjectModel;
using MigraDoc.Rendering;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using Excel = Microsoft.Office.Interop.Excel;
using PdfDocument = Spire.Pdf.PdfDocument;

namespace Packlists.Model.Printing
{
    public class PrintingService : IPrintingService
    {
        private static void PrintXls(string path)
        {
            var app = new Excel.Application();

            try
            {
                var wb = app.Workbooks.Open(path);
                var ws = (Excel.Worksheet) wb.Worksheets[1];

                // Print out 1 copy to the default printer:
                ws.PrintOut();

                // Cleanup:
                GC.Collect();
                GC.WaitForPendingFinalizers();

                Marshal.FinalReleaseComObject(ws);

                wb.Close(false, Type.Missing, Type.Missing);
                Marshal.FinalReleaseComObject(wb);

                app.Quit();
                Marshal.FinalReleaseComObject(app);
            }
            finally
            {
                File.Delete(path);
            }
        }

        private static void PrintPdf(string path)
        {
            try
            {
                var doc = new PdfDocument();
                doc.LoadFromFile(path);
                doc.PrintDocument.Print();
            }
            finally
            {
                File.Delete(path);
            }
        }

        #region MonthlyImports

        public Task<string> PrintMonthlyImportReport(ICollection<ImportTransport> import)
        {
            var path = CreateMonthlyImportReport(import);
            PrintXls(path);

            return Task.FromResult("Printing successful");
        }

        private static string CreateMonthlyImportReport(ICollection<ImportTransport> imports)
        {
            var path = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\Month_import_temp.xlsx";

            var file = new FileInfo(path);

            if (file.Exists)
            {
                file.Delete();
            }

            using (var excel = new ExcelPackage(file))
            {
                var worksheet = excel.Workbook.Worksheets.Add(
                    $"{imports.ElementAt(0).ImportDate.Year}-{imports.ElementAt(0).ImportDate.Month}");

                worksheet.Row(1).Height = 30;
                worksheet.Cells[1, 1].Value = imports.ElementAt(0).ImportDate.Year;
                worksheet.Cells[1, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                
                worksheet.Cells[1, 2].Value =
                    CultureInfo.InvariantCulture.DateTimeFormat.GetMonthName(imports.ElementAt(0).ImportDate.Month);

                using (var range = worksheet.Cells[1, 1, 1, 2])
                {
                    range.Style.Font.Bold = true;
                    range.Style.Font.Size = 14;
                    range.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                }

                worksheet.Cells[2, 1].Value = "Material";
                worksheet.Cells[2, 1].Style.Font.Bold = true;
                worksheet.Cells[2, 2].Value = "Unit";
                worksheet.Cells[2, 2].Style.Font.Bold = true;
                worksheet.Column(2).AutoFit(7.14);

                var materials = imports.SelectMany(s => s.ImportedMaterials).GroupBy(m => m.Material)
                    .Select(g => g.Key).OrderBy(o => o.MaterialName)
                    .ToList();

                worksheet.Cells[3, 1].LoadFromCollection(materials.Select(m => m.MaterialName));
                worksheet.Cells[3, 2].LoadFromCollection(materials.Select(m => m.Unit));
                var column = 3;

                foreach (var import in imports)
                {
                    if (worksheet.Cells[1, column -1].Text == import.ImportDate.ToString("yyyy-MM-dd"))
                    {
                        continue;
                    }

                    worksheet.Cells[1, column].Value = import.ImportDate.ToString("yyyy-MM-dd");
                    worksheet.Cells[1, column, 2, column].Merge = true;
                    worksheet.Column(column).Width = 12;
                    worksheet.Cells[1, column, 2, column].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    worksheet.Cells[1, column, 2, column].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                    var rawUsage = imports.Where(p => p.ImportDate == import.ImportDate).SelectMany(s => s.ImportedMaterials).GroupBy(m => m.Material)
                        .Select(g => new MaterialAmount{Material = g.Key, Amount = g.First().Amount}).OrderBy(o => o.Material)
                        .ToList();

                    foreach (var usage in rawUsage)
                    {
                        var materialRow =
                            worksheet.Cells.SingleOrDefault(c => string.Equals(c.Text, usage.Material.MaterialName)).End
                                .Row;

                        worksheet.Cells[materialRow, column].Value = usage.Amount;
                        worksheet.Cells[materialRow, column].Style.Numberformat.Format = "0.00";
                    }

                    column++;
                }

                using (var range = worksheet.Cells[1, 1, materials.Count + 2, column-1])
                {
                    range.Style.Border.BorderAround(ExcelBorderStyle.Thin);
                    range.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                    range.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                    range.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                    range.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                    range.AutoFitColumns(11.5);
                }

                for (var i = 2; i <= materials.Count + 2; i++)
                {
                    worksheet.Row(i).Height = 18.1;
                }

                worksheet.PrinterSettings.PaperSize = ePaperSize.A4;
                worksheet.PrinterSettings.Orientation = eOrientation.Landscape;
                worksheet.PrinterSettings.FitToPage = true;
                worksheet.PrinterSettings.FitToWidth = 0;
                worksheet.PrinterSettings.FitToHeight = 1;
                worksheet.PrinterSettings.RepeatColumns = new ExcelAddress("A:B");

                excel.Save();
            }

            return path;
        }

        #endregion

        #region MonthlyUsage

        public Task<string> PrintMonthlyReport(ICollection<Packliste> packlists)
        {
            if (packlists == null || packlists.Count <1)
            {
                throw new ArgumentException(@"Packlist collection was null or empty", nameof(packlists));
            }

            var path = CreateMonthlyReport(packlists);

            PrintXls(path);

            return Task.FromResult("Printing successful");
        }

        private static string CreateMonthlyReport(ICollection<Packliste> packlists)
        {
            var path = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\Month_temp.xlsx";

            var file = new FileInfo(path);

            if (file.Exists)
            {
                file.Delete();
            }

            using (var excel = new ExcelPackage(file))
            {
                var worksheet = excel.Workbook.Worksheets.Add(
                    $"{packlists.ElementAt(0).PacklisteDate.Year}-{packlists.ElementAt(0).PacklisteDate.Month}");

                worksheet.Row(1).Height = 30;
                worksheet.Cells[1, 1].Value = packlists.ElementAt(0).PacklisteDate.Year;
                worksheet.Cells[1, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                
                worksheet.Cells[1, 2].Value =
                    CultureInfo.InvariantCulture.DateTimeFormat.GetMonthName(packlists.ElementAt(0).PacklisteDate
                        .Month);

                using (var range = worksheet.Cells[1, 1, 1, 2])
                {
                    range.Style.Font.Bold = true;
                    range.Style.Font.Size = 14;
                    range.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                }

                worksheet.Cells[2, 1].Value = "Material";
                worksheet.Cells[2, 1].Style.Font.Bold = true;
                worksheet.Cells[2, 2].Value = "Unit";
                worksheet.Cells[2, 2].Style.Font.Bold = true;
                worksheet.Column(2).AutoFit(7.14);

                var materials = packlists.SelectMany(s => s.RawUsage).GroupBy(m => m.Item1)
                    .Select(g => g.Key).OrderBy(o => o.MaterialName)
                    .ToList();

                worksheet.Cells[3, 1].LoadFromCollection(materials.Select(m => m.MaterialName));
                worksheet.Cells[3, 2].LoadFromCollection(materials.Select(m => m.Unit));
                var column = 3;

                foreach (var packliste in packlists)
                {
                    if (worksheet.Cells[1, column -1].Text == packliste.PacklisteDate.ToString("yyyy-MM-dd"))
                    {
                        continue;
                    }

                    worksheet.Cells[1, column].Value = packliste.PacklisteDate.ToString("yyyy-MM-dd");
                    worksheet.Cells[1, column, 2, column].Merge = true;
                    worksheet.Column(column).Width = 12;
                    worksheet.Cells[1, column, 2, column].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    worksheet.Cells[1, column, 2, column].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                    var rawUsage = packlists.Where(p => p.PacklisteDate == packliste.PacklisteDate).SelectMany(s => s.RawUsage).GroupBy(m => m.Item1)
                        .Select(g => Tuple.Create(g.Key, g.Sum(l => l.Item2), g.First().Item3)).OrderBy(o => o.Item1)
                        .ToList();

                    foreach (var usage in rawUsage)
                    {
                        var materialRow =
                            worksheet.Cells.SingleOrDefault(c => string.Equals(c.Text, usage.Item1.MaterialName)).End
                                .Row;

                        worksheet.Cells[materialRow, column].Value = usage.Item2;
                        worksheet.Cells[materialRow, column].Style.Numberformat.Format = "0.00";
                    }

                    column++;

                }

                using (var range = worksheet.Cells[1, 1, materials.Count + 2, column-1])
                {
                    range.Style.Border.BorderAround(ExcelBorderStyle.Thin);
                    range.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                    range.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                    range.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                    range.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                    range.AutoFitColumns(11.5);
                }

                for (var i = 2; i <= materials.Count + 2; i++)
                {
                    worksheet.Row(i).Height = 18.1;
                }

                worksheet.PrinterSettings.PaperSize = ePaperSize.A4;
                worksheet.PrinterSettings.Orientation = eOrientation.Landscape;
                worksheet.PrinterSettings.FitToPage = true;
                worksheet.PrinterSettings.FitToWidth = 0;
                worksheet.PrinterSettings.FitToHeight = 1;

                excel.Save();
            }

            return path;
        }

        #endregion

        #region ItemTable

        public Task<string> PrintItemTable(Packliste packliste)
        {
            var path = CreateItemTable(packliste);

            PrintXls(path);
            return Task.FromResult("Printing successful");
        }

        private static string CreateItemTable(Packliste packliste)
        {
            var path = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\temp.xlsx";

            using (var excel = new ExcelPackage(new FileInfo(path)))
            {
                var worksheet = excel.Workbook.Worksheets.Add("Sheet 1");

                worksheet.Cells[1, 1].Value = packliste.PacklisteDate;
                worksheet.Cells[1,1].Style.Numberformat.Format = "yyyy-mm-dd";
                worksheet.Cells[1, 2].Value = "Middle parts shipment";
                worksheet.Cells[2, 1].Value = "Item number";
                worksheet.Cells[2, 2].Value = "Quantity";

                using (var range = worksheet.Cells[1,1,2,2])
                {
                    range.Style.Font.Bold = true;
                    range.Style.Border.BorderAround(ExcelBorderStyle.Medium);
                    range.Style.Border.Left.Style = ExcelBorderStyle.Medium;
                    range.Style.Border.Right.Style = ExcelBorderStyle.Medium;
                    range.Style.Border.Bottom.Style = ExcelBorderStyle.Medium;
                    range.Style.Border.Top.Style = ExcelBorderStyle.Medium;
                    range.Style.Font.Bold = true;
                    range.Style.Font.Size = 20;
                    range.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    range.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                }

                worksheet.Column(1).Width = 30.51d;
                worksheet.Column(2).Width = 43.13d;
                worksheet.Row(1).Height = 63.75d;
                worksheet.Row(2).Height = 28.5d;

                var row = 3;

                foreach (var data in packliste.ItemsWithQties)
                {
                    worksheet.Cells[row, 1].Value = data.Item.ItemName;
                    worksheet.Cells[row, 2].Value = data.Quantity;

                    worksheet.Row(row).Height = 28.5d;

                    row++;
                }

                using (var range = worksheet.Cells[2,1,row -1,2])
                {
                    range.Style.Border.BorderAround(ExcelBorderStyle.Medium);
                    range.Style.Border.BorderAround(ExcelBorderStyle.Medium);
                    range.Style.Border.Left.Style = ExcelBorderStyle.Medium;
                    range.Style.Border.Right.Style = ExcelBorderStyle.Medium;
                    range.Style.Border.Bottom.Style = ExcelBorderStyle.Medium;
                    range.Style.Border.Top.Style = ExcelBorderStyle.Medium;
                    range.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    range.Style.Font.Size = 14;
                }

                worksheet.PrinterSettings.PaperSize = ePaperSize.A4;
                worksheet.PrinterSettings.Orientation = eOrientation.Portrait;
                worksheet.PrinterSettings.HorizontalCentered = true;
                worksheet.PrinterSettings.FitToPage = true;
                worksheet.PrinterSettings.FitToWidth = 1;
                worksheet.PrinterSettings.FitToHeight = 0;

                excel.Save();
            }

            return path;
        }

        #endregion

        #region Packliste

        public Task<string> Print(Dictionary<Tuple<int, int>, object> packlisteData)
        {
            var path = CreatePdf(packlisteData);

            PrintPdf(path);

            return Task.FromResult("Printing successful");
        }

        private static string CreatePdf(Dictionary<Tuple<int, int>, object> packlisteData)
        {
            var path = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\temp.pdf";

            var document = new Document();
            var page = document.AddSection();
            page.PageSetup.LeftMargin = 25;
            page.PageSetup.RightMargin = 25;
            
            var headParagraph = page.AddParagraph();
            headParagraph.Format.SpaceAfter = 18;
            var table = page.AddTable();

            var column = table.AddColumn("2.5cm");
            column.Format.Alignment = ParagraphAlignment.Left;
            column = table.AddColumn("7.8cm");
            column.Format.Alignment = ParagraphAlignment.Left;
            column = table.AddColumn("3.8cm");
            column.Format.Alignment = ParagraphAlignment.Left;
            column = table.AddColumn("1.5cm");
            column.Format.Alignment = ParagraphAlignment.Left;
            column = table.AddColumn("1.5cm");
            column.Format.Alignment = ParagraphAlignment.Left;
            column = table.AddColumn("1.5cm");
            column.Format.Alignment = ParagraphAlignment.Left;

            var lineCount = packlisteData.Last().Key.Item1;
            var headContent = new StringBuilder();

            //First line is the line where items list start
            for (var i = 1; i <= packlisteData.First().Key.Item1-1; i++)
            {
                var iLine = packlisteData.Where(l => l.Key.Item1 == i).ToList();

                if (!iLine.Any())
                {
                    iLine = new List<KeyValuePair<Tuple<int, int>, object>>{new KeyValuePair<Tuple<int, int>, object>(Tuple.Create(1,1), Environment.NewLine)};
                }

                var line = string.Empty;

                for (var j = 0; j < iLine.Count(); j++)
                {
                    var newLine = iLine[j].Value.ToString();

                    if (iLine.Count == 1)
                    {
                        line += newLine.PadLeft(newLine.Length + iLine[j].Key.Item2, '\t');
                        break;
                    }
                    line += newLine;

                    if(j+1 == iLine.Count) continue;

                    var placementDifference = iLine[j + 1].Key.Item2 - iLine[j].Key.Item2;

                    line = line.PadRight(line.Length + placementDifference, '\t');
                }

                headContent.AppendLine(line);
            }


            for (var i = packlisteData.First().Key.Item1; i <= lineCount; i++)
            {
                var iLine = packlisteData.Where(l => l.Key.Item1 == i).ToList();

                if (!iLine.Any())
                {
                    continue;
                }

                var row = table.AddRow();
                row.Format.Alignment = ParagraphAlignment.Left;
                row.Format.Font = new Font("Courier New", 8);
                row.Cells[0].AddParagraph(iLine[0].Value.ToString());
                row.Cells[1].AddParagraph(iLine[1].Value.ToString());
                row.Cells[2].AddParagraph(iLine[2].Value.ToString());
                row.Cells[3].AddParagraph(iLine[3].Value.ToString());
                row.Cells[4].AddParagraph(iLine[4].Value.ToString());
                row.Cells[5].AddParagraph(iLine[5].Value.ToString());
                
            }

            
            var headFont = new Font("Courier New", 9);

            headParagraph.AddFormattedText(headContent.ToString(), headFont);
            
            var pdfRenderer = new PdfDocumentRenderer {Document = document};
            pdfRenderer.RenderDocument();

            pdfRenderer.PdfDocument.Save(path);

            return path;
        }

        //private static string CreateXlsx(Dictionary<Tuple<int, int>, object> packlisteData)
        //{
        //    var path = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\temp.xlsx";

        //    using (var excel = new ExcelPackage(new FileInfo(path)))
        //    {
        //        var worksheet = excel.Workbook.Worksheets.Add("Sheet 1");

        //        var headerRow = packlisteData.ElementAt(0).Key.Item1;

        //        foreach (var data in packlisteData)
        //        {
        //            worksheet.Cells[data.Key.Item1, data.Key.Item2].Value = data.Value;
        //            if (data.Value is DateTime)
        //            {
        //                worksheet.Cells[data.Key.Item1, data.Key.Item2].Style.Numberformat.Format = "yyyy-mm-dd";
        //                worksheet.Cells[data.Key.Item1, data.Key.Item2].AutoFitColumns();
        //            }
        //        }
                
        //        worksheet.PrinterSettings.RepeatRows = new ExcelAddress("$1:$" + headerRow);

        //        worksheet.PrinterSettings.PaperSize = ePaperSize.A4;
        //        worksheet.PrinterSettings.Orientation = eOrientation.Portrait;
        //        worksheet.PrinterSettings.HorizontalCentered = true;
        //        worksheet.PrinterSettings.FitToPage = true;
        //        worksheet.PrinterSettings.FitToWidth = 1;
        //        worksheet.PrinterSettings.FitToHeight = 0;

        //        excel.Save();
        //    }

        //    return path;
        //}

        #endregion
        
    }
}