using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using OfficeOpenXml;
using Excel = Microsoft.Office.Interop.Excel;

namespace Packlists.Model.Printing
{
    public class PrintingService : IPrintingService
    {
        public void Print(Dictionary<Tuple<int, int>, object> packlisteData)
        {
            var path = CreateXlsx(packlisteData);

            var app = new Excel.Application();

            try
            {
                var wb = app.Workbooks.Open(path);
                var ws = (Excel.Worksheet) wb.Worksheets[1];
            
                // Print out 1 copy to the default printer:
                ws.PrintOut(
                    Type.Missing, Type.Missing, Type.Missing, Type.Missing,
                    Type.Missing, Type.Missing, Type.Missing, Type.Missing);

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


        /// <summary>
        /// Create a temporary excel file and returns its path
        /// </summary>
        /// <returns></returns>
        private static string CreateXlsx(Dictionary<Tuple<int, int>, object> packlisteData)
        {
            var path = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\temp.xlsx";

            using (var excel = new ExcelPackage(new FileInfo(path)))
            {
                var worksheet = excel.Workbook.Worksheets.Add("Sheet 1");

                var headerRow = packlisteData.ElementAt(0).Key.Item1;

                foreach (var data in packlisteData)
                {
                    worksheet.Cells[data.Key.Item1, data.Key.Item2].Value = data.Value;
                }

                worksheet.PrinterSettings.FitToWidth = 1;
                worksheet.PrinterSettings.RepeatRows = new ExcelAddress("$1:$" + headerRow);

                excel.Save();
            }

            return path;
        }
        
    }
}