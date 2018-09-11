using System;
using System.Collections.Generic;

namespace Packlists.Model.Printing
{
    public interface IPrintingService
    {
        void Print(Dictionary<Tuple<int, int>, object> packlisteData);
        void PrintItemTable(Packliste packliste);
        void PrintMonthlyReport(ICollection<Packliste> packlists);
    }
}
