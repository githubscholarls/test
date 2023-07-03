using Npgsql;
using System.Data.Common;
using System.Data.Entity.Infrastructure;
using System.Diagnostics;
using WebApiTest.Application.Common.Interface;

namespace WebApiTest.Application.Database
{
    public class NpgsqlConnectionFactory : IDbConnectionFactoryls
    {
        private readonly string conn = string.Empty;
        public NpgsqlConnectionFactory(string conn)
        {
            this.conn = conn;
        }
        public async Task<DbConnection> CreateConnectionAsync()
        {
            return new NpgsqlConnection(conn);
        }
        public DbConnection CreateConnection(string nameOrConnectionString)
        {
            return new NpgsqlConnection(nameOrConnectionString);
        }
    }
}
