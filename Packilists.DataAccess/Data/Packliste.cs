using System;
using System.Collections.Generic;

namespace Packilists.Shared.Data
{
    public class Packliste
    {
        public List<MaterialAmount> RawUsage { get; set; }
        public string Destination { get; set; }
        public DateTime PacklisteDate { get; set; }
        public int PacklisteNumber { get; set; }
        public List<ItemWithQty> ItemsWithQties { get; set; }
        public ICollection<PacklisteData> PacklisteData { get; set; }
        public int PacklisteId { get; set; }
        public override string ToString()
        {
            return PacklisteNumber == -1 ? "EmptyPackliste" : PacklisteNumber.ToString();
        }

    }
}