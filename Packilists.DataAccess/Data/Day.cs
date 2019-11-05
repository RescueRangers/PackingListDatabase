using System;
using System.Collections.Generic;

namespace Packilists.Shared.Data
{
    public class Day
    {
        public List<MaterialAmount> NetMaterialCount { get; set; }

        public DateTime Date { get; set; }

        public int DayId { get; set; }

        public override string ToString()
        {
            return Date.ToString();
        }
    }
}
