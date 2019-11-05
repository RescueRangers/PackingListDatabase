using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Packilists.Shared.Data
{
    public class PacklisteData
    {
        public int RowNumber { get; set; }
        public int ColumnNumber { get; set; }
        public string Data { get; set; }
        public virtual Packliste Packliste { get; set; }
        public int PacklisteDataId { get; set; }
    }
}
