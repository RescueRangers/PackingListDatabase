using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OfficeOpenXml;

namespace ItemNumberPrep
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length < 1 && !File.Exists(args[0])) return;

            var something = ImportPackliste.FromExcel(args);

            foreach (var keyValuePair in something)
            {
                    Console.WriteLine($@"{keyValuePair.Key}: {keyValuePair.Value}");
            }

            Console.WriteLine(something.Count);

            Console.ReadKey();

        }
    }
}