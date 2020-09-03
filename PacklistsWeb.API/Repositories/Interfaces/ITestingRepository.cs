using System;
using System.Threading.Tasks;

namespace PacklistsWeb.API.Repositories.Interfaces
{
    public interface ITestingRepository
    {
        Task<TimeSpan> BenchMultiMapping(DateTime month);

        Task<TimeSpan> BenchMultipleQuerries(DateTime month);

        Task<TimeSpan> BenchItemMultiMapping(int id);

        Task<TimeSpan> BenchItemMultiQuery(int id);
    }
}