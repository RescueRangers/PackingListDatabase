using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Packlists.Model.Printing
{
    public interface IPrintingService
    {
        Task<string> Print(Dictionary<Tuple<int, int>, object> packlisteData);
        Task<string> PrintItemTable(Packliste packliste);
        Task<string> PrintMonthlyReport(ICollection<Packliste> packlists);
        Task<string> PrintMonthlyImportReport(ICollection<ImportTransport> import);
    }
}
