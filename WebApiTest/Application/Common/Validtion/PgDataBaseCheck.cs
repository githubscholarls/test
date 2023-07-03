using Microsoft.Extensions.Diagnostics.HealthChecks;
using System.Data.Entity.Infrastructure;
using WebApiTest.Application.Common.Interface;

namespace WebApiTest.Application.Common.Validtion
{
    public class PgDataBaseCheck : IHealthCheck
    {
        private readonly IDbConnectionFactoryls dbConnectionFactory;

        public PgDataBaseCheck(IDbConnectionFactoryls dbConnectionFactory)
        {
            this.dbConnectionFactory = dbConnectionFactory;
        }

        public async Task<HealthCheckResult> CheckHealthAsync(
            HealthCheckContext context,
            CancellationToken cancellationToken = default)
        {
            try
            {
                using var connection = await dbConnectionFactory.CreateConnectionAsync();
                connection.Open();
                using var comand = connection.CreateCommand();
                comand.CommandText = "SELECT 1";
                comand.ExecuteScalar();
                return HealthCheckResult.Healthy();
            }
            catch (Exception e)
            {
                return HealthCheckResult.Unhealthy(exception: e);
            }

        }
    }
}
