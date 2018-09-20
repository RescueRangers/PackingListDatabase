using System.IO;
using System.Linq;
using OfficeOpenXml;

namespace ItemNumberPrep
{
    public static class PrepareForImport
    {
        public static void PrepareXls(string[] args)
        {
            if (args.Length < 1 && !File.Exists(args[0])) return;

            var file = new FileInfo(args[0]);

            using (var excel = new ExcelPackage(file))
            {
                var worksheet = excel.Workbook.Worksheets[1];
                var targetWorksheet = excel.Workbook.Worksheets.Add("Exported");

                var lastColumn = worksheet.Dimension.End.Column;

                for (var i = 1; i <= lastColumn; i += 2)
                {
                    var lastRow = worksheet.Cells.Last(c => c.Start.Column == i).End.Row;

                    for (var j = 2; j <= lastRow; j++)
                    {
                        var source = worksheet.Cells[j, i];

                        var existingTarget = targetWorksheet.Cells.FirstOrDefault(c => c.Value == source.Value);

                        if (existingTarget != null)
                        {
                            var lastTargetRow = targetWorksheet.Cells.Last(c => c.Start.Column == existingTarget.End.Column).End.Row;

                            existingTarget.Offset(lastTargetRow, 0).Value = source.Offset(-(j-1), 0).Value;
                            existingTarget.Offset(lastTargetRow, 1).Value = source.Offset(0, 1).Value;

                            source.Value = null;
                            source.Value = null;
                            source.Offset(0, 1).Value = null;

                        }
                        else
                        {
                            var lastTargetColumn = 0;

                            if (targetWorksheet.Dimension != null)
                            {
                                lastTargetColumn = targetWorksheet.Dimension.End.Column;
                            }

                            var target = targetWorksheet.Cells[1, lastTargetColumn + 1];

                            target.Value = source.Value;
                            target.Offset(1, 0).Value = source.Offset(-(j-1), 0).Value;
                            target.Offset(1, 1).Value = source.Offset(0, 1).Value;

                            source.Value = null;
                            source.Offset(0, 1).Value = null;
                        }

                    }
                }

                excel.SaveAs(new FileInfo(file.Directory.FullName + @"\Exported.xlsx"));
            }
        }
    }
}
