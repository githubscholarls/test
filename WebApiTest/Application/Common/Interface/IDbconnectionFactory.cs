using System.Data.Common;
using System.Data.Entity.Infrastructure;

namespace WebApiTest.Application.Common.Interface
{
    public interface IDbConnectionFactoryls : IDbConnectionFactory
    {
        Task<DbConnection> CreateConnectionAsync();
    }
}
