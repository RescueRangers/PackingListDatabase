using System;
using System.IO;

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