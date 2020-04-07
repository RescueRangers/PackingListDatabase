using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Microsoft.Extensions.Configuration;
using Packilists.Shared.Data;
using Packlists.Api.Repositories.Interfaces;

namespace Packlists.Api.Repositories
{
    public class ImportsRepository : RepositoryBase, IImportsRepository
    {
        public ImportsRepository(IConfiguration config) : base(config)
        { }

        public async Task<IEnumerable<ImportTransport>> Get()
        {
            // Select
            using (var db = Connection)
            {
                const string sql = @"SELECT Sender, ImportDate, ImportTransportId FROM [ImportTransports]";

                return await db.QueryAsync<ImportTransport>(sql).ConfigureAwait(false);
            }
        }

        public async Task<ImportTransport> GetById(int id)
        {
            // Select
            using (var db = Connection)
            {
                const string sql = @"SELECT Sender, ImportDate, ImportTransportId FROM [ImportTransports] WHERE ImportTransportId = @ImportTransportId";

                return await db.QueryFirstOrDefaultAsync<ImportTransport>(sql, new { ImportTransportId = id }).ConfigureAwait(false);
            }
        }

        public async Task<bool> Insert(ImportTransport importtransport)
        {
            // Insert
            using (var db = Connection)
            {
                const string sql = @"INSERT INTO [ImportTransports] (Sender, ImportDate) VALUES (@Sender, @ImportDate)";

                var result = await db.ExecuteAsync(sql, new { Sender = importtransport.Sender, ImportDate = importtransport.ImportDate }).ConfigureAwait(false);

                return result != 0;
            }
        }

        public async Task<bool> Update(int id, ImportTransport importtransport)
        {
            // Update
            using (var db = Connection)
            {
                const string sql = @"UPDATE [ImportTransports] SET Sender = @Sender, ImportDate = @ImportDate WHERE ImportTransportId = @ImportTransportId";

                var result = await db.ExecuteAsync(sql, new { Sender = importtransport.Sender, ImportDate = importtransport.ImportDate, ImportTransportId = id });

                return result != 0;
            }
        }

        public async Task<bool> Delete(int id)
        {
            // Delete
            using (var db = Connection)
            {
                const string sql = @"DELETE FROM [ImportTransports] WHERE ImportTransportId = @ImportTransportId";

                var result = await db.ExecuteAsync(sql, new { id });

                return result != 0;
            }
        }
    }
}
