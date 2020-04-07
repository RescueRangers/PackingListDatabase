using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using OfficeOpenXml;
using Packlists.Model;

namespace Packlists.ExcelImport
{
    public static class ImportItems
    {
        public static Task<string> FromExcel(Action<ICollection<Item>, Exception> callback, FileInfo excelFile, IDataService dataService)
        {
            var existingMaterials = new List<Material>();
            var items = new List<Item>();
            var addedMaterialCount = 0;
            var addedItemCount = 0;

            dataService.GetItems(((itms, materials, exception) =>
            {
                if (exception != null)
                {
                    throw exception;
                }

                existingMaterials = materials.ToList();
                items = itms.ToList();
            }));

            if (!excelFile.Exists)
            {
                callback(null, new FileLoadException());
                return Task.FromResult(result: "failurs");
            }

            var addedMaterials = new List<Material>();
            //var items = new List<Item>();

            using (var excel = new ExcelPackage(excelFile))
            {
                var worksheet = excel.Workbook.Worksheets[1];

                for (var i = 1; i < worksheet.Dimension.End.Column; i += 2)
                {
                    var exists = items.Any(it =>
                        string.Equals(it.ItemName, worksheet.Cells[1, i].Text.Trim(Environment.NewLine.ToCharArray()), StringComparison.InvariantCultureIgnoreCase));

                    if (exists)
                    {
                        continue;
                    }

                    var item = new Item
                    {
                        ItemName = worksheet.Cells[1, i].Text.Trim(Environment.NewLine.ToCharArray()),
                        Materials = new ObservableCollection<MaterialAmount>()
                    };

                    var lastRow = worksheet.Cells.Last(c => c.Start.Column == i).End.Row;

                    for (var j = 2; j <= lastRow; j++)
                    {
                        var material = existingMaterials.SingleOrDefault(m =>
                            string.Equals(m.MaterialName, worksheet.Cells[j, i].Text.Trim(Environment.NewLine.ToCharArray()),
                                StringComparison.OrdinalIgnoreCase));

                        MaterialAmount newMaterialWithUsage;

                        if (material != null)
                        {
                            newMaterialWithUsage = new MaterialAmount
                            {
                                Material = material,
                                Amount = float.Parse(worksheet.Cells[j, i + 1].Text)
                            };
                        }
                        else
                        {
                            var newMaterial = new Material { MaterialName = worksheet.Cells[j, i].Text.Trim(Environment.NewLine.ToCharArray()) };
                            existingMaterials.Add(newMaterial);
                            dataService.Add(newMaterial);

                            newMaterialWithUsage = new MaterialAmount
                            {
                                Material = newMaterial,
                                Amount = float.Parse(worksheet.Cells[j, i + 1].Text)
                            };
                            addedMaterialCount++;
                            addedMaterials.Add(newMaterial);
                            //existingMaterials.Add(newMaterial);
                        }

                        item.Materials.Add(newMaterialWithUsage);
                    }

                    addedItemCount++;
                    items.Add(item);
                    dataService.Add(item);
                }
            }

            var message = string.Empty;

            if (addedItemCount > 0)
            {
                message = $"Successfully added {addedItemCount} new items and {addedMaterials} new materials.";
            }

            dataService.SaveData();

            return Task.FromResult(result: message);
        }
    }
}