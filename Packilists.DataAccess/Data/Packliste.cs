using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Packilists.Shared.Data
{
    public class Packliste
    {
        public List<MaterialAmount> RawUsage { get; set; }
        public string Destination { get; set; }
        [Required(ErrorMessage = "Field should not be empty")]
        public DateTime PacklisteDate { get; set; }
        [Required(ErrorMessage = "Field should not be empty")]
        public int PacklisteNumber { get; set; }
        public List<ItemWithQty> ItemsWithQties { get; set; }
        public ICollection<PacklisteData> PacklisteData { get; set; }
        public int PacklisteId { get; set; }
        public override string ToString()
        {
            return PacklisteNumber == -1 ? "EmptyPackliste" : PacklisteNumber.ToString();
        }

        public bool EmptyItems { get { return ItemsWithQties?.Any(i => i.IsEmpty) == true; } }
    }
}