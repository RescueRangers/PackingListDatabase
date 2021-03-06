﻿using System.Collections.Generic;
using System.Threading.Tasks;
using Packilists.Shared.Data;

namespace Packlists.Api.Repositories.Interfaces
{
    public interface IImportsRepository
    {
        Task<bool> Delete(int id);
        Task<IEnumerable<ImportTransport>> Get();
        Task<ImportTransport> GetById(int id);
        Task<bool> Insert(ImportTransport importtransport);
        Task<bool> Update(int id, ImportTransport importtransport);
    }
}